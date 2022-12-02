using System;
using System.Text.Json.Serialization;
using Hestia.Base.Geometry.Enums;

namespace Hestia.Base.Geometry.Models
{
    /// <summary>
    /// Defines a line segment in two-dimensional space 
    /// </summary>
    public readonly struct Line2D : IEquatable<Line2D>
    {
        #region Fields

        private const string LINE_ZERO_LENGTH_EXCEPTION = "Line cannot have zero length.";

        #endregion Fields

        #region Properties

        /// <summary>
        /// Two-dimensional point representing the start of the line
        /// </summary>
        public Point2D Start { get; init; }

        /// <summary>
        /// Two-dimensional point representing the end of the line
        /// </summary>
        public Point2D End { get; init; }

        /// <summary>
        /// Two-dimensional bounds this line segment fits within
        /// </summary>
        [JsonIgnore]
        public Rectangle2D Bounds { get; }

        /// <summary>
        /// Two-dimensional point representing the midpoint of this line segment
        /// </summary>
        [JsonIgnore]
        public Point2D MidPoint { get; }

        /// <summary>
        /// Length of the line segment
        /// </summary>
        [JsonIgnore]
        public double Length { get; }

        /// <summary>
        /// Slope of the line
        /// </summary>
        [JsonIgnore]
        public double Slope { get; }

        /// <summary>
        /// Y-intercept of the line
        /// </summary>
        [JsonIgnore]
        public double Intercept { get; }

        #endregion Properties

        #region Constructors

        /// <summary>
        /// Default constructor; creates a line between (0, 0) and (1, 0)
        /// </summary>
        public Line2D()
            : this(Point2D.Zero, new Point2D(1, 0))
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
            if (start.Distance(end) == 0)
            {
                throw new InvalidOperationException(LINE_ZERO_LENGTH_EXCEPTION);
            }

            Start = start;
            End = end;

            // Create bounds
            var xMin = Math.Min(Start.X, End.X);
            var xMax = Math.Max(Start.X, End.X);
            var yMin = Math.Min(Start.Y, End.Y);
            var yMax = Math.Max(Start.Y, End.Y);
            var topLeft = new Point2D(xMin, yMax);
            var bottomRight = new Point2D(xMax, yMin);
            Bounds = new Rectangle2D(topLeft, bottomRight);

            // Other derived values
            MidPoint = new Point2D((Start.X + End.X) / 2f, (Start.Y + End.Y) / 2f);
            Length = Start.Distance(End);
            Slope = (End.Y - Start.Y) / (End.X - Start.X);
            Intercept = Start.Y - (Slope * Start.X);
        }

        #endregion Constructors

        #region Public Methods

        /// <summary>
        /// Returns true if the given point is on this line
        /// </summary>
        /// <param name="point">Point to check</param>
        /// <returns>True if point is on this line</returns>
        public bool IsPointOnLine(Point2D point)
        {
            return GetOrientationOfPoint(point) == PointOrientationType.Colinear;
        }

        /// <summary>
        /// Returns true if the given point is on this line segment as defined
        /// </summary>
        /// <param name="point">Point to check</param>
        /// <returns>True if point is on this line segment</returns>
        public bool IsPointOnLineSegment(Point2D point)
        {
            return IsPointOnLine(point) && Bounds.IsPointInsideRect(point);
        }

        /// <summary>
        /// Returns the orientation of <paramref name="point"/> C to this line AB
        /// </summary>
        /// <param name="point">Point to return the orientation of</param>
        /// <returns><see cref="PointOrientationType"/></returns>
        /// <seealso href="https://www.geeksforgeeks.org/orientation-3-ordered-points/"/>
        public PointOrientationType GetOrientationOfPoint(Point2D point)
        {
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
            var cross = ((End.Y - Start.Y) * (point.X - End.X)) - ((point.Y - End.Y) * (End.X - Start.X));

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
        public bool DoLinesIntersect(Line2D line)
        {
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
            var denominator = ((Start.X - End.X) * (line.Start.Y - line.End.Y)) - ((Start.Y - End.Y) * (line.Start.X - line.End.X));

            return Math.Abs(denominator) != 0d;
        }

        /// <summary>
        /// Returns true if this line segment and the given line segment intersect
        /// </summary>
        /// <param name="line">Line to check</param>
        /// <returns>True if line segments intersect</returns>
        /// <seealso href="https://en.wikipedia.org/wiki/Line%E2%80%93line_intersection"/>
        public bool DoLineSegmentsIntersect(Line2D line)
        {
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
            var denominator = ((Start.X - End.X) * (line.Start.Y - line.End.Y)) - ((Start.Y - End.Y) * (line.Start.X - line.End.X));

            // If denominator is zero then the lines are coincident or parallel
            if (Math.Abs(denominator) == 0d)
            {
                return false;
            }

            // (x1 - x3) * (y3 - y4) - (y1 - y3) * (x3 - x4)
            var tNumerator = ((Start.X - line.Start.X) * (line.Start.Y - line.End.Y)) - ((Start.Y - line.Start.Y) * (line.Start.X - line.End.X));
            var t = tNumerator / denominator;

            if (t >= 0d && t <= 1d)
            {
                return true;
            }

            // (x1 - x3) * (y1 - y2) - (y1 - y3) * (x1 - x2)
            var uNumerator = ((Start.X - line.Start.X) * (Start.Y - End.Y)) - ((Start.Y - line.Start.Y) * (Start.X - End.X));
            var u = uNumerator / denominator;

            return u >= 0d && u <= 1d;
        }

        /// <summary>
        /// Returns the intersection of the two lines, or null if lines are coincident or parallel
        /// </summary>
        /// <param name="line">Line to intersect</param>
        /// <returns>Intersection or null if lines are coincident or parallel</returns>
        /// <seealso href="https://en.wikipedia.org/wiki/Line%E2%80%93line_intersection"/>
        public Point2D? GetIntersectionPointOfLines(Line2D line)
        {
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

            var x1x2Diff = Start.X - End.X;
            var y3y4Diff = line.Start.Y - line.End.Y;
            var y1y2Diff = Start.Y - End.Y;
            var x3x4Diff = line.Start.X - line.End.X;

            // (x1 - x2) * (y3 - y4) - (y1 - y2) * (x3 - x4)
            var denominator = (x1x2Diff * y3y4Diff) - (y1y2Diff * x3x4Diff);

            // If denominator is zero then the lines are coincident or parallel
            if (Math.Abs(denominator) == 0d)
            {
                return null;
            }

            var x1y2_y1x2Diff = (Start.X * End.Y) - (Start.Y * End.X);
            var x3y4_y3x4Diff = (line.Start.X * line.End.Y) - (line.Start.Y * line.End.X);

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
        public Point2D? GetIntersectionPointOfLineSegments(Line2D line)
        {
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
            var denominator = ((Start.X - End.X) * (line.Start.Y - line.End.Y)) - ((Start.Y - End.Y) * (line.Start.X - line.End.X));

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
            var tNumerator = ((Start.X - line.Start.X) * (line.Start.Y - line.End.Y)) - ((Start.Y - line.Start.Y) * (line.Start.X - line.End.X));
            var t = tNumerator / denominator;

            if (t >= 0d && t <= 1d)
            {
                // x1 + t(x2 - x1)
                var x = Start.X + (t * (End.X - Start.X));

                // y1 + t(y2 - y1)
                var y = Start.Y + (t * (End.Y - Start.Y));

                return new Point2D(x, y);
            }

            // (x1 - x3) * (y1 - y2) - (y1 - y3) * (x1 - x2)
            var uNumerator = ((Start.X - line.Start.X) * (Start.Y - End.Y)) - ((Start.Y - line.Start.Y) * (Start.X - End.X));
            var u = uNumerator / denominator;

            if (u >= 0d && u <= 1d)
            {
                // x3 + u(x4 - x3)
                var x = line.Start.X + (u * (line.End.X - line.Start.X));

                // y3 + u(y4 - y3)
                var y = line.Start.Y + (u * (line.End.Y - line.Start.Y));

                return new Point2D(x, y);
            }

            return null;
        }

        /// <summary>
        /// Returns true if this and the given instances are considered equal
        /// </summary>
        /// <param name="other">Instance to compare</param>
        /// <returns>True if instances are equal</returns>
        public bool Equals(Line2D other)
        {
            return (Start.Equals(other.Start) && End.Equals(other.End)) ||
                   (Start.Equals(other.End) && End.Equals(other.Start));
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
            return Start.X.GetHashCode() ^
                   Start.Y.GetHashCode() ^
                   End.X.GetHashCode() ^
                   End.Y.GetHashCode();
        }

        /// <summary>
        /// Returns a string representation of this object
        /// </summary>
        public override string ToString()
        {
            return $"({Start}, {End})";
        }

        /// <summary>
        /// Returns true if the two given values equate
        /// </summary>
        /// <param name="left">Left value</param>
        /// <param name="right">Right value</param>
        /// <returns>True if the two given values equate</returns>
        public static bool operator ==(Line2D left, Line2D right)
        {
            return left.Equals(right);
        }

        /// <summary>
        /// Returns true if the two given values do not equate
        /// </summary>
        /// <param name="left">Left value</param>
        /// <param name="right">Right value</param>
        /// <returns>True if the two given values do not equate</returns>
        public static bool operator !=(Line2D left, Line2D right)
        {
            return !(left == right);
        }

        #endregion Public Methods
    }
}
