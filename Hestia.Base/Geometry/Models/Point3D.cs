using System;
using System.Text.Json.Serialization;

namespace Hestia.Base.Geometry.Models
{
    /// <summary>
    /// Defines a three-dimensional point
    /// </summary>
    public readonly struct Point3D : IEquatable<Point3D>
    {
        #region Properties

        /// <summary>
        /// X position
        /// </summary>
        public double X { get; }

        /// <summary>
        /// Y position
        /// </summary>
        public double Y { get; }

        /// <summary>
        /// Z position
        /// </summary>
        public double Z { get; }

        /// <summary>
        /// Magnitude
        /// </summary>
        [JsonIgnore]
        public double Magnitude { get; }

        #endregion Properties

        #region Constructors

        /// <summary>
        /// Constructor taking x, y, and z position
        /// </summary>
        /// <param name="x">X position</param>
        /// <param name="y">Y position</param>
        /// <param name="z">Z position</param>
        [JsonConstructor]
        public Point3D(double x, double y, double z)
        {
            X = x;
            Y = y;
            Z = z;
            Magnitude = Math.Sqrt((X * X) + (Y * Y) + (Z * Z));
        }

        #endregion Constructors

        #region Public Methods

        /// <summary>
        /// Returns the dot product between this and a given point
        /// </summary>
        /// <param name="point">Other point</param>
        /// <returns>Dot product</returns>
        public double Dot(Point3D point)
        {
            return (X * point.X) + (Y * point.Y) + (Z * point.Z);
        }

        /// <summary>
        /// Returns the cross product between this and a given point
        /// </summary>
        /// <param name="point">Other point</param>
        /// <returns>Cross product</returns>
        public Point3D Cross(Point3D point)
        {
            // cX = ay * bz - az * by
            // cY = az * bx - ax * bz
            // cZ = ax * by - ay * bx

            var crossX = (Y * point.Z) - (Z * point.Y);
            var crossY = (Z * point.X) - (X * point.Z);
            var crossZ = (X * point.Y) - (Y * point.X);

            return new Point3D(crossX, crossY, crossZ);
        }

        /// <summary>
        /// Returns the distance between this and a given point
        /// </summary>
        /// <param name="point">Other point</param>
        /// <returns>Distance</returns>
        public double Distance(Point3D point)
        {
            return Math.Sqrt(DistanceSquared(point));
        }

        /// <summary>
        /// Returns the distance squared between this and a given point
        /// </summary>
        /// <param name="point">Other point</param>
        /// <returns>Distance squared</returns>
        public double DistanceSquared(Point3D point)
        {
            var deltaX = point.X - X;
            var deltaY = point.Y - Y;
            var deltaZ = point.Z - Z;

            return (deltaX * deltaX) + (deltaY * deltaY) + (deltaZ * deltaZ);
        }

        /// <summary>
        /// Returns true if this and the given instances are considered equal
        /// </summary>
        /// <param name="other">Instance to compare</param>
        /// <returns>True if instances are equal</returns>
        public bool Equals(Point3D other)
        {
            return X == other.X && Y == other.Y && Z == other.Z;
        }

        /// <summary>
        /// Returns true if this and the given instances are considered equal
        /// </summary>
        /// <param name="obj">Instance to compare</param>
        /// <returns>True if instances are equal</returns>
        public override bool Equals(object? obj)
        {
            return obj is Point3D point && Equals(point);
        }

        /// <summary>
        /// Returns the hash code for this instance
        /// </summary>
        /// <returns>Hash code</returns>
        public override int GetHashCode()
        {
            return X.GetHashCode() ^ Y.GetHashCode() ^ Z.GetHashCode();
        }

        /// <summary>
        /// Returns true if the two given values equate
        /// </summary>
        /// <param name="left">Left value</param>
        /// <param name="right">Right value</param>
        /// <returns>True if the two given values equate</returns>
        public static bool operator ==(Point3D left, Point3D right)
        {
            return left.Equals(right);
        }

        /// <summary>
        /// Returns true if the two given values do not equate
        /// </summary>
        /// <param name="left">Left value</param>
        /// <param name="right">Right value</param>
        /// <returns>True if the two given values do not equate</returns>
        public static bool operator !=(Point3D left, Point3D right)
        {
            return !(left == right);
        }

        /// <summary>
        /// Returns the result of adding the given points together
        /// </summary>
        /// <param name="left">Left value</param>
        /// <param name="right">Right value</param>
        /// <returns>Result of addition</returns>
        public static Point3D operator +(Point3D left, Point3D right)
        {
            return new Point3D(left.X + right.X, left.Y + right.Y, left.Z + right.Z);
        }

        /// <summary>
        /// Returns the result of subtracting <paramref name="right"/> from <paramref name="left"/>
        /// </summary>
        /// <param name="left">Left value</param>
        /// <param name="right">Right value</param>
        /// <returns>Result of subtraction</returns>
        public static Point3D operator -(Point3D left, Point3D right)
        {
            return new Point3D(left.X - right.X, left.Y - right.Y, left.Z - right.Z);
        }

        /// <summary>
        /// Returns the result of multiplying the given point by a scalar
        /// </summary>
        /// <param name="point">Left value</param>
        /// <param name="scalar">Scalar</param>
        /// <returns>Result of multiplication</returns>
        public static Point3D operator *(Point3D point, double scalar)
        {
            return new Point3D(point.X * scalar, point.Y * scalar, point.Z * scalar);
        }

        #endregion Public Methods
    }
}
