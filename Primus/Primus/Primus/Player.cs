using System;
using System.Diagnostics;
using SkiaSharp;

namespace Primus
{
    public sealed class Player : IDisposable
    {
        private static readonly Random s_rnd = new();

        private readonly float _actualRadius;

        private readonly float _arcRandomStart;

        private readonly float _baseRadius;

        private readonly int[] _cycles = { 500, 500, 650, 1000, 1600 };

        private readonly float _density;

        private readonly float _margin;

        private readonly float _marginRatio = 4;

        private readonly float _size = 115;

        private readonly Stopwatch _stopwatch = Stopwatch.StartNew();

        private SKPaint _arcPaint;

        private SKPath _arcPath = new();

        private float _arcRadius;

        private float _arcStart;

        private int _cycleIndex;

        private int _cycleTime;

        private bool _direction;

        private float _easing;

        private TimeSpan _elapsed = new(0);

        private bool _isDisposed;

        private SKPaint _overArcPaint;

        private SKPaint _paint;

        private double _previousEasing = -1;

        private float _radius;

        private double _time;

        public event EventHandler Gone;

        public event EventHandler Ready;

        public event EventHandler SelectionFinished;

        public SKPoint Center { get; set; }
        public SKColor Color { get; }
        public int Id { get; }
        public bool IsReady { get; private set; }

        public Player(SKPoint center, SKColor sKColor, int id, float screenDensity)
        {
            Color = sKColor;
            Center = center;
            Id = id;
            _density = screenDensity;

            _paint = new SKPaint
            {
                Style = SKPaintStyle.Fill,
                StrokeWidth = 0,
                StrokeCap = SKStrokeCap.Round,
                StrokeJoin = SKStrokeJoin.Round,
                IsAntialias = true,
                Color = Color
            };

            _arcPaint = new SKPaint
            {
                Style = SKPaintStyle.Stroke,
                StrokeWidth = 10 * _density,
                Color = Color,
                IsAntialias = true
            };

            _overArcPaint = new SKPaint
            {
                Style = SKPaintStyle.Stroke,
                StrokeWidth = 10 * _density,
                Color = SKColors.Black.WithAlpha(160),
                IsAntialias = true
            };

            SetCycle(1);

            _size *= _density;
            _arcRandomStart = s_rnd.Next(360) * -1;
            _margin = _size / _marginRatio;
            _baseRadius = _size / 2f;
            _actualRadius = _baseRadius - (_size / _marginRatio);
            _direction = true;
        }

        public void Dispose()
        {
            Dispose(true);
        }

        internal void Expand()
        {
            ResetValue();
            SetCycle(1);
            _direction = true;
            var restartTime = Easings.RevCubicEaseInOut(_radius / _actualRadius) * _cycleTime;
            _elapsed = TimeSpan.FromMilliseconds(restartTime);
            _stopwatch.Restart();
        }

        internal void Paint(SKCanvas canvas)
        {
            _time = _stopwatch.Elapsed.Add(_elapsed).TotalMilliseconds % _cycleTime / _cycleTime;
            _easing = (float)Easings.QuadraticEaseInOut(_time);
            NextStep(_easing);
            if (_direction)
            {
                DrawExpansion(canvas);
            }
            else
            {
                DrawShrink(canvas);
            }
        }

        internal void Shrink()
        {
            ResetValue();
            SetCycle(0);
            _direction = false;
            _stopwatch.Restart();
        }

        internal void StartSelection()
        {
            ResetValue();
            SetCycle(4);
            _stopwatch.Restart();
        }

        internal void StopSelection()
        {
            IsReady = false;
            if (_cycleIndex == 4)
            {
                SetCycle(3);
                _stopwatch.Restart();
            }
        }

        private void Dispose(bool disposing)
        {
            if (!_isDisposed)
            {
                if (disposing)
                {
                    _stopwatch.Stop();
                    _paint?.Dispose();
                    _paint = null;
                    _arcPaint?.Dispose();
                    _arcPaint = null;
                    _overArcPaint?.Dispose();
                    _overArcPaint = null;
                    _arcPath?.Dispose();
                    _arcPath = null;
                }

                _isDisposed = true;
            }
        }

        private void DrawArc(SKCanvas canvas, SKPaint arcPaint, float arcStart, float arcRadius)
        {
            if (_isDisposed)
            {
                return;
            }

            if (arcStart == 0 && arcRadius == 0)
            {
                return;
            }

            _arcPath.Reset();
            _arcPath.AddArc(GetBounds(), arcStart, arcRadius);
            canvas.DrawPath(_arcPath, arcPaint);
        }

        private void DrawCircle(SKCanvas canvas)
        {
            if (_isDisposed)
            {
                return;
            }

            canvas.DrawCircle(Center.X, Center.Y, _radius, _paint);
        }

        private void DrawExpansion(SKCanvas canvas)
        {
            if (_isDisposed)
            {
                return;
            }

            var drawOverArc = false;
            switch (_cycleIndex)
            {
                case 1:
                    _arcStart = 0;
                    _arcRadius = 0;
                    _radius = _actualRadius * _easing;
                    break;

                case 2:
                    _arcStart = _arcRandomStart + (90 * _easing);
                    _arcRadius = _easing * 360f;
                    _radius = _actualRadius + GetCircleSizeDiff();
                    break;

                case 3:
                    OnReady();
                    _arcStart = 0;
                    _arcRadius = 360;
                    _radius = _actualRadius + GetCircleSizeDiff();
                    break;

                case 4:
                    _radius = _actualRadius + GetCircleSizeDiff();
                    _arcStart = 0;
                    _arcRadius = 360;
                    drawOverArc = true;
                    break;
            }

            DrawCircle(canvas);
            DrawArc(canvas, _arcPaint, _arcStart, _arcRadius);
            if (drawOverArc)
            {
                DrawArc(canvas, _overArcPaint, _arcRandomStart + (_arcRandomStart * _easing), _easing * 360f);
            }
        }

        private void DrawShrink(SKCanvas canvas)
        {
            if (_isDisposed)
            {
                return;
            }

            _radius -= _radius * _easing;
            _arcStart -= _arcStart * _easing;
            _arcRadius -= _arcRadius * _easing;

            DrawCircle(canvas);
            DrawArc(canvas, _arcPaint, _arcStart, _arcRadius);
        }

        private SKRect GetBounds()
        {
            return new SKRect(Center.X - _baseRadius, Center.Y - _baseRadius, Center.X + _baseRadius, Center.Y + _baseRadius);
        }

        private float GetCircleSizeDiff()
        {
            return (float)Math.Sin(Math.PI * _time) * _margin / 2f;
        }

        private void Next()
        {
            _previousEasing = -1;
            if (_cycleIndex < _cycles.Length)
            {
                SetCycle(_cycleIndex + 1);
            }

            _stopwatch.Restart();
        }

        private void NextStep(double e)
        {
            if (!_direction)
            {
                if (e < _previousEasing && _cycleIndex == 0)
                {
                    OnGone();
                }
                else
                {
                    _previousEasing = e;
                }
            }
            else
            {
                if (e < _previousEasing && _cycleIndex < 3)
                {
                    Next();
                }
                else if (e < _previousEasing && _cycleIndex == 3)
                {
                    OnReady();
                }
                else if (e < _previousEasing && _cycleIndex == 4)
                {
                    OnSelectionFinished();
                }
                else
                {
                    _previousEasing = e;
                }
            }
        }

        private void OnGone()
        {
            _stopwatch.Stop();
            _time = 0;
            Gone?.Invoke(this, null);
        }

        private void OnReady()
        {
            IsReady = true;
            Ready?.Invoke(this, null);
        }

        private void OnSelectionFinished()
        {
            SelectionFinished?.Invoke(this, null);
        }

        private void ResetValue()
        {
            _stopwatch.Reset();
            _previousEasing = -1;
            _easing = 0;
            _elapsed = new TimeSpan(0);
        }

        private void SetCycle(int cycleIndex)
        {
            _cycleIndex = cycleIndex;
            _cycleTime = _cycles[_cycleIndex];
        }
    }
}
