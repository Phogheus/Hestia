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
        public void DotTests()
        {
            Assert.DoesNotThrow(() => new Point2D().Dot(null));

            var point1 = GeometryTestHelpers.GetRandomIntegerPoint2D();
            var point2 = GeometryTestHelpers.GetRandomIntegerPoint2D();

            Assert.That(point1.Dot(null), Is.EqualTo(0));
            Assert.That(point1.Dot(point2), Is.Not.EqualTo(0));
        }

        [Test]
        public void CrossTests()
        {
            Assert.DoesNotThrow(() => new Point2D().Cross(null));

            var point1 = GeometryTestHelpers.GetRandomIntegerPoint2D();
            var point2 = GeometryTestHelpers.GetRandomIntegerPoint2D();

            Assert.That(point1.Cross(null), Is.EqualTo(0));
            Assert.That(point1.Cross(point2), Is.Not.EqualTo(0));
        }

        [Test]
        public void DistanceTests()
        {
            Assert.DoesNotThrow(() => new Point2D().Distance(null));
            Assert.DoesNotThrow(() => new Point2D().DistanceSquared(null));

            var point1 = GeometryTestHelpers.GetRandomIntegerPoint2D(1, 10);
            var point2 = GeometryTestHelpers.GetRandomIntegerPoint2D(25, 50);

            Assert.That(point1.Distance(null), Is.EqualTo(0));

            var distance = point1.Distance(point2);
            Assert.That(distance, Is.Not.EqualTo(0));

            var distanceSquared = point1.DistanceSquared(point2);
            Assert.That(distanceSquared, Is.Not.EqualTo(0));
        }

        [Test]
        public void NormalizedTests()
        {
            Assert.DoesNotThrow(() => Point2D.Zero.Normalized());

            var point = GeometryTestHelpers.GetRandomIntegerPoint2D(1000, 10000);
            var normalizedResult = point.Normalized();

            Assert.Multiple(() =>
            {
                Assert.That(normalizedResult.X, Is.GreaterThanOrEqualTo(0d));
                Assert.That(normalizedResult.X, Is.LessThanOrEqualTo(1d));
                Assert.That(normalizedResult.Y, Is.LessThanOrEqualTo(1d));
                Assert.That(normalizedResult.Y, Is.GreaterThanOrEqualTo(0d));
            });
        }

        [Test]
        public void ApproximatelyEqualsTests()
        {
            Assert.DoesNotThrow(() => new Point2D().ApproximatelyEquals(null, 0));
            Assert.DoesNotThrow(() => new Point2D().ApproximatelyEquals(Point2D.Zero, -100d));

            var point1 = GeometryTestHelpers.GetRandomIntegerPoint2D(1, 10);
            var point2 = GeometryTestHelpers.GetRandomIntegerPoint2D(25, 50);

            Assert.Multiple(() =>
            {
                Assert.That(point1.ApproximatelyEquals(point2, 0), Is.False);
                Assert.That(point1.ApproximatelyEquals(point2, 1000), Is.True);
            });
        }

        [Test]
        public void OperatorTests()
        {
            Assert.Multiple(() =>
            {
                // +
                Assert.That(Point2D.Up + Point2D.Down, Is.EqualTo(Point2D.Zero));
                Assert.That(Point2D.Left + Point2D.Right, Is.EqualTo(Point2D.Zero));
                Assert.That(Point2D.Up + Point2D.Right, Is.EqualTo(new Point2D(1, 1)));
                Assert.That(Point2D.Up + null, Is.EqualTo(Point2D.Up));

                // -
                Assert.That(Point2D.Up - Point2D.Down, Is.EqualTo(new Point2D(0, 2)));
                Assert.That(Point2D.Left - Point2D.Right, Is.EqualTo(new Point2D(-2, 0)));
                Assert.That(Point2D.Up - Point2D.Right, Is.EqualTo(new Point2D(-1, 1)));
                Assert.That(Point2D.Up + null, Is.EqualTo(Point2D.Up));

                // *
                Assert.That(Point2D.Up * 2, Is.EqualTo(new Point2D(0, 2)));
                Assert.That(Point2D.Left * 2, Is.EqualTo(new Point2D(-2, 0)));
                Assert.That(Point2D.Up * 0, Is.EqualTo(Point2D.Zero));
            });
        }

        [Test]
        public void PositionChangeTests()
        {
            var point1 = Point2D.Zero;
            var point2 = Point2D.Right;

            Assert.That(point1.Distance(point2), Is.EqualTo(1));

            point2.X += 1;
            Assert.That(point1.Distance(point2), Is.EqualTo(2));

            point2.Y += 2;
            Assert.That(point1.Distance(point2), Is.EqualTo(Math.Sqrt(8)));
        }

        [TestCase(true, false)] // Radians
        [TestCase(false, false)] // Degrees 0 to +/-180
        [TestCase(false, true)] // Degrees 0 to 360
        public void AngleInTests(bool inRadians, bool as360)
        {
            for (var i = 0; i <= 8; i++)
            {
                Point2D target;
                double expectedResult;

                switch (i)
                {
                    case 0: // Up
                        target = new Point2D(0, 1);
                        expectedResult = inRadians ? 1.57d : 90d;
                        break;

                    case 1: // Up-Right
                        target = new Point2D(1, 1);
                        expectedResult = inRadians ? 0.79d : 45d;
                        break;

                    case 2: // Right
                        target = new Point2D(1, 0);
                        expectedResult = inRadians ? 0d : 0d;
                        break;

                    case 3: // Bottom-Right
                        target = new Point2D(1, -1);
                        expectedResult = inRadians ? -0.79d : -45d;
                        break;

                    case 4: // Bottom
                        target = new Point2D(0, -1);
                        expectedResult = inRadians ? -1.57d : -90d;
                        break;

                    case 5: // Bottom-Left
                        target = new Point2D(-1, -1);
                        expectedResult = inRadians ? -2.36d : -135d;
                        break;

                    case 6: // Left
                        target = new Point2D(-1, 0);
                        expectedResult = inRadians ? 3.14d : 180d;
                        break;

                    case 7: // Up-Left
                        target = new Point2D(-1, 1);
                        expectedResult = inRadians ? 2.36 : 135d;
                        break;

                    default: // Origin
                        target = new Point2D(0, 0);
                        expectedResult = inRadians ? 0d : 0d;
                        break;
                }

                if (as360 && expectedResult < 0d)
                {
                    expectedResult = 180d + (180d + expectedResult);
                }

                var result = inRadians ? Point2D.Zero.AngleInRadians(target)
                           : as360 ? Point2D.Zero.AngleInDegrees360(target)
                           : Point2D.Zero.AngleInDegrees(target);

                Assert.That(Math.Round(result, 2), Is.EqualTo(expectedResult));
            }
        }
    }
}
