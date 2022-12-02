using System;
using System.Text.Json.Serialization;

namespace Hestia.Base.Geometry.Models
{
    /// <summary>
    /// Defines a two-dimensional rectangle in 2D space
    /// </summary>
    public readonly struct Rectangle2D : IEquatable<Rectangle2D>
    {
        #region Properties

        /// <summary>
        /// Top left point
        /// </summary>
        public Point2D TopLeft { get; }

        /// <summary>
        /// Top right point
        /// </summary>
        [JsonIgnore]
        public Point2D TopRight { get; }

        /// <summary>
        /// Bottom right point
        /// </summary>
        public Point2D BottomRight { get; }

        /// <summary>
        /// Bottom left point
        /// </summary>
        [JsonIgnore]
        public Point2D BottomLeft { get; }

        /// <summary>
        /// Highest Y value
        /// </summary>
        [JsonIgnore]
        public double Top { get; }

        /// <summary>
        /// Lowest Y value
        /// </summary>
        [JsonIgnore]
        public double Bottom { get; }

        /// <summary>
        /// Lowest X value
        /// </summary>
        [JsonIgnore]
        public double Left { get; }

        /// <summary>
        /// Highest X value
        /// </summary>
        [JsonIgnore]
        public double Right { get; }

        /// <summary>
        /// Width of the rectangle
        /// </summary>
        [JsonIgnore]
        public double Width { get; }

        /// <summary>
        /// Height of the rectangle
        /// </summary>
        [JsonIgnore]
        public double Height { get; }

        /// <summary>
        /// Area of the rectangle
        /// </summary>
        [JsonIgnore]
        public double Area { get; }

        /// <summary>
        /// Perimeter of the rectangle
        /// </summary>
        [JsonIgnore]
        public double Perimeter { get; }

        #endregion Properties

        #region Constructors

        /// <summary>
        /// Constructor taking the top left and bottom right points
        /// </summary>
        /// <param name="topLeft">Top left point</param>
        /// <param name="bottomRight">Bottom right point</param>
        [JsonConstructor]
        public Rectangle2D(Point2D topLeft, Point2D bottomRight)
        {
            // Ensure points are in X-Right Y-Up format
            var xMin = Math.Min(topLeft.X, bottomRight.X);
            var xMax = Math.Max(topLeft.X, bottomRight.X);

            var width = Math.Abs(xMax - xMin);

            var yMin = Math.Min(topLeft.Y, bottomRight.Y);
            var yMax = Math.Max(topLeft.Y, bottomRight.Y);

            var height = Math.Abs(yMax - yMin);

            Top = yMax;
            Bottom = yMin;
            Left = xMin;
            Right = xMax;

            Width = width;
            Height = height;

            TopLeft = new Point2D(Left, Top);
            TopRight = new Point2D(Right, Top);
            BottomRight = new Point2D(Right, Bottom);
            BottomLeft = new Point2D(Left, Bottom);

            Area = Width * Height;
            Perimeter = 2 * (Width + Height);
        }

        #endregion Constructors

        #region Public Methods

        /// <summary>
        /// Returns true if <paramref name="point"/> is within the bounds of this rectangle
        /// </summary>
        /// <param name="point">Point to check</param>
        /// <returns>True if point is considered inside the circle</returns>
        public bool IsPointInsideRect(Point2D point)
        {
            return point.X >= Left && point.X <= Right &&
                   point.Y <= Top && point.Y >= Bottom;
        }

        /// <summary>
        /// Returns true if this and the given instances are considered equal
        /// </summary>
        /// <param name="other">Instance to compare</param>
        /// <returns>True if instances are equal</returns>
        public bool Equals(Rectangle2D other)
        {
            return TopLeft.Equals(other.TopLeft) &&
                   BottomRight.Equals(other.BottomRight);
        }

        /// <summary>
        /// Returns true if this and the given instances are considered equal
        /// </summary>
        /// <param name="obj">Instance to compare</param>
        /// <returns>True if instances are equal</returns>
        public override bool Equals(object? obj)
        {
            return obj is Rectangle2D rectangle && Equals(rectangle);
        }

        /// <summary>
        /// Returns the hash code for this instance
        /// </summary>
        /// <returns>Hash code</returns>
        public override int GetHashCode()
        {
            return HashCode.Combine(TopLeft, BottomRight);
        }

        /// <summary>
        /// Returns true if the two given values equate
        /// </summary>
        /// <param name="left">Left value</param>
        /// <param name="right">Right value</param>
        /// <returns>True if the two given values equate</returns>
        public static bool operator ==(Rectangle2D left, Rectangle2D right)
        {
            return left.Equals(right);
        }

        /// <summary>
        /// Returns true if the two given values do not equate
        /// </summary>
        /// <param name="left">Left value</param>
        /// <param name="right">Right value</param>
        /// <returns>True if the two given values do not equate</returns>
        public static bool operator !=(Rectangle2D left, Rectangle2D right)
        {
            return !(left == right);
        }

        #endregion Public Methods
    }
}
