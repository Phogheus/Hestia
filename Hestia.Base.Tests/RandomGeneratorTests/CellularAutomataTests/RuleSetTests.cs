using System;
using System.Text.Json;
using Hestia.Base.RandomGenerators.CellularAutomata;
using NUnit.Framework;

namespace Hestia.Base.Tests.RandomGeneratorTests.CellularAutomataTests
{
    public class RuleSetTests
    {
        [Test]
        public void ConstructorTests()
        {
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
            _ = Assert.Throws<ArgumentNullException>(() => new RuleSet(null));
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
            _ = Assert.Throws<ArgumentNullException>(() => new RuleSet(string.Empty));
            _ = Assert.Throws<ArgumentOutOfRangeException>(() => new RuleSet("123"));
            _ = Assert.Throws<ArgumentOutOfRangeException>(() => new RuleSet("123/"));
            _ = Assert.Throws<ArgumentOutOfRangeException>(() => new RuleSet("123/456/7"));
            _ = Assert.Throws<FormatException>(() => new RuleSet("123/A45"));
            Assert.DoesNotThrow(() => new RuleSet("12/34"));
            Assert.DoesNotThrow(() => new RuleSet("12\\34"));
        }

        [Test]
        public void EqualityTests()
        {
            var ruleSet1 = new RuleSet("12/3");
            var ruleSet2 = new RuleSet("12/3");
            var ruleSet3 = new RuleSet("12/34");

            Assert.Multiple(() =>
            {
                Assert.That(ruleSet1, Is.EqualTo(ruleSet2));
                Assert.That(ruleSet1, Is.Not.EqualTo(ruleSet3));
            });
        }

        [Test]
        public void SerializationTests()
        {
            var ruleSet = new RuleSet("12/3");
            var serialized = JsonSerializer.Serialize(ruleSet);
            var deserialized = JsonSerializer.Deserialize<RuleSet>(serialized);
            Assert.That(ruleSet, Is.EqualTo(deserialized));
        }

        [Test]
        public void DistinctValuesTests()
        {
            var ruleSet = new RuleSet("44/565");

            Assert.Multiple(() =>
            {
                Assert.That(ruleSet.NeighborCountCellStaysAlive, Has.Length.EqualTo(1));
                Assert.That(ruleSet.NeighborCountCellIsBorn, Has.Length.EqualTo(2));
            });
        }
    }
}
