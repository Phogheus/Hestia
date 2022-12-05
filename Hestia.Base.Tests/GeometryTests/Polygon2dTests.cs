using System;
using System.Linq;
using System.Text.Json;
using Hestia.Base.Geometry.Models;
using NUnit.Framework;

// TODO: ContainsPoint, TriangulateAtPoint

namespace Hestia.Base.Tests.GeometryTests
{
    public class Polygon2dTests
    {
        [Test]
        public void ConstructorTests()
        {
            _ = Assert.Throws<ArgumentOutOfRangeException>(() => new Polygon2D(null));
            _ = Assert.Throws<ArgumentOutOfRangeException>(() => new Polygon2D(new Point2D?[] { null, Point2D.Zero, Point2D.Zero }));
            _ = Assert.Throws<ArgumentOutOfRangeException>(() => new Polygon2D(new Point2D?[] { Point2D.Zero, null, Point2D.Zero }));
            _ = Assert.Throws<ArgumentOutOfRangeException>(() => new Polygon2D(new Point2D?[] { Point2D.Zero, Point2D.Zero, null }));
            _ = Assert.Throws<ArgumentOutOfRangeException>(() => new Polygon2D(new Point2D[] { Point2D.Zero, Point2D.Zero, Point2D.Up }));
            _ = Assert.Throws<InvalidOperationException>(() => new Polygon2D(new Point2D[] { Point2D.Zero, Point2D.Right, Point2D.Right * 2 }));
            Assert.DoesNotThrow(() => new Polygon2D(new Point2D[] { Point2D.Zero, Point2D.Up, Point2D.Right }));
        }

        [Test]
        public void OrderPointsTests()
        {
            static Polygon2D[] buildTestPolygons(Point2D[] points)
            {
                // All permutations for funsies
                return new Polygon2D[]
                {
                    new Polygon2D(new Point2D[] { points[0], points[1], points[2], points[3] }),
                    new Polygon2D(new Point2D[] { points[0], points[1], points[3], points[2] }),
                    new Polygon2D(new Point2D[] { points[0], points[2], points[1], points[3] }),
                    new Polygon2D(new Point2D[] { points[0], points[2], points[3], points[1] }),
                    new Polygon2D(new Point2D[] { points[0], points[3], points[1], points[2] }),
                    new Polygon2D(new Point2D[] { points[0], points[3], points[2], points[1] }),

                    new Polygon2D(new Point2D[] { points[1], points[0], points[2], points[3] }),
                    new Polygon2D(new Point2D[] { points[1], points[0], points[3], points[2] }),
                    new Polygon2D(new Point2D[] { points[1], points[2], points[0], points[3] }),
                    new Polygon2D(new Point2D[] { points[1], points[2], points[3], points[0] }),
                    new Polygon2D(new Point2D[] { points[1], points[3], points[0], points[2] }),
                    new Polygon2D(new Point2D[] { points[1], points[3], points[2], points[0] }),

                    new Polygon2D(new Point2D[] { points[2], points[1], points[0], points[3] }),
                    new Polygon2D(new Point2D[] { points[2], points[1], points[3], points[0] }),
                    new Polygon2D(new Point2D[] { points[2], points[0], points[1], points[3] }),
                    new Polygon2D(new Point2D[] { points[2], points[0], points[3], points[1] }),
                    new Polygon2D(new Point2D[] { points[2], points[3], points[1], points[0] }),
                    new Polygon2D(new Point2D[] { points[2], points[3], points[0], points[1] }),

                    new Polygon2D(new Point2D[] { points[3], points[1], points[2], points[0] }),
                    new Polygon2D(new Point2D[] { points[3], points[1], points[0], points[2] }),
                    new Polygon2D(new Point2D[] { points[3], points[2], points[1], points[0] }),
                    new Polygon2D(new Point2D[] { points[3], points[2], points[0], points[1] }),
                    new Polygon2D(new Point2D[] { points[3], points[0], points[1], points[2] }),
                    new Polygon2D(new Point2D[] { points[3], points[0], points[2], points[1] }),
                };
            };

            // Square
            var expected = new Point2D[] { new Point2D(0, 1), new Point2D(1, 1), new Point2D(1, 0), new Point2D(0, 0) };
            var polygons = buildTestPolygons(expected);

            Assert.That(polygons.All(x => x.Points[0] == expected[0] && x.Points[1] == expected[1] && x.Points[2] == expected[2] && x.Points[3] == expected[3]));
        }

        [Test]
        public void EqualityTests()
        {
            var polygon1 = GeometryTestHelpers.GetRandomPolygon2D();
            var polygon2 = new Polygon2D(polygon1.Points);
            var polygon3 = new Polygon2D(polygon1.Points.Concat(new Point2D[] { GeometryTestHelpers.GetRandomIntegerPoint2D() }).ToArray());

            Assert.That(polygon1, Is.EqualTo(polygon2));
            Assert.That(polygon1, Is.Not.EqualTo(polygon3));
        }

        [Test]
        public void SerializationTests()
        {
            var polygon = GeometryTestHelpers.GetRandomPolygon2D();
            var serialized = JsonSerializer.Serialize(polygon);
            var deserialized = JsonSerializer.Deserialize<Polygon2D>(serialized);
            Assert.That(polygon, Is.EqualTo(deserialized));
        }

        [Test]
        public void PositionChangeTests()
        {
            var trianglePolygon = new Polygon2D(new Point2D[] { new Point2D(0, 0), new Point2D(1, 1), new Point2D(2, 0) });
            Assert.Multiple(() =>
            {
                Assert.That(trianglePolygon.Bounds.Width, Is.EqualTo(2));
                Assert.That(trianglePolygon.Bounds.Height, Is.EqualTo(1));
            });

            trianglePolygon[0] = new Point2D(-1, 0);
            Assert.Multiple(() =>
            {
                Assert.That(trianglePolygon.Bounds.Width, Is.EqualTo(3));
                Assert.That(trianglePolygon.Bounds.Height, Is.EqualTo(1));
            });

            trianglePolygon[1] = new Point2D(2, 2);
            Assert.Multiple(() =>
            {
                Assert.That(trianglePolygon.Bounds.Width, Is.EqualTo(3));
                Assert.That(trianglePolygon.Bounds.Height, Is.EqualTo(2));
            });

            trianglePolygon[2] = new Point2D(4, -8);
            Assert.Multiple(() =>
            {
                Assert.That(trianglePolygon.Bounds.Width, Is.EqualTo(5));
                Assert.That(trianglePolygon.Bounds.Height, Is.EqualTo(10));
            });

            trianglePolygon[0] = null;
            Assert.Multiple(() =>
            {
                Assert.That(trianglePolygon.Bounds.Width, Is.EqualTo(4));
                Assert.That(trianglePolygon.Bounds.Height, Is.EqualTo(10));
            });

            // Confirm .Points won't change actual points
            trianglePolygon.Points[0] = new Point2D(-200, 200);
            Assert.Multiple(() =>
            {
                Assert.That(trianglePolygon.Bounds.Width, Is.EqualTo(4));
                Assert.That(trianglePolygon.Bounds.Height, Is.EqualTo(10));
            });
        }
    }
}
