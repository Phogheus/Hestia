using System;
using System.Collections.Generic;
using System.Linq;
using Hestia.Base.Geometry.Enums;
using Hestia.Base.Geometry.Models;

namespace Hestia.Base.Geometry.Utilities
{
    /// <summary>
    /// Defines general geometric utility methods
    /// </summary>
    public static class GeometryUtilities
    {
        #region Public Methods

        /// <summary>
        /// Returns the bounds the given set of points fits within
        /// </summary>
        /// <param name="points">Set of points</param>
        /// <returns>Bounds points fit within</returns>
        public static Rectangle2D GetBoundsFromPoints(IEnumerable<Point2D?>? points)
        {
            var validPoints = GetValidDistinctPoints(points);

            if (validPoints.Length == 0)
            {
                return new Rectangle2D();
            }

            var firstPoint = validPoints[0];
            var xMin = firstPoint.X;
            var xMax = firstPoint.X;
            var yMin = firstPoint.Y;
            var yMax = firstPoint.Y;

            foreach (var point in validPoints)
            {
                xMin = Math.Min(xMin, point.X);
                xMax = Math.Max(xMax, point.X);
                yMin = Math.Min(yMin, point.Y);
                yMax = Math.Max(yMax, point.Y);
            }

            var topLeft = new Point2D(xMin, yMax);
            var bottomRight = new Point2D(xMax, yMin);

            return new Rectangle2D(topLeft, bottomRight);
        }

        /// <summary>
        /// Returns the given points in clockwise order
        /// </summary>
        /// <param name="points">Points to orient</param>
        /// <remarks>
        /// If <paramref name="points"/> is null or has any null members, they will be removed before ordering occurs
        /// </remarks>
        /// <returns>Points in clockwise order</returns>
        public static Point2D[] OrientPointsClockwise(IEnumerable<Point2D?>? points)
        {
            var validPoints = GetValidPoints(points);

            if (validPoints.Length < 2)
            {
                return validPoints;
            }

            // Get the first point, which is the left most, up most point
            var firstPoint = validPoints.OrderBy(x => x!.X) // Order by left most X
                .GroupBy(x => x!.X) // Group by left most X if two points form a vertical line
                .Select(x => x.OrderByDescending(x => x!.Y).First()) // Order by up most Y
                .First()!;

            var remainingPoints = validPoints.Where(x => x != firstPoint).ToList();
            remainingPoints.Sort((x, y) => firstPoint.AngleInRadians(y) >= firstPoint.AngleInRadians(x) ? 1 : -1);

            remainingPoints.Insert(0, firstPoint);

            return remainingPoints.ToArray()!;
        }

        /// <summary>
        /// Orients the given points in a clockwise fashion, removing duplicates,
        /// and throws exceptions if not enough valid points remain to create a polygon,
        /// or the remaining points are all colinear. If no errors are found, the pruned
        /// and oriented points are returned.
        /// </summary>
        /// <param name="points"></param>
        /// <returns>Oriented points</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="points"/> has less than 3 valid points</exception>
        /// <exception cref="InvalidOperationException">Thrown if all <paramref name="points"/> are colinear with each other</exception>
        public static Point2D[] ValidateAndOrderPointsForPolygon(IEnumerable<Point2D?>? points)
        {
            // Orient points (removing invalid entries in the process) and prune duplicate points
            var orientedPoints = OrientPointsClockwise(points).Distinct().ToArray();

            // We need at least 3 distinct points to make a triangle
            if (orientedPoints.Length < GeometryConstants.MINIMUM_POINT_COUNT_FOR_POLYGON)
            {
                throw new ArgumentOutOfRangeException(nameof(points), GeometryConstants.ErrorMessages.POLYGON_MUST_HAVE_AT_LEAST_THREE_POINTS_ERROR);
            }
            // Ensure points are not all colinear
            else if (ArePointsAllColinear(points))
            {
                throw new InvalidOperationException(GeometryConstants.ErrorMessages.POLYGON_POINTS_CANNOT_BE_COLINEAR_ERROR);
            }

            return orientedPoints;
        }

        /// <summary>
        /// Returns true if all given points are colinear
        /// </summary>
        /// <param name="points">Points to compare</param>
        /// <remarks>
        /// If <paramref name="points"/> is null or has any null members, they will be removed before ordering occurs.
        /// 
        /// If there are not enough valid points remaining to create a polygon, this will return true so long as there are exactly 2 points,
        /// otherwise this will return true if all points are colinear.
        /// </remarks>
        /// <returns>True if points are all colinear</returns>
        public static bool ArePointsAllColinear(IEnumerable<Point2D?>? points)
        {
            var validPoints = GetValidPoints(points);

            // If we don't have enough points for a polygon, points are colinear if there's exactly 2
            if (validPoints.Length < GeometryConstants.MINIMUM_POINT_COUNT_FOR_POLYGON)
            {
                return validPoints.Length == 2;
            }

            var testLine = new Line2D(validPoints[0], validPoints[1]);

            return validPoints.Skip(2).All(x => testLine.GetOrientationOfPoint(x) == PointOrientationType.Colinear);
        }

        /// <summary>
        /// Returns the centroid point of the given set of points
        /// </summary>
        /// <param name="points">Points to use</param>
        /// <remarks>
        /// If collection is null or has no members, this will return an empty array.
        /// If only one valid member remains, it will be returned.
        /// </remarks>
        /// <returns>Centroid point</returns>
        public static Point2D GetCentroidPoint(IEnumerable<Point2D?>? points)
        {
            var validPoints = GetValidDistinctPoints(points);

            if (validPoints.Length == 0)
            {
                return Point2D.Zero;
            }
            else if (validPoints.Length == 1)
            {
                return validPoints[0];
            }

            var xPos = 0d;
            var yPos = 0d;

            foreach (var point in validPoints)
            {
                xPos += point.X;
                yPos += point.Y;
            }

            return new Point2D(xPos / validPoints.Length, yPos / validPoints.Length);
        }

        /// <summary>
        /// Returns true if the given point is inside the given polygon, or false if either is null
        /// </summary>
        /// <param name="polygon">Polygon to check</param>
        /// <param name="point">Point to check</param>
        /// <returns>True if point is in polygon</returns>
        public static bool IsPointInPolygon(Polygon2D? polygon, Point2D? point)
        {
            if (polygon == null || point == null)
            {
                return false;
            }

            var edges = polygon.Edges;
            var pointIsInside = false;

            for (var i = 0; i < edges.Length; i++)
            {
                var edge = edges[i];

                if (((edge.End.Y > point.Y) != (edge.Start.Y > point.Y)) &&
                    (point.X < ((edge.Start.X - edge.End.X) * (point.Y - edge.End.Y) / (edge.Start.Y - edge.End.Y)) + edge.End.X))
                {
                    pointIsInside = !pointIsInside;
                }
            }

            return pointIsInside;
        }

        /// <summary>
        /// Rotates a given point around an origin by some number of degrees
        /// </summary>
        /// <param name="origin">Origin to rorate around</param>
        /// <param name="angleInRadians">Angle in radians to rotate</param>
        /// <param name="point">Point to rotate</param>
        /// <remarks>
        /// If either point or origin is null, this will return null. If angle is less than
        /// or equal to zero, this will return point.
        /// </remarks>
        /// <returns>Rotated point</returns>
        public static Point2D? RotatePointAroundOrigin(Point2D? point, Point2D? origin, double angleInRadians)
        {
            if (point == null || origin == null)
            {
                return null;
            }
            else if (Math.Max(angleInRadians, 0) == 0)
            {
                return point;
            }

            return new Point2D((Math.Cos(angleInRadians) * (point.X - origin.X)) - (Math.Sin(angleInRadians) * (point.Y - origin.Y)) + origin.X,
                               (Math.Sin(angleInRadians) * (point.X - origin.X)) + (Math.Cos(angleInRadians) * (point.Y - origin.Y)) + origin.Y);
        }

        #endregion Public Methods

        #region Private Methods

        private static Point2D[] GetValidDistinctPoints(IEnumerable<Point2D?>? points)
        {
            return GetValidPoints(points).Distinct().ToArray();
        }

        private static Point2D[] GetValidPoints(IEnumerable<Point2D?>? points)
        {
            return points?.Where(x => x != null).Cast<Point2D>().ToArray() ?? Array.Empty<Point2D>();
        }

        #endregion Private Methods
    }
}
