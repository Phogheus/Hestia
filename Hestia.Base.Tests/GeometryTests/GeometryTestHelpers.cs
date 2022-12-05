using System;
using System.Linq;
using Hestia.Base.Geometry.Models;
using Hestia.Base.Geometry.Utilities;

namespace Hestia.Base.Tests.GeometryTests
{
    public static class GeometryTestHelpers
    {
        private const int MIN = -100;
        private const int MAX = 101;

        public static Point2D GetRandomIntegerPoint2D(int min = MIN, int max = MAX)
        {
            return new Point2D(Random.Shared.Next(min, max), Random.Shared.Next(min, max));
        }

        public static Point3D GetRandomIntegerPoint3D(int min = MIN, int max = MAX)
        {
            return new Point3D(Random.Shared.Next(min, max), Random.Shared.Next(min, max), Random.Shared.Next(min, max));
        }

        public static Line2D GetRandomIntegerLineSegment()
        {
            return new Line2D(GetRandomIntegerPoint2D(), GetRandomIntegerPoint2D());
        }

        public static Circle2D GetRandomIntegerCircle()
        {
            return new Circle2D(Random.Shared.Next(1, MAX));
        }

        public static Rectangle2D GetRandomIntegerRectangle()
        {
            return new Rectangle2D(GetRandomIntegerPoint2D(), GetRandomIntegerPoint2D());
        }

        public static Triangle2D GetRandomTriangle2D()
        {
            return new Triangle2D(GetRandomIntegerPoint2D(), GetRandomIntegerPoint2D(), GetRandomIntegerPoint2D());
        }

        public static Polygon2D GetRandomPolygon2D(int pointCount = 5)
        {
            pointCount = Math.Max(pointCount, GeometryConstants.MINIMUM_POINT_COUNT_FOR_POLYGON);

            var points = Enumerable.Range(0, pointCount).Select(x => GetRandomIntegerPoint2D()).ToArray();

            return new Polygon2D(points);
        }
    }
}
