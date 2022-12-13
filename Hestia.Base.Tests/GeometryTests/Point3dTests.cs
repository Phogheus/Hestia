using System;
using System.Text.Json;
using Hestia.Base.Geometry.Models;
using NUnit.Framework;

namespace Hestia.Base.Tests.GeometryTests
{
    public class Point3dTests
    {
        [Test]
        public void EqualityTests()
        {
            var point1 = GeometryTestHelpers.GetRandomIntegerPoint3D();
            var point2 = new Point3D(point1.X, point1.Y, point1.Z);
            var point3 = new Point3D(-point1.X, point1.Y, point1.Z);

            Assert.That(point1, Is.EqualTo(point2));
            Assert.That(point1, Is.Not.EqualTo(point3));
        }

        [Test]
        public void SerializationTests()
        {
            var point = GeometryTestHelpers.GetRandomIntegerPoint3D();
            var serialized = JsonSerializer.Serialize(point);
            var deserialized = JsonSerializer.Deserialize<Point3D>(serialized);
            Assert.That(point, Is.EqualTo(deserialized));
        }

        [Test]
        public void DotTests()
        {
            Assert.DoesNotThrow(() => new Point3D().Dot(null));

            var point1 = GeometryTestHelpers.GetRandomIntegerPoint3D(1, 10);
            var point2 = GeometryTestHelpers.GetRandomIntegerPoint3D(1, 10);

            Assert.That(point1.Dot(null), Is.EqualTo(0));

            var dot = point1.Dot(point2);
            Assert.That(dot, Is.Not.EqualTo(0));
        }

        [Test]
        public void CrossTests()
        {
            Assert.DoesNotThrow(() => new Point3D().Cross(null));

            var point1 = GeometryTestHelpers.GetRandomIntegerPoint3D(1, 10);
            var point2 = GeometryTestHelpers.GetRandomIntegerPoint3D(25, 50);

            Assert.That(point1.Cross(null), Is.EqualTo(Point3D.Zero));

            var cross = point1.Cross(point2);
            Assert.That(cross.Magnitude, Is.Not.EqualTo(0));
        }

        [Test]
        public void DistanceTests()
        {
            Assert.DoesNotThrow(() => new Point3D().Distance(null));
            Assert.DoesNotThrow(() => new Point3D().DistanceSquared(null));

            var point1 = GeometryTestHelpers.GetRandomIntegerPoint3D(1, 10);
            var point2 = GeometryTestHelpers.GetRandomIntegerPoint3D(25, 50);

            Assert.That(point1.Distance(null), Is.EqualTo(0));

            var distance = point1.Distance(point2);
            Assert.That(distance, Is.Not.EqualTo(0));

            var distanceSquared = point1.DistanceSquared(point2);
            Assert.That(distanceSquared, Is.Not.EqualTo(0));
        }

        [Test]
        public void NormalizedTests()
        {
            Assert.DoesNotThrow(() => Point3D.Zero.Normalized());

            var point = GeometryTestHelpers.GetRandomIntegerPoint3D(1000, 10000);
            var normalizedResult = point.Normalized();

            Assert.Multiple(() =>
            {
                Assert.That(normalizedResult.X, Is.GreaterThanOrEqualTo(0d));
                Assert.That(normalizedResult.X, Is.LessThanOrEqualTo(1d));
                Assert.That(normalizedResult.Y, Is.GreaterThanOrEqualTo(0d));
                Assert.That(normalizedResult.Y, Is.LessThanOrEqualTo(1d));
                Assert.That(normalizedResult.Z, Is.GreaterThanOrEqualTo(0d));
                Assert.That(normalizedResult.Z, Is.LessThanOrEqualTo(1d));
            });
        }

        [Test]
        public void ApproximatelyEqualsTests()
        {
            Assert.DoesNotThrow(() => new Point3D().ApproximatelyEquals(null, 0));
            Assert.DoesNotThrow(() => new Point3D().ApproximatelyEquals(Point3D.Zero, -100d));

            var point1 = GeometryTestHelpers.GetRandomIntegerPoint3D(1, 10);
            var point2 = GeometryTestHelpers.GetRandomIntegerPoint3D(25, 50);

            Assert.Multiple(() =>
            {
                Assert.That(point1.ApproximatelyEquals(null, 0d), Is.False);
                Assert.That(point1.ApproximatelyEquals(point2, 0d), Is.False);
                Assert.That(point1.ApproximatelyEquals(point2, -1d), Is.False);
                Assert.That(point1.ApproximatelyEquals(point2, 1000d), Is.True);
            });
        }

        [Test]
        public void OperatorTests()
        {
            Assert.Multiple(() =>
            {
                // +
                Assert.That(Point3D.Up + Point3D.Down, Is.EqualTo(Point3D.Zero));
                Assert.That(Point3D.Left + Point3D.Right, Is.EqualTo(Point3D.Zero));
                Assert.That(Point3D.Up + Point3D.Right, Is.EqualTo(new Point3D(1, 1, 0)));
                Assert.That(Point3D.Up + null, Is.EqualTo(Point3D.Up));

                // -
                Assert.That(Point3D.Up - Point3D.Down, Is.EqualTo(new Point3D(0, 2, 0)));
                Assert.That(Point3D.Left - Point3D.Right, Is.EqualTo(new Point3D(-2, 0, 0)));
                Assert.That(Point3D.Up - Point3D.Right, Is.EqualTo(new Point3D(-1, 1, 0)));
                Assert.That(Point3D.Up - null, Is.EqualTo(Point3D.Up));

                // *
                Assert.That(Point3D.Up * 2, Is.EqualTo(new Point3D(0, 2, 0)));
                Assert.That(Point3D.Left * 2, Is.EqualTo(new Point3D(-2, 0, 0)));
                Assert.That(Point3D.Up * 0, Is.EqualTo(Point3D.Zero));
            });
        }

        [Test]
        public void PositionChangeTests()
        {
            var point1 = Point3D.Zero;
            var point2 = Point3D.Right;

            Assert.That(point1.Distance(point2), Is.EqualTo(1));

            point2.X += 1;
            Assert.That(point1.Distance(point2), Is.EqualTo(2));

            point2.Y += 2;
            Assert.That(point1.Distance(point2), Is.EqualTo(Math.Sqrt(8)));

            point2.Y -= 2;
            point2.Z += 2;
            Assert.That(point1.Distance(point2), Is.EqualTo(Math.Sqrt(8)));
        }
    }
}
