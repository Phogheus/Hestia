using System;
using System.Linq;
using System.Text.Json.Serialization;
using Hestia.Base.Geometry.Enums;
using Hestia.Base.Geometry.Utilities;
using Hestia.Base.Utilities;

namespace Hestia.Base.Geometry.Models
{
    /// <summary>
    /// Defines a two-dimensional triangle in 2D space
    /// </summary>
    public readonly struct Triangle2D : IEquatable<Triangle2D>
    {
        #region Fields

        private const string INVALID_POINT_COUNT_ERROR = "A triangle must have no more and no less than three points.";
        private const string NON_DISTINCT_POINTS_ERROR = "A triangle must have three distinct points.";
        private const string CANNOT_BE_COLINEAR_ERROR = "Triangle cannot be made of only colinear points.";

        #endregion Fields

        #region Properties

        /// <summary>
        /// Returns the points of the triangle
        /// </summary>
        public Point2D[] Points { get; }

        /// <summary>
        /// Returns the edges of the triangle
        /// </summary>
        [JsonIgnore]
        public Line2D[] Edges { get; }

        /// <summary>
        /// Returns the area of the triangle
        /// </summary>
        [JsonIgnore]
        public double Area { get; }

        /// <summary>
        /// Returns the circumcircle of the triangle
        /// </summary>
        [JsonIgnore]
        public Circle2D Circumcircle { get; }

        /// <summary>
        /// Returns the bounds of the triangle
        /// </summary>
        [JsonIgnore]
        public Rectangle2D Bounds { get; }

        #endregion Properties

        #region Constructors

        /// <summary>
        /// Constructor creating a triangle from three points
        /// </summary>
        /// <param name="point1">Point 1</param>
        /// <param name="point2">Point 2</param>
        /// <param name="point3">Point 3</param>
        public Triangle2D(Point2D point1, Point2D point2, Point2D point3)
            : this(new Point2D[] { point1, point2, point3 })
        {
        }

        /// <summary>
        /// Constructor creating a triangle from points
        /// </summary>
        /// <param name="points">Points defining the triangle</param>
        [JsonConstructor]
        public Triangle2D(Point2D[] points)
        {
            Points = GetOrderedPoints(points);
            Edges = GetEdgesFromPoints(points);
            Area = GetAreaFromSides(Edges);
            Circumcircle = GetCircumcircleFromPoints(points);
            Bounds = GeometryUtilities.GetBoundsFromPoints(points);
        }

        #endregion Constructors

        #region Public Methods

        /// <summary>
        /// Returns true if the given point is inside the circumcircle
        /// </summary>
        /// <param name="point">Point to check</param>
        /// <returns>True if the given point is inside the circumcircle</returns>
        public bool IsPointInsideCircumcircle(Point2D point)
        {
            return Circumcircle.IsPointInCircle(point);
        }

        /// <summary>
        /// Returns true if this and the given triangle share an edge
        /// </summary>
        /// <param name="otherTriangle">Triangle to check</param>
        /// <returns>True if this and the given triangle share an edge</returns>
        public bool SharesEdgeWithTriangle(Triangle2D otherTriangle)
        {
            return Edges.Any(x => otherTriangle.Edges.Any(y => x.Equals(y)));
        }

        /// <summary>
        /// Returns true if this and the given instances are considered equal
        /// </summary>
        /// <param name="other">Instance to compare</param>
        /// <returns>True if instances are equal</returns>
        public bool Equals(Triangle2D other)
        {
            return CompareUtility.EnumerablesAreEqual(Points, other.Points);
        }

        /// <summary>
        /// Returns true if this and the given instances are considered equal
        /// </summary>
        /// <param name="obj">Instance to compare</param>
        /// <returns>True if instances are equal</returns>
        public override bool Equals(object? obj)
        {
            return obj is Triangle2D d && Equals(d);
        }

        /// <summary>
        /// Returns the hash code for this instance
        /// </summary>
        /// <returns>Hash code</returns>
        public override int GetHashCode()
        {
            return Points.GetHashCode() ^ Area.GetHashCode();
        }

        /// <summary>
        /// Returns true if the two given values equate
        /// </summary>
        /// <param name="left">Left value</param>
        /// <param name="right">Right value</param>
        /// <returns>True if the two given values equate</returns>
        public static bool operator ==(Triangle2D left, Triangle2D right)
        {
            return left.Equals(right);
        }

        /// <summary>
        /// Returns true if the two given values do not equate
        /// </summary>
        /// <param name="left">Left value</param>
        /// <param name="right">Right value</param>
        /// <returns>True if the two given values do not equate</returns>
        public static bool operator !=(Triangle2D left, Triangle2D right)
        {
            return !(left == right);
        }

        #endregion Public Methods

        #region Private Methods

        /// <summary>
        /// Ensures the points given in the constructor are in clockwise order
        /// </summary>
        private static Point2D[] GetOrderedPoints(Point2D[] points)
        {
            if ((points?.Length ?? 0) != 3)
            {
                throw new ArgumentOutOfRangeException(nameof(points), INVALID_POINT_COUNT_ERROR);
            }

            if (points![0] == points[1] || points[0] == points[2])
            {
                throw new InvalidOperationException(NON_DISTINCT_POINTS_ERROR);
            }

            var testLine = new Line2D(points[0], points[1]);
            var orientation = testLine.GetOrientationOfPoint(points[2]);

            if (orientation == PointOrientationType.Colinear)
            {
                throw new InvalidOperationException(CANNOT_BE_COLINEAR_ERROR);
            }

            // Ensure clockwise orientation
            return orientation == PointOrientationType.Clockwise
                ? points
                : (new Point2D[] { points[0], points[2], points[1] });
        }

        /// <summary>
        /// Converts the points given in the constructor to edges
        /// </summary>
        private static Line2D[] GetEdgesFromPoints(Point2D[] points)
        {
            return new Line2D[]
            {
                new Line2D(points[0], points[1]),
                new Line2D(points[1], points[2]),
                new Line2D(points[2], points[0])
            };
        }

        /// <summary>
        /// Returns the area calculation for a triangle with 3 known sides
        /// </summary>
        private static double GetAreaFromSides(Line2D[] edges)
        {
            // Heron's Formula for area
            var distA = edges[0].Length;
            var distB = edges[1].Length;
            var distC = edges[2].Length;
            var s = (distA + distB + distC) / 2d;

            return Math.Sqrt(s * (s - distA) * (s - distB) * (s - distC));
        }

        /// <summary>
        /// Returns the circumcircle of the points given in the constructor 
        /// </summary>
        private static Circle2D GetCircumcircleFromPoints(Point2D[] points)
        {
            var circumcenter = GetCircumcenterPoint(points, out var radius);
            return new Circle2D(radius, circumcenter);
        }

        /// <summary>
        /// Returns the circumcenter point and radius of circumcircle using
        /// the points given in the constructor
        /// </summary>
        private static Point2D GetCircumcenterPoint(Point2D[] points, out double radius)
        {
            var A = points[0];
            var B = points[1];
            var C = points[2];

            var d = 2d * ((A.X * (B.Y - C.Y)) + (B.X * (C.Y - A.Y)) + (C.X * (A.Y - B.Y)));

            if (d == 0d)
            {
                radius = 0d;
                return new Point2D();
            }

            var x = ((A.MagnitudeSquared * (B.Y - C.Y)) + (B.MagnitudeSquared * (C.Y - A.Y)) + (C.MagnitudeSquared * (A.Y - B.Y))) / d;
            var y = ((A.MagnitudeSquared * (C.X - B.X)) + (B.MagnitudeSquared * (A.X - C.X)) + (C.MagnitudeSquared * (B.X - A.X))) / d;

            var circumcenter = new Point2D(x, y);
            radius = A.Distance(circumcenter);

            return circumcenter;
        }

        #endregion Private Methods
    }
}
