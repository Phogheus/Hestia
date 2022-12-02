using System;
using Hestia.Base.Geometry.Models;

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
    }
}
