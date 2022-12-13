using System;
using System.Text.Json.Serialization;
using Hestia.Base.Utilities;

namespace Hestia.Base.Geometry.Models
{
    /// <summary>
    /// Defines a two-dimensional point
    /// </summary>
    public sealed class Point2D : IEquatable<Point2D>
    {
        #region Fields

        private double _x;
        private double _y;
        private double? _mag;
        private double? _magSqrd;

        #endregion Fields

        #region Properties

        /// <summary>
        /// Returns a point at origin (0, 0)
        /// </summary>
        public static Point2D Zero => new Point2D();

        /// <summary>
        /// Returns a point located towards Y "Up" (0, 1)
        /// </summary>
        public static Point2D Up => new Point2D(0, 1);

        /// <summary>
        /// Returns a point located towards Y "Down" (0, -1)
        /// </summary>
        public static Point2D Down => new Point2D(0, -1);

        /// <summary>
        /// Returns a point located towards X "Right" (1, 0)
        /// </summary>
        public static Point2D Right => new Point2D(1, 0);

        /// <summary>
        /// Returns a point located towards X "Left" (-1, 0)
        /// </summary>
        public static Point2D Left => new Point2D(-1, 0);

        /// <summary>
        /// X position
        /// </summary>
        public double X
        {
            get => _x;
            set
            {
                if (_x != value)
                {
                    MarkDirty();
                }

                _x = value;
            }
        }

        /// <summary>
        /// Y position
        /// </summary>
        public double Y
        {
            get => _y;
            set
            {
                if (_y != value)
                {
                    MarkDirty();
                }

                _y = value;
            }
        }

        /// <summary>
        /// Magnitude
        /// </summary>
        [JsonIgnore]
        public double Magnitude => _mag ??= Math.Sqrt(MagnitudeSquared);

        /// <summary>
        /// Magnitude squared
        /// </summary>
        [JsonIgnore]
        public double MagnitudeSquared => _magSqrd ??= (_x * _x) + (_y * _y);

        internal bool IsDirty { get; set; }

        #endregion Properties

        #region Constructors

        /// <summary>
        /// Default constructor creating a point at (0, 0)
        /// </summary>
        public Point2D()
            : this(0, 0)
        {
        }

        /// <summary>
        /// Constructor taking x and y position
        /// </summary>
        /// <param name="x">X position</param>
        /// <param name="y">Y position</param>
        [JsonConstructor]
        public Point2D(double x, double y)
        {
            _x = x;
            _y = y;
        }

        #endregion Constructors

        #region Public Methods

        /// <summary>
        /// Returns the dot product between this and a given point
        /// </summary>
        /// <param name="point">Other point</param>
        /// <returns>Dot product</returns>
        public double Dot(Point2D? point)
        {
            return point != null
                ? (_x * point._x) + (_y * point._y)
                : 0d;
        }

        /// <summary>
        /// Returns the cross product between this and a given point
        /// </summary>
        /// <param name="point">Other point</param>
        /// <returns>Cross product</returns>
        public double Cross(Point2D? point)
        {
            // The Cross Product for a vector in 2 dimensions is a scalar
            return point != null
                ? (_x * point._y) - (_y * point._x)
                : 0d;
        }

        /// <summary>
        /// Returns the distance between this and a given point
        /// </summary>
        /// <param name="point">Other point</param>
        /// <returns>Distance</returns>
        public double Distance(Point2D? point)
        {
            return Math.Sqrt(DistanceSquared(point));
        }

        /// <summary>
        /// Returns the distance squared between this and a given point
        /// </summary>
        /// <param name="point">Other point</param>
        /// <returns>Distance squared</returns>
        public double DistanceSquared(Point2D? point)
        {
            if (point == null)
            {
                return 0d;
            }

            var deltaX = point._x - _x;
            var deltaY = point._y - _y;

            return (deltaX * deltaX) + (deltaY * deltaY);
        }

        /// <summary>
        /// Returns the angle between this and a given point in radians
        /// </summary>
        /// <param name="other">Other point</param>
        /// <remarks>
        /// If <paramref name="other"/> is null, this will return 0.
        /// 
        /// This returns an angle orientation system of:
        /// <br/>- origin -> right = 0 degrees
        /// <br/>- origin -> up = 90 degrees
        /// <br/>- origin -> left = +/-180 degrees
        /// </remarks>
        /// <returns>Degree in radians between the two points</returns>
        public double AngleInRadians(Point2D? other)
        {
            if (other is null)
            {
                return 0d;
            }

            var delta = other - this;
            return Math.Atan2(delta._y, delta._x);
        }

        /// <summary>
        /// Returns the angle between this and a given point in degrees (0 to +/-180)
        /// </summary>
        /// <param name="other">Other point</param>
        /// <remarks>
        /// If <paramref name="other"/> is null, this will return 0.
        /// 
        /// This returns an angle orientation system of:
        /// <br/>- origin -> right = 0 degrees
        /// <br/>- origin -> up = 90 degrees
        /// <br/>- origin -> left = +/-180 degrees
        /// </remarks>
        /// <returns>Degree between 0 and +/-180</returns>
        public double AngleInDegrees(Point2D? other)
        {
            return AngleInRadians(other) * MathEnhanced.RAD_2_DEG;
        }

        /// <summary>
        /// Returns the angle between this and a given point in degrees (0 to 360)
        /// </summary>
        /// <param name="other">Other point</param>
        /// <remarks>
        /// If <paramref name="other"/> is null, this will return 0.
        /// 
        /// This returns an angle orientation system of:
        /// <br/>- origin -> right = 0 degrees
        /// <br/>- origin -> up = 90 degrees
        /// <br/>- origin -> left = 360 degrees
        /// </remarks>
        /// <returns>Degree between 0 and 360</returns>
        public double AngleInDegrees360(Point2D? other)
        {
            var degrees = AngleInDegrees(other);

            if (degrees < 0)
            {
                degrees = 180d + (degrees + 180d);
            }

            return degrees;
        }

        /// <summary>
        /// Returns this point as a normalized value based on <see cref="Magnitude"/>
        /// </summary>
        /// <remarks>If <see cref="Magnitude"/> is zero, this returns <see cref="Zero"/></remarks>
        /// <returns>Normalized point</returns>
        public Point2D Normalized()
        {
            if (Magnitude == 0)
            {
                return Zero;
            }

            var inverseMag = 1d / Magnitude;
            var x = Math.Clamp(_x * inverseMag, 0d, 1d);
            var y = Math.Clamp(_y * inverseMag, 0d, 1d);

            return new Point2D(x, y);
        }

        /// <summary>
        /// Returns true if this and the given point no more than <paramref name="approximationThreshold"/> units apart
        /// </summary>
        /// <param name="other">Other point</param>
        /// <param name="approximationThreshold">Approximation threshold</param>
        /// <returns>True if points are approximately equal</returns>
        public bool ApproximatelyEquals(Point2D? other, double approximationThreshold)
        {
            return other != null && Distance(other) <= Math.Max(0, approximationThreshold);
        }

        /// <summary>
        /// Returns true if this and the given instances are considered equal
        /// </summary>
        /// <param name="other">Instance to compare</param>
        /// <returns>True if instances are equal</returns>
        public bool Equals(Point2D? other)
        {
            return _x == other?._x && _y == other?._y;
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
            return _x.GetHashCode() ^ _y.GetHashCode();
        }

        /// <summary>
        /// Returns a string representation of this object
        /// </summary>
        public override string ToString()
        {
            return $"({_x}, {_y})";
        }

        /// <summary>
        /// Returns true if the two given values equate
        /// </summary>
        /// <param name="left">Left value</param>
        /// <param name="right">Right value</param>
        /// <returns>True if the two given values equate</returns>
        public static bool operator ==(Point2D? left, Point2D? right)
        {
            return (left is null && right is null) || (left?.Equals(right) ?? false);
        }

        /// <summary>
        /// Returns true if the two given values do not equate
        /// </summary>
        /// <param name="left">Left value</param>
        /// <param name="right">Right value</param>
        /// <returns>True if the two given values do not equate</returns>
        public static bool operator !=(Point2D? left, Point2D? right)
        {
            return !(left == right);
        }

        /// <summary>
        /// Returns the result of adding the given points together
        /// </summary>
        /// <param name="left">Left value</param>
        /// <param name="right">Right value</param>
        /// <returns>Result of addition</returns>
        public static Point2D operator +(Point2D? left, Point2D? right)
        {
            return left is not null && right is not null
                ? new Point2D(left._x + right._x, left._y + right._y)
                : left ?? right ?? Zero;
        }

        /// <summary>
        /// Returns the result of subtracting <paramref name="right"/> from <paramref name="left"/>
        /// </summary>
        /// <param name="left">Left value</param>
        /// <param name="right">Right value</param>
        /// <returns>Result of subtraction</returns>
        public static Point2D operator -(Point2D? left, Point2D? right)
        {
            return left is not null && right is not null
                ? new Point2D(left._x - right._x, left._y - right._y)
                : left ?? right ?? Zero;
        }

        /// <summary>
        /// Returns the result of multiplying the given point by a scalar
        /// </summary>
        /// <param name="point">Left value</param>
        /// <param name="scalar">Scalar</param>
        /// <returns>Result of multiplication</returns>
        public static Point2D operator *(Point2D? point, double scalar)
        {
            return point is not null
                ? new Point2D(point._x * scalar, point._y * scalar)
                : Zero;
        }

        #endregion Public Methods

        #region Private Methods

        private void MarkDirty()
        {
            IsDirty = true;
            _mag = _magSqrd = null;
        }

        #endregion Private Methods
    }
}
