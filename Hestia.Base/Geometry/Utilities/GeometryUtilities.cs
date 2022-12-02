using System;
using Hestia.Base.Geometry.Models;

namespace Hestia.Base.Geometry.Utilities
{
    /// <summary>
    /// Defines general geometric utility methods
    /// </summary>
    public static class GeometryUtilities
    {
        /// <summary>
        /// Returns the bounds the given set of points fits within
        /// </summary>
        /// <param name="points">Set of points</param>
        /// <returns>Bounds points fit within</returns>
        public static Rectangle2D GetBoundsFromPoints(Point2D[] points)
        {
            if ((points?.Length ?? 0) == 0)
            {
                return new Rectangle2D();
            }

            var firstPoint = points![0];
            var xMin = firstPoint.X;
            var xMax = firstPoint.X;
            var yMin = firstPoint.Y;
            var yMax = firstPoint.Y;

            foreach (var point in points!)
            {
                xMin = Math.Min(xMin, point.X);
                xMax = Math.Max(xMax, point.X);
                yMin = Math.Min(yMin, point.Y);
                yMax = Math.Max(yMax, point.Y);
            }

            var topLeft = new Point2D(xMin, yMax);
            var bottomRight = new Point2D(xMax, yMin);

            return new Rectangle2D(topLeft, bottomRight);
        }
    }
}
