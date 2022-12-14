using System;
using System.Linq;
using System.Text.Json.Serialization;
using Hestia.Base.Geometry.Utilities;
using Hestia.Base.RandomGenerators;
using Hestia.Base.Utilities;

namespace Hestia.Base.Geometry.Models
{
    /// <summary>
    /// Defines a two-dimensional polygon without holes
    /// </summary>
    public sealed class Polygon2D : IEquatable<Polygon2D>
    {
        #region Fields

        private readonly Point2D[] _points;

        private Line2D[]? _edges;
        private double? _area;
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
            get => index >= 0 && index < _points.Length ? _points[index] : null;
            set
            {
                if (index >= 0 && index < _points.Length)
                {
                    _points[index] = value ?? Point2D.Zero;
                    SetDirty();
                }
            }
        }

        /// <summary>
        /// Returns all the underlying points of this polygon
        /// </summary>
        public Point2D[] Points => ClonePoints();

        /// <summary>
        /// Returns all edges that create this polygon
        /// </summary>
        [JsonIgnore]
        public Line2D[] Edges => GetEdges();

        /// <summary>
        /// Returns the area of the triangle
        /// </summary>
        [JsonIgnore]
        public double Area => GetArea();

        /// <summary>
        /// Centroid point of all defining points
        /// </summary>
        [JsonIgnore]
        public Point2D CentroidPoint => GetCentroidPoint();

        /// <summary>
        /// Returns the bounds this polygon fits within
        /// </summary>
        [JsonIgnore]
        public Rectangle2D Bounds => GetBounds();

        #endregion Properties

        #region Constructors

        /// <summary>
        /// Constructor for a polygon taking defining points
        /// </summary>
        /// <param name="points">Defining points</param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Thrown if <paramref name="points"/> has less than <see cref="GeometryConstants.MINIMUM_POINT_COUNT_FOR_POLYGON"/> points
        /// </exception>
        [JsonConstructor]
        public Polygon2D(Point2D?[]? points)
        {
            _points = GeometryUtilities.ValidateAndOrderPointsForPolygon(points);
        }

        #endregion Constructors

        #region Public Methods

        /// <summary>
        /// Returns true if the given point is considered inside this polygon
        /// </summary>
        /// <param name="point">Point to check</param>
        /// <returns>True if polygon contains point</returns>
        public bool ContainsPoint(Point2D? point)
        {
            return point != null && GeometryUtilities.IsPointInPolygon(this, point);
        }

        /// <summary>
        /// Attempts to triangulate this polygon using the given point
        /// </summary>
        /// <param name="point">Point to triangulate around</param>
        /// <returns></returns>
        public Triangle2D[] TriangulateAtPoint(Point2D? point)
        {
            return DelaunayVoronoi.TriangulatePolygonAtPoint(this, point);
        }

        /// <summary>
        /// Returns true if this and the given instances are considered equal
        /// </summary>
        /// <param name="other">Instance to compare</param>
        /// <returns>True if instances are equal</returns>
        public bool Equals(Polygon2D? other)
        {
            return CompareUtility.EnumerablesAreEqual(Points, other?.Points);
        }

        /// <summary>
        /// Returns true if this and the given instances are considered equal
        /// </summary>
        /// <param name="obj">Instance to compare</param>
        /// <returns>True if instances are equal</returns>
        public override bool Equals(object? obj)
        {
            return obj is Polygon2D d && Equals(d);
        }

        /// <summary>
        /// Returns the hash code for this instance
        /// </summary>
        /// <returns>Hash code</returns>
        public override int GetHashCode()
        {
            return Points.GetHashCode();
        }

        /// <summary>
        /// Returns true if the two given values equate
        /// </summary>
        /// <param name="left">Left value</param>
        /// <param name="right">Right value</param>
        /// <returns>True if the two given values equate</returns>
        public static bool operator ==(Polygon2D? left, Polygon2D? right)
        {
            return (left is null && right is null) || (left?.Equals(right) ?? false);
        }

        /// <summary>
        /// Returns true if the two given values do not equate
        /// </summary>
        /// <param name="left">Left value</param>
        /// <param name="right">Right value</param>
        /// <returns>True if the two given values do not equate</returns>
        public static bool operator !=(Polygon2D? left, Polygon2D? right)
        {
            return !(left == right);
        }

        #endregion Public Methods

        #region Private Methods

        private Point2D[] ClonePoints()
        {
            return _points.Select(x => new Point2D(x.X, x.Y)).ToArray();
        }

        private Line2D[] GetEdges()
        {
            MarkDirtyIfPointsAreDirty();

            return _edges ??= Enumerable.Range(0, _points.Length)
                .Select(x => x == _points.Length - 1 ? new Line2D(_points[x], _points[0]) : new Line2D(_points[x], _points[x + 1]))
                .ToArray();
        }

        private double GetArea()
        {
            MarkDirtyIfPointsAreDirty();

            if (_area == null)
            {
                // If triangle: Heron's Formula
                if (_points.Length == GeometryConstants.MINIMUM_POINT_COUNT_FOR_POLYGON)
                {
                    var edges = Edges;
                    var distA = edges[0].Length;
                    var distB = edges[1].Length;
                    var distC = edges[2].Length;
                    var s = (distA + distB + distC) / 2d;

                    _area = Math.Sqrt(s * (s - distA) * (s - distB) * (s - distC));
                }
                // Otherwise: get area of all composite triangles
                else
                {
                    _area = TriangulateAtPoint(_points[0]).Sum(x => x.Area);
                }
            }

            return _area!.Value;
        }

        private Point2D GetCentroidPoint()
        {
            MarkDirtyIfPointsAreDirty();

            return _centroidPoint ??= GeometryUtilities.GetCentroidPoint(_points);
        }

        private Rectangle2D GetBounds()
        {
            MarkDirtyIfPointsAreDirty();

            return _bounds ??= GeometryUtilities.GetBoundsFromPoints(_points);
        }

        private void SetDirty()
        {
            _edges = null;
            _bounds = null;
        }

        private void MarkDirtyIfPointsAreDirty()
        {
            if (_points.Any(x => x.IsDirty))
            {
                // Set ourselves as dirty now that we can acknowledge some underlying points changed
                SetDirty();

                // Reset dirty status for points
                for (var i = 0; i < _points.Length; i++)
                {
                    _points[i].IsDirty = false;
                }
            }
        }

        #endregion Private Methods
    }
}
