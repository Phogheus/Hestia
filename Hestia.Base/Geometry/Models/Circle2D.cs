using System;
using System.Text.Json.Serialization;

namespace Hestia.Base.Geometry.Models
{
    /// <summary>
    /// Defines a two-dimensional circle that can also exist at a point in 2D space
    /// </summary>
    public sealed class Circle2D : IEquatable<Circle2D>
    {
        #region Fields

        private double _radius;
        private double? _diameter;
        private double? _circumference;
        private double? _area;
        private Rectangle2D? _bounds;

        #endregion Fields

        #region Properties

        /// <summary>
        /// Radius of the circle
        /// </summary>
        public double Radius
        {
            get => _radius;
            set
            {
                _radius = Math.Max(0, value);
                _diameter = _circumference = _area = null;
                _bounds = null;
            }
        }

        /// <summary>
        /// Center point of the circle
        /// </summary>
        public Point2D CenterPoint { get; set; }

        /// <summary>
        /// Diameter of the circle
        /// </summary>
        [JsonIgnore]
        public double Diameter => _diameter ??= Radius * 2d;

        /// <summary>
        /// Circumference of the circle
        /// </summary>
        [JsonIgnore]
        public double Circumference => _circumference ??= Math.PI * Diameter;

        /// <summary>
        /// Area of the circle
        /// </summary>
        [JsonIgnore]
        public double Area => _area ??= Math.PI * (Radius * Radius);

        /// <summary>
        /// Rectangular bounds of the circle
        /// </summary>
        [JsonIgnore]
        public Rectangle2D Bounds => _bounds ??= GetBoundsFromCenter(Radius, CenterPoint);

        #endregion Properties

        #region Constructors

        /// <summary>
        /// Default constructor, creates a circle with a <see cref="Radius"/> of 1
        /// </summary>
        public Circle2D()
            : this(0d, null)
        {
        }

        /// <summary>
        /// Constructor taking a radius and putting the circle at <see cref="Point2D.Zero"/>
        /// </summary>
        /// <param name="radius">Radius of the circle</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if radius is &lt;= 0</exception>
        public Circle2D(double radius)
            : this(radius, null)
        {
        }

        /// <summary>
        /// Constructor taking a radius and center point
        /// </summary>
        /// <param name="radius">Radius of the circle</param>
        /// <param name="centerPoint">Center point of the circle</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if radius is &lt;= 0</exception>
        [JsonConstructor]
        public Circle2D(double radius, Point2D? centerPoint)
        {
            Radius = Math.Max(0, radius);
            CenterPoint = centerPoint ?? Point2D.Zero;
        }

        #endregion Constructors

        #region Public Methods

        /// <summary>
        /// Returns true if <paramref name="point"/> is less than or equal to <see cref="Radius"/> units
        /// away from <see cref="CenterPoint"/>
        /// </summary>
        /// <param name="point">Point to check</param>
        /// <returns>True if point is considered inside the circle</returns>
        public bool IsPointInCircle(Point2D? point)
        {
            return point is not null && CenterPoint.Distance(point) <= Radius;
        }

        /// <summary>
        /// Returns true if this and the given instances are considered equal
        /// </summary>
        /// <param name="other">Instance to compare</param>
        /// <returns>True if instances are equal</returns>
        public bool Equals(Circle2D? other)
        {
            return Radius == other?.Radius && CenterPoint.Equals(other?.CenterPoint);
        }

        /// <summary>
        /// Returns true if this and the given instances are considered equal
        /// </summary>
        /// <param name="obj">Instance to compare</param>
        /// <returns>True if instances are equal</returns>
        public override bool Equals(object? obj)
        {
            return obj is Circle2D circle && Equals(circle);
        }

        /// <summary>
        /// Returns the hash code for this instance
        /// </summary>
        /// <returns>Hash code</returns>
        public override int GetHashCode()
        {
            return HashCode.Combine(Radius, CenterPoint);
        }

        /// <summary>
        /// Returns true if the two given values equate
        /// </summary>
        /// <param name="left">Left value</param>
        /// <param name="right">Right value</param>
        /// <returns>True if the two given values equate</returns>
        public static bool operator ==(Circle2D? left, Circle2D? right)
        {
            return (left is null && right is null) || (left?.Equals(right) ?? false);
        }

        /// <summary>
        /// Returns true if the two given values do not equate
        /// </summary>
        /// <param name="left">Left value</param>
        /// <param name="right">Right value</param>
        /// <returns>True if the two given values do not equate</returns>
        public static bool operator !=(Circle2D? left, Circle2D? right)
        {
            return !(left == right);
        }

        #endregion Public Methods

        #region Private Methods

        private static Rectangle2D GetBoundsFromCenter(double radius, Point2D centerPoint)
        {
            var topLeft = new Point2D(centerPoint.X - radius, centerPoint.Y + radius);
            var bottomRight = new Point2D(centerPoint.X + radius, centerPoint.Y - radius);

            return new Rectangle2D(topLeft, bottomRight);
        }

        #endregion Private Methods
    }
}
