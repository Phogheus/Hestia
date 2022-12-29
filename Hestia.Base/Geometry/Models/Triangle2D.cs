using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using Hestia.Base.Geometry.Utilities;

namespace Hestia.Base.Geometry.Models
{
    /// <summary>
    /// Defines a two-dimensional triangle in 2D space
    /// </summary>
    public sealed class Triangle2D : IEquatable<Triangle2D>
    {
        #region Fields

        private readonly Point2D[] _points;

        private Line2D[]? _edges;
        private double? _area;
        private Circle2D? _circumcircle;
        private Point2D? _centroidPoint;
        private Rectangle2D? _bounds;

        #endregion Fields

        #region Properties

        /// <summary>
        /// Returns the point at index if index is valid
        /// </summary>
        /// <param name="index">Index between 0 and 2</param>
        /// <returns>Point at index or null</returns>
        public Point2D? this[int index]
        {
            get => index >= 0 && index <= 2 ? _points[index] : null;
            set
            {
                if (index >= 0 && index <= 2)
                {
                    if (_points[index] != value)
                    {
                        _points[index] = value ?? Point2D.Zero;
                        _points[index]._onPointChanged = () => ResetDerivedValues();
                        ResetDerivedValues();
                    }
                }
            }
        }

        /// <summary>
        /// Returns the underlying points of this triangle
        /// </summary>
        public Point2D[] Points => GetPoints();

        /// <summary>
        /// Returns the edges of the triangle
        /// </summary>
        [JsonIgnore]
        public Line2D[] Edges => _edges ??= GetEdges();

        /// <summary>
        /// Returns the area of the triangle
        /// </summary>
        [JsonIgnore]
        public double Area => _area ??= GetArea();

        /// <summary>
        /// Returns the circumcircle of the triangle
        /// </summary>
        [JsonIgnore]
        public Circle2D Circumcircle => _circumcircle ??= GetCircumcircle();

        /// <summary>
        /// Centroid point of all defining points
        /// </summary>
        [JsonIgnore]
        public Point2D CentroidPoint => _centroidPoint ??= GeometryUtilities.GetCentroidPoint(_points);

        /// <summary>
        /// Returns the bounds of the triangle
        /// </summary>
        [JsonIgnore]
        public Rectangle2D Bounds => _bounds ??= GeometryUtilities.GetBoundsFromPoints(_points);

        #endregion Properties

        #region Constructors

        /// <summary>
        /// Constructor creating a triangle from three points
        /// </summary>
        /// <param name="point1">Point 1</param>
        /// <param name="point2">Point 2</param>
        /// <param name="point3">Point 3</param>
        public Triangle2D(Point2D? point1, Point2D? point2, Point2D? point3)
            : this(new Point2D?[] { point1, point2, point3 })
        {
        }

        /// <summary>
        /// Constructor creating a triangle from points
        /// </summary>
        /// <param name="points">Points defining the triangle</param>
        [JsonConstructor]
        public Triangle2D(Point2D?[]? points)
        {
            _points = GeometryUtilities.ValidateAndOrderPointsForPolygon(points);

            foreach (var point in _points)
            {
                point._onPointChanged = () => ResetDerivedValues();
            }
        }

        #endregion Constructors

        #region Public Methods

        /// <summary>
        /// Returns true if the given point is inside the circumcircle
        /// </summary>
        /// <param name="point">Point to check</param>
        /// <returns>True if the given point is inside the circumcircle</returns>
        public bool IsPointInsideCircumcircle(Point2D? point)
        {
            return Circumcircle.IsPointInCircle(point);
        }

        /// <summary>
        /// Returns true if this and the given triangle share an edge
        /// </summary>
        /// <param name="otherTriangle">Triangle to check</param>
        /// <returns>True if this and the given triangle share an edge</returns>
        public bool SharesEdgeWithTriangle(Triangle2D? otherTriangle)
        {
            return otherTriangle is not null && Edges.Any(x => otherTriangle.Edges.Any(y => x.Equals(y)));
        }

        /// <summary>
        /// Returns true if this and the given instances are considered equal
        /// </summary>
        /// <param name="other">Instance to compare</param>
        /// <returns>True if instances are equal</returns>
        public bool Equals(Triangle2D? other)
        {
            return other is not null &&
                   _points[0] == other[0] &&
                   _points[1] == other[1] &&
                   _points[2] == other[2];
        }

        /// <summary>
        /// Returns true if this and the given instances are considered equal
        /// </summary>
        /// <param name="obj">Instance to compare</param>
        /// <returns>True if instances are equal</returns>
        public override bool Equals(object? obj)
        {
            return obj is Triangle2D triangle && Equals(triangle);
        }

        /// <summary>
        /// Returns the hash code for this instance
        /// </summary>
        /// <returns>Hash code</returns>
        public override int GetHashCode()
        {
            return _points[0].GetHashCode() ^
                   _points[1].GetHashCode() ^
                   _points[2].GetHashCode();
        }

        /// <summary>
        /// Returns true if the two given values equate
        /// </summary>
        /// <param name="left">Left value</param>
        /// <param name="right">Right value</param>
        /// <returns>True if the two given values equate</returns>
        public static bool operator ==(Triangle2D? left, Triangle2D? right)
        {
            return (left is null && right is null) || (left?.Equals(right) ?? false);
        }

        /// <summary>
        /// Returns true if the two given values do not equate
        /// </summary>
        /// <param name="left">Left value</param>
        /// <param name="right">Right value</param>
        /// <returns>True if the two given values do not equate</returns>
        public static bool operator !=(Triangle2D? left, Triangle2D? right)
        {
            return !(left == right);
        }

        #endregion Public Methods

        #region Private Methods

        private Point2D[] GetPoints()
        {
            return new Point2D[]
            {
                new Point2D(_points[0].X, _points[0].Y),
                new Point2D(_points[1].X, _points[1].Y),
                new Point2D(_points[2].X, _points[2].Y)
            };
        }

        private Line2D[] GetEdges()
        {
            return new Line2D[]
            {
                new Line2D(_points[0], _points[1]),
                new Line2D(_points[1], _points[2]),
                new Line2D(_points[2], _points[0])
            };
        }

        private double GetArea()
        {
            var edges = Edges;

            // Heron's Formula for area
            var distA = edges[0].Length;
            var distB = edges[1].Length;
            var distC = edges[2].Length;
            var s = (distA + distB + distC) / 2d;

            return Math.Sqrt(s * (s - distA) * (s - distB) * (s - distC));
        }

        private Circle2D GetCircumcircle()
        {
            var circumcenter = GetCircumcenterPoint(_points, out var radius);
            return new Circle2D(radius, circumcenter);
        }

        /// <summary>
        /// Resetting derived values sets said values to null so that they
        /// will be (re)calculated on next access. This method is called when
        /// the underlying values said derived values are based on, are changed.
        /// </summary>
        private void ResetDerivedValues()
        {
            _edges = null;
            _area = null;
            _circumcircle = null;
            _centroidPoint = null;
            _bounds = null;
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
