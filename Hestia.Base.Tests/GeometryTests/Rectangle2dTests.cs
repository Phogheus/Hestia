using System.Text.Json;
using Hestia.Base.Geometry.Models;
using NUnit.Framework;

namespace Hestia.Base.Tests.GeometryTests
{
    public class Rectangle2dTests
    {
        [Test]
        public void ConstructorTests()
        {
            Assert.DoesNotThrow(() => new Rectangle2D());
            Assert.DoesNotThrow(() => new Rectangle2D(Point2D.Zero, Point2D.Zero));
        }

        [Test]
        public void EqualityTests()
        {
            var rectangle1 = GeometryTestHelpers.GetRandomIntegerRectangle();
            var rectangle2 = new Rectangle2D(rectangle1.TopLeft, rectangle1.BottomRight);
            var rectangle3 = new Rectangle2D(rectangle1.TopLeft, rectangle1.BottomRight * 2);

            Assert.That(rectangle1, Is.EqualTo(rectangle2));
            Assert.That(rectangle1, Is.Not.EqualTo(rectangle3));
        }

        [Test]
        public void SerializationTests()
        {
            var rectangle = GeometryTestHelpers.GetRandomIntegerRectangle();
            var serialized = JsonSerializer.Serialize(rectangle);
            var deserialized = JsonSerializer.Deserialize<Rectangle2D>(serialized);
            Assert.That(rectangle, Is.EqualTo(deserialized));
        }

        [Test]
        public void IsPointInsideRectTests()
        {
            var rectangle = new Rectangle2D(new Point2D(-1, 1), new Point2D(1, -1));

            Assert.Multiple(() =>
            {
                Assert.That(rectangle.IsPointInsideRect(rectangle.TopLeft));
                Assert.That(rectangle.IsPointInsideRect(rectangle.TopRight));
                Assert.That(rectangle.IsPointInsideRect(rectangle.BottomRight));
                Assert.That(rectangle.IsPointInsideRect(rectangle.BottomLeft));
                Assert.That(rectangle.IsPointInsideRect(rectangle.TopLeft + Point2D.Up), Is.False);
                Assert.That(rectangle.IsPointInsideRect(rectangle.TopLeft + Point2D.Left), Is.False);
                Assert.That(rectangle.IsPointInsideRect(rectangle.TopRight + Point2D.Up), Is.False);
                Assert.That(rectangle.IsPointInsideRect(rectangle.TopRight + Point2D.Right), Is.False);
                Assert.That(rectangle.IsPointInsideRect(rectangle.BottomRight + Point2D.Down), Is.False);
                Assert.That(rectangle.IsPointInsideRect(rectangle.BottomRight + Point2D.Right), Is.False);
                Assert.That(rectangle.IsPointInsideRect(rectangle.BottomLeft + Point2D.Down), Is.False);
                Assert.That(rectangle.IsPointInsideRect(rectangle.BottomLeft + Point2D.Left), Is.False);
            });
        }

        [Test]
        public void PositionChangeTests()
        {
            var rect = new Rectangle2D(); // 0 x 0 square
            Assert.Multiple(() =>
            {
                Assert.That(rect.Width, Is.EqualTo(0));
                Assert.That(rect.Height, Is.EqualTo(0));
            });

            rect.TopRight = new Point2D(1, 1); // Will expand to a 1x1 square
            Assert.Multiple(() =>
            {
                Assert.That(rect.Width, Is.EqualTo(1));
                Assert.That(rect.Height, Is.EqualTo(1));
            });

            rect.BottomRight = new Point2D(1, -1); // Will expand to a 1 x 2 rectangle
            Assert.Multiple(() =>
            {
                Assert.That(rect.Width, Is.EqualTo(1));
                Assert.That(rect.Height, Is.EqualTo(2));
            });

            rect.BottomLeft = new Point2D(-1, -1); // Will expand to a 2 x 2 rectangle
            Assert.Multiple(() =>
            {
                Assert.That(rect.Width, Is.EqualTo(2));
                Assert.That(rect.Height, Is.EqualTo(2));
                Assert.That(rect.TopLeft, Is.EqualTo(new Point2D(-1, 1)));
            });

#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
            rect.TopLeft = null; // Will drop to a 1 x 1 square from origin to 1, -1
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
            Assert.Multiple(() =>
            {
                Assert.That(rect.Width, Is.EqualTo(1));
                Assert.That(rect.Height, Is.EqualTo(1));
                Assert.That(rect.TopLeft, Is.EqualTo(Point2D.Zero));
                Assert.That(rect.BottomRight, Is.EqualTo(new Point2D(1, -1)));
            });
        }
    }
}
