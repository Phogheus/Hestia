using System;
using System.Security.Cryptography;

namespace Hestia.Base.Utilities
{
    /// <summary>
    /// Defines enhancements to the built in <see cref="Math"/> library
    /// </summary>
    public static class MathEnhanced
    {
        #region Fields

        /// <summary>
        /// Conversion mutliplier to convert radians to degrees (180 / PI)
        /// </summary>
        public const double RAD_2_DEG = 180d / Math.PI;

        /// <summary>
        /// Conversion mutliplier to convert degrees to radians (PI / 180)
        /// </summary>
        public const double DEG_2_RAD = Math.PI / 180d;

        #endregion Fields

        #region Clamp

        /// <summary>
        /// Returns <paramref name="value"/> clamped to the inclusive range of <paramref name="min"/> and <paramref name="max"/>
        /// </summary>
        /// <param name="value">The value to be clamped</param>
        /// <param name="min">The lower bound of the result</param>
        /// <param name="max">The upper bound of the result</param>
        /// <returns>value if min &lt;= value &lt;= max, min if value &lt; min, max if value &gt; max, or value if min &gt;= max</returns>
        public static DateTime Clamp(DateTime value, DateTime min, DateTime max)
        {
            return min >= max ? value
                 : value < min ? min
                 : value > max ? max
                 : value;
        }

        /// <summary>
        /// Returns <paramref name="value"/> clamped to the inclusive range of <paramref name="min"/> and <paramref name="max"/>
        /// </summary>
        /// <param name="value">The value to be clamped</param>
        /// <param name="min">The lower bound of the result</param>
        /// <param name="max">The upper bound of the result</param>
        /// <returns>value if min &lt;= value &lt;= max, min if value &lt; min, max if value &gt; max, or value if min &gt;= max</returns>
        public static TimeSpan Clamp(TimeSpan value, TimeSpan min, TimeSpan max)
        {
            return min >= max ? value
                 : value < min ? min
                 : value > max ? max
                 : value;
        }

        #endregion Clamp

        #region Lerp

        /// <summary>
        /// Linearly interpolates (lerp) between two given values by a normalized completion step value (0.0 - 1.0)
        /// </summary>
        /// <param name="start">Start value to interpolate between</param>
        /// <param name="end">End value to interpolate between</param>
        /// <param name="completionStep">Completion step (0.0 - 1.0)</param>
        /// <returns>Linearly interpolated value between the given range</returns>
        public static byte Lerp(byte start, byte end, double completionStep)
        {
            return (byte)Lerp((double)start, end, completionStep);
        }

        /// <summary>
        /// Linearly interpolates (lerp) between two given values by a normalized completion step value (0.0 - 1.0)
        /// </summary>
        /// <param name="start">Start value to interpolate between</param>
        /// <param name="end">End value to interpolate between</param>
        /// <param name="completionStep">Completion step (0.0 - 1.0)</param>
        /// <returns>Linearly interpolated value between the given range</returns>
        public static sbyte Lerp(sbyte start, sbyte end, double completionStep)
        {
            return (sbyte)Lerp((double)start, end, completionStep);
        }

        /// <summary>
        /// Linearly interpolates (lerp) between two given values by a normalized completion step value (0.0 - 1.0)
        /// </summary>
        /// <param name="start">Start value to interpolate between</param>
        /// <param name="end">End value to interpolate between</param>
        /// <param name="completionStep">Completion step (0.0 - 1.0)</param>
        /// <returns>Linearly interpolated value between the given range</returns>
        public static ushort Lerp(ushort start, ushort end, double completionStep)
        {
            return (ushort)Lerp((double)start, end, completionStep);
        }

        /// <summary>
        /// Linearly interpolates (lerp) between two given values by a normalized completion step value (0.0 - 1.0)
        /// </summary>
        /// <param name="start">Start value to interpolate between</param>
        /// <param name="end">End value to interpolate between</param>
        /// <param name="completionStep">Completion step (0.0 - 1.0)</param>
        /// <returns>Linearly interpolated value between the given range</returns>
        public static short Lerp(short start, short end, double completionStep)
        {
            return (short)Lerp((double)start, end, completionStep);
        }

        /// <summary>
        /// Linearly interpolates (lerp) between two given values by a normalized completion step value (0.0 - 1.0)
        /// </summary>
        /// <param name="start">Start value to interpolate between</param>
        /// <param name="end">End value to interpolate between</param>
        /// <param name="completionStep">Completion step (0.0 - 1.0)</param>
        /// <returns>Linearly interpolated value between the given range</returns>
        public static uint Lerp(uint start, uint end, double completionStep)
        {
            return (uint)Lerp((double)start, end, completionStep);
        }

        /// <summary>
        /// Linearly interpolates (lerp) between two given values by a normalized completion step value (0.0 - 1.0)
        /// </summary>
        /// <param name="start">Start value to interpolate between</param>
        /// <param name="end">End value to interpolate between</param>
        /// <param name="completionStep">Completion step (0.0 - 1.0)</param>
        /// <returns>Linearly interpolated value between the given range</returns>
        public static int Lerp(int start, int end, double completionStep)
        {
            return (int)Lerp((double)start, end, completionStep);
        }

        /// <summary>
        /// Linearly interpolates (lerp) between two given values by a normalized completion step value (0.0 - 1.0)
        /// </summary>
        /// <param name="start">Start value to interpolate between</param>
        /// <param name="end">End value to interpolate between</param>
        /// <param name="completionStep">Completion step (0.0 - 1.0)</param>
        /// <returns>Linearly interpolated value between the given range</returns>
        public static ulong Lerp(ulong start, ulong end, double completionStep)
        {
            return (ulong)Lerp((double)start, end, completionStep);
        }

        /// <summary>
        /// Linearly interpolates (lerp) between two given values by a normalized completion step value (0.0 - 1.0)
        /// </summary>
        /// <param name="start">Start value to interpolate between</param>
        /// <param name="end">End value to interpolate between</param>
        /// <param name="completionStep">Completion step (0.0 - 1.0)</param>
        /// <returns>Linearly interpolated value between the given range</returns>
        public static long Lerp(long start, long end, double completionStep)
        {
            return (long)Lerp((double)start, end, completionStep);
        }

        /// <summary>
        /// Linearly interpolates (lerp) between two given values by a normalized completion step value (0.0 - 1.0)
        /// </summary>
        /// <param name="start">Start value to interpolate between</param>
        /// <param name="end">End value to interpolate between</param>
        /// <param name="completionStep">Completion step (0.0 - 1.0)</param>
        /// <returns>Linearly interpolated value between the given range</returns>
        public static float Lerp(float start, float end, double completionStep)
        {
            return (float)Lerp((double)start, end, completionStep);
        }

        /// <summary>
        /// Linearly interpolates (lerp) between two given values by a normalized completion step value (0.0 - 1.0)
        /// </summary>
        /// <param name="start">Start value to interpolate between</param>
        /// <param name="end">End value to interpolate between</param>
        /// <param name="completionStep">Completion step (0.0 - 1.0)</param>
        /// <returns>Linearly interpolated value between the given range</returns>
        public static double Lerp(double start, double end, double completionStep)
        {
            return start >= end ? start
                 : completionStep <= 0 ? start
                 : completionStep >= 1 ? end
                 : start + ((end - start) * completionStep);
        }

        /// <summary>
        /// Linearly interpolates (lerp) between two given values by a normalized completion step value (0.0 - 1.0)
        /// </summary>
        /// <param name="start">Start value to interpolate between</param>
        /// <param name="end">End value to interpolate between</param>
        /// <param name="completionStep">Completion step (0.0 - 1.0)</param>
        /// <returns>Linearly interpolated value between the given range</returns>
        public static decimal Lerp(decimal start, decimal end, decimal completionStep)
        {
            return start >= end ? start
                 : completionStep <= 0 ? start
                 : completionStep >= 1 ? end
                 : start + ((end - start) * completionStep);
        }

        /// <summary>
        /// Linearly interpolates (lerp) between two given values by a normalized completion step value (0.0 - 1.0)
        /// </summary>
        /// <param name="start">Start value to interpolate between</param>
        /// <param name="end">End value to interpolate between</param>
        /// <param name="completionStep">Completion step (0.0 - 1.0)</param>
        /// <returns>Linearly interpolated value between the given range</returns>
        public static DateTime Lerp(DateTime start, DateTime end, double completionStep)
        {
            var lerpedTicks = Lerp(start.Ticks, end.Ticks, completionStep);

            return new DateTime(lerpedTicks);
        }

        /// <summary>
        /// Linearly interpolates (lerp) between two given values by a normalized completion step value (0.0 - 1.0)
        /// </summary>
        /// <param name="start">Start value to interpolate between</param>
        /// <param name="end">End value to interpolate between</param>
        /// <param name="completionStep">Completion step (0.0 - 1.0)</param>
        /// <returns>Linearly interpolated value between the given range</returns>
        public static TimeSpan Lerp(TimeSpan start, TimeSpan end, double completionStep)
        {
            var lerpedTicks = Lerp(start.Ticks, end.Ticks, completionStep);

            return new TimeSpan(lerpedTicks);
        }

        #endregion Lerp

#pragma warning disable CS1591

        // TODO: Comments for below

        #region Wrap

        public static byte Wrap(byte start, byte end, double completionStep)
        {
            return (byte)Wrap((double)start, end, completionStep);
        }

        public static sbyte Wrap(sbyte start, sbyte end, double completionStep)
        {
            return (sbyte)Wrap((double)start, end, completionStep);
        }

        public static ushort Wrap(ushort start, ushort end, double completionStep)
        {
            return (ushort)Wrap((double)start, end, completionStep);
        }

        public static short Wrap(short start, short end, double completionStep)
        {
            return (short)Wrap((double)start, end, completionStep);
        }

        public static uint Wrap(uint start, uint end, double completionStep)
        {
            return (uint)Wrap((double)start, end, completionStep);
        }

        public static int Wrap(int start, int end, double completionStep)
        {
            return (int)Wrap((double)start, end, completionStep);
        }

        public static ulong Wrap(ulong start, ulong end, double completionStep)
        {
            return (ulong)Wrap((double)start, end, completionStep);
        }

        public static long Wrap(long start, long end, double completionStep)
        {
            return (long)Wrap((double)start, end, completionStep);
        }

        public static float Wrap(float start, float end, double completionStep)
        {
            return (float)Wrap((double)start, end, completionStep);
        }

        public static double Wrap(double value, double min, double max)
        {
            if (min >= max)
            {
                return min;
            }
            else if (value >= min && value <= max)
            {
                return value;
            }

            var range = max - min + 1;

            while (value < min || value > max)
            {
                if (value > max)
                {
                    value -= range;
                }
                else
                {
                    value += range;
                }
            }

            return value;
        }

        public static decimal Wrap(decimal start, decimal end, double completionStep)
        {
            return (decimal)Wrap((double)start, (double)end, completionStep);
        }

        public static DateTime Wrap(DateTime start, DateTime end, double completionStep)
        {
            var wrappedTicks = Wrap(start.Ticks, end.Ticks, completionStep);

            return new DateTime(wrappedTicks);
        }

        public static TimeSpan Wrap(TimeSpan start, TimeSpan end, double completionStep)
        {
            var wrappedTicks = Wrap(start.Ticks, end.Ticks, completionStep);

            return new TimeSpan(wrappedTicks);
        }

        #endregion Wrap

        #region Random Next

        public static byte RandomNext(byte min, byte max)
        {
            return (byte)RandomNext((double)min, max);
        }

        public static sbyte RandomNext(sbyte min, sbyte max)
        {
            return (sbyte)RandomNext((double)min, max);
        }

        public static ushort RandomNext(ushort min, ushort max)
        {
            return (ushort)RandomNext((double)min, max);
        }

        public static short RandomNext(short min, short max)
        {
            return (short)RandomNext((double)min, max);
        }

        public static uint RandomNext(uint min, uint max)
        {
            return (uint)RandomNext((double)min, max);
        }

        public static int RandomNext(int min, int max)
        {
            return (int)RandomNext((double)min, max);
        }

        public static ulong RandomNext(ulong min, ulong max)
        {
            return (ulong)RandomNext((double)min, max);
        }

        public static long RandomNext(long min, long max)
        {
            return (long)RandomNext((double)min, max);
        }

        public static float RandomNext(float min, float max)
        {
            return (float)RandomNext((double)min, max);
        }

        public static double RandomNext(double min, double max)
        {
            return min < max
                ? ((max - min) * RandomNormalizedValue()) + min
                : min;
        }

        public static decimal RandomNext(decimal min, double max)
        {
            return (decimal)RandomNext((double)min, max);
        }

        public static DateTime RandomNext(DateTime start, DateTime end)
        {
            var wrappedTicks = RandomNext(start.Ticks, end.Ticks);

            return new DateTime(wrappedTicks);
        }

        public static TimeSpan RandomNext(TimeSpan start, TimeSpan end)
        {
            var wrappedTicks = RandomNext(start.Ticks, end.Ticks);

            return new TimeSpan(wrappedTicks);
        }

        #endregion Random Next

        public static double RandomNormalizedValue(bool includeNegativeRange = false)
        {
            return RandomNumberGenerator.GetInt32(includeNegativeRange ? int.MinValue : 0, int.MaxValue) / (double)(int.MaxValue - 1);
        }
    }
}
