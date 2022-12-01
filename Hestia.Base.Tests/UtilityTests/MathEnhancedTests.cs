using System;
using Hestia.Base.Utilities;
using NUnit.Framework;

namespace Hestia.Base.Tests.UtilityTests
{
    public class MathEnhancedTests
    {
        private const string DATE_TIME_FORMAT = "dd/MM/yyyy HH:mm:ss";
        private const string TIME_FORMAT = "dd\\.hh\\:mm\\:ss";

        [Test]
        public void ClampTests()
        {
            // Clamp DateTime
            var now = DateTime.UtcNow.Date;

            var date1 = now.AddDays(-1);
            var date2 = now;
            var date3 = now.AddDays(1);

            Assert.Multiple(() =>
            {
                Assert.That(MathEnhanced.Clamp(date1, date2, date3), Is.EqualTo(date2));
                Assert.That(MathEnhanced.Clamp(date1, date3, date2), Is.EqualTo(date1)); // Invalid range returns 'value'
                Assert.That(MathEnhanced.Clamp(date2, date1, date3), Is.EqualTo(date2));
                Assert.That(MathEnhanced.Clamp(date2, date3, date1), Is.EqualTo(date2)); // Invalid range returns 'value'
                Assert.That(MathEnhanced.Clamp(date3, date2, date1), Is.EqualTo(date3)); // Invalid range returns 'value'
                Assert.That(MathEnhanced.Clamp(date3, date1, date2), Is.EqualTo(date2));
            });

            // Clamp TimeSpan
            var span1 = TimeSpan.FromDays(5);
            var span2 = TimeSpan.FromDays(6);
            var span3 = TimeSpan.FromDays(7);

            Assert.Multiple(() =>
            {
                Assert.That(MathEnhanced.Clamp(span1, span2, span3), Is.EqualTo(span2));
                Assert.That(MathEnhanced.Clamp(span1, span3, span2), Is.EqualTo(span1)); // Invalid range returns 'value'
                Assert.That(MathEnhanced.Clamp(span2, span1, span3), Is.EqualTo(span2));
                Assert.That(MathEnhanced.Clamp(span2, span3, span1), Is.EqualTo(span2)); // Invalid range returns 'value'
                Assert.That(MathEnhanced.Clamp(span3, span2, span1), Is.EqualTo(span3)); // Invalid range returns 'value'
                Assert.That(MathEnhanced.Clamp(span3, span1, span2), Is.EqualTo(span2));
            });
        }

        [Test]
        public void LerpTests()
        {
            const float COMPLETION_STEP_START = 0f;
            const float COMPLETION_STEP_MIDDLE = .5f;
            const float COMPLETION_STEP_END = 1f;

            for (var i = 0; i < 13; i++)
            {
                switch (i)
                {
                    case 0: // byte
                        Assert.That(MathEnhanced.Lerp((byte)0, (byte)100, COMPLETION_STEP_START), Is.EqualTo(0));
                        Assert.That(MathEnhanced.Lerp((byte)0, (byte)100, COMPLETION_STEP_MIDDLE), Is.EqualTo(50));
                        Assert.That(MathEnhanced.Lerp((byte)0, (byte)100, COMPLETION_STEP_END), Is.EqualTo(100));
                        break;

                    case 1: // sbyte
                        Assert.That(MathEnhanced.Lerp((sbyte)0, (sbyte)100, COMPLETION_STEP_START), Is.EqualTo(0));
                        Assert.That(MathEnhanced.Lerp((sbyte)0, (sbyte)100, COMPLETION_STEP_MIDDLE), Is.EqualTo(50));
                        Assert.That(MathEnhanced.Lerp((sbyte)0, (sbyte)100, COMPLETION_STEP_END), Is.EqualTo(100));
                        break;

                    case 2: // short
                        Assert.That(MathEnhanced.Lerp((short)0, (short)100, COMPLETION_STEP_START), Is.EqualTo(0));
                        Assert.That(MathEnhanced.Lerp((short)0, (short)100, COMPLETION_STEP_MIDDLE), Is.EqualTo(50));
                        Assert.That(MathEnhanced.Lerp((short)0, (short)100, COMPLETION_STEP_END), Is.EqualTo(100));
                        break;

                    case 3: // ushort
                        Assert.That(MathEnhanced.Lerp((ushort)0, (ushort)100, COMPLETION_STEP_START), Is.EqualTo(0));
                        Assert.That(MathEnhanced.Lerp((ushort)0, (ushort)100, COMPLETION_STEP_MIDDLE), Is.EqualTo(50));
                        Assert.That(MathEnhanced.Lerp((ushort)0, (ushort)100, COMPLETION_STEP_END), Is.EqualTo(100));
                        break;

                    case 4: // int
                        Assert.That(MathEnhanced.Lerp(0, 100, COMPLETION_STEP_START), Is.EqualTo(0));
                        Assert.That(MathEnhanced.Lerp(0, 100, COMPLETION_STEP_MIDDLE), Is.EqualTo(50));
                        Assert.That(MathEnhanced.Lerp(0, 100, COMPLETION_STEP_END), Is.EqualTo(100));
                        break;

                    case 5: // uint
                        Assert.That(MathEnhanced.Lerp(0U, 100, COMPLETION_STEP_START), Is.EqualTo(0));
                        Assert.That(MathEnhanced.Lerp(0U, 100, COMPLETION_STEP_MIDDLE), Is.EqualTo(50));
                        Assert.That(MathEnhanced.Lerp(0U, 100, COMPLETION_STEP_END), Is.EqualTo(100));
                        break;

                    case 6: // long
                        Assert.That(MathEnhanced.Lerp(0L, 100, COMPLETION_STEP_START), Is.EqualTo(0));
                        Assert.That(MathEnhanced.Lerp(0L, 100, COMPLETION_STEP_MIDDLE), Is.EqualTo(50));
                        Assert.That(MathEnhanced.Lerp(0L, 100, COMPLETION_STEP_END), Is.EqualTo(100));
                        break;

                    case 7: // ulong
                        Assert.That(MathEnhanced.Lerp(0UL, 100, COMPLETION_STEP_START), Is.EqualTo(0));
                        Assert.That(MathEnhanced.Lerp(0UL, 100, COMPLETION_STEP_MIDDLE), Is.EqualTo(50));
                        Assert.That(MathEnhanced.Lerp(0UL, 100, COMPLETION_STEP_END), Is.EqualTo(100));
                        break;

                    case 8: // float
                        Assert.That(MathEnhanced.Lerp(0f, 100, COMPLETION_STEP_START), Is.EqualTo(0f));
                        Assert.That(MathEnhanced.Lerp(0f, 100, COMPLETION_STEP_MIDDLE), Is.EqualTo(50f));
                        Assert.That(MathEnhanced.Lerp(0f, 100, COMPLETION_STEP_END), Is.EqualTo(100f));
                        break;

                    case 9: // double
                        Assert.That(MathEnhanced.Lerp(0d, 100, COMPLETION_STEP_START), Is.EqualTo(0d));
                        Assert.That(MathEnhanced.Lerp(0d, 100, COMPLETION_STEP_MIDDLE), Is.EqualTo(50d));
                        Assert.That(MathEnhanced.Lerp(0d, 100, COMPLETION_STEP_END), Is.EqualTo(100d));
                        break;

                    case 10: // decimal
                        Assert.That(MathEnhanced.Lerp(0.00m, 100.00m, COMPLETION_STEP_START), Is.EqualTo(0.00m));
                        Assert.That(MathEnhanced.Lerp(0.00m, 100.00m, COMPLETION_STEP_MIDDLE), Is.EqualTo(50.00m));
                        Assert.That(MathEnhanced.Lerp(0.00m, 100.00m, COMPLETION_STEP_END), Is.EqualTo(100.00m));
                        break;

                    case 11: // DateTime
                        var startDateTime = DateTime.Now.Date;
                        var middleDateTime = startDateTime.AddHours(12);
                        var endDateTime = startDateTime.AddDays(1);
                        Assert.That(MathEnhanced.Lerp(startDateTime, endDateTime, COMPLETION_STEP_START), Is.EqualTo(startDateTime));
                        // We could see variance in ms, so only compare Date and HH:mm:ss
                        Assert.That(MathEnhanced.Lerp(startDateTime, endDateTime, COMPLETION_STEP_MIDDLE).ToString(DATE_TIME_FORMAT), Is.EqualTo(middleDateTime.ToString(DATE_TIME_FORMAT)));
                        Assert.That(MathEnhanced.Lerp(startDateTime, endDateTime, COMPLETION_STEP_END), Is.EqualTo(endDateTime));
                        break;

                    case 12: // TimeSpan
                        var startTime = TimeSpan.MinValue;
                        var middleTime = startTime.Add(TimeSpan.FromHours(12));
                        var endTime = startTime.Add(TimeSpan.FromHours(24));
                        Assert.That(MathEnhanced.Lerp(startTime, endTime, COMPLETION_STEP_START), Is.EqualTo(startTime));
                        // We could see variance in ms, so only compare days and hh:mm:ss
                        Assert.That(MathEnhanced.Lerp(startTime, endTime, COMPLETION_STEP_MIDDLE).ToString(TIME_FORMAT), Is.EqualTo(middleTime.ToString(TIME_FORMAT)));
                        Assert.That(MathEnhanced.Lerp(startTime, endTime, COMPLETION_STEP_END), Is.EqualTo(endTime));
                        break;
                }
            }
        }
    }
}
