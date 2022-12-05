using System;
using System.Linq;
using System.Text.Json;
using Hestia.Base.Geometry.Models;
using NUnit.Framework;

// TODO: IsPointInsideCircumcircle, SharesEdgeWithTriangle

namespace Hestia.Base.Tests.GeometryTests
{
    internal class Triangle2dTests
    {
        [Test]
        public void ConstructorTests()
        {
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
            _ = Assert.Throws<ArgumentOutOfRangeException>(() => new Triangle2D(null, Point2D.Zero, Point2D.Zero));
            _ = Assert.Throws<ArgumentOutOfRangeException>(() => new Triangle2D(Point2D.Zero, null, Point2D.Zero));
            _ = Assert.Throws<ArgumentOutOfRangeException>(() => new Triangle2D(Point2D.Zero, Point2D.Zero, null));
            _ = Assert.Throws<InvalidOperationException>(() => new Triangle2D(Point2D.Zero, Point2D.Zero, Point2D.Up));
            _ = Assert.Throws<ArgumentOutOfRangeException>(() => new Triangle2D(null));
            _ = Assert.Throws<ArgumentOutOfRangeException>(() => new Triangle2D(new Point2D[] { null, Point2D.Zero, Point2D.Zero }));
            _ = Assert.Throws<ArgumentOutOfRangeException>(() => new Triangle2D(new Point2D[] { Point2D.Zero, null, Point2D.Zero }));
            _ = Assert.Throws<ArgumentOutOfRangeException>(() => new Triangle2D(new Point2D[] { Point2D.Zero, Point2D.Zero, null }));
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
            _ = Assert.Throws<InvalidOperationException>(() => new Triangle2D(new Point2D[] { Point2D.Zero, Point2D.Zero, Point2D.Up }));
            _ = Assert.Throws<InvalidOperationException>(() => new Triangle2D(new Point2D[] { Point2D.Zero, Point2D.Right, Point2D.Right * 2 }));
            Assert.DoesNotThrow(() => new Triangle2D(Point2D.Zero, Point2D.Up, Point2D.Right));
            Assert.DoesNotThrow(() => new Triangle2D(new Point2D[] { Point2D.Zero, Point2D.Up, Point2D.Right }));
        }

        [Test]
        public void OrderPointsTests()
        {
            static Triangle2D[] buildTestTris(Point2D[] points)
            {
                return new Triangle2D[]
                {
                    new Triangle2D(points[0], points[1], points[2]),
                    new Triangle2D(points[0], points[2], points[1]),
                    new Triangle2D(points[1], points[0], points[2]),
                    new Triangle2D(points[1], points[2], points[0]),
                    new Triangle2D(points[2], points[1], points[0]),
                    new Triangle2D(points[2], points[0], points[1])
                };
            };

            // Pyramid up
            var expected = new Point2D[] { new Point2D(0, 0), new Point2D(1, 1), new Point2D(2, 0) };
            var triangles = buildTestTris(expected);
            Assert.That(triangles.All(x => x[0] == expected[0] && x[1] == expected[1] && x[2] == expected[2]));

            // Pyramid down
            expected = new Point2D[] { new Point2D(0, 0), new Point2D(2, 0), new Point2D(1, -1) };
            triangles = buildTestTris(expected);
            Assert.That(triangles.All(x => x[0] == expected[0] && x[1] == expected[1] && x[2] == expected[2]));

            // Right angle top-left
            expected = new Point2D[] { new Point2D(0, 1), new Point2D(1, 1), new Point2D(0, 0) };
            triangles = buildTestTris(expected);
            Assert.That(triangles.All(x => x[0] == expected[0] && x[1] == expected[1] && x[2] == expected[2]));

            // Right angle bottom-left
            expected = new Point2D[] { new Point2D(0, 1), new Point2D(1, 0), new Point2D(0, 0) };
            triangles = buildTestTris(expected);
            Assert.That(triangles.All(x => x[0] == expected[0] && x[1] == expected[1] && x[2] == expected[2]));

            // Right angle top-right
            expected = new Point2D[] { new Point2D(0, 1), new Point2D(1, 1), new Point2D(1, 0) };
            triangles = buildTestTris(expected);
            Assert.That(triangles.All(x => x[0] == expected[0] && x[1] == expected[1] && x[2] == expected[2]));

            // Right angle bottom-right
            expected = new Point2D[] { new Point2D(0, 0), new Point2D(1, 1), new Point2D(1, 0) };
            triangles = buildTestTris(expected);
            Assert.That(triangles.All(x => x[0] == expected[0] && x[1] == expected[1] && x[2] == expected[2]));

            // Skewed to the right
            expected = new Point2D[] { new Point2D(0, 0), new Point2D(2, 1), new Point2D(1, 0) };
            triangles = buildTestTris(expected);
            Assert.That(triangles.All(x => x[0] == expected[0] && x[1] == expected[1] && x[2] == expected[2]));

            // Non-colinear sliver triangle
            expected = new Point2D[] { new Point2D(0, 0), new Point2D(10, -4), new Point2D(5, -3) };
            triangles = buildTestTris(expected);
            Assert.That(triangles.All(x => x[0] == expected[0] && x[1] == expected[1] && x[2] == expected[2]));

        }

        [Test]
        public void EqualityTests()
        {
            var point1 = GeometryTestHelpers.GetRandomIntegerPoint2D();
            var point2 = new Point2D(point1.X, point1.Y);
            var point3 = new Point2D(-point1.X, point1.Y);

            Assert.That(point1, Is.EqualTo(point2));
            Assert.That(point1, Is.Not.EqualTo(point3));
        }

        [Test]
        public void SerializationTests()
        {
            var point = GeometryTestHelpers.GetRandomIntegerPoint2D();
            var serialized = JsonSerializer.Serialize(point);
            var deserialized = JsonSerializer.Deserialize<Point2D>(serialized);
            Assert.That(point, Is.EqualTo(deserialized));
        }

        [Test]
        public void PositionChangeTests()
        {
            var triangle = new Triangle2D(new Point2D[] { new Point2D(0, 0), new Point2D(1, 1), new Point2D(2, 0) });
            Assert.Multiple(() =>
            {
                Assert.That(triangle.Bounds.Width, Is.EqualTo(2));
                Assert.That(triangle.Bounds.Height, Is.EqualTo(1));
            });

            triangle[0] = new Point2D(-1, 0);
            Assert.Multiple(() =>
            {
                Assert.That(triangle.Bounds.Width, Is.EqualTo(3));
                Assert.That(triangle.Bounds.Height, Is.EqualTo(1));
            });

            triangle[1] = new Point2D(2, 2);
            Assert.Multiple(() =>
            {
                Assert.That(triangle.Bounds.Width, Is.EqualTo(3));
                Assert.That(triangle.Bounds.Height, Is.EqualTo(2));
            });

            triangle[2] = new Point2D(4, -8);
            Assert.Multiple(() =>
            {
                Assert.That(triangle.Bounds.Width, Is.EqualTo(5));
                Assert.That(triangle.Bounds.Height, Is.EqualTo(10));
            });

            triangle[0] = null;
            Assert.Multiple(() =>
            {
                Assert.That(triangle.Bounds.Width, Is.EqualTo(4));
                Assert.That(triangle.Bounds.Height, Is.EqualTo(10));
            });
        }
    }
}
