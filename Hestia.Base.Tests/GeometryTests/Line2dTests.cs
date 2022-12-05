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
            Assert.DoesNotThrow(() => new Line2D(Point2D.Zero, Point2D.Zero));
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
            _ = Assert.Throws<ArgumentNullException>(() => new Line2D(null, Point2D.Zero));
            _ = Assert.Throws<ArgumentNullException>(() => new Line2D(Point2D.Zero, null));
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
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
                Assert.That(line.IsPointOnLine(null), Is.False);
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
                Assert.That(line.IsPointOnLineSegment(null), Is.False);
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
            // Null
            Assert.That(new Line2D(Point2D.Zero, Point2D.Up).DoLinesIntersect(null), Is.False);

            // Parallel
            var line1 = new Line2D(Point2D.Zero, Point2D.Right + Point2D.Up);
            var line2 = new Line2D(Point2D.Up, Point2D.Right + (Point2D.Up * 2));
            Assert.That(line1.DoLinesIntersect(line2), Is.False);

            // Coincident
            line1 = new Line2D(Point2D.Zero, Point2D.Right + Point2D.Up);
            line2 = new Line2D(Point2D.Zero, Point2D.Right + Point2D.Up);
            Assert.That(line1.DoLinesIntersect(line2), Is.False);

            // Perpendicular
            line1 = new Line2D(Point2D.Zero, Point2D.Right + Point2D.Up);
            line2 = new Line2D(Point2D.Up, Point2D.Right);
            Assert.That(line1.DoLinesIntersect(line2), Is.True);

            // Lines intersect but do not touch
            line1 = new Line2D(Point2D.Zero, Point2D.Right);
            line2 = new Line2D(Point2D.Up, Point2D.Right + (Point2D.Up * 2));
            Assert.That(line1.DoLinesIntersect(line2), Is.True);
        }

        [Test]
        public void DoLineSegmentsIntersectTests()
        {
            // Null
            Assert.That(new Line2D(Point2D.Zero, Point2D.Up).DoLineSegmentsIntersect(null), Is.False);

            // Parallel
            var line1 = new Line2D(Point2D.Zero, Point2D.Right + Point2D.Up);
            var line2 = new Line2D(Point2D.Up, Point2D.Right + (Point2D.Up * 2));
            Assert.That(line1.DoLineSegmentsIntersect(line2), Is.False);

            // Coincident
            line1 = new Line2D(Point2D.Zero, Point2D.Right + Point2D.Up);
            line2 = new Line2D(Point2D.Zero, Point2D.Right + Point2D.Up);
            Assert.That(line1.DoLineSegmentsIntersect(line2), Is.False);

            // Perpendicular
            line1 = new Line2D(Point2D.Zero, Point2D.Right + Point2D.Up);
            line2 = new Line2D(Point2D.Up, Point2D.Right);
            Assert.That(line1.DoLineSegmentsIntersect(line2), Is.True);

            // Lines intersect but do not touch
            line1 = new Line2D(Point2D.Zero, Point2D.Right);
            line2 = new Line2D(Point2D.Up, Point2D.Right + (Point2D.Up * 2));
            Assert.That(line1.DoLineSegmentsIntersect(line2), Is.False);
        }

        [Test]
        public void GetIntersectionPointOfLinesTests()
        {
            // Null
            Assert.That(new Line2D(Point2D.Zero, Point2D.Up).GetIntersectionPointOfLines(null), Is.Null);

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
            // Null
            Assert.That(new Line2D(Point2D.Zero, Point2D.Up).GetIntersectionPointOfLineSegments(null), Is.Null);

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

        [Test]
        public void PositionChangeTests()
        {
            var line = new Line2D(Point2D.Zero, Point2D.Right);
            Assert.Multiple(() =>
            {
                Assert.That(line.Length, Is.EqualTo(1));
                Assert.That(line.Bounds.Width, Is.EqualTo(1));
                Assert.That(line.Bounds.Height, Is.EqualTo(0));
            });

            line.End.X += 1;
            Assert.Multiple(() =>
            {
                Assert.That(line.Length, Is.EqualTo(2));
                Assert.That(line.Bounds.Width, Is.EqualTo(2));
                Assert.That(line.Bounds.Height, Is.EqualTo(0));
            });

            line.End.Y += 2;
            Assert.Multiple(() =>
            {
                Assert.That(line.Length, Is.EqualTo(Math.Sqrt(8)));
                Assert.That(line.Bounds.Width, Is.EqualTo(2));
                Assert.That(line.Bounds.Height, Is.EqualTo(2));
            });

#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
            line.End = null;
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
            Assert.Multiple(() =>
            {
                Assert.That(line.Length, Is.EqualTo(0));
                Assert.That(line.Bounds.Width, Is.EqualTo(0));
                Assert.That(line.Bounds.Height, Is.EqualTo(0));
            });
        }
    }
}
