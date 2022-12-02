using System.Collections.Generic;
using Hestia.Base.Utilities;
using NUnit.Framework;

namespace Hestia.Base.Tests.UtilityTests
{
    public class GeneralUtilitiesTests
    {
        [Test]
        public void GetUnderlyingIEnumerableElementTypeTests()
        {
            Assert.Multiple(() =>
            {
                Assert.That(GeneralUtilities.GetElementTypeOfIEnumerable<int[]>(), Is.EqualTo(typeof(int)));
                Assert.That(GeneralUtilities.GetElementTypeOfIEnumerable<List<int>>(), Is.EqualTo(typeof(int)));
                Assert.That(GeneralUtilities.GetElementTypeOfIEnumerable<int[][]>(), Is.EqualTo(typeof(int[])));
                Assert.That(GeneralUtilities.GetElementTypeOfIEnumerable<int[,]>(), Is.EqualTo(typeof(int)));
                Assert.That(GeneralUtilities.GetElementTypeOfIEnumerable<string>(), Is.EqualTo(typeof(char)));
                Assert.That(GeneralUtilities.GetElementTypeOfIEnumerable<Dictionary<int, string>>(), Is.EqualTo(typeof(KeyValuePair<int, string>)));
            });
        }
    }
}
