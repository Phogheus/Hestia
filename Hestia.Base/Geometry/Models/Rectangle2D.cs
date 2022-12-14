using System;
using System.Text.Json.Serialization;

namespace Hestia.Base.Geometry.Models
{
    /// <summary>
    /// Defines a two-dimensional rectangle in 2D space
    /// </summary>
    public sealed class Rectangle2D : IEquatable<Rectangle2D>
    {
        #region Fields

        private Point2D _topLeft;
        private Point2D _bottomRight;

        private Point2D? _topRight;
        private Point2D? _bottomLeft;
        private Point2D? _centerPoint;
        private double? _width;
        private double? _height;
        private double? _area;
        private double? _perimeter;

        #endregion Fields

        #region Properties

        /// <summary>
        /// Top left point
        /// </summary>
        public Point2D TopLeft
        {
            get => _topLeft;
            set
            {
                if (_topLeft != value)
                {
                    _topLeft = value ?? Point2D.Zero;
                    _topLeft.IsDirty = false;

                    SetDirty();
                }
            }
        }

        /// <summary>
        /// Top right point
        /// </summary>
        [JsonIgnore]
        public Point2D TopRight
        {
            get => _topRight ??= new Point2D(BottomRight.X, TopLeft.Y);
            set
            {
                if (value != null && _topRight != value)
                {
                    var newValue = value ?? Point2D.Zero;

                    _topLeft = new Point2D(_topLeft.X, newValue.Y);
                    _bottomRight = new Point2D(newValue.X, _bottomRight.Y);

                    SetDirty();
                }
            }
        }

        /// <summary>
        /// Bottom right point
        /// </summary>
        public Point2D BottomRight
        {
            get => _bottomRight;
            set
            {
                if (_bottomRight != value)
                {
                    _bottomRight = value ?? Point2D.Zero;
                    _bottomRight.IsDirty = false;

                    SetDirty();
                }
            }
        }

        /// <summary>
        /// Bottom left point
        /// </summary>
        [JsonIgnore]
        public Point2D BottomLeft
        {
            get => _bottomLeft ??= new Point2D(TopLeft.X, BottomRight.Y);
            set
            {
                if (value != null && _bottomLeft != value)
                {
                    var newValue = value ?? Point2D.Zero;

                    _topLeft = new Point2D(newValue.X, _topLeft.Y);
                    _bottomRight = new Point2D(_bottomRight.X, newValue.Y);

                    SetDirty();
                }
            }
        }

        /// <summary>
        /// Center point
        /// </summary>
        [JsonIgnore]
        public Point2D CenterPoint => GetCenterPoint();

        /// <summary>
        /// Highest Y value
        /// </summary>
        [JsonIgnore]
        public double Top => TopLeft.Y;

        /// <summary>
        /// Lowest Y value
        /// </summary>
        [JsonIgnore]
        public double Bottom => BottomRight.Y;

        /// <summary>
        /// Lowest X value
        /// </summary>
        [JsonIgnore]
        public double Left => TopLeft.X;

        /// <summary>
        /// Highest X value
        /// </summary>
        [JsonIgnore]
        public double Right => BottomRight.X;

        /// <summary>
        /// Width of the rectangle
        /// </summary>
        [JsonIgnore]
        public double Width => GetWidth();

        /// <summary>
        /// Height of the rectangle
        /// </summary>
        [JsonIgnore]
        public double Height => GetHeight();

        /// <summary>
        /// Area of the rectangle
        /// </summary>
        [JsonIgnore]
        public double Area => GetArea();

        /// <summary>
        /// Perimeter of the rectangle
        /// </summary>
        [JsonIgnore]
        public double Perimeter => GetPerimeter();

        #endregion Properties

        #region Constructors

        /// <summary>
        /// Default constructor creating a zero area rectangle
        /// </summary>
        public Rectangle2D()
            : this(Point2D.Zero, Point2D.Zero)
        {
        }

        /// <summary>
        /// Constructor taking the top left and bottom right points
        /// </summary>
        /// <param name="topLeft">Top left point</param>
        /// <param name="bottomRight">Bottom right point</param>
        [JsonConstructor]
        public Rectangle2D(Point2D? topLeft, Point2D? bottomRight)
        {
            _topLeft = topLeft ?? Point2D.Zero;
            _bottomRight = bottomRight ?? Point2D.Zero;
            _topRight = new Point2D(_bottomRight.X, _topLeft.Y);
            _bottomLeft = new Point2D(_topLeft.X, _bottomRight.Y);
        }

        #endregion Constructors

        #region Public Methods

        /// <summary>
        /// Returns true if <paramref name="point"/> is within the bounds of this rectangle
        /// </summary>
        /// <param name="point">Point to check</param>
        /// <returns>True if point is considered inside the circle</returns>
        public bool IsPointInsideRect(Point2D? point)
        {
            return point != null &&
                   point.X >= Left && point.X <= Right &&
                   point.Y <= Top && point.Y >= Bottom;
        }

        /// <summary>
        /// Returns true if this and the given instances are considered equal
        /// </summary>
        /// <param name="other">Instance to compare</param>
        /// <returns>True if instances are equal</returns>
        public bool Equals(Rectangle2D? other)
        {
            return other is not null &&
                   TopLeft.Equals(other.TopLeft) &&
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
            return Top.GetHashCode() ^ Right.GetHashCode() ^ Area.GetHashCode();
        }

        /// <summary>
        /// Returns true if the two given values equate
        /// </summary>
        /// <param name="left">Left value</param>
        /// <param name="right">Right value</param>
        /// <returns>True if the two given values equate</returns>
        public static bool operator ==(Rectangle2D? left, Rectangle2D? right)
        {
            return (left is null && right is null) || (left?.Equals(right) ?? false);
        }

        /// <summary>
        /// Returns true if the two given values do not equate
        /// </summary>
        /// <param name="left">Left value</param>
        /// <param name="right">Right value</param>
        /// <returns>True if the two given values do not equate</returns>
        public static bool operator !=(Rectangle2D? left, Rectangle2D? right)
        {
            return !(left == right);
        }

        #endregion Public Methods

        #region Private Methods

        private Point2D GetCenterPoint()
        {
            MarkDirtyIfPointsAreDirty();

            return _centerPoint ??= new Point2D(Left + (Width / 2d), Bottom + (Height / 2d));
        }

        private double GetWidth()
        {
            MarkDirtyIfPointsAreDirty();

            return _width ??= Math.Abs(Right - Left);
        }

        private double GetHeight()
        {
            MarkDirtyIfPointsAreDirty();

            return _height ??= Math.Abs(Top - Bottom);
        }

        private double GetArea()
        {
            MarkDirtyIfPointsAreDirty();

            return _area ??= Width * Height;
        }

        private double GetPerimeter()
        {
            MarkDirtyIfPointsAreDirty();

            return _perimeter ??= 2 * (Width + Height);
        }

        private void SetDirty()
        {
            _topRight = null;
            _bottomLeft = null;
            _centerPoint = null;
            _width = null;
            _height = null;
            _area = null;
            _perimeter = null;
        }

        private void MarkDirtyIfPointsAreDirty()
        {
            if (_topLeft.IsDirty || _bottomRight.IsDirty)
            {
                // Set ourselves as dirty now that we can acknowledge some underlying points changed
                SetDirty();

                // Reset dirty status for points
                _topLeft.IsDirty = false;
                _bottomRight.IsDirty = false;
            }
        }

        #endregion Private Methods
    }
}
