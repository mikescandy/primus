using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Timers;
using Avalonia;
using Avalonia.Input;
using Avalonia.Labs.Controls;
using Avalonia.Threading;
using Primus.Models;
using SkiaSharp;
using SKPaintSurfaceEventArgs = Avalonia.Labs.Controls.SKPaintSurfaceEventArgs;

namespace Primus;

using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;

public class ColorPool
{
    private class ColorEntry
    {
        public SKColor Color { get; }
        public bool IsAvailable { get; set; }

        public ColorEntry(SKColor color)
        {
            Color = color;
            IsAvailable = true;
        }
    }

    private readonly List<ColorEntry> _colors;

    public ColorPool(IEnumerable<SKColor> initialColors)
    {
        if (initialColors == null) throw new ArgumentNullException(nameof(initialColors));
        _colors = initialColors.Select(color => new ColorEntry(color)).ToList();
    }

    public SKColor? BorrowColor()
    {
        var availableColor = _colors.FirstOrDefault(c => c.IsAvailable);
        if (availableColor != null)
        {
            availableColor.IsAvailable = false;
            return availableColor.Color;
        }
        return null; // Or throw an exception if you prefer
    }

    public void ReturnColor(SKColor? color)
    {
        var colorEntry = _colors.FirstOrDefault(c => c.Color == color);
        if (colorEntry != null)
        {
            colorEntry.IsAvailable = true;
        }
        else
        {
            throw new InvalidOperationException("This color does not belong to the pool.");
        }
    }
}

public class SkPlayersView : SKCanvasView
{
     
    private readonly SKColor[] _baseColors =
    [
        SKColors.Red,
        SKColors.Green,
        SKColors.Blue,
        SKColors.Yellow,
        SKColors.White,
        SKColors.Purple,
        SKColors.Orange,
        SKColors.Turquoise,
        SKColors.Pink,
        SKColors.Gray
    ];

    private readonly SKPaint _blackPaint = new()
    {
        Style = SKPaintStyle.Fill,
        Color = SKColors.Black
    };

    // private readonly AvailableColor[] _colors;

    private readonly float _endBlackCircleRadius;

    private readonly Stopwatch _endStopwatch;

    private readonly object _o = new();

    private readonly ConcurrentDictionary<int, Player> _players = new();

    private readonly Random _rnd = new();

    private readonly float _screenDensity;

    private readonly Stopwatch _stopwatch;

    private readonly double _textCycleTime = 3000;

    private readonly SKPaint _textPaint = new()
    {
        IsAntialias = true,
        Color = SKColors.White,
        StrokeWidth = 1,
        Style = SKPaintStyle.Fill,
        Typeface = GetTypeface("LondrinaSolid-Regular.otf")
    };

    private readonly SKPath _textPath = new();
    private readonly Stopwatch _textStopwatch;
    private double _easing;
    private bool _isRunning;
    private int _playerSelection = -1;
    private bool _playerSelectionComplete;
    private double _previousEasing;
    private bool _reset;
    private bool _selectionCompleteStarted;
    private readonly Timer _timer = new();
    private double _t2;
    private string _text;
    private double _textT;

    private ColorPool _colorPool;
    
    public SkPlayersView()
    {
        _colorPool = new ColorPool(_baseColors);
        PointerPressed += OnPointerPressed;
        PointerMoved += OnPointerMoved;
        PointerReleased += OnPointerReleased;
        PointerExited += OnPointerExited;
        
       // _colors = new AvailableColor[_baseColors.Length];
        _screenDensity = 1f;
        _textPaint.TextSize = 50f * _screenDensity;
        IgnorePixelScaling = true;
        // for (var index = 0; index < _baseColors.Length; index++)
        // {
        //     var baseColor = _baseColors[index];
        //     _colors[index] = new AvailableColor(baseColor);
        // }

        _endBlackCircleRadius = _screenDensity * 70f * 2f;

        _text = "Tap And Hold";
        _stopwatch = new Stopwatch();
        _endStopwatch = new Stopwatch();
        _textStopwatch = new Stopwatch();
        Init();
    }

    private void OnPointerExited(object sender, PointerEventArgs e)
    {
        Console.WriteLine($"OnPointerExited - {e.Pointer.Id}");
        if (!TryGetPlayerFromPointerEvenArgs(e, out var player, out _))
        {
            return;
        }
        
        if (_playerSelectionComplete)
        {
            return;
        }

        Stop();
        player?.Shrink();
    }


    private void Init()
    {
        _reset = true;
        _easing = 0;
        _previousEasing = 0;
        _t2 = 0;
        _playerSelectionComplete = false;
        _text = "Tap And Hold";

        if (_playerSelection >= 0)
        {
            _players[_playerSelection].Shrink();
        }

        _playerSelection = -1;

        _textStopwatch.Restart();

        RestartStopWatch();
        StartTimer();

        ShuffleColors();
    }

    private void ShuffleColors()
    {
        for (var i = 0; i < 5; i++)
        {
            ShuffleInternal(_baseColors, 0);
        }
    }

    private void ShuffleInternal<T>(T[] sequence, int take)
    {
        for (var i = sequence.Length - 1; i > take; i--)
        {
            var swapIndex = _rnd.Next(0, i);
            if (swapIndex == i)
            {
                continue;
            }

            (sequence[i], sequence[swapIndex]) = (sequence[swapIndex], sequence[i]);
        }
    }

    private void RestartStopWatch()
    {
        _stopwatch.Restart();
        _endStopwatch.Restart();
        _t2 = _endStopwatch.Elapsed.TotalMilliseconds % 1000 / 1000;
    }

    private void StartTimer()
    {
        if (_isRunning)
        {
            return;
        }

        _isRunning = true;
        _timer.Interval = 10;
        _timer.Elapsed += (_,_) =>
        {
            if (_endStopwatch.IsRunning)
            {
                _t2 = _endStopwatch.Elapsed.TotalMilliseconds % 1000 / 1000;
            }

            _textT = _textStopwatch.Elapsed.TotalMilliseconds % _textCycleTime / _textCycleTime;

            InvalidateSurface();

        };
        _timer.AutoReset = true;
        _timer.Enabled = true;
        _timer.Start();
    }

    private void OnPointerReleased(object sender, PointerReleasedEventArgs e)
    {
        Console.WriteLine($"OnPointerReleased - {e.Pointer.Id}");
        if (!TryGetPlayerFromPointerEvenArgs(e, out var player, out var id))
        {
            return;
        }
        
        if (_playerSelectionComplete)
        {
            return;
        }

        Stop();
        player?.Shrink();
        //var color = _playerColors[id];
        //_colorPool.ReturnColor(color);

    }

    private void OnPointerMoved(object sender, PointerEventArgs e)
    {
        if (!TryGetPlayerFromPointerEvenArgs(e, out var player, out _))
        {
            return;
        }
        
        if (player != null)
        {
            player.Center = ConvertToPixel(e.GetPosition(this));
        }
    }


    private bool TryGetPlayerFromPointerEvenArgs(PointerEventArgs e, out Player player, out int id)
    {
        player = null;
        id = e.Pointer.Id ;

        if (_playerSelectionComplete)
        {
            return false;
        }

        player = _players.GetValueOrDefault(id);
        return true;
    }

    private void OnPointerPressed(object sender, PointerPressedEventArgs e)
    {
        Console.WriteLine($"OnPointerPressed - {e.Pointer.Id}");
        
        // if (!TryGetPlayerFromPointerEvenArgs(e, out var player, out var id))
        // {
        //     return;
        // }
        
        if (_text == "Tap And Hold")
        {
            _text = "One More Finger";
        }
        else if (_text == "One More Finger" && _players.Count > 1)
        {
            _text = "Wait";
        }
         

        if (_playerSelectionComplete)
        {
            return;
        }
        
        Stop();
        // if (player is null)
        // {
            var color = _colorPool.BorrowColor();
            Console.WriteLine(color.Value);
            var b = new Player(ConvertToPixel(e.GetPosition(this)), color.Value, e.Pointer.Id, _screenDensity);
            b.Expand();
            b.Gone += B_Gone;
            b.Ready += B_Ready;
            b.SelectionFinished += B_SelectionFinished;
            _players[e.Pointer.Id] = b;
            
        //}
        // else
        // {
        //     player.Center = ConvertToPixel(e.GetPosition(this));
        //     player.Expand();
        // }
    }

    private void ClearBox(int id)
    {
        var index = id;
        if (_players[index] != null)
        {
            // for (var i = 0; i < _colors.Length; i++)
            // {
            //     if (_players[index].Color == _colors[i].Color)
            //     {
            //         _colors[i].Available = true;
            //         break;
            //     }
            // }
            _colorPool.ReturnColor(_players[id].Color);

            _players[id].Shrink();
            _players[id].Ready -= B_Ready;
            _players[id].SelectionFinished -= B_SelectionFinished;
            _players[id].Gone -= B_Gone;
            _players[id].Dispose();
            _players[id] = null;
            _players.TryRemove(id, out _);
        }
    }

    private void B_Gone(object sender, EventArgs e)
    {
        _reset = true;
        if (sender is Player p)
        {
            var id = p.Id;
            ClearBox(id);
        }

        switch (_players.Count(b => b.Value != null))
        {
            case 0:
                _text = "Tap And Hold";
                break;

            case 1:
                _text = "One More Finger";
                break;

            default:
                _text = "Wait";
                break;
        }

        if (_players.Values.All(m => m is null))
        {
            _text = "Tap And Hold";
            _playerSelection = -1;
            ShuffleColors();
        }
    }

    private void B_Ready(object sender, EventArgs e)
    {
        var boxesCount = 0;
        var readyCount = 0;
        foreach (var player in _players.Values)
        {
            boxesCount++;

            if (player.IsReady)
            {
                readyCount++;
            }
        }
        
        // for (var i = 0; i < _players.Count; i++)
        // {
        //     if (_players[i] != null)
        //     {
        //         boxesCount++;
        //
        //         if (_players[i].IsReady)
        //         {
        //             readyCount++;
        //         }
        //     }
        // }

        if (readyCount >= 2 && boxesCount == readyCount)
        {
            Start();
        }
    }

    private void Start()
    {
        _playerSelection = -1;

        foreach (var player in _players.Values)
        {
            player.StartSelection();
        }
        
        // for (var i = 0; i < _players.Count; i++)
        // {
        //     var index = i;
        //     if (_players.TryGetValue(index, out var player))
        //     {
        //         player?.StartSelection();
        //     }
        // }
    }

    private void B_SelectionFinished(object sender, EventArgs e)
    {
        lock (_o)
        {
            if (_playerSelection >= 0) return;
            
            var ids = _players.Where(m => m.Value != null).Select(m => m.Key).ToArray();
            var boxCount = ids.Length;
            var raid = _rnd.Next(boxCount);
            
            _playerSelection = ids[raid];

            foreach (var player in _players)
            {
                if (player.Key != _playerSelection)
                {
                    ClearBox(player.Key);
                }
            }
            // for (var i = 0; i < _players.Count; i++)
            // {
            //     if (i != ids[raid])
            //     {
            //         ClearBox(i);
            //     }
            // }

            _players[_playerSelection].StopSelection();
            _playerSelectionComplete = true;
            _selectionCompleteStarted = true;
            RestartStopWatch();
        }
    }

    private void Stop()
    {
        foreach (var player in _players.Values)
        {
            player?.StopSelection();
        }
        // for (var i = 0; i < _players.Count; i++)
        // {
        //     var index = i;
        //     if (_players.TryGetValue(index, out var player))
        //     {
        //         player?.StopSelection();
        //     }
        // }
    }

    private SKPoint ConvertToPixel(Point pt)
    {
        var bounds = Bounds;
        return new SKPoint((float) (CanvasSize.Width * pt.X / bounds.Width * _screenDensity),
            (float) (CanvasSize.Height * pt.Y / bounds.Height * _screenDensity));
    }

    private static SKTypeface GetTypeface(string fullFontName)
    {
        var assembly = typeof(SkPlayersView).Assembly;
        var stream = assembly.GetManifestResourceStream("Primus.Font." + fullFontName);
        if (stream == null)
            return null;

        return SKTypeface.FromStream(stream);
    }

    private void DrawText(SKCanvas canvas)
    {
        if (string.IsNullOrWhiteSpace(_text))
        {
            return;
        }

        var measure = _textPaint.MeasureText(_text);

        _textPath.Reset();

        var x = (CanvasSize.Width * _screenDensity / 2f) - (measure / 2f);

        var step = measure / 7f;

        var start = Math.PI * _textT;
        for (var i = 0; i < 20; i++)
        {
            var v = start + (9 * i);

            var yy = (130f * _screenDensity) + (8 * (float) Math.Pow(Math.Sin(v), 2));

            var pp = new SKPoint((float) x, yy);
            if (i == 0)
            {
                _textPath.MoveTo(pp);
            }
            else
            {
                _textPath.LineTo(pp);
            }

            x += step;
        }

        _textPaint.Color = _textPaint.Color.WithAlpha((byte) (Math.Pow(Math.Sin(Math.PI * _textT), 2) * 255));
        canvas.DrawTextOnPath(_text, _textPath, 0, 8f * _screenDensity, _textPaint);
    }

    private void DrawBlackCircle(SKCanvas canvas, Player player, float r)
    {
        if (player != null)
        {
            var x = player.Center.X;
            var y = player.Center.Y;
            canvas.DrawCircle(x, y, r, _blackPaint);
        }
    }

    protected override void OnPaintSurface(SKPaintSurfaceEventArgs args)
    {
        base.OnPaintSurface(args);
        var canvas = args.Surface.Canvas;
        canvas.Clear(SKColors.Transparent);
        DrawText(canvas);

        if (_reset)
        {
            _reset = false;
        }

        if (_selectionCompleteStarted)
        {
            RestartStopWatch();
            _selectionCompleteStarted = false;
        }

        if (_playerSelectionComplete)
        {
            _text = "";
            var b = _players[_playerSelection];
            var c = b.Color;

            canvas.Clear(c);

            _previousEasing = _easing;
            _easing = Easings.CircularEaseInOut(_t2);

            if (_easing >= _previousEasing)
            {
                var r = CanvasSize.Height - (CanvasSize.Height * (float) _easing) + _endBlackCircleRadius;

                DrawBlackCircle(canvas, b, (float) r);
            }
            else
            {
                _stopwatch.Stop();
                _endStopwatch.Stop();
                DrawBlackCircle(canvas, b, _endBlackCircleRadius);

                _t2 = 1;
                Task.Run(async () =>
                {
                    await Task.Delay(2000).ConfigureAwait(false);
                    Dispatcher.UIThread.InvokeAsync(Init);
                });
            }
        }

        foreach (var player in _players.Values)
        {
            player?.Paint(args.Surface.Canvas);
        }
        
    }
}