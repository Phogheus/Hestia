using System.Text.Json;
using Hestia.Base.Geometry.Models;
using NUnit.Framework;

// TODO: Dot, Cross, Distance, DistanceSquared, Normalized, ApproximatelyEquals, and +/-/* operators

namespace Hestia.Base.Tests.GeometryTests
{
    public class Point2dTests
    {
        [Test]
        public void EqualityTests()
        {
            var point1 = GeometryTestHelpers.GetRandomIntegerPoint();
            var point2 = new Point2D(point1.X, point1.Y);
            var point3 = new Point2D(-point1.X, point1.Y);

            Assert.That(point1, Is.EqualTo(point2));
            Assert.That(point1, Is.Not.EqualTo(point3));
        }

        [Test]
        public void SerializationTests()
        {
            var point = GeometryTestHelpers.GetRandomIntegerPoint();
            var serialized = JsonSerializer.Serialize(point);
            var deserialized = JsonSerializer.Deserialize<Point2D>(serialized);
            Assert.That(point, Is.EqualTo(deserialized));
        }
    }
}
