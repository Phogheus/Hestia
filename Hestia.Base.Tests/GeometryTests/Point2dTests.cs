using System;
using System.Text.Json;
using Hestia.Base.Geometry.Models;
using NUnit.Framework;

namespace Hestia.Base.Tests.GeometryTests
{
    public class Point2dTests
    {
        [Test]
        public void EqualityTests()
        {
            var point1 = GetRandomPoint();
            var point2 = new Point2D(point1.X, point1.Y);
            var point3 = new Point2D(-point1.X, point1.Y);

            Assert.That(point1, Is.EqualTo(point2));
            Assert.That(point1, Is.Not.EqualTo(point3));
        }

        [Test]
        public void SerializationTests()
        {
            var point = GetRandomPoint();
            var serialized = JsonSerializer.Serialize(point);
            var deserialized = JsonSerializer.Deserialize<Point2D>(serialized);
            Assert.That(point, Is.EqualTo(deserialized));
        }

        private static Point2D GetRandomPoint()
        {
            return new Point2D((float)Random.Shared.NextDouble(), (float)Random.Shared.NextDouble());
        }
    }
}
