using System;
using System.Text.Json;
using Hestia.Base.RandomGenerators.CellularAutomata;
using NUnit.Framework;

namespace Hestia.Base.Tests.RandomGeneratorTests.CellularAutomataTests
{
    public class GenerationalCellMapTests
    {
        private static readonly RuleSet GOOD_RULE_SET = new RuleSet("12/34");

        [Test]
        public void ConstructorTests()
        {
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
            _ = Assert.Throws<ArgumentNullException>(() => new GenerationalCellMap(null, 10, 10));
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
            _ = Assert.Throws<ArgumentOutOfRangeException>(() => new GenerationalCellMap(GOOD_RULE_SET, 0, 10));
            _ = Assert.Throws<ArgumentOutOfRangeException>(() => new GenerationalCellMap(GOOD_RULE_SET, 10, 0));
            Assert.DoesNotThrow(() => new GenerationalCellMap(GOOD_RULE_SET, 10, 10));
            Assert.DoesNotThrow(() => new GenerationalCellMap(GOOD_RULE_SET, 10, 10, -100));
        }

        [Test]
        public void EqualityTests()
        {
            var cellMap1 = new GenerationalCellMap(GOOD_RULE_SET, 10, 10);
            var cellMap2 = new GenerationalCellMap(GOOD_RULE_SET, 10, 10);
            var cellMap3 = new GenerationalCellMap(GOOD_RULE_SET, 10, 10, cellMap1.Seed);
            var cellMap4 = new GenerationalCellMap(GOOD_RULE_SET, 10, 15, cellMap1.Seed);
            var cellMap5 = new GenerationalCellMap(new RuleSet("12/345"), 10, 10, cellMap1.Seed);

            Assert.Multiple(() =>
            {
                Assert.That(cellMap1, Is.Not.EqualTo(cellMap2));
                Assert.That(cellMap1, Is.EqualTo(cellMap3));
                Assert.That(cellMap1, Is.Not.EqualTo(cellMap4));
                Assert.That(cellMap1, Is.Not.EqualTo(cellMap5));
            });
        }

        [Test]
        public void SerializationTests()
        {
            var cellMap = new GenerationalCellMap(GOOD_RULE_SET, 10, 20);
            var serialized = JsonSerializer.Serialize(cellMap);
            var deserialized = JsonSerializer.Deserialize<GenerationalCellMap>(serialized);
            Assert.That(cellMap, Is.EqualTo(deserialized));
        }
    }
}
