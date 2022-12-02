using System;
using System.Text.Json;
using ProtoBuf;

namespace Hestia.Base.Services.ProtoContracts
{
    /// <summary>
    /// Defines a generic protobuf wrapper for any kind of required message
    /// </summary>
    /// <typeparam name="T">Underlying type carried in <see cref="MessageBytes"/></typeparam>
    [ProtoContract]
    public class ProtoMessage<T>
    {
        #region Properties

        /// <summary>
        /// Returns the serialized bytes of the underlying message object
        /// </summary>
        [ProtoMember(1, IsPacked = true, OverwriteList = true)]
        public byte[] MessageBytes { get; init; }

        #endregion Properties

        #region Constructors

        private ProtoMessage()
        {
            MessageBytes = Array.Empty<byte>();
        }

        /// <summary>
        /// Constructor taking the value to serialize and optional serializer options
        /// </summary>
        /// <param name="message">Message to serialize and transmit</param>
        /// <param name="options">Options for serialization (default null)</param>
        public ProtoMessage(T message, JsonSerializerOptions? options = null)
        {
            MessageBytes = JsonSerializer.SerializeToUtf8Bytes(message, options);
        }

        #endregion Constructors

        #region Public Methods

        /// <summary>
        /// Deserializes and returns the underlying message
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        public T? GetMessage(JsonSerializerOptions? options = null)
        {
            return (MessageBytes?.Length ?? 0) > 0
                ? JsonSerializer.Deserialize<T>(MessageBytes, options)
                : default;
        }

        #endregion Public Methods
    }
}
