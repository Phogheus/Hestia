using System;
using System.Text.Json.Serialization;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace Hestia.Base.Geometry.Models
{
    public readonly struct Circle2D
    {
        public double Radius { get; }
        public Point2D CenterPoint { get; }

        [JsonIgnore]
        public double Diameter { get; }

        [JsonIgnore]
        public double Circumference { get; }

        [JsonIgnore]
        public double Area { get; }

        public Circle2D(double radius)
            : this(radius, new Point2D())
        {
        }

        [JsonConstructor]
        public Circle2D(double radius, Point2D centerPoint)
        {
            Radius = radius;
            CenterPoint = centerPoint;
            Diameter = radius * 2d;
            Circumference = Math.PI * Diameter;
            Area = Math.PI * (radius * radius);
        }
    }
}
