using System;
using System.Security.Cryptography;
using Hestia.Base.Utilities;
using NUnit.Framework;

namespace Hestia.Base.Tests.UtilityTests
{
    public class MathEnhancedTests
    {
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
            const double COMPLETION_STEP_START = 0d;
            const double COMPLETION_STEP_START_TO_MIDDLE = .193d;
            const double COMPLETION_STEP_MIDDLE = .5d;
            const double COMPLETION_STEP_MIDDLE_TO_END = .725d;
            const double COMPLETION_STEP_END = 1d;

            (int Min, int Max) bounds;

            for (var i = 0; i < 13; i++)
            {
                switch (i)
                {
                    case 0: // byte
                        bounds = GetRandomRange(0, byte.MaxValue);
                        Assert.That(MathEnhanced.Lerp((byte)bounds.Min, (byte)bounds.Max, COMPLETION_STEP_START), Is.EqualTo(bounds.Min));
                        Assert.That(MathEnhanced.Lerp((byte)bounds.Min, (byte)bounds.Max, COMPLETION_STEP_START_TO_MIDDLE), Is.EqualTo((byte)GetRoundedLerpValue(bounds.Min, bounds.Max, COMPLETION_STEP_START_TO_MIDDLE)));
                        Assert.That(MathEnhanced.Lerp((byte)bounds.Min, (byte)bounds.Max, COMPLETION_STEP_MIDDLE), Is.EqualTo((byte)GetRoundedLerpValue(bounds.Min, bounds.Max, COMPLETION_STEP_MIDDLE)));
                        Assert.That(MathEnhanced.Lerp((byte)bounds.Min, (byte)bounds.Max, COMPLETION_STEP_MIDDLE_TO_END), Is.EqualTo((byte)GetRoundedLerpValue(bounds.Min, bounds.Max, COMPLETION_STEP_MIDDLE_TO_END)));
                        Assert.That(MathEnhanced.Lerp((byte)bounds.Min, (byte)bounds.Max, COMPLETION_STEP_END), Is.EqualTo(bounds.Max));
                        break;

                    case 1: // sbyte
                        bounds = GetRandomRange(sbyte.MinValue, sbyte.MaxValue);
                        Assert.That(MathEnhanced.Lerp((sbyte)bounds.Min, (sbyte)bounds.Max, COMPLETION_STEP_START), Is.EqualTo(bounds.Min));
                        Assert.That(MathEnhanced.Lerp((sbyte)bounds.Min, (sbyte)bounds.Max, COMPLETION_STEP_START_TO_MIDDLE), Is.EqualTo((sbyte)GetRoundedLerpValue(bounds.Min, bounds.Max, COMPLETION_STEP_START_TO_MIDDLE)));
                        Assert.That(MathEnhanced.Lerp((sbyte)bounds.Min, (sbyte)bounds.Max, COMPLETION_STEP_MIDDLE), Is.EqualTo((sbyte)GetRoundedLerpValue(bounds.Min, bounds.Max, COMPLETION_STEP_MIDDLE)));
                        Assert.That(MathEnhanced.Lerp((sbyte)bounds.Min, (sbyte)bounds.Max, COMPLETION_STEP_MIDDLE_TO_END), Is.EqualTo((sbyte)GetRoundedLerpValue(bounds.Min, bounds.Max, COMPLETION_STEP_MIDDLE_TO_END)));
                        Assert.That(MathEnhanced.Lerp((sbyte)bounds.Min, (sbyte)bounds.Max, COMPLETION_STEP_END), Is.EqualTo(bounds.Max));
                        break;

                    case 2: // short
                        bounds = GetRandomRange(short.MinValue, short.MaxValue);
                        Assert.That(MathEnhanced.Lerp((short)bounds.Min, (short)bounds.Max, COMPLETION_STEP_START), Is.EqualTo(bounds.Min));
                        Assert.That(MathEnhanced.Lerp((short)bounds.Min, (short)bounds.Max, COMPLETION_STEP_START_TO_MIDDLE), Is.EqualTo((short)GetRoundedLerpValue(bounds.Min, bounds.Max, COMPLETION_STEP_START_TO_MIDDLE)));
                        Assert.That(MathEnhanced.Lerp((short)bounds.Min, (short)bounds.Max, COMPLETION_STEP_MIDDLE), Is.EqualTo((short)GetRoundedLerpValue(bounds.Min, bounds.Max, COMPLETION_STEP_MIDDLE)));
                        Assert.That(MathEnhanced.Lerp((short)bounds.Min, (short)bounds.Max, COMPLETION_STEP_MIDDLE_TO_END), Is.EqualTo((short)GetRoundedLerpValue(bounds.Min, bounds.Max, COMPLETION_STEP_MIDDLE_TO_END)));
                        Assert.That(MathEnhanced.Lerp((short)bounds.Min, (short)bounds.Max, COMPLETION_STEP_END), Is.EqualTo(bounds.Max));
                        break;

                    case 3: // ushort
                        bounds = GetRandomRange(0, ushort.MaxValue);
                        Assert.That(MathEnhanced.Lerp((ushort)bounds.Min, (ushort)bounds.Max, COMPLETION_STEP_START), Is.EqualTo(bounds.Min));
                        Assert.That(MathEnhanced.Lerp((ushort)bounds.Min, (ushort)bounds.Max, COMPLETION_STEP_START_TO_MIDDLE), Is.EqualTo((ushort)GetRoundedLerpValue(bounds.Min, bounds.Max, COMPLETION_STEP_START_TO_MIDDLE)));
                        Assert.That(MathEnhanced.Lerp((ushort)bounds.Min, (ushort)bounds.Max, COMPLETION_STEP_MIDDLE), Is.EqualTo((ushort)GetRoundedLerpValue(bounds.Min, bounds.Max, COMPLETION_STEP_MIDDLE)));
                        Assert.That(MathEnhanced.Lerp((ushort)bounds.Min, (ushort)bounds.Max, COMPLETION_STEP_MIDDLE_TO_END), Is.EqualTo((ushort)GetRoundedLerpValue(bounds.Min, bounds.Max, COMPLETION_STEP_MIDDLE_TO_END)));
                        Assert.That(MathEnhanced.Lerp((ushort)bounds.Min, (ushort)bounds.Max, COMPLETION_STEP_END), Is.EqualTo(bounds.Max));
                        break;

                    case 4: // int
                        bounds = GetRandomRange(int.MinValue, int.MaxValue);
                        Assert.That(MathEnhanced.Lerp(bounds.Min, bounds.Max, COMPLETION_STEP_START), Is.EqualTo(bounds.Min));
                        Assert.That(MathEnhanced.Lerp(bounds.Min, bounds.Max, COMPLETION_STEP_START_TO_MIDDLE), Is.EqualTo((int)GetRoundedLerpValue(bounds.Min, bounds.Max, COMPLETION_STEP_START_TO_MIDDLE)));
                        Assert.That(MathEnhanced.Lerp(bounds.Min, bounds.Max, COMPLETION_STEP_MIDDLE), Is.EqualTo((int)GetRoundedLerpValue(bounds.Min, bounds.Max, COMPLETION_STEP_MIDDLE)));
                        Assert.That(MathEnhanced.Lerp(bounds.Min, bounds.Max, COMPLETION_STEP_MIDDLE_TO_END), Is.EqualTo((int)GetRoundedLerpValue(bounds.Min, bounds.Max, COMPLETION_STEP_MIDDLE_TO_END)));
                        Assert.That(MathEnhanced.Lerp(bounds.Min, bounds.Max, COMPLETION_STEP_END), Is.EqualTo(bounds.Max));
                        break;

                    case 5: // uint
                        bounds = GetRandomRange(0, int.MaxValue);
                        Assert.That(MathEnhanced.Lerp((uint)bounds.Min, (uint)bounds.Max, COMPLETION_STEP_START), Is.EqualTo(bounds.Min));
                        Assert.That(MathEnhanced.Lerp((uint)bounds.Min, (uint)bounds.Max, COMPLETION_STEP_START_TO_MIDDLE), Is.EqualTo((uint)GetRoundedLerpValue(bounds.Min, bounds.Max, COMPLETION_STEP_START_TO_MIDDLE)));
                        Assert.That(MathEnhanced.Lerp((uint)bounds.Min, (uint)bounds.Max, COMPLETION_STEP_MIDDLE), Is.EqualTo((uint)GetRoundedLerpValue(bounds.Min, bounds.Max, COMPLETION_STEP_MIDDLE)));
                        Assert.That(MathEnhanced.Lerp((uint)bounds.Min, (uint)bounds.Max, COMPLETION_STEP_MIDDLE_TO_END), Is.EqualTo((uint)GetRoundedLerpValue(bounds.Min, bounds.Max, COMPLETION_STEP_MIDDLE_TO_END)));
                        Assert.That(MathEnhanced.Lerp((uint)bounds.Min, (uint)bounds.Max, COMPLETION_STEP_END), Is.EqualTo(bounds.Max));
                        break;

                    case 6: // long
                        bounds = GetRandomRange(int.MinValue, int.MaxValue);
                        Assert.That(MathEnhanced.Lerp((long)bounds.Min, bounds.Max, COMPLETION_STEP_START), Is.EqualTo(bounds.Min));
                        Assert.That(MathEnhanced.Lerp((long)bounds.Min, bounds.Max, COMPLETION_STEP_START_TO_MIDDLE), Is.EqualTo((long)GetRoundedLerpValue(bounds.Min, bounds.Max, COMPLETION_STEP_START_TO_MIDDLE)));
                        Assert.That(MathEnhanced.Lerp((long)bounds.Min, bounds.Max, COMPLETION_STEP_MIDDLE), Is.EqualTo((long)GetRoundedLerpValue(bounds.Min, bounds.Max, COMPLETION_STEP_MIDDLE)));
                        Assert.That(MathEnhanced.Lerp((long)bounds.Min, bounds.Max, COMPLETION_STEP_MIDDLE_TO_END), Is.EqualTo((long)GetRoundedLerpValue(bounds.Min, bounds.Max, COMPLETION_STEP_MIDDLE_TO_END)));
                        Assert.That(MathEnhanced.Lerp((long)bounds.Min, bounds.Max, COMPLETION_STEP_END), Is.EqualTo(bounds.Max));
                        break;

                    case 7: // ulong
                        bounds = GetRandomRange(0, int.MaxValue);
                        Assert.That(MathEnhanced.Lerp((ulong)bounds.Min, (ulong)bounds.Max, COMPLETION_STEP_START), Is.EqualTo(bounds.Min));
                        Assert.That(MathEnhanced.Lerp((ulong)bounds.Min, (ulong)bounds.Max, COMPLETION_STEP_START_TO_MIDDLE), Is.EqualTo((ulong)GetRoundedLerpValue(bounds.Min, bounds.Max, COMPLETION_STEP_START_TO_MIDDLE)));
                        Assert.That(MathEnhanced.Lerp((ulong)bounds.Min, (ulong)bounds.Max, COMPLETION_STEP_MIDDLE), Is.EqualTo((ulong)GetRoundedLerpValue(bounds.Min, bounds.Max, COMPLETION_STEP_MIDDLE)));
                        Assert.That(MathEnhanced.Lerp((ulong)bounds.Min, (ulong)bounds.Max, COMPLETION_STEP_MIDDLE_TO_END), Is.EqualTo((ulong)GetRoundedLerpValue(bounds.Min, bounds.Max, COMPLETION_STEP_MIDDLE_TO_END)));
                        Assert.That(MathEnhanced.Lerp((ulong)bounds.Min, (ulong)bounds.Max, COMPLETION_STEP_END), Is.EqualTo(bounds.Max));
                        break;

                    case 8: // float
                        break;

                    case 9: // double
                        break;

                    case 10: // decimal
                        break;

                    case 11: // DateTime
                        break;

                    case 12: // TimeSpan
                        break;
                }
            }
        }

        [Test]
        public void WrapTests()
        {
            var range = GetRandomRange(1, 100);

            var rng = RandomNumberGenerator.Create();
            var rngBuffer = new byte[sizeof(short)];

            for (var i = 0; i < 1000; i++)
            {
                rng.GetBytes(rngBuffer);
                var randVal = BitConverter.ToInt16(rngBuffer);

                var wrappedValue = MathEnhanced.Wrap(randVal, range.Min, range.Max);
                Assert.That(wrappedValue, Is.GreaterThanOrEqualTo(range.Min));
                Assert.That(wrappedValue, Is.LessThanOrEqualTo(range.Max));
            }
        }

        [Test]
        public void RandomNextTests()
        {
        }

        private static (int Min, int Max) GetRandomRange(int minValue, int maxValue)
        {
            var bound1 = Random.Shared.Next(minValue, maxValue);
            var bound2 = Random.Shared.Next(minValue, maxValue);

            return bound1 >= bound2
                ? (bound2, bound1)
                : (bound1, bound2);
        }

        private static double GetRoundedLerpValue(double min, double max, double t)
        {
            return Math.Round(min + (max - min) * t, MidpointRounding.ToZero);
        }
    }
}
