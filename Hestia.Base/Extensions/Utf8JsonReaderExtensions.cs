using System.Text.Json;

namespace Hestia.Base.Extensions
{
    /// <summary>
    /// Defines extension methods for use with <see cref="Utf8JsonReader"/>
    /// </summary>
    public static class Utf8JsonReaderExtensions
    {
        /// <summary>
        /// Peeks at the next token and returns the <see cref="JsonTokenType"/> without advancing the source reader
        /// </summary>
        /// <param name="reader">Source reader</param>
        /// <returns>Token type or <see cref="JsonTokenType.None"/></returns>
        public static JsonTokenType PeekAtNextTokenType(this Utf8JsonReader reader)
        {
            var tokenType = JsonTokenType.None;

            try
            {
                var readerCopy = reader;

                if (readerCopy.Read())
                {
                    tokenType = readerCopy.TokenType;
                }
            }
            catch
            {
                // Intentionally left blank
            }

            return tokenType;
        }

        /// <summary>
        /// Attempts to seek the source reader to the target index
        /// </summary>
        /// <param name="reader">Source reader</param>
        /// <param name="targetIndex">Target index</param>
        /// <param name="success">Out param indicating success</param>
        /// <returns>Reader at position when <paramref name="success"/> is <see langword="true"/></returns>
        public static Utf8JsonReader SeekToPosition(this Utf8JsonReader reader, long targetIndex, out bool success)
        {
            success = false;

            // If index is invalid, or reader is already ahead of target index, return reader
            if (targetIndex < 0 || reader.TokenStartIndex >= targetIndex)
            {
                return reader;
            }

            var readerCopy = reader;

            try
            {
                while (readerCopy.Read())
                {
                    var tokenStartIndex = readerCopy.TokenStartIndex;

                    // If we hit the target index without issue, return the reader at position
                    if (tokenStartIndex == targetIndex)
                    {
                        success = true;
                        return readerCopy;
                    }

                    // If we've moved beyond the target, we failed somehow
                    if (tokenStartIndex > targetIndex)
                    {
                        break;
                    }
                }
            }
            catch
            {
                // Intentionally left blank
            }

            return reader;
        }
    }
}
