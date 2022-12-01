using System;
using System.Text.Json;
using Hestia.Base.Geometry.Models;
using NUnit.Framework;

// TODO: GetOrientationOfPoint

namespace Hestia.Base.Tests.GeometryTests
{
    public class Line2dTests
    {
        [Test]
        public void ConstructorTests()
        {
            Assert.DoesNotThrow(() => new Line2D());
            _ = Assert.Throws<InvalidOperationException>(() => new Line2D(Point2D.Zero, Point2D.Zero));
            Assert.DoesNotThrow(() => new Line2D(Point2D.Zero, new Point2D(0, 1)));
        }

        [TestCase(-1, 1, true)] // Upper left quadrant
        [TestCase(0, 1, false)] // Upper center
        [TestCase(1, 1, false)] // Upper right quadrant
        [TestCase(1, 0, false)] // Center right
        [TestCase(1, -1, false)] // Bottom right quadrant
        [TestCase(0, -1, true)] // Bottom center
        [TestCase(-1, -1, true)] // Bottom left quadrant
        [TestCase(-1, 0, true)] // Center left
        public void ConstructorOrderedPointTests(int targetX, int targetY, bool orderFlipExpectedFirst)
        {
            // Assert Start and End points are in correct order of X-Right then Y-Up

            // Test upper left quadrant
            var start = Point2D.Zero;
            var end = new Point2D(targetX, targetY);
            var line = new Line2D(start, end);

            if (orderFlipExpectedFirst)
            {
                Assert.Multiple(() =>
                {
                    Assert.That(line.Start, Is.EqualTo(end));
                    Assert.That(line.End, Is.EqualTo(start));
                });
            }
            else
            {
                Assert.Multiple(() =>
                {
                    Assert.That(line.Start, Is.EqualTo(start));
                    Assert.That(line.End, Is.EqualTo(end));
                });
            }

            // Flip points to confirm the opposite order returns the same results
            start = end;
            end = Point2D.Zero;
            line = new Line2D(start, end);

            if (!orderFlipExpectedFirst)
            {
                Assert.Multiple(() =>
                {
                    Assert.That(line.Start, Is.EqualTo(end));
                    Assert.That(line.End, Is.EqualTo(start));
                });
            }
            else
            {
                Assert.Multiple(() =>
                {
                    Assert.That(line.Start, Is.EqualTo(start));
                    Assert.That(line.End, Is.EqualTo(end));
                });
            }
        }

        [Test]
        public void EqualityTests()
        {
            var line1 = GeometryTestHelpers.GetRandomIntegerLineSegment();
            var line2 = new Line2D(line1.Start, line1.End);
            var line3 = new Line2D(line1.Start * -1, line1.End);

            Assert.That(line1, Is.EqualTo(line2));
            Assert.That(line1, Is.Not.EqualTo(line3));
        }

        [Test]
        public void SerializationTests()
        {
            var line = GeometryTestHelpers.GetRandomIntegerLineSegment();
            var serialized = JsonSerializer.Serialize(line);
            var deserialized = JsonSerializer.Deserialize<Line2D>(serialized);
            Assert.That(line, Is.EqualTo(deserialized));
        }

        [Test]
        public void IsPointOnLineTests()
        {
            var line = GeometryTestHelpers.GetRandomIntegerLineSegment();
            var outsidePoint = line.Start + ((line.End - line.Start) * 1000f);
            var minorAdjustment = new Point2D(1f, 0);

            Assert.Multiple(() =>
            {
                Assert.That(line.IsPointOnLine(line.Start));
                Assert.That(line.IsPointOnLine(line.Start + minorAdjustment), Is.False);
                Assert.That(line.IsPointOnLine(line.MidPoint));
                Assert.That(line.IsPointOnLine(line.MidPoint + minorAdjustment), Is.False);
                Assert.That(line.IsPointOnLine(line.End));
                Assert.That(line.IsPointOnLine(line.End + minorAdjustment), Is.False);
                Assert.That(line.IsPointOnLine(outsidePoint));
                Assert.That(line.IsPointOnLine(outsidePoint + minorAdjustment), Is.False);
            });
        }

        [Test]
        public void IsPointOnLineSegmentTests()
        {
            var line = GeometryTestHelpers.GetRandomIntegerLineSegment();
            var outsidePoint = line.Start + ((line.End - line.Start) * 1000f);
            var minorAdjustment = new Point2D(1f, 0);

            Assert.Multiple(() =>
            {
                Assert.That(line.IsPointOnLineSegment(line.Start));
                Assert.That(line.IsPointOnLineSegment(line.Start + minorAdjustment), Is.False);
                Assert.That(line.IsPointOnLineSegment(line.MidPoint));
                Assert.That(line.IsPointOnLineSegment(line.MidPoint + minorAdjustment), Is.False);
                Assert.That(line.IsPointOnLineSegment(line.End));
                Assert.That(line.IsPointOnLineSegment(line.End + minorAdjustment), Is.False);
                Assert.That(line.IsPointOnLineSegment(outsidePoint), Is.False);
                Assert.That(line.IsPointOnLineSegment(outsidePoint + minorAdjustment), Is.False);
            });
        }

        [Test]
        public void DoLinesIntersectTests()
        {
            // Parallel
            var line1 = GeometryTestHelpers.GetRandomIntegerLineSegment();
            var line2 = new Line2D(line1.Start * 2, line1.End * 2);
            Assert.That(line1.DoLinesIntersect(line2), Is.False);

            // Coincident
            line1 = GeometryTestHelpers.GetRandomIntegerLineSegment();
            Assert.That(line1.DoLinesIntersect(line1), Is.False);

            // Perpendicular
            line1 = GeometryTestHelpers.GetRandomIntegerLineSegment();
            line2 = new Line2D(new Point2D(line1.Start.X, line1.End.Y), new Point2D(line1.End.X, line1.Start.Y));
            Assert.That(line1.DoLinesIntersect(line2), Is.True);

            // Lines intersect but do not touch
            line1 = GeometryTestHelpers.GetRandomIntegerLineSegment();
            line2 = new Line2D(line1.Start * 2, (line1.End * 2) + new Point2D(0, 10));
            Assert.That(line1.DoLinesIntersect(line2), Is.True);
        }

        [Test]
        public void DoLineSegmentsIntersectTests()
        {
            // Parallel
            var line1 = GeometryTestHelpers.GetRandomIntegerLineSegment();
            var line2 = new Line2D(line1.Start * 2, line1.End * 2);
            Assert.That(line1.DoLineSegmentsIntersect(line2), Is.False);

            // Coincident
            line1 = GeometryTestHelpers.GetRandomIntegerLineSegment();
            Assert.That(line1.DoLineSegmentsIntersect(line1), Is.False);

            // Perpendicular
            var bounds = line1.Bounds;
            line1 = new Line2D(bounds.TopLeft, bounds.BottomRight);
            line2 = new Line2D(bounds.BottomLeft, bounds.TopRight);
            Assert.That(line1.DoLineSegmentsIntersect(line2), Is.True);

            // Lines intersect but do not touch
            line1 = GeometryTestHelpers.GetRandomIntegerLineSegment();
            line2 = new Line2D(line1.Start * 2, (line1.End * 2) + new Point2D(0, 10));
            Assert.That(line1.DoLineSegmentsIntersect(line2), Is.False);
        }

        [Test]
        public void GetIntersectionPointOfLinesTests()
        {
            var line1 = GeometryTestHelpers.GetRandomIntegerLineSegment();
            var line2 = GeometryTestHelpers.GetRandomIntegerLineSegment();

            // If slope is equal, they're parallel and won't intersect
            if (line1.Slope == line2.Slope)
            {
                line2 = new Line2D(line2.Start, line2.End * 2);
            }

            Assert.Multiple(() =>
            {
                Assert.That(line1.GetIntersectionPointOfLines(line1), Is.Null);
                Assert.That(line1.GetIntersectionPointOfLines(line2), Is.Not.Null);
            });
        }

        [Test]
        public void GetIntersectionPointOfLineSegments()
        {
            var line1 = GeometryTestHelpers.GetRandomIntegerLineSegment();
            var line2 = new Line2D(line1.Start, GeometryTestHelpers.GetRandomIntegerLineSegment().End);
            var line3 = new Line2D(line1.MidPoint, GeometryTestHelpers.GetRandomIntegerLineSegment().End);
            var line4 = new Line2D(line1.End, GeometryTestHelpers.GetRandomIntegerLineSegment().End);

            Assert.Multiple(() =>
            {
                Assert.That(line1.GetIntersectionPointOfLineSegments(line1), Is.Null);
                Assert.That(line1.GetIntersectionPointOfLineSegments(line2), Is.Not.Null);
                Assert.That(line1.GetIntersectionPointOfLineSegments(line3), Is.Not.Null);
                Assert.That(line1.GetIntersectionPointOfLineSegments(line4), Is.Not.Null);
            });
        }
    }
}
