using System;
using System.Linq;
using System.Text.Json;
using Hestia.Base.Geometry.Models;
using NUnit.Framework;

namespace Hestia.Base.Tests.GeometryTests
{
    public class Circle2dTests
    {
        [Test]
        public void ConstructorTests()
        {
            _ = Assert.Throws<ArgumentOutOfRangeException>(() => new Circle2D(-double.Epsilon));
            _ = Assert.Throws<ArgumentOutOfRangeException>(() => new Circle2D(0));
            Assert.DoesNotThrow(() => new Circle2D());
            Assert.DoesNotThrow(() => new Circle2D(Random.Shared.NextDouble() + double.Epsilon));
        }

        [Test]
        public void EqualityTests()
        {
            var circle1 = GeometryTestHelpers.GetRandomIntegerCircle();
            var circle2 = new Circle2D(circle1.Radius);
            var circle3 = new Circle2D(circle1.Radius + 1);

            Assert.That(circle1, Is.EqualTo(circle2));
            Assert.That(circle1, Is.Not.EqualTo(circle3));
        }

        [Test]
        public void SerializationTests()
        {
            var circle = GeometryTestHelpers.GetRandomIntegerCircle();
            var serialized = JsonSerializer.Serialize(circle);
            var deserialized = JsonSerializer.Deserialize<Circle2D>(serialized);
            Assert.That(circle, Is.EqualTo(deserialized));
        }

        [Test]
        public void BoundsTests()
        {
            var circle = GeometryTestHelpers.GetRandomIntegerCircle();

            Assert.Multiple(() =>
            {
                Assert.That(circle.Bounds.Top, Is.EqualTo(circle.Radius));
                Assert.That(circle.Bounds.Bottom, Is.EqualTo(-circle.Radius));
                Assert.That(circle.Bounds.Left, Is.EqualTo(-circle.Radius));
                Assert.That(circle.Bounds.Right, Is.EqualTo(circle.Radius));
            });
        }

        [Test]
        public void IsPointInCircleTests()
        {
            var circle = GeometryTestHelpers.GetRandomIntegerCircle();

            // Test all points in bounds, splitting boundary edges into
            // additional points
            var boundsPoints = new Point2D[]
            {
                // Top edge points + midpoint
                circle.Bounds.TopLeft,
                new Point2D(circle.CenterPoint.X, circle.Bounds.Top),
                circle.Bounds.TopRight,

                // Right edge midpoint
                new Point2D(circle.Bounds.Right, circle.CenterPoint.Y),

                // Bottom edge points + midpoint
                circle.Bounds.BottomRight,
                new Point2D(circle.CenterPoint.X, circle.Bounds.Bottom),
                circle.Bounds.BottomLeft,

                // Left edge midpoint
                new Point2D(circle.Bounds.Left, circle.CenterPoint.Y)
            };

            Assert.That(Enumerable.Range(0, boundsPoints.Length).All(x => circle.IsPointInCircle(boundsPoints[x]) == (x % 2 == 1)));

            for (var i = 0; i < 100; i++)
            {
                var testPoint = GeometryTestHelpers.GetRandomIntegerPoint(-(int)(circle.Radius / 2d), (int)(circle.Radius /2d));
                Assert.That(circle.IsPointInCircle(testPoint));
            }
        }
    }
}
