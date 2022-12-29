using System;
using System.Text.Json.Serialization;

#pragma warning disable CS0649 // Removes error for _onPointChanged being unused since nothing is using Point3D in this lib yet

namespace Hestia.Base.Geometry.Models
{
    /// <summary>
    /// Defines a three-dimensional point
    /// </summary>
    public sealed class Point3D : IEquatable<Point3D>
    {
        #region Fields

        internal delegate void OnChange();
        internal OnChange? _onPointChanged;

        private double _x;
        private double _y;
        private double _z;
        private double? _mag;
        private double? _magSqrd;

        #endregion Fields

        #region Properties

        /// <summary>
        /// Returns a point at origin (0, 0, 0)
        /// </summary>
        public static Point3D Zero => new Point3D();

        /// <summary>
        /// Returns a point located towards Y "Up" (0, 1)
        /// </summary>
        public static Point3D Up => new Point3D(0, 1, 0);

        /// <summary>
        /// Returns a point located towards Y "Down" (0, -1)
        /// </summary>
        public static Point3D Down => new Point3D(0, -1, 0);

        /// <summary>
        /// Returns a point located towards X "Right" (1, 0)
        /// </summary>
        public static Point3D Right => new Point3D(1, 0, 0);

        /// <summary>
        /// Returns a point located towards X "Left" (-1, 0)
        /// </summary>
        public static Point3D Left => new Point3D(-1, 0, 0);

        /// <summary>
        /// Returns a point located towards Z "Forward" (0, 1)
        /// </summary>
        public static Point3D Forward => new Point3D(0, 0, 1);

        /// <summary>
        /// Returns a point located towards Z "Backward" (0, -1)
        /// </summary>
        public static Point3D Backward => new Point3D(0, 0, -1);

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
                    _x = value;
                    ResetDerivedValues();
                }
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
                    _y = value;
                    ResetDerivedValues();
                }
            }
        }

        /// <summary>
        /// Z position
        /// </summary>
        public double Z
        {
            get => _z;
            set
            {
                if (_z != value)
                {
                    _z = value;
                    ResetDerivedValues();
                }
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
        public double MagnitudeSquared => _magSqrd ??= (_x * _x) + (_y * _y) + (_z * _z);

        #endregion Properties

        #region Constructors

        /// <summary>
        /// Default constructor creating a point at (0, 0)
        /// </summary>
        public Point3D()
            : this(0, 0, 0)
        {
        }

        /// <summary>
        /// Constructor taking x, y, and z position
        /// </summary>
        /// <param name="x">X position</param>
        /// <param name="y">Y position</param>
        /// <param name="z">Z position</param>
        [JsonConstructor]
        public Point3D(double x, double y, double z)
        {
            _x = x;
            _y = y;
            _z = z;
        }

        #endregion Constructors

        #region Public Methods

        /// <summary>
        /// Returns the dot product between this and a given point
        /// </summary>
        /// <param name="point">Other point</param>
        /// <returns>Dot product</returns>
        public double Dot(Point3D? point)
        {
            return point != null
                ? (_x * point._x) + (_y * point._y) + (_z * point._z)
                : 0d;
        }

        /// <summary>
        /// Returns the cross product between this and a given point
        /// </summary>
        /// <param name="point">Other point</param>
        /// <remarks>If <paramref name="point"/> is null, this will return <see cref="Zero"/></remarks>
        /// <returns>Cross product</returns>
        public Point3D Cross(Point3D? point)
        {
            if (point == null)
            {
                return Zero;
            }

            // cX = ay * bz - az * by
            // cY = az * bx - ax * bz
            // cZ = ax * by - ay * bx

            var crossX = (_y * point._z) - (_z * point._y);
            var crossY = (_z * point._x) - (_x * point._z);
            var crossZ = (_x * point._y) - (_y * point._x);

            return new Point3D(crossX, crossY, crossZ);
        }

        /// <summary>
        /// Returns the distance between this and a given point
        /// </summary>
        /// <param name="point">Other point</param>
        /// <returns>Distance</returns>
        public double Distance(Point3D? point)
        {
            return Math.Sqrt(DistanceSquared(point));
        }

        /// <summary>
        /// Returns the distance squared between this and a given point
        /// </summary>
        /// <param name="point">Other point</param>
        /// <returns>Distance squared</returns>
        public double DistanceSquared(Point3D? point)
        {
            if (point == null)
            {
                return 0d;
            }

            var deltaX = point._x - _x;
            var deltaY = point._y - _y;
            var deltaZ = point._z - _z;

            return (deltaX * deltaX) + (deltaY * deltaY) + (deltaZ * deltaZ);
        }

        /// <summary>
        /// Returns this point as a normalized value based on <see cref="Magnitude"/>
        /// </summary>
        /// <remarks>If <see cref="Magnitude"/> is zero, this returns <see cref="Zero"/></remarks>
        /// <returns>Normalized point</returns>
        public Point3D Normalized()
        {
            if (Magnitude == 0)
            {
                return Zero;
            }

            var inverseMag = 1d / Magnitude;
            var x = Math.Clamp(_x * inverseMag, 0d, 1d);
            var y = Math.Clamp(_y * inverseMag, 0d, 1d);
            var z = Math.Clamp(_z * inverseMag, 0d, 1d);

            return new Point3D(x, y, z);
        }

        /// <summary>
        /// Returns true if this and the given point no more than <paramref name="approximationThreshold"/> units apart
        /// </summary>
        /// <param name="other">Other point</param>
        /// <param name="approximationThreshold">Approximation threshold</param>
        /// <returns>True if points are approximately equal</returns>
        public bool ApproximatelyEquals(Point3D? other, double approximationThreshold)
        {
            return other != null && Distance(other) <= Math.Max(0, approximationThreshold);
        }

        /// <summary>
        /// Returns true if this and the given instances are considered equal
        /// </summary>
        /// <param name="other">Instance to compare</param>
        /// <returns>True if instances are equal</returns>
        public bool Equals(Point3D? other)
        {
            return other is not null &&
                   _x == other._x &&
                   _y == other._y &&
                   _z == other._z;
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
            return _x.GetHashCode() ^ _y.GetHashCode() ^ _z.GetHashCode();
        }

        /// <summary>
        /// Returns true if the two given values equate
        /// </summary>
        /// <param name="left">Left value</param>
        /// <param name="right">Right value</param>
        /// <returns>True if the two given values equate</returns>
        public static bool operator ==(Point3D? left, Point3D? right)
        {
            return (left is null && right is null) || (left?.Equals(right) ?? false);
        }

        /// <summary>
        /// Returns true if the two given values do not equate
        /// </summary>
        /// <param name="left">Left value</param>
        /// <param name="right">Right value</param>
        /// <returns>True if the two given values do not equate</returns>
        public static bool operator !=(Point3D? left, Point3D? right)
        {
            return !(left == right);
        }

        /// <summary>
        /// Returns the result of adding the given points together
        /// </summary>
        /// <param name="left">Left value</param>
        /// <param name="right">Right value</param>
        /// <returns>Result of addition</returns>
        public static Point3D operator +(Point3D? left, Point3D? right)
        {
            return left is not null && right is not null
                ? new Point3D(left._x + right._x, left._y + right._y, left._z + right._z)
                : left ?? right ?? Zero;
        }

        /// <summary>
        /// Returns the result of subtracting <paramref name="right"/> from <paramref name="left"/>
        /// </summary>
        /// <param name="left">Left value</param>
        /// <param name="right">Right value</param>
        /// <returns>Result of subtraction</returns>
        public static Point3D operator -(Point3D? left, Point3D? right)
        {
            return left is not null && right is not null
                ? new Point3D(left._x - right._x, left._y - right._y, left._z - right._z)
                : left ?? right ?? Zero;
        }

        /// <summary>
        /// Returns the result of multiplying the given point by a scalar
        /// </summary>
        /// <param name="point">Left value</param>
        /// <param name="scalar">Scalar</param>
        /// <returns>Result of multiplication</returns>
        public static Point3D operator *(Point3D? point, double scalar)
        {
            return point is not null
                ? new Point3D(point._x * scalar, point._y * scalar, point._z * scalar)
                : Zero;
        }

        #endregion Public Methods

        #region Private Methods

        /// <summary>
        /// Resetting derived values sets said values to null so that they
        /// will be (re)calculated on next access. This method is called when
        /// the underlying values said derived values are based on, are changed.
        /// </summary>
        private void ResetDerivedValues()
        {
            _mag = _magSqrd = null;
            _onPointChanged?.Invoke();
        }

        #endregion Private Methods
    }
}
