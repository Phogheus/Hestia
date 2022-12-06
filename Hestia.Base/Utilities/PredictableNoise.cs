using System;
using System.Linq;
using System.Text;

namespace Hestia.Base.Utilities
{
    /// <summary>
    /// Defines utilities for generating predictable noise
    /// </summary>
    public static class PredictableNoise
    {
        /// <summary>
        /// Generates a string of length <paramref name="length"/> using reproduceable operations
        /// to output a predictable "random" string
        /// </summary>
        /// <param name="seed">Initial seed for the <see cref="Random"/> generator</param>
        /// <param name="offset">Offset digit for burning through returned values</param>
        /// <param name="length">Length of the </param>
        /// <param name="asciiCodeInclusiveMin">Byte code for the minimum character that may appear</param>
        /// <param name="asciiCodeInclusiveMax">Byte code for the maximum character that may appear</param>
        /// <remarks>
        /// This generates predictable random noise by creating a new instance of <see cref="Random"/> using
        /// <paramref name="seed"/>, rolling an offset based on <paramref name="offset"/>, and returning a
        /// string of length <paramref name="length"/>, with only characters appearing inclusively between
        /// <paramref name="asciiCodeInclusiveMin"/> and <paramref name="asciiCodeInclusiveMax"/> on the ASCII
        /// table.
        /// 
        /// <br/> <br/>
        /// 
        /// This returns predictable values by using predictable generation methods. The seed is used to
        /// initialize the random number generator, and each digit from the offset, from left to right, is used
        /// to burn through random numbers before returning random characters. This predictable pattern means
        /// generated noise is /not/ random.
        /// 
        /// <br/> <br/>
        /// 
        /// Bytes are converted to string using <see cref="Encoding.UTF8"/>.
        /// </remarks>
        /// <returns>Predictable string of noise</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Thrown if any of the required values are less than or equal to 0, or if <paramref name="asciiCodeInclusiveMin"/>
        /// is greater than <paramref name="asciiCodeInclusiveMax"/>. You can send the same min max, but this is probably not
        /// an efficient way to generate an array of the same single character.
        /// </exception>
        public static string GenerateNoise(int seed, int offset, int length, byte asciiCodeInclusiveMin = 32, byte asciiCodeInclusiveMax = 126)
        {
            if (seed <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(seed), "Seed must be a positive non-zero number.");
            }

            if (offset <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(offset), "Offset must be a positive non-zero number.");
            }

            if (length <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(length), "Noise length must be a positive non-zero number.");
            }

            if (asciiCodeInclusiveMin > asciiCodeInclusiveMax)
            {
                throw new ArgumentOutOfRangeException(nameof(asciiCodeInclusiveMin), "Min byte cannot be greater than max byte.");
            }

            // We need the predictableness of the standard Random, so we shouldn't
            // use the "closer to true" random generators of more modern times.
            var random = new Random(seed);

            // Burn through returned random values using each digit from the offset
            foreach (var pinVal in offset.ToString())
            {
                var number = pinVal - 48;

                // Run N + times, which ensures a '0' still makes this run
                for (var i = 0; i <= number; i++)
                {
                    _ = random.Next();
                }
            }

            // Get random bytes for required length
            var randomBytes = Enumerable.Range(0, length).Select(x => (byte)random.Next(asciiCodeInclusiveMin, asciiCodeInclusiveMax + 1)).ToArray();

            return Encoding.UTF8.GetString(randomBytes);
        }
    }
}
