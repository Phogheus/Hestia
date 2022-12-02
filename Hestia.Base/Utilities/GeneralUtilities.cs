using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Hestia.Base.Utilities
{
    /// <summary>
    /// Defines common general utility methods
    /// </summary>
    public static class GeneralUtilities
    {
        /// <summary>
        /// Returns the underlying type of the given <see cref="IEnumerable"/> Type
        /// </summary>
        /// <typeparam name="T">Type of <see cref="IEnumerable"/></typeparam>
        /// <returns>Underlying type, or <typeparamref name="T"/> if Type can't be determined</returns>
        public static Type GetElementTypeOfIEnumerable<T>() where T : IEnumerable
        {
            var type = typeof(T);

            if (type.IsArray)
            {
                return type.GetElementType()!;
            }
            else if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IEnumerable<>))
            {
                return type.GetGenericArguments()[0];
            }

            return type.GetInterfaces()
                .Where(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IEnumerable<>))
                .Select(x => x.GenericTypeArguments[0])
                .FirstOrDefault() ?? type;
        }
    }
}
