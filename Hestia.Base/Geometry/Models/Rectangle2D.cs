using System;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace Hestia.Base.Geometry.Models
{
    public readonly struct Rectangle2D
    {
        public Point2D TopLeft { get; }
        public Point2D TopRight { get; }
        public Point2D BottomRight { get; }
        public Point2D BottomLeft { get; }

        public double Top { get; }
        public double Bottom { get; }
        public double Left { get; }
        public double Right { get; }
        public double Width { get; }
        public double Height { get; }

        public Rectangle2D(Point2D topLeft, Point2D bottomRight)
        {
            // Ensure points are in X-Right Y-Up format
            var xMin = Math.Min(topLeft.X, bottomRight.X);
            var xMax = Math.Max(topLeft.X, bottomRight.X);
            var yMin = Math.Min(topLeft.Y, bottomRight.Y);
            var yMax = Math.Max(topLeft.Y, bottomRight.Y);

            Top = yMax;
            Bottom = yMin;
            Left = xMin;
            Right = xMax;

            TopLeft = new Point2D(Left, Top);
            TopRight = new Point2D(Right, Top);
            BottomRight = new Point2D(Right, Bottom);
            BottomLeft = new Point2D(Left, Bottom);

            Width = Math.Abs(Right - Left);
            Height = Math.Abs(Top - Bottom);
        }

        public bool IsPointInsideRect(Point2D point)
        {
            return point.X >= Left && point.X <= Right &&
                   point.Y <= Top && point.Y >= Bottom;
        }
    }
}
