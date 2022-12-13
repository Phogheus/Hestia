using System;
using System.Linq;
using Hestia.Base.Geometry.Models;
using Hestia.Base.Geometry.Utilities;
using NUnit.Framework;

// TODO: ValidateAndOrderPointsForPolygon, ArePointsAllColinear, GetCentroidPoint, IsPointInPolygon, RotatePointAroundOrigin

namespace Hestia.Base.Tests.GeometryTests
{
    public class GeometryUtilityTests
    {
        [Test]
        public void GetBoundsFromPointsTests()
        {
            // Test 3+ points
            var points = Enumerable.Range(0, Random.Shared.Next(3, 10)).Select(x => GeometryTestHelpers.GetRandomIntegerPoint2D()).ToArray();
            var bounds = GeometryUtilities.GetBoundsFromPoints(points);

            var expectedMinX = points.Min(x => x.X);
            var expectedMaxX = points.Max(x => x.X);
            var expectedMinY = points.Min(x => x.Y);
            var expectedMaxY = points.Max(x => x.Y);

            Assert.Multiple(() =>
            {
                Assert.That(bounds.Top, Is.EqualTo(expectedMaxY));
                Assert.That(bounds.Bottom, Is.EqualTo(expectedMinY));
                Assert.That(bounds.Left, Is.EqualTo(expectedMinX));
                Assert.That(bounds.Right, Is.EqualTo(expectedMaxX));
            });

            // Test 2 points
            points = Enumerable.Range(0, 2).Select(x => GeometryTestHelpers.GetRandomIntegerPoint2D()).ToArray();
            bounds = GeometryUtilities.GetBoundsFromPoints(points);

            expectedMinX = points.Min(x => x.X);
            expectedMaxX = points.Max(x => x.X);
            expectedMinY = points.Min(x => x.Y);
            expectedMaxY = points.Max(x => x.Y);

            Assert.Multiple(() =>
            {
                Assert.That(bounds.Top, Is.EqualTo(expectedMaxY));
                Assert.That(bounds.Bottom, Is.EqualTo(expectedMinY));
                Assert.That(bounds.Left, Is.EqualTo(expectedMinX));
                Assert.That(bounds.Right, Is.EqualTo(expectedMaxX));
            });

            // Test 1 point
            points = Enumerable.Range(0, 1).Select(x => GeometryTestHelpers.GetRandomIntegerPoint2D()).ToArray();
            bounds = GeometryUtilities.GetBoundsFromPoints(points);

            expectedMinX = points.Min(x => x.X);
            expectedMaxX = points.Max(x => x.X);
            expectedMinY = points.Min(x => x.Y);
            expectedMaxY = points.Max(x => x.Y);

            Assert.Multiple(() =>
            {
                Assert.That(bounds.Top, Is.EqualTo(expectedMaxY));
                Assert.That(bounds.Bottom, Is.EqualTo(expectedMinY));
                Assert.That(bounds.Left, Is.EqualTo(expectedMinX));
                Assert.That(bounds.Right, Is.EqualTo(expectedMaxX));
            });

            // Test 0 points
            bounds = GeometryUtilities.GetBoundsFromPoints(Array.Empty<Point2D>());

            Assert.Multiple(() =>
            {
                Assert.That(bounds.Top, Is.EqualTo(0));
                Assert.That(bounds.Bottom, Is.EqualTo(0));
                Assert.That(bounds.Left, Is.EqualTo(0));
                Assert.That(bounds.Right, Is.EqualTo(0));
            });
        }

        [Test]
        public void OrientPointsClockwiseTests()
        {
            Assert.Multiple(() =>
            {
                Assert.That(GeometryUtilities.OrientPointsClockwise(null), Has.Length.EqualTo(0));
                Assert.That(GeometryUtilities.OrientPointsClockwise(new Point2D[] { new Point2D() }), Has.Length.EqualTo(1));
            });

            // Pyramid up
            var expected = new Point2D?[] { new Point2D(0, 0), new Point2D(1, 1), new Point2D(2, 0) };
            var result = GeometryUtilities.OrientPointsClockwise(expected);
            Assert.That(result, Has.Length.EqualTo(3));
            Assert.That(result[0] == expected[0] && result[1] == expected[1] && result[2] == expected[2]);

            expected = new Point2D?[] { null, new Point2D(1, 1), new Point2D(2, 0) };
            result = GeometryUtilities.OrientPointsClockwise(expected);
            Assert.That(result, Has.Length.EqualTo(2));
            Assert.That(result[0] == expected[1] && result[1] == expected[2]);

            expected = new Point2D?[] { new Point2D(0, 0), null, new Point2D(2, 0) };
            result = GeometryUtilities.OrientPointsClockwise(expected);
            Assert.That(result, Has.Length.EqualTo(2));
            Assert.That(result[0] == expected[0] && result[1] == expected[2]);
        }
    }
}
