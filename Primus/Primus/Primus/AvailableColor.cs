using SkiaSharp;

namespace Primus.Models
{
    public struct AvailableColor
    {
        public SKColor Color { get; }
        public bool Available { get; set; }

        public AvailableColor(SKColor color)
        {
            Color = color;
            Available = true;
        }
    }
}
