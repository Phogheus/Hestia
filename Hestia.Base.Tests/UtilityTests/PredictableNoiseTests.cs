using System;
using Hestia.Base.Utilities;
using NUnit.Framework;

namespace Hestia.Base.Tests.UtilityTests
{
    public class PredictableNoiseTests
    {
        [Test]
        public void GenerateNoiseTests()
        {
            var seed = 123456;
            var offset = 9870;
            var length = 12;

            var noise1 = PredictableNoise.GenerateNoise(seed, offset, length);
            var noise2 = PredictableNoise.GenerateNoise(seed, offset, length);

            Assert.That(noise1, Is.EqualTo(noise2));

            // Add a zero to the end -> 98700
            noise2 = PredictableNoise.GenerateNoise(seed, offset * 10, length);

            Assert.That(noise1, Is.Not.EqualTo(noise2));

            // Remove the zero from to the end -> 987
            noise2 = PredictableNoise.GenerateNoise(seed, offset * 10, length);

            Assert.That(noise1, Is.Not.EqualTo(noise2));
        }

        [Test]
        public void InvalidValueTests()
        {
            _ = Assert.Throws<ArgumentOutOfRangeException>(() => PredictableNoise.GenerateNoise(Random.Shared.Next(-10, 0), 1, 1));
            _ = Assert.Throws<ArgumentOutOfRangeException>(() => PredictableNoise.GenerateNoise(1, Random.Shared.Next(-10, 0), 1));
            _ = Assert.Throws<ArgumentOutOfRangeException>(() => PredictableNoise.GenerateNoise(1, 1, Random.Shared.Next(-10, 0)));
            _ = Assert.Throws<ArgumentOutOfRangeException>(() => PredictableNoise.GenerateNoise(1, 1, 1, 126, 32));
            Assert.DoesNotThrow(() => PredictableNoise.GenerateNoise(1, 1, 1));
        }

        [TestCase(10000)]
        [TestCase(100000)]
        [TestCase(1000000)]
        [TestCase(10000000)]
        public void ExtremeTests(int length)
        {
            var seed = Random.Shared.Next(0, int.MaxValue);
            var offset = int.MaxValue;

            var noise1 = PredictableNoise.GenerateNoise(seed, offset, length);
            var noise2 = PredictableNoise.GenerateNoise(seed, offset, length);

            Assert.That(noise1, Is.EqualTo(noise2));
        }
    }
}
