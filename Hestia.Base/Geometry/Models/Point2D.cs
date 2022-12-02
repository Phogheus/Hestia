using System;
using System.Text.Json.Serialization;

namespace Hestia.Base.Geometry.Models
{
    /// <summary>
    /// Defines a two-dimensional point
    /// </summary>
    public readonly struct Point2D : IEquatable<Point2D>
    {
        #region Properties

        /// <summary>
        /// Returns a point at origin (0, 0)
        /// </summary>
        public static Point2D Zero { get; } = new Point2D();

        /// <summary>
        /// X position
        /// </summary>
        public double X { get; }

        /// <summary>
        /// Y position
        /// </summary>
        public double Y { get; }

        /// <summary>
        /// Magnitude
        /// </summary>
        [JsonIgnore]
        public double Magnitude { get; }

        /// <summary>
        /// Magnitude squared
        /// </summary>
        [JsonIgnore]
        public double MagnitudeSquared { get; }

        #endregion Properties

        #region Constructors

        /// <summary>
        /// Constructor taking x and y position
        /// </summary>
        /// <param name="x">X position</param>
        /// <param name="y">Y position</param>
        [JsonConstructor]
        public Point2D(double x, double y)
        {
            X = x;
            Y = y;
            MagnitudeSquared = (X * X) + (Y * Y);
            Magnitude = Math.Sqrt(MagnitudeSquared);
        }

        #endregion Constructors

        #region Public Methods

        /// <summary>
        /// Returns the dot product between this and a given point
        /// </summary>
        /// <param name="point">Other point</param>
        /// <returns>Dot product</returns>
        public double Dot(Point2D point)
        {
            return (X * point.X) + (Y * point.Y);
        }

        /// <summary>
        /// Returns the cross product between this and a given point
        /// </summary>
        /// <param name="point">Other point</param>
        /// <returns>Cross product</returns>
        public double Cross(Point2D point)
        {
            // The Cross Product for a vector in 2 dimensions is a scalar
            return (X * point.Y) - (Y * point.X);
        }

        /// <summary>
        /// Returns the distance between this and a given point
        /// </summary>
        /// <param name="point">Other point</param>
        /// <returns>Distance</returns>
        public double Distance(Point2D point)
        {
            return Math.Sqrt(DistanceSquared(point));
        }

        /// <summary>
        /// Returns the distance squared between this and a given point
        /// </summary>
        /// <param name="point">Other point</param>
        /// <returns>Distance squared</returns>
        public double DistanceSquared(Point2D point)
        {
            var deltaX = point.X - X;
            var deltaY = point.Y - Y;

            return (deltaX * deltaX) + (deltaY * deltaY);
        }

        /// <summary>
        /// Returns this point as a normalized value based on <see cref="Magnitude"/>
        /// </summary>
        /// <returns>Normalized point</returns>
        public Point2D Normalized()
        {
            return this * (1d / Magnitude);
        }

        /// <summary>
        /// Returns true if this and a given point are approximately equal
        /// </summary>
        /// <param name="other">Other point</param>
        /// <param name="approximationThreshold">Approximation threshold</param>
        /// <returns>True if points are approximately equal</returns>
        public bool ApproximatelyEquals(Point2D other, double approximationThreshold)
        {
            approximationThreshold = Math.Max(0, approximationThreshold);

            return Math.Abs(X - other.X) <= approximationThreshold &&
                   Math.Abs(Y - other.Y) <= approximationThreshold;
        }

        /// <summary>
        /// Returns true if this and the given instances are considered equal
        /// </summary>
        /// <param name="other">Instance to compare</param>
        /// <returns>True if instances are equal</returns>
        public bool Equals(Point2D other)
        {
            return X == other.X && Y == other.Y;
        }

        /// <summary>
        /// Returns true if this and the given instances are considered equal
        /// </summary>
        /// <param name="obj">Instance to compare</param>
        /// <returns>True if instances are equal</returns>
        public override bool Equals(object? obj)
        {
            return obj is Point2D point && Equals(point);
        }

        /// <summary>
        /// Returns the hash code for this instance
        /// </summary>
        /// <returns>Hash code</returns>
        public override int GetHashCode()
        {
            return X.GetHashCode() ^ Y.GetHashCode();
        }

        /// <summary>
        /// Returns a string representation of this object
        /// </summary>
        public override string ToString()
        {
            return $"({X}, {Y})";
        }

        /// <summary>
        /// Returns true if the two given values equate
        /// </summary>
        /// <param name="left">Left value</param>
        /// <param name="right">Right value</param>
        /// <returns>True if the two given values equate</returns>
        public static bool operator ==(Point2D left, Point2D right)
        {
            return left.Equals(right);
        }

        /// <summary>
        /// Returns true if the two given values do not equate
        /// </summary>
        /// <param name="left">Left value</param>
        /// <param name="right">Right value</param>
        /// <returns>True if the two given values do not equate</returns>
        public static bool operator !=(Point2D left, Point2D right)
        {
            return !(left == right);
        }

        /// <summary>
        /// Returns the result of adding the given points together
        /// </summary>
        /// <param name="left">Left value</param>
        /// <param name="right">Right value</param>
        /// <returns>Result of addition</returns>
        public static Point2D operator +(Point2D left, Point2D right)
        {
            return new Point2D(left.X + right.X, left.Y + right.Y);
        }

        /// <summary>
        /// Returns the result of subtracting <paramref name="right"/> from <paramref name="left"/>
        /// </summary>
        /// <param name="left">Left value</param>
        /// <param name="right">Right value</param>
        /// <returns>Result of subtraction</returns>
        public static Point2D operator -(Point2D left, Point2D right)
        {
            return new Point2D(left.X - right.X, left.Y - right.Y);
        }

        /// <summary>
        /// Returns the result of multiplying the given point by a scalar
        /// </summary>
        /// <param name="point">Left value</param>
        /// <param name="scalar">Scalar</param>
        /// <returns>Result of multiplication</returns>
        public static Point2D operator *(Point2D point, double scalar)
        {
            return new Point2D(point.X * scalar, point.Y * scalar);
        }

        #endregion Public Methods
    }
}
