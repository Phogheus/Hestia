using System;
using System.Text.Json;
using Hestia.Base.Geometry.Models;
using NUnit.Framework;

namespace Hestia.Base.Tests.GeometryTests
{
    public class Line2dTests
    {
        [Test]
        public void EqualityTests()
        {
            var line1 = GetRandomIntegerLineSegment();
            var line2 = new Line2D(line1.Start, line1.End);
            var line3 = new Line2D(line1.Start * -1, line1.End);

            Assert.That(line1, Is.EqualTo(line2));
            Assert.That(line1, Is.Not.EqualTo(line3));
        }

        [Test]
        public void SerializationTests()
        {
            var line = GetRandomIntegerLineSegment();
            var serialized = JsonSerializer.Serialize(line);
            var deserialized = JsonSerializer.Deserialize<Line2D>(serialized);
            Assert.That(line, Is.EqualTo(deserialized));
        }

        [Test]
        public void IsPointOnLineTests()
        {
            var line = GetRandomIntegerLineSegment();
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
            var line = GetRandomIntegerLineSegment();
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
            var line1 = GetRandomIntegerLineSegment();
            var line2 = GetRandomIntegerLineSegment();

            // If slope is equal, they're parallel and won't intersect
            if (line1.Slope == line2.Slope)
            {
                line2 = new Line2D(line2.Start, line2.End * 2);
            }

            Assert.Multiple(() =>
            {
                Assert.That(line1.DoLinesIntersect(line1), Is.False);
                Assert.That(line1.DoLinesIntersect(line2), Is.True);
            });
        }

        [Test]
        public void DoLineSegmentsIntersectTests()
        {
            var line1 = GetRandomIntegerLineSegment();
            var line2 = new Line2D(line1.Start, GetRandomIntegerLineSegment().End);
            var line3 = new Line2D(line1.MidPoint, GetRandomIntegerLineSegment().End);
            var line4 = new Line2D(line1.End, GetRandomIntegerLineSegment().End);

            Assert.Multiple(() =>
            {
                Assert.That(line1.DoLineSegmentsIntersect(line1), Is.False);
                Assert.That(line1.DoLineSegmentsIntersect(line2), Is.True);
                Assert.That(line1.DoLineSegmentsIntersect(line3), Is.True);
                Assert.That(line1.DoLineSegmentsIntersect(line4), Is.True);
            });
        }

        [Test]
        public void GetIntersectionPointOfLinesTests()
        {
            var line1 = GetRandomIntegerLineSegment();
            var line2 = GetRandomIntegerLineSegment();

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
            var line1 = GetRandomIntegerLineSegment();
            var line2 = new Line2D(line1.Start, GetRandomIntegerLineSegment().End);
            var line3 = new Line2D(line1.MidPoint, GetRandomIntegerLineSegment().End);
            var line4 = new Line2D(line1.End, GetRandomIntegerLineSegment().End);

            Assert.Multiple(() =>
            {
                Assert.That(line1.GetIntersectionPointOfLineSegments(line1), Is.Null);
                Assert.That(line1.GetIntersectionPointOfLineSegments(line2), Is.Not.Null);
                Assert.That(line1.GetIntersectionPointOfLineSegments(line3), Is.Not.Null);
                Assert.That(line1.GetIntersectionPointOfLineSegments(line4), Is.Not.Null);
            });
        }

        private static Line2D GetRandomIntegerLineSegment()
        {
            const int MIN = -100;
            const int MAX = 101;

            var start = new Point2D(Random.Shared.Next(MIN, MAX), Random.Shared.Next(MIN, MAX));
            var end = new Point2D(Random.Shared.Next(MIN, MAX), Random.Shared.Next(MIN, MAX));

            return new Line2D(start, end);
        }
    }
}
