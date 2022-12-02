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
    }
}
