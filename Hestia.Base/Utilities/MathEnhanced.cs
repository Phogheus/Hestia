using System;
using System.Security.Cryptography;

// TODO: Wrap, RandomNext

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
        public static byte Lerp(byte start, byte end, float completionStep)
        {
            return start >= end ? start
                 : completionStep <= 0f ? start
                 : completionStep >= 1f ? end
                 : (byte)(start + (byte)((end - start) * completionStep));
        }

        /// <summary>
        /// Linearly interpolates (lerp) between two given values by a normalized completion step value (0.0 - 1.0)
        /// </summary>
        /// <param name="start">Start value to interpolate between</param>
        /// <param name="end">End value to interpolate between</param>
        /// <param name="completionStep">Completion step (0.0 - 1.0)</param>
        /// <returns>Linearly interpolated value between the given range</returns>
        public static sbyte Lerp(sbyte start, sbyte end, float completionStep)
        {
            return start >= end ? start
                 : completionStep <= 0f ? start
                 : completionStep >= 1f ? end
                 : (sbyte)(start + (sbyte)((end - start) * completionStep));
        }

        /// <summary>
        /// Linearly interpolates (lerp) between two given values by a normalized completion step value (0.0 - 1.0)
        /// </summary>
        /// <param name="start">Start value to interpolate between</param>
        /// <param name="end">End value to interpolate between</param>
        /// <param name="completionStep">Completion step (0.0 - 1.0)</param>
        /// <returns>Linearly interpolated value between the given range</returns>
        public static ushort Lerp(ushort start, ushort end, float completionStep)
        {
            return start >= end ? start
                 : completionStep <= 0f ? start
                 : completionStep >= 1f ? end
                 : (ushort)(start + (ushort)((end - start) * completionStep));
        }

        /// <summary>
        /// Linearly interpolates (lerp) between two given values by a normalized completion step value (0.0 - 1.0)
        /// </summary>
        /// <param name="start">Start value to interpolate between</param>
        /// <param name="end">End value to interpolate between</param>
        /// <param name="completionStep">Completion step (0.0 - 1.0)</param>
        /// <returns>Linearly interpolated value between the given range</returns>
        public static short Lerp(short start, short end, float completionStep)
        {
            return start >= end ? start
                 : completionStep <= 0f ? start
                 : completionStep >= 1f ? end
                 : (short)(start + (short)((end - start) * completionStep));
        }

        /// <summary>
        /// Linearly interpolates (lerp) between two given values by a normalized completion step value (0.0 - 1.0)
        /// </summary>
        /// <param name="start">Start value to interpolate between</param>
        /// <param name="end">End value to interpolate between</param>
        /// <param name="completionStep">Completion step (0.0 - 1.0)</param>
        /// <returns>Linearly interpolated value between the given range</returns>
        public static uint Lerp(uint start, uint end, float completionStep)
        {
            return start >= end ? start
                 : completionStep <= 0f ? start
                 : completionStep >= 1f ? end
                 : start + (uint)((end - start) * completionStep);
        }

        /// <summary>
        /// Linearly interpolates (lerp) between two given values by a normalized completion step value (0.0 - 1.0)
        /// </summary>
        /// <param name="start">Start value to interpolate between</param>
        /// <param name="end">End value to interpolate between</param>
        /// <param name="completionStep">Completion step (0.0 - 1.0)</param>
        /// <returns>Linearly interpolated value between the given range</returns>
        public static int Lerp(int start, int end, float completionStep)
        {
            return start >= end ? start
                 : completionStep <= 0f ? start
                 : completionStep >= 1f ? end
                 : start + (int)((end - start) * completionStep);
        }

        /// <summary>
        /// Linearly interpolates (lerp) between two given values by a normalized completion step value (0.0 - 1.0)
        /// </summary>
        /// <param name="start">Start value to interpolate between</param>
        /// <param name="end">End value to interpolate between</param>
        /// <param name="completionStep">Completion step (0.0 - 1.0)</param>
        /// <returns>Linearly interpolated value between the given range</returns>
        public static ulong Lerp(ulong start, ulong end, float completionStep)
        {
            return start >= end ? start
                 : completionStep <= 0f ? start
                 : completionStep >= 1f ? end
                 : start + (ulong)((end - start) * completionStep);
        }

        /// <summary>
        /// Linearly interpolates (lerp) between two given values by a normalized completion step value (0.0 - 1.0)
        /// </summary>
        /// <param name="start">Start value to interpolate between</param>
        /// <param name="end">End value to interpolate between</param>
        /// <param name="completionStep">Completion step (0.0 - 1.0)</param>
        /// <returns>Linearly interpolated value between the given range</returns>
        public static long Lerp(long start, long end, float completionStep)
        {
            return start >= end ? start
                 : completionStep <= 0f ? start
                 : completionStep >= 1f ? end
                 : start + (long)((end - start) * completionStep);
        }

        /// <summary>
        /// Linearly interpolates (lerp) between two given values by a normalized completion step value (0.0 - 1.0)
        /// </summary>
        /// <param name="start">Start value to interpolate between</param>
        /// <param name="end">End value to interpolate between</param>
        /// <param name="completionStep">Completion step (0.0 - 1.0)</param>
        /// <returns>Linearly interpolated value between the given range</returns>
        public static float Lerp(float start, float end, float completionStep)
        {
            return start >= end ? start
                 : completionStep <= 0f ? start
                 : completionStep >= 1f ? end
                 : start + ((end - start) * completionStep);
        }

        /// <summary>
        /// Linearly interpolates (lerp) between two given values by a normalized completion step value (0.0 - 1.0)
        /// </summary>
        /// <param name="start">Start value to interpolate between</param>
        /// <param name="end">End value to interpolate between</param>
        /// <param name="completionStep">Completion step (0.0 - 1.0)</param>
        /// <returns>Linearly interpolated value between the given range</returns>
        public static double Lerp(double start, double end, float completionStep)
        {
            return start >= end ? start
                 : completionStep <= 0f ? start
                 : completionStep >= 1f ? end
                 : start + ((end - start) * completionStep);
        }

        /// <summary>
        /// Linearly interpolates (lerp) between two given values by a normalized completion step value (0.0 - 1.0)
        /// </summary>
        /// <param name="start">Start value to interpolate between</param>
        /// <param name="end">End value to interpolate between</param>
        /// <param name="completionStep">Completion step (0.0 - 1.0)</param>
        /// <returns>Linearly interpolated value between the given range</returns>
        public static decimal Lerp(decimal start, decimal end, float completionStep)
        {
            return start >= end ? start
                 : completionStep <= 0d ? start
                 : completionStep >= 1d ? end
                 : start + ((end - start) * (decimal)completionStep);
        }

        /// <summary>
        /// Linearly interpolates (lerp) between two given values by a normalized completion step value (0.0 - 1.0)
        /// </summary>
        /// <param name="start">Start value to interpolate between</param>
        /// <param name="end">End value to interpolate between</param>
        /// <param name="completionStep">Completion step (0.0 - 1.0)</param>
        /// <returns>Linearly interpolated value between the given range</returns>
        public static DateTime Lerp(DateTime start, DateTime end, float completionStep)
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
        public static TimeSpan Lerp(TimeSpan start, TimeSpan end, float completionStep)
        {
            var lerpedTicks = Lerp(start.Ticks, end.Ticks, completionStep);

            return new TimeSpan(lerpedTicks);
        }

        #endregion Lerp

        /// <summary>
        /// Returns a random normalized value between 0.0 and 1.0, or -1.0 and 1.0 if <paramref name="includeNegativeRange"/> is true
        /// </summary>
        /// <param name="includeNegativeRange">True to include negative range</param>
        /// <returns>Value in range</returns>
        public static double RandomNormalizedValue(bool includeNegativeRange = false)
        {
            var lowerBounds = includeNegativeRange ? int.MinValue + 2 : 0;

            return RandomNumberGenerator.GetInt32(lowerBounds, int.MaxValue) / (double)(int.MaxValue - 1);
        }
    }
}
