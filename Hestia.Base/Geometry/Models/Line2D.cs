using System;
using System.Text.Json.Serialization;
using Hestia.Base.Geometry.Enums;
using Hestia.Base.Geometry.Utilities;

namespace Hestia.Base.Geometry.Models
{
    /// <summary>
    /// Defines a line segment in two-dimensional space 
    /// </summary>
    public sealed class Line2D : IEquatable<Line2D>
    {
        #region Fields

        private Point2D _start;
        private Point2D _end;
        private Rectangle2D? _bounds;
        private Point2D? _midPoint;
        private double? _length;
        private double? _slope;
        private double? _intercept;

        #endregion Fields

        #region Properties

        /// <summary>
        /// Two-dimensional point representing the start of the line
        /// </summary>
        public Point2D Start
        {
            get => _start;
            set
            {
                if (_start != value)
                {
                    _start = value ?? Point2D.Zero;
                    _start.IsDirty = false;

                    // Set last values so we can determine if we're dirty next time a call to derived values is done
                    SetDirty();
                }
            }
        }

        /// <summary>
        /// Two-dimensional point representing the end of the line
        /// </summary>
        public Point2D End
        {
            get => _end;
            set
            {
                if (_end != value)
                {
                    _end = value ?? Point2D.Zero;
                    _end.IsDirty = false;

                    // Set last values so we can determine if we're dirty next time a call to derived values is done
                    SetDirty();
                }
            }
        }

        /// <summary>
        /// Two-dimensional bounds this line segment fits within
        /// </summary>
        [JsonIgnore]
        public Rectangle2D Bounds => GetBounds();

        /// <summary>
        /// Two-dimensional point representing the midpoint of this line segment
        /// </summary>
        [JsonIgnore]
        public Point2D MidPoint => GetMidPoint();

        /// <summary>
        /// Length of the line segment
        /// </summary>
        [JsonIgnore]
        public double Length => GetLength();

        /// <summary>
        /// Slope of the line
        /// </summary>
        [JsonIgnore]
        public double Slope => GetSlope();

        /// <summary>
        /// Y-intercept of the line
        /// </summary>
        [JsonIgnore]
        public double Intercept => GetIntercept();

        #endregion Properties

        #region Constructors

        /// <summary>
        /// Default constructor; creates a line of zero length
        /// </summary>
        public Line2D()
            : this(Point2D.Zero, Point2D.Zero)
        {
        }

        /// <summary>
        /// Constructor taking the start and end of the line segment defining the line
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        [JsonConstructor]
        public Line2D(Point2D start, Point2D end)
        {
            _start = start ?? throw new ArgumentNullException(nameof(start));
            _end = end ?? throw new ArgumentNullException(nameof(end));
        }

        #endregion Constructors

        #region Public Methods

        /// <summary>
        /// Returns true if the given point is on this line
        /// </summary>
        /// <param name="point">Point to check</param>
        /// <returns>True if point is on this line</returns>
        public bool IsPointOnLine(Point2D? point)
        {
            return GetOrientationOfPoint(point) == PointOrientationType.Colinear;
        }

        /// <summary>
        /// Returns true if the given point is on this line segment as defined
        /// </summary>
        /// <param name="point">Point to check</param>
        /// <returns>True if point is on this line segment</returns>
        public bool IsPointOnLineSegment(Point2D? point)
        {
            return IsPointOnLine(point) && Bounds.IsPointInsideRect(point);
        }

        /// <summary>
        /// Returns the orientation of <paramref name="point"/> C to this line AB
        /// </summary>
        /// <param name="point">Point to return the orientation of</param>
        /// <returns><see cref="PointOrientationType"/></returns>
        /// <seealso href="https://www.geeksforgeeks.org/orientation-3-ordered-points/"/>
        public PointOrientationType? GetOrientationOfPoint(Point2D? point)
        {
            if (point == null)
            {
                return null;
            }

            // To determine if Point C is conlinear with line AB, we compare the slopes of lines AB and CB.
            //  Slope AB = (y2 - y1) / (x2 - x1)
            //  Slope CB = (y3 - y2) / (x3 - x2)
            // 
            // This relationship can be expressed as the result of the Cross Product:
            //  (y2 - y1) * (x3 - x2) - (y3 - y2) * (x2 - x1).
            // 
            // Point C is colinear when the cross product is 0, clockwise when greater than 0, and
            // counter-clockwise when less than 0.

            // (y2 - y1) * (x3 - x2) - (y3 - y2) * (x2 - x1)
            var cross = ((_end.Y - _start.Y) * (point.X - _end.X)) - ((point.Y - _end.Y) * (_end.X - _start.X));

            // As a developer's note, this formula suffers from rounding point errors and can be made more accurate by
            // including a threshold value to match against the cross product. However, valid thresholds scale with the
            // level of precision given to the unit points, and my intention with these geometry classes are for generally
            // low precision work. That being said, I'm opting for no threshold checking, and simply leaving it to 0.

            return cross == 0f ? PointOrientationType.Colinear
                 : cross > 0 ? PointOrientationType.Clockwise
                 : PointOrientationType.CounterClockwise;
        }

        /// <summary>
        /// Returns true if this line and the given line intersect
        /// </summary>
        /// <param name="line">Line to check</param>
        /// <returns>True if lines intersect</returns>
        /// <seealso href="https://en.wikipedia.org/wiki/Line%E2%80%93line_intersection"/>
        public bool DoLinesIntersect(Line2D? line)
        {
            if (line == null)
            {
                return false;
            }

            // Using determinants written out as:
            // 
            // x = (x1 * y2 - y1 * x2) * (x3 - x4) - (x1 - x2) * (x3 * y4 - y3 * x4)
            //     -----------------------------------------------------------------
            //               (x1 - x2) * (y3 - y4) - (y1 - y2) * (x3 - x4)
            // 
            // y = (x1 * y2 - y1 * x2) * (y3 - y4) - (y1 - y2) * (x3 * y4 - y3 * x4)
            //     -----------------------------------------------------------------
            //               (x1 - x2) * (y3 - y4) - (y1 - y2) * (x3 - x4)
            // 
            // we can determine if lines intersect by computing the denominator. If the
            // value is 0, the lines are coincident or parallel.

            // (x1 - x2) * (y3 - y4) - (y1 - y2) * (x3 - x4)
            var denominator = ((_start.X - _end.X) * (line._start.Y - line._end.Y)) - ((_start.Y - _end.Y) * (line._start.X - line._end.X));

            return Math.Abs(denominator) != 0d;
        }

        /// <summary>
        /// Returns true if this line segment and the given line segment intersect
        /// </summary>
        /// <param name="line">Line to check</param>
        /// <returns>True if line segments intersect</returns>
        /// <seealso href="https://en.wikipedia.org/wiki/Line%E2%80%93line_intersection"/>
        public bool DoLineSegmentsIntersect(Line2D? line)
        {
            if (line == null)
            {
                return false;
            }

            // Given determinents t and u, line segments intersect when 0 <= t <= 1 or 0 <= u <= 1
            // 
            // t = (x1 - x3) * (y3 - y4) - (y1 - y3) * (x3 - x4)
            //     ---------------------------------------------
            //     (x1 - x2) * (y3 - y4) - (y1 - y2) * (x3 - x4)
            // 
            // u = (x1 - x3) * (y1 - y2) - (y1 - y3) * (x1 - x2)
            //     ---------------------------------------------
            //     (x1 - x2) * (y3 - y4) - (y1 - y2) * (x3 - x4)

            // (x1 - x2) * (y3 - y4) - (y1 - y2) * (x3 - x4)
            var denominator = ((_start.X - _end.X) * (line._start.Y - line._end.Y)) - ((_start.Y - _end.Y) * (line._start.X - line._end.X));

            // If denominator is zero then the lines are coincident or parallel
            if (Math.Abs(denominator) == 0d)
            {
                return false;
            }

            // (x1 - x3) * (y3 - y4) - (y1 - y3) * (x3 - x4)
            var tNumerator = ((_start.X - line._start.X) * (line._start.Y - line._end.Y)) - ((_start.Y - line._start.Y) * (line._start.X - line._end.X));
            var t = tNumerator / denominator;

            if (t >= 0d && t <= 1d)
            {
                return true;
            }

            // (x1 - x3) * (y1 - y2) - (y1 - y3) * (x1 - x2)
            var uNumerator = ((_start.X - line._start.X) * (_start.Y - _end.Y)) - ((_start.Y - line._start.Y) * (_start.X - _end.X));
            var u = uNumerator / denominator;

            return u >= 0d && u <= 1d;
        }

        /// <summary>
        /// Returns the intersection of the two lines, or null if lines are coincident or parallel
        /// </summary>
        /// <param name="line">Line to intersect</param>
        /// <returns>Intersection or null if lines are coincident or parallel</returns>
        /// <seealso href="https://en.wikipedia.org/wiki/Line%E2%80%93line_intersection"/>
        public Point2D? GetIntersectionPointOfLines(Line2D? line)
        {
            if (line == null)
            {
                return null;
            }

            // Using determinants written out as:
            // 
            // x = (x1 * y2 - y1 * x2) * (x3 - x4) - (x1 - x2) * (x3 * y4 - y3 * x4)
            //     -----------------------------------------------------------------
            //               (x1 - x2) * (y3 - y4) - (y1 - y2) * (x3 - x4)
            // 
            // y = (x1 * y2 - y1 * x2) * (y3 - y4) - (y1 - y2) * (x3 * y4 - y3 * x4)
            //     -----------------------------------------------------------------
            //               (x1 - x2) * (y3 - y4) - (y1 - y2) * (x3 - x4)
            // 
            // we can determine the position the lines intersect, provided the lines are
            // not coincident or parallel.

            var x1x2Diff = _start.X - _end.X;
            var y3y4Diff = line._start.Y - line._end.Y;
            var y1y2Diff = _start.Y - _end.Y;
            var x3x4Diff = line._start.X - line._end.X;

            // (x1 - x2) * (y3 - y4) - (y1 - y2) * (x3 - x4)
            var denominator = (x1x2Diff * y3y4Diff) - (y1y2Diff * x3x4Diff);

            // If denominator is zero then the lines are coincident or parallel
            if (Math.Abs(denominator) == 0d)
            {
                return null;
            }

            var x1y2_y1x2Diff = (_start.X * _end.Y) - (_start.Y * _end.X);
            var x3y4_y3x4Diff = (line._start.X * line._end.Y) - (line._start.Y * line._end.X);

            // (x1 * y2 - y1 * x2) * (x3 - x4) - (x1 - x2) * (x3 * y4 - y3 * x4)
            var xNumerator = (x1y2_y1x2Diff * x3x4Diff) - (x1x2Diff * x3y4_y3x4Diff);

            // (x1 * y2 - y1 * x2) * (y3 - y4) - (y1 - y2) * (x3 * y4 - y3 * x4)
            var yNumerator = (x1y2_y1x2Diff * y3y4Diff) - (y1y2Diff * x3y4_y3x4Diff);

            var x = xNumerator / denominator;
            var y = yNumerator / denominator;

            return new Point2D(x, y);
        }

        /// <summary>
        /// Returns the intersection of the two line segments, or null if line segments do not intersect
        /// </summary>
        /// <param name="line">Line to intersect</param>
        /// <returns>Intersection or null if line segments do not intersect</returns>
        /// <seealso href="https://en.wikipedia.org/wiki/Line%E2%80%93line_intersection"/>
        public Point2D? GetIntersectionPointOfLineSegments(Line2D? line)
        {
            if (line == null)
            {
                return null;
            }

            // Given determinents t and u, line segments intersect when 0 <= t <= 1 or 0 <= u <= 1
            // 
            // t = (x1 - x3) * (y3 - y4) - (y1 - y3) * (x3 - x4)
            //     ---------------------------------------------
            //     (x1 - x2) * (y3 - y4) - (y1 - y2) * (x3 - x4)
            // 
            // u = (x1 - x3) * (y1 - y2) - (y1 - y3) * (x1 - x2)
            //     ---------------------------------------------
            //     (x1 - x2) * (y3 - y4) - (y1 - y2) * (x3 - x4)

            // (x1 - x2) * (y3 - y4) - (y1 - y2) * (x3 - x4)
            var denominator = ((_start.X - _end.X) * (line._start.Y - line._end.Y)) - ((_start.Y - _end.Y) * (line._start.X - line._end.X));

            // If denominator is zero then the lines are coincident or parallel
            if (Math.Abs(denominator) == 0d)
            {
                return null;
            }

            // Given determinents t and u:
            //   When 0 <= t <= 1:
            //     x = x1 + t(x2 - x1)
            //     y = y1 + t(y2 - y1)
            //   When 0 <= u <= 1:
            //     x = x3 + u(x4 - x3)
            //     y = y3 + u(y4 - y3)

            // (x1 - x3) * (y3 - y4) - (y1 - y3) * (x3 - x4)
            var tNumerator = ((_start.X - line._start.X) * (line._start.Y - line._end.Y)) - ((_start.Y - line._start.Y) * (line._start.X - line._end.X));
            var t = tNumerator / denominator;

            if (t >= 0d && t <= 1d)
            {
                // x1 + t(x2 - x1)
                var x = _start.X + (t * (_end.X - _start.X));

                // y1 + t(y2 - y1)
                var y = _start.Y + (t * (_end.Y - _start.Y));

                return new Point2D(x, y);
            }

            // (x1 - x3) * (y1 - y2) - (y1 - y3) * (x1 - x2)
            var uNumerator = ((_start.X - line._start.X) * (_start.Y - _end.Y)) - ((_start.Y - line._start.Y) * (_start.X - _end.X));
            var u = uNumerator / denominator;

            if (u >= 0d && u <= 1d)
            {
                // x3 + u(x4 - x3)
                var x = line._start.X + (u * (line._end.X - line._start.X));

                // y3 + u(y4 - y3)
                var y = line._start.Y + (u * (line._end.Y - line._start.Y));

                return new Point2D(x, y);
            }

            return null;
        }

        /// <summary>
        /// Returns true if this and the given instances are considered equal
        /// </summary>
        /// <param name="other">Instance to compare</param>
        /// <returns>True if instances are equal</returns>
        public bool Equals(Line2D? other)
        {
            return other is not null &&
                   ((_start.Equals(other._start) && _end.Equals(other._end)) ||
                    (_start.Equals(other._end) && _end.Equals(other._start)));
        }

        /// <summary>
        /// Returns true if this and the given instances are considered equal
        /// </summary>
        /// <param name="obj">Instance to compare</param>
        /// <returns>True if instances are equal</returns>
        public override bool Equals(object? obj)
        {
            return obj is Line2D line && Equals(line);
        }

        /// <summary>
        /// Returns the hash code for this instance
        /// </summary>
        /// <returns>Hash code</returns>
        public override int GetHashCode()
        {
            return _start.X.GetHashCode() ^
                   _start.Y.GetHashCode() ^
                   _end.X.GetHashCode() ^
                   _end.Y.GetHashCode();
        }

        /// <summary>
        /// Returns a string representation of this object
        /// </summary>
        public override string ToString()
        {
            return $"({_start}, {_end})";
        }

        /// <summary>
        /// Returns true if the two given values equate
        /// </summary>
        /// <param name="left">Left value</param>
        /// <param name="right">Right value</param>
        /// <returns>True if the two given values equate</returns>
        public static bool operator ==(Line2D? left, Line2D? right)
        {
            return (left is null && right is null) || (left?.Equals(right) ?? false);
        }

        /// <summary>
        /// Returns true if the two given values do not equate
        /// </summary>
        /// <param name="left">Left value</param>
        /// <param name="right">Right value</param>
        /// <returns>True if the two given values do not equate</returns>
        public static bool operator !=(Line2D? left, Line2D? right)
        {
            return !(left == right);
        }

        #endregion Public Methods

        #region Private Methods

        private Rectangle2D GetBounds()
        {
            MarkDirtyIfPointsAreDirty();

            return _bounds ??= GeometryUtilities.GetBoundsFromPoints(new Point2D[] { _start, _end });
        }

        private Point2D GetMidPoint()
        {
            MarkDirtyIfPointsAreDirty();

            return _midPoint ??= new Point2D((_start.X + _end.X) / 2f, (_start.Y + _end.Y) / 2f);
        }

        private double GetLength()
        {
            MarkDirtyIfPointsAreDirty();

            return _length ??= _start.Distance(_end);
        }

        private double GetSlope()
        {
            MarkDirtyIfPointsAreDirty();

            return _slope ??= (_end.Y - _start.Y) / (_end.X - _start.X);
        }

        private double GetIntercept()
        {
            MarkDirtyIfPointsAreDirty();

            return _intercept ??= _start.Y - (Slope * _start.X);
        }

        private void SetDirty()
        {
            _bounds = null;
            _midPoint = null;
            _length = null;
            _slope = null;
            _intercept = null;
        }

        private void MarkDirtyIfPointsAreDirty()
        {
            if (_start.IsDirty || _end.IsDirty)
            {
                // Set ourselves as dirty now that we can acknowledge some underlying points changed
                SetDirty();

                // Reset dirty status for points
                _start.IsDirty = false;
                _end.IsDirty = false;
            }
        }

        #endregion Private Methods
    }
}
