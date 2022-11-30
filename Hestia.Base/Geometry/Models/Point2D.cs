using System;
using System.Text.Json.Serialization;
using Hestia.Base.Utilities;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace Hestia.Base.Geometry.Models
{
    public readonly struct Point2D : IEquatable<Point2D>
    {
        public double X { get; }
        public double Y { get; }

        [JsonIgnore]
        public double Magnitude { get; }

        [JsonIgnore]
        public double MagnitudeSquared { get; }

        [JsonConstructor]
        public Point2D(double x, double y)
        {
            X = x;
            Y = y;
            MagnitudeSquared = (X * X) + (Y * Y);
            Magnitude = Math.Sqrt(MagnitudeSquared);
        }

        public double AngleBetweenPointsInRadians(Point2D point)
        {
            return Magnitude != 0d && point.Magnitude != 0
                ? Math.Acos(Dot(point) / (Magnitude * point.Magnitude))
                : 0d;
        }

        public double AngleBetweenPointsInDegrees(Point2D point)
        {
            return AngleBetweenPointsInRadians(point) * MathEnhanced.RAD_2_DEG;
        }

        public double Dot(Point2D point)
        {
            return (X * point.X) + (Y * point.Y);
        }

        public double Cross(Point2D point)
        {
            // The Cross Product for a vector in 2 dimensions is a scalar
            return (X * point.Y) - (Y * point.X);
        }

        public double Distance(Point2D point)
        {
            var deltaX = point.X - X;
            var deltaY = point.Y - Y;

            return Math.Sqrt((deltaX * deltaX) + (deltaY * deltaY));
        }

        public bool ApproximatelyEquals(Point2D other, double approximationThreshold)
        {
            var deltaX = Math.Abs(X - other.X);
            var deltaY = Math.Abs(Y - other.Y);

            approximationThreshold = Math.Max(0, approximationThreshold);

            return deltaX <= approximationThreshold && deltaY <= approximationThreshold;
        }

        public bool Equals(Point2D other)
        {
            return X == other.X && Y == other.Y;
        }

        public override bool Equals(object? obj)
        {
            return obj is Point2D d && Equals(d);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(X, Y);
        }

        public static bool operator ==(Point2D left, Point2D right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Point2D left, Point2D right)
        {
            return !(left == right);
        }

        public static Point2D operator +(Point2D left, Point2D right)
        {
            return new Point2D(left.X + right.X, left.Y + right.Y);
        }

        public static Point2D operator -(Point2D left, Point2D right)
        {
            return new Point2D(left.X - right.X, left.Y - right.Y);
        }

        public static Point2D operator *(Point2D point, double scalar)
        {
            return new Point2D(point.X * scalar, point.Y * scalar);
        }
    }
}
