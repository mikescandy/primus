using System;
using System.Collections.Generic;
using System.Linq;
using SkiaSharp;

namespace Primus;

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