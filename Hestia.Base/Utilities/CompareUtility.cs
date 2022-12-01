using System;
using System.Collections;

namespace Hestia.Base.Utilities
{
    /// <summary>
    /// Defines a basic set of compare utilities for comparing various object types
    /// </summary>
    public static class CompareUtility
    {
        /// <summary>
        /// Returns true if both <paramref name="left"/> and <paramref name="right"/> are null,
        /// or if <paramref name="left"/> equals <paramref name="right"/> using an optional
        /// <see cref="StringComparison"/> type
        /// </summary>
        /// <param name="left">String to compare against <paramref name="right"/></param>
        /// <param name="right">String to compare against <paramref name="left"/></param>
        /// <param name="comparisonType">Optional <see cref="StringComparison"/>; default is <see cref="StringComparison.Ordinal"/></param>
        /// <returns>True if strings are both null or equal</returns>
        public static bool StringsAreEqual(string? left, string? right, StringComparison comparisonType = StringComparison.Ordinal)
        {
            return (left is null && right is null) || (left?.Equals(right, comparisonType) ?? false);
        }

        /// <summary>
        /// Returns true if both <paramref name="left"/> and <paramref name="right"/> are considered equal
        /// </summary>
        /// <typeparam name="T">Type of values being compared</typeparam>
        /// <param name="left">Value to compare against <paramref name="right"/></param>
        /// <param name="right">Value to compare against <paramref name="left"/></param>
        /// <returns>True if both <paramref name="left"/> and <paramref name="right"/> are considered equal</returns>
        public static bool ObjectsAreEqual<T>(T? left, T? right)
        {
            var leftIsNull = left is null;
            var rightIsNull = right is null;

            // If both are null, return true
            if (leftIsNull && rightIsNull)
            {
                return true;
            }

            // If neither are null
            if (!leftIsNull && !rightIsNull)
            {
                var objectType = left!.GetType();

                // If T implements IEquatable, prefer IEquatable.Equals over object.Equals
                if (objectType.IsAssignableTo(typeof(IEquatable<T>)))
                {
                    return ((IEquatable<T>)left!).Equals(right);
                }
                else if (objectType.IsArray)
                {
                    return EnumerablesAreEqual((IEnumerable?)left, (IEnumerable?)right);
                }
                else if (objectType.IsAssignableTo(typeof(IEnumerable)))
                {
                    return EnumerablesAreEqual((IEnumerable?)left, (IEnumerable?)right);
                }

                return left!.Equals(right);
            }

            return false;
        }

        /// <summary>
        /// Returns true if both <paramref name="left"/> and <paramref name="right"/> appear to be equal
        /// </summary>
        /// <param name="left">Value to compare against <paramref name="right"/></param>
        /// <param name="right">Value to compare against <paramref name="left"/></param>
        /// <returns>True if both <paramref name="left"/> and <paramref name="right"/> appear to be equal</returns>
        public static bool EnumerablesAreEqual(IEnumerable? left, IEnumerable? right)
        {
            if (left is null && right is null)
            {
                return true;
            }

            if (left is not null && right is not null)
            {
                var leftType = left.GetType();

                if (leftType != right.GetType())
                {
                    return false;
                }

                // Type string is assignable to IEnumerable, so check for that edge case
                if (leftType == typeof(string))
                {
                    return StringsAreEqual(left as string, right as string);
                }

                // Compare each collection, value for value
                var leftEnumerator = left.GetEnumerator();
                var rightEnumerator = right.GetEnumerator();

                while (true)
                {
                    var leftMoveNext = leftEnumerator.MoveNext();
                    var rightMoveNext = rightEnumerator.MoveNext();

                    if (leftMoveNext && rightMoveNext)
                    {
                        if (!ObjectsAreEqual(leftEnumerator.Current, rightEnumerator.Current))
                        {
                            return false;
                        }
                    }
                    else if (leftMoveNext != rightMoveNext)
                    {
                        // Different size collections
                        return false;
                    }
                    else
                    {
                        // Both false; collections were empty, or we've successfully compared all values
                        return true;
                    }
                }
            }

            return false;
        }
    }
}
