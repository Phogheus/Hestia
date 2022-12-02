using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Hestia.Base.Utilities
{
    /// <summary>
    /// Defines a predictable UUID following RFC 4122 §4.3
    /// </summary>
    /// <see href="https://www.rfc-editor.org/rfc/rfc4122.txt"/>
    public static class PredictableGuid
    {
        #region Fields

        private const int VERSION_5_SHIFT = 5 << 4;

        private static readonly Guid DEFAULT_NAMESPACE_GUID = Guid.Parse("304F4362-6D9E-4380-82FE-68E66464202F");

        #endregion Fields

        #region Public Methods

        /// <summary>
        /// Creates a new UUID using the default namespace UUID
        /// </summary>
        /// <param name="name">UUID input name</param>
        /// <returns>Predictable UUID based on name</returns>
        public static Guid NewGuid(string name)
        {
            return NewGuid(DEFAULT_NAMESPACE_GUID, name);
        }

        /// <summary>
        /// Creates a new UUID using a specific namespace UUID
        /// </summary>
        /// <param name="namespaceUUID">Specific namespace UUID</param>
        /// <param name="name">UUID input name</param>
        /// <returns>Predictable UUID based on name</returns>
        [SuppressMessage("Security", "CA5350:Do Not Use Weak Cryptographic Algorithms", Justification = "RFC 4122 §4.3")]
        public static Guid NewGuid(Guid namespaceUUID, string name)
        {
            // Convert the name to a canonical sequence of octets (bytes)
            var nameBytes = Encoding.UTF8.GetBytes(name);

            // Put the name space UUID in network byte order (RFC 4122 §4.1.2)
            var namespaceBytes = namespaceUUID.ToByteArray();
            ChangeUuidByteOrder(namespaceBytes);

            // Compute the hash of the name space ID concatenated with the name
            var bytesToComputeHashWith = namespaceBytes.Concat(nameBytes).ToArray();

            byte[] hash;
            using (var algorithm = SHA1.Create())
            {
                hash = algorithm.ComputeHash(bytesToComputeHashWith);
            }

            var newGuidBytes = new byte[namespaceBytes.Length];

            // Set octets zero through 3 of the time_low field to octets zero through 3 of the hash
            Array.Copy(hash, 0, newGuidBytes, 0, sizeof(uint));

            // Set octets zero and one of the time_mid field to octets 4 and 5 of the hash
            Array.Copy(hash, 4, newGuidBytes, 4, sizeof(ushort));

            // Set octets zero and one of the time_hi_and_version field to octets 6 and 7 of the hash
            Array.Copy(hash, 6, newGuidBytes, 6, sizeof(ushort));

            // Set the four most significant bits (bits 12 through 15) of the time_hi_and_version field to the appropriate 4 - bit version number from Section 4.1.3.
            newGuidBytes[6] = (byte)((newGuidBytes[6] & 0x0F) | VERSION_5_SHIFT);

            // Set the clock_seq_hi_and_reserved field to octet 8 of the hash
            newGuidBytes[8] = hash[8];

            // Set the two most significant bits (bits 6 and 7) of the clock_seq_hi_and_reserved to zero and one, respectively
            newGuidBytes[8] = (byte)((newGuidBytes[8] & 0x3F) | 0x80);

            // Set the clock_seq_low field to octet 9 of the hash
            newGuidBytes[9] = hash[9];

            // Set octets zero through five of the node field to octets 10 through 15 of the hash
            Array.Copy(hash, 10, newGuidBytes, 10, sizeof(uint) + sizeof(ushort));

            // Convert the resulting UUID to local byte order
            ChangeUuidByteOrder(newGuidBytes);

            return new Guid(newGuidBytes);
        }

        #endregion Public Methods

        #region Private Methods

        private static void ChangeUuidByteOrder(byte[] source)
        {
            static void Swap(byte[] input, int sourceIndex, int targetIndex)
            {
                (input[targetIndex], input[sourceIndex]) = (input[sourceIndex], input[targetIndex]);
            };

            // Octet 0 - 3
            Swap(source, 0, 3);
            Swap(source, 1, 2);

            // Octet 4 - 5
            Swap(source, 4, 5);

            // Octet 6 - 7
            Swap(source, 6, 7);

            // Octet 8
            // Octet 9

            // Octet 10 - 15
            Swap(source, 10, 15);
            Swap(source, 11, 14);
            Swap(source, 12, 13);
        }

        #endregion Private Methods
    }
}
