using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Hestia.Base.Geometry.Enums;
using Hestia.Base.Geometry.Models;

namespace Hestia.Base.Geometry.Utilities
{
    /// <summary>
    /// Generator class for creating the Delaunay/Voronoi dual graph 
    /// </summary>
    public class DelaunayVoronoi
    {
        #region Fields

        private readonly Rectangle2D _bounds;

        #endregion Fields

        #region Properties

        /// <summary>
        /// Delaunay triangles
        /// </summary>
        public Triangle2D[] DelaunayTriangles { get; private set; } = Array.Empty<Triangle2D>();

        /// <summary>
        /// Returns only distinct edges from <see cref="DelaunayTriangles"/> if only edges are needed
        /// </summary>
        public Line2D[] DelaunayTriangleDistinctEdges { get; private set; } = Array.Empty<Line2D>();

        /// <summary>
        /// Voronoi diagram polygons
        /// </summary>
        public Polygon2D[] VoronoiPolygons { get; private set; } = Array.Empty<Polygon2D>();

        /// <summary>
        /// Returns only distinct edges from <see cref="VoronoiPolygons"/> if only edges are needed
        /// </summary>
        public Line2D[] VoronoiPolygonDistinctEdges { get; private set; } = Array.Empty<Line2D>();

        #endregion Properties

        #region Constructors

        /// <summary>
        /// Constructor taking a the plane dimensions to generate against
        /// </summary>
        /// <param name="planeWidth">Width</param>
        /// <param name="planeHeight">Height</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if either <paramref name="planeWidth"/> or  <paramref name="planeHeight"/> is &lt;= 0</exception>
        private DelaunayVoronoi(double planeWidth, double planeHeight)
        {
            if (planeWidth <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(planeWidth));
            }

            if (planeHeight <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(planeHeight));
            }

            var topLeft = new Point2D(0, planeHeight);
            var bottomRight = new Point2D(planeWidth, 0);
            _bounds = new Rectangle2D(topLeft, bottomRight);
        }

        #endregion Constructors

        #region Public Methods

        /// <summary>
        /// Performs the Bowyer–Watson algorithm, computing the Delaunay triangulation
        /// and Voronoi diagram, and returns the results
        /// </summary>
        /// <param name="planeWidth">Width of the plane</param>
        /// <param name="planeHeight">Height of the plane</param>
        /// <param name="pointCount">Points to generate</param>
        /// <returns>Results</returns>
        /// <see Href="https://en.wikipedia.org/wiki/Bowyer%E2%80%93Watson_algorithm"/>
        public static DelaunayVoronoi GenerateBowyerWatsonResult(double planeWidth, double planeHeight, int pointCount = 100)
        {
            var returnObject = new DelaunayVoronoi(planeWidth, planeHeight);

            pointCount = Math.Max(0, pointCount);

            var stopwatch = new Stopwatch();
            stopwatch.Start();

            // Generate random points
            var points = GenerateRandomPoints(pointCount, (int)returnObject._bounds.Width, (int)returnObject._bounds.Height);

            stopwatch.Stop();
            Console.WriteLine($"Generated {pointCount} points in {stopwatch.ElapsedMilliseconds}ms");

            // Get our starting triangles
            var triangles = new List<Triangle2D>
            {
                new Triangle2D(new Point2D[] { returnObject._bounds.BottomLeft, returnObject._bounds.TopLeft, returnObject._bounds.TopRight }),
                new Triangle2D(new Point2D[] { returnObject._bounds.BottomLeft, returnObject._bounds.TopRight, returnObject._bounds.BottomRight })
            };

            stopwatch.Restart();

            // Perform Bowyer-Watson; first we do Delaunay triangluation
            returnObject.DelaunayTriangles = Triangulate(triangles, points);
            stopwatch.Stop();
            Console.WriteLine($"Performed Delaunay triangluation in {stopwatch.ElapsedMilliseconds}ms");

            // Get distinct edges for better time on operations such as drawing
            returnObject.DelaunayTriangleDistinctEdges = returnObject.DelaunayTriangles.SelectMany(x => x.Edges).Distinct().ToArray();
            stopwatch.Restart();

            // Then we derive the Voronoi pattern
            var voronoiLines = DeriveVoronoi(returnObject.DelaunayTriangles);
            stopwatch.Stop();
            Console.WriteLine($"Derived Voronoi pattern in {stopwatch.ElapsedMilliseconds}ms");

            // Get distinct edges for better time on operations such as drawing
            returnObject.VoronoiPolygonDistinctEdges = voronoiLines.Distinct().ToArray();

            // TODO: Set these
            returnObject.VoronoiPolygons = Array.Empty<Polygon2D>();

            return returnObject;
        }

        /// <summary>
        /// Attempts to triangulate a given polygon at a given point
        /// </summary>
        /// <param name="polygon">Polygon to triangulate</param>
        /// <param name="triangulationPoint">Point to triangulate around</param>
        public static Triangle2D[] TriangulatePolygonAtPoint(Polygon2D? polygon, Point2D? triangulationPoint)
        {
            if (polygon == null || triangulationPoint == null || !polygon.ContainsPoint(triangulationPoint))
            {
                return Array.Empty<Triangle2D>();
            }

            // Create encompassing triangle of polygon
            var point1 = new Point2D(polygon.Bounds.BottomLeft.X - polygon.Bounds.Height, polygon.Bounds.BottomLeft.Y);
            var point3 = new Point2D(polygon.Bounds.BottomRight.X + polygon.Bounds.Height, polygon.Bounds.BottomRight.Y);

            var line1 = new Line2D(point1, polygon.Bounds.TopLeft);
            var line2 = new Line2D(point3, polygon.Bounds.TopRight);

            var point2 = line1.GetIntersectionPointOfLines(line2);

            var bigTriangle = new Triangle2D(point1, point2, point3);

            var triangulation = Triangulate(new Triangle2D[] { bigTriangle }, polygon.Points);

            return triangulation.Where(x => polygon.ContainsPoint(x.CentroidPoint)).ToArray();
        }

        #endregion Public Methods

        #region Private Methods

        private static Point2D[] GenerateRandomPoints(int pointCount, int maxX, int maxY)
        {
            return Enumerable.Range(0, pointCount).Select(x =>
            {
                var xPos = Random.Shared.Next(0, maxX);
                var yPos = Random.Shared.Next(0, maxY);
                return new Point2D(xPos, yPos);
            }).ToArray();
        }

        private static Line2D[] DeriveVoronoi(Triangle2D[] triangulationResults)
        {
            var voronoiEdges = new List<Line2D>();

            // TODO: if triangles are "aware" of their neighbors, this look up will be much faster

            for (var i = 0; i < triangulationResults.Length; i++)
            {
                var triangleLeft = triangulationResults[i];

                for (var j = 0; j < triangulationResults.Length; j++)
                {
                    if (i == j)
                    {
                        continue;
                    }

                    var triangleRight = triangulationResults[j];

                    if (triangleLeft.SharesEdgeWithTriangle(triangleRight))
                    {
                        var circumcenterLeft = triangleLeft.Circumcircle.CenterPoint;
                        var circumcenterRight = triangleRight.Circumcircle.CenterPoint;

                        if (circumcenterLeft.Distance(circumcenterRight) > 0)
                        {
                            voronoiEdges.Add(new Line2D(circumcenterLeft, circumcenterRight));
                        }
                    }
                }
            }

            return voronoiEdges.ToArray();
        }

        private static Triangle2D[] Triangulate(IEnumerable<Triangle2D> startingTriangles, Point2D[] polygonPoints)
        {
            // Add our first two triangles
            var triangles = new List<Triangle2D>(startingTriangles);

            // Triangulate with each point
            foreach (var point in polygonPoints)
            {
                // Find all "bad triangles". That is, find all triangles whose circumcircle contains
                // the current point in question.
                var badTriangles = triangles.Where(x => x.IsPointInsideCircumcircle(point)).ToList();

                // Remove each bad triangle from the list of triangles
                badTriangles.ForEach(x => triangles.Remove(x));

                // From the collection of "bad triangles", get all edges that do not overlap with another edge
                // such that all we're left with is polygonal holes. Then, fill the polygonal holes with newly
                // created triangles based on the current point.
                var newTriangulations = badTriangles
                    .SelectMany(x => x.Edges)
                    .GroupBy(x => x)
                    .Where(x => x.Count() == 1) // Non-overlapping edges will have only 1 grouped member
                    .Select(x => x.First())
                    .Where(x => x.Start != point &&
                                x.End != point &&
                                x.GetOrientationOfPoint(point) != PointOrientationType.Colinear) // Edge case to handle slivers
                    .Select(x => new Triangle2D(new Point2D[] { point, x.Start, x.End }))
                    .ToList();

                triangles.AddRange(newTriangulations);
            }

            return triangles.ToArray();
        }

        #endregion Private Methods
    }
}
