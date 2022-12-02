using System;
using System.Collections.Generic;
using System.Linq;
using Hestia.Base.Utilities;
using NUnit.Framework;

namespace Hestia.Base.Tests.UtilityTests
{
    public class CompareUtilityTests
    {
        private const string STRING_COMPARE_VALUE = nameof(CompareUtilityTests);

        [Test]
        public void StringsAreEqualTests()
        {
            Assert.Multiple(() =>
            {
                Assert.That(CompareUtility.StringsAreEqual(string.Empty, string.Empty), Is.True);
                Assert.That(CompareUtility.StringsAreEqual("", string.Empty), Is.True);
                Assert.That(CompareUtility.StringsAreEqual(" ", string.Empty), Is.False); // whitespace != empty
                Assert.That(CompareUtility.StringsAreEqual(null, null), Is.True);
                Assert.That(CompareUtility.StringsAreEqual(STRING_COMPARE_VALUE, STRING_COMPARE_VALUE), Is.True); // Reference
                Assert.That(CompareUtility.StringsAreEqual(STRING_COMPARE_VALUE, nameof(CompareUtilityTests)), Is.True); // Value
                Assert.That(CompareUtility.StringsAreEqual(STRING_COMPARE_VALUE, STRING_COMPARE_VALUE.ToLower()), Is.False);
                Assert.That(CompareUtility.StringsAreEqual(STRING_COMPARE_VALUE, STRING_COMPARE_VALUE.ToLower(), StringComparison.OrdinalIgnoreCase), Is.True);
            });
        }

        [Test]
        public void ObjectsAreEqualTests()
        {
            var object1 = new object();
            var object2 = new object();

            var ambiguousObject1 = new { AnAwesomeValue = 0 };
            var ambiguousObject2 = new { AnAwesomeValue = 0 };
            var ambiguousObject3 = new { AnAwesomeValue = 1 };

            var array1 = new int[] { 1, 2, 3, 4, 5 };
            var array2 = new int[] { 1, 2, 3, 4, 5 };
            var array3 = new int[] { 5, 4, 3, 2, 1 };
            var array4 = new int[] { 1, 2, 3, 4, 5, 6 };

            var list1 = array1.ToList();
            var list2 = array2.ToList();
            var list3 = array3.ToList();
            var list4 = array4.ToList();

            var multiDimensionalArray1 = new int[][] { new int[] { 1, 2, 3 }, new int[] { 4, 5, 6 } };
            var multiDimensionalArray2 = new int[][] { new int[] { 1, 2, 3 }, new int[] { 4, 5, 6 } };
            var multiDimensionalArray3 = new int[][] { new int[] { 1, 2, 3 }, new int[] { 4, 5, 6, 7 } };

            var dictionary1 = new Dictionary<int, string>() { { 1, "one" }, { 2, "two" } };
            var dictionary2 = new Dictionary<int, string>() { { 1, "one" }, { 2, "two" } };
            var dictionary3 = new Dictionary<int, string>() { { 1, "one" }, { 2, "Two" } };

            Assert.Multiple(() =>
            {
                // Primitive Types
                Assert.That(CompareUtility.ObjectsAreEqual(1, 1), Is.True);
                Assert.That(CompareUtility.ObjectsAreEqual('a', 'z'), Is.False);
                Assert.That(CompareUtility.ObjectsAreEqual('a', 'A'), Is.False);
                Assert.That(CompareUtility.ObjectsAreEqual('0', 48), Is.True);
                Assert.That(CompareUtility.ObjectsAreEqual(true, true), Is.True);
                Assert.That(CompareUtility.ObjectsAreEqual(true, false), Is.False);
                Assert.That(CompareUtility.ObjectsAreEqual((object)42, false), Is.False);

                // Strings
                Assert.That(CompareUtility.ObjectsAreEqual(STRING_COMPARE_VALUE, STRING_COMPARE_VALUE), Is.True);

                // Objects
                Assert.That(CompareUtility.ObjectsAreEqual(object1, object1), Is.True);
                Assert.That(CompareUtility.ObjectsAreEqual(new object(), new object()), Is.False);
                Assert.That(CompareUtility.ObjectsAreEqual(object1, object2), Is.False);
                Assert.That(CompareUtility.ObjectsAreEqual(object1, null), Is.False);
                Assert.That(CompareUtility.ObjectsAreEqual((object?)null, null), Is.True);

                // Ambiguous objects
                Assert.That(CompareUtility.ObjectsAreEqual(ambiguousObject1, ambiguousObject2), Is.True);
                Assert.That(CompareUtility.ObjectsAreEqual(ambiguousObject1, ambiguousObject3), Is.False);

                // Collections
                Assert.That(CompareUtility.ObjectsAreEqual(array1, array2), Is.True);
                Assert.That(CompareUtility.ObjectsAreEqual(array1, array3), Is.False);
                Assert.That(CompareUtility.ObjectsAreEqual(array1, array4), Is.False);
                Assert.That(CompareUtility.ObjectsAreEqual(array1, array3.Reverse().ToArray()), Is.True);
                Assert.That(CompareUtility.ObjectsAreEqual(array1, array3.Reverse()), Is.False); // Different data Type
                Assert.That(CompareUtility.ObjectsAreEqual(list1, list2), Is.True);
                Assert.That(CompareUtility.ObjectsAreEqual(list1, list3), Is.False);
                Assert.That(CompareUtility.ObjectsAreEqual(list1, list4), Is.False);
                Assert.That(CompareUtility.ObjectsAreEqual(list1, list3.Reverse<int>().ToList()), Is.True);
                Assert.That(CompareUtility.ObjectsAreEqual(list1, list3.Reverse<int>()), Is.False); // Different data Type
                Assert.That(CompareUtility.ObjectsAreEqual(multiDimensionalArray1, multiDimensionalArray2), Is.True);
                Assert.That(CompareUtility.ObjectsAreEqual(multiDimensionalArray1, multiDimensionalArray3), Is.False);
                Assert.That(CompareUtility.ObjectsAreEqual(dictionary1, dictionary2), Is.True);
                Assert.That(CompareUtility.ObjectsAreEqual(dictionary1, dictionary3), Is.False);
            });
        }

        [Test]
        public void EnumerablesAreEqualTests()
        {
            var array1 = new int[] { 1, 2, 3, 4, 5 };
            var array2 = new int[] { 1, 2, 3, 4, 5 };
            var array3 = new int[] { 5, 4, 3, 2, 1 };
            var array4 = new int[] { 1, 2, 3, 4, 5, 6 };

            var list1 = array1.ToList();
            var list2 = array2.ToList();
            var list3 = array3.ToList();
            var list4 = array4.ToList();

            var multiDimensionalArray1 = new int[,] { { 0, 0 }, { 0, 1 }, { 1, 0 }, { 1, 1 } };
            var multiDimensionalArray2 = new int[,] { { 0, 0 }, { 0, 1 }, { 1, 0 }, { 1, 1 } };
            var multiDimensionalArray3 = new int[,] { { 0, 0 }, { 0, 1 }, { 1, 0 }, { 1, 1 }, { 2, 0 } };
            var multiDimensionalArray4 = new int[,] { { 0, 1 }, { 0, 0 }, { 1, 1 }, { 1, 0 } };

            var jaggedArray1 = new int[][] { new int[] { 1, 2, 3 }, new int[] { 4, 5, 6 } };
            var jaggedArray2 = new int[][] { new int[] { 1, 2, 3 }, new int[] { 4, 5, 6 } };
            var jaggedArray3 = new int[][] { new int[] { 1, 2, 3 }, new int[] { 4, 5, 6, 7 } };

            var dictionary1 = new Dictionary<int, string>() { { 1, "one" }, { 2, "two" } };
            var dictionary2 = new Dictionary<int, string>() { { 1, "one" }, { 2, "two" } };
            var dictionary3 = new Dictionary<int, string>() { { 1, "one" }, { 2, "Two" } };

            Assert.Multiple(() =>
            {
                Assert.That(CompareUtility.EnumerablesAreEqual(string.Empty, string.Empty), Is.True);
                Assert.That(CompareUtility.EnumerablesAreEqual("", string.Empty), Is.True);
                Assert.That(CompareUtility.EnumerablesAreEqual(" ", string.Empty), Is.False); // whitespace != empty
                Assert.That(CompareUtility.EnumerablesAreEqual(null, null), Is.True);
                Assert.That(CompareUtility.EnumerablesAreEqual(STRING_COMPARE_VALUE, STRING_COMPARE_VALUE), Is.True);
                Assert.That(CompareUtility.EnumerablesAreEqual(STRING_COMPARE_VALUE, STRING_COMPARE_VALUE.ToLower()), Is.False);
                Assert.That(CompareUtility.EnumerablesAreEqual(array1, array2), Is.True);
                Assert.That(CompareUtility.EnumerablesAreEqual(array1, array3), Is.False);
                Assert.That(CompareUtility.EnumerablesAreEqual(array1, array4), Is.False);
                Assert.That(CompareUtility.EnumerablesAreEqual(array1, array3.Reverse().ToArray()), Is.True); // Same data Type (int[] vs int[])
                Assert.That(CompareUtility.EnumerablesAreEqual(array1, array3.Reverse()), Is.False); // Different data Type (int[] vs IEnumerable<int>)
                Assert.That(CompareUtility.EnumerablesAreEqual(list1, list2), Is.True);
                Assert.That(CompareUtility.EnumerablesAreEqual(list1, list3), Is.False);
                Assert.That(CompareUtility.EnumerablesAreEqual(list1, list4), Is.False);
                Assert.That(CompareUtility.EnumerablesAreEqual(list1, list3.Reverse<int>().ToList()), Is.True); // Same data Type (List<int> vs List<int>)
                Assert.That(CompareUtility.EnumerablesAreEqual(list1, list3.Reverse<int>().ToArray()), Is.False); // Same data Type (List<int> vs int[])
                Assert.That(CompareUtility.EnumerablesAreEqual(list1, list3.Reverse<int>()), Is.False); // Same data Type (List<int> vs IEnumerable<int>)
                Assert.That(CompareUtility.EnumerablesAreEqual(multiDimensionalArray1, multiDimensionalArray2), Is.True);
                Assert.That(CompareUtility.EnumerablesAreEqual(multiDimensionalArray1, multiDimensionalArray3), Is.False);
                Assert.That(CompareUtility.EnumerablesAreEqual(multiDimensionalArray1, multiDimensionalArray4), Is.False);
                Assert.That(CompareUtility.EnumerablesAreEqual(jaggedArray1, jaggedArray2), Is.True);
                Assert.That(CompareUtility.EnumerablesAreEqual(jaggedArray1, jaggedArray3), Is.False);
                Assert.That(CompareUtility.EnumerablesAreEqual(dictionary1, dictionary2), Is.True);
                Assert.That(CompareUtility.EnumerablesAreEqual(dictionary1, dictionary3), Is.False);
            });
        }
    }
}
