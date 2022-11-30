using System;
using System.Text.Json.Serialization;
using Hestia.Base.Geometry.Enums;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace Hestia.Base.Geometry.Models
{
    public readonly struct Triangle2D
    {
        public Point2D[] Points { get; }

        [JsonIgnore]
        public Line2D[] Edges { get; }

        [JsonIgnore]
        public double Area { get; }

        [JsonConstructor]
        public Triangle2D(Point2D[] points)
        {
            if ((points?.Length ?? 0) != 3)
            {
                throw new ArgumentOutOfRangeException(nameof(points), "A triangle must have no more and no less than 3 points.");
            }

            var testLine = new Line2D(points![0], points[1]);
            var orientation = testLine.GetOrientationOfPoint(points[2]);

            if (orientation == PointOrientationType.Colinear)
            {
                throw new InvalidOperationException("Triangle cannot be made of only colinear points");
            }

            // Ensure clockwise orientation
            Points = orientation == PointOrientationType.Clockwise
                ? points
                : (new Point2D[] { points[0], points[2], points[1] });

            // Set edges
            Edges = new Line2D[]
            {
                new Line2D(points[0], points[1]),
                new Line2D(points[1], points[2]),
                new Line2D(points[2], points[0])
            };

            // Heron's Formula for area
            var distA = Edges[0].Length;
            var distB = Edges[1].Length;
            var distC = Edges[2].Length;
            var s = (distA + distB + distC) / 2d;

            Area = Math.Sqrt(s * (s - distA) * (s - distB) * (s - distC));
        }

        public Point2D? GetCircumcenterPoint(out double radius)
        {
            var A = Points[0];
            var B = Points[1];
            var C = Points[2];

            var d = 2d * ((A.X * (B.Y - C.Y)) + (B.X * (C.Y - A.Y)) + (C.X * (A.Y - B.Y)));

            if (d == 0d)
            {
                radius = 0d;
                return null;
            }

            var x = ((A.MagnitudeSquared * (B.Y - C.Y)) + (B.MagnitudeSquared * (C.Y - A.Y)) + (C.MagnitudeSquared * (A.Y - B.Y))) / d;
            var y = ((A.MagnitudeSquared * (C.X - B.X)) + (B.MagnitudeSquared * (A.X - C.X)) + (C.MagnitudeSquared * (B.X - A.X))) / d;

            var circumcenter = new Point2D(x, y);
            radius = A.Distance(circumcenter);

            return circumcenter;
        }
    }
}
