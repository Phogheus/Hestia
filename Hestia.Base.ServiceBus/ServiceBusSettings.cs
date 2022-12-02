using System;
using System.Text.Json.Serialization;
using Azure.Messaging.ServiceBus;

namespace Hestia.Base.ServiceBus
{
    /// <summary>
    /// Defines connection and operation settings for an active Azure Service Bus connection
    /// </summary>
    public class ServiceBusSettings
    {
        #region Fields

        /// <summary>
        /// Microsoft defined minimum for how small a single batch can be
        /// </summary>
        public const int MINIMUM_BATCH_SIZE = 24;

        /// <summary>
        /// Error thrown when neither connection strings are set during a call to <see cref="ValidateOrThrow"/>
        /// </summary>
        public const string NO_CONNECTION_STRINGS_ERROR = "At least one connection string must be set.";

        /// <summary>
        /// Error thrown when <see cref="DefaultMaxSizeInBytes"/> is less than <see cref="MINIMUM_BATCH_SIZE"/>
        /// during a call to <see cref="ValidateOrThrow"/> 
        /// </summary>
        public const string BATCH_SIZE_INVALID_ERROR = "Microsoft defined minimum size for a single batch is 24 bytes.";

        #endregion Fields

        #region Properties

        /// <summary>
        /// Primary Azure Service Bus connection string
        /// </summary>
        public string PrimaryConnectionString { get; init; } = string.Empty;

        /// <summary>
        /// Secondary Azure Service Bus connection string
        /// </summary>
        public string SecondaryConnectionString { get; init; } = string.Empty;

        /// <summary>
        /// Max size in bytes for a single batch of messages from a producer
        /// </summary>
        public long DefaultMaxSizeInBytes { get; init; } = 1024 * 1024; // 1 megabyte

        /// <summary>
        /// Returns processor options if set
        /// </summary>
        public ServiceBusProcessorOptions? ProcessorOptions { get; init; }

        /// <summary>
        /// Returns <see cref="PrimaryConnectionString"/> if set, or <see cref="SecondaryConnectionString"/>
        /// </summary>
        [JsonIgnore]
        public string PrimaryOrSecondaryConnectionString => string.IsNullOrWhiteSpace(PrimaryConnectionString) ? SecondaryConnectionString : PrimaryConnectionString;

        /// <summary>
        /// Returns <see cref="SecondaryConnectionString"/> if set, or <see cref="PrimaryConnectionString"/>
        /// </summary>
        [JsonIgnore]
        public string SecondaryOrPrimaryConnectionString => string.IsNullOrWhiteSpace(SecondaryConnectionString) ? PrimaryConnectionString : SecondaryConnectionString;

        #endregion Properties

        #region Constructors

        /// <summary>
        /// Default constructor
        /// </summary>
        public ServiceBusSettings()
        {
        }

        /// <summary>
        /// Constructor creating, and validating, required settings
        /// </summary>
        /// <param name="primaryConnectionString">Primary Azure Service Bus connection string</param>
        /// <param name="secondaryConnectionString">Secondary Azure Service Bus connection string</param>
        /// <param name="defaultMaxSizeInBytes">Max size in bytes for a single batch of messages from a producer (see: <see cref="MINIMUM_BATCH_SIZE"/>)</param>
        /// <param name="processorOptions">Processor options for defining consumer settings</param>
        [JsonConstructor]
        public ServiceBusSettings(string primaryConnectionString, string secondaryConnectionString, long defaultMaxSizeInBytes, ServiceBusProcessorOptions? processorOptions)
        {
            PrimaryConnectionString = primaryConnectionString ?? string.Empty;
            SecondaryConnectionString = secondaryConnectionString ?? string.Empty;
            DefaultMaxSizeInBytes = defaultMaxSizeInBytes;
            ProcessorOptions = processorOptions;

            ValidateOrThrow();
        }

        #endregion Constructors

        #region Public Methods

        /// <summary>
        /// Validates the settings and either returns if validation passes, or throws an exception if validation fails
        /// </summary>
        public void ValidateOrThrow()
        {
            if (string.IsNullOrWhiteSpace(PrimaryConnectionString) && string.IsNullOrWhiteSpace(SecondaryConnectionString))
            {
                throw new InvalidOperationException(NO_CONNECTION_STRINGS_ERROR);
            }

            if (DefaultMaxSizeInBytes < MINIMUM_BATCH_SIZE)
            {
                throw new InvalidOperationException(BATCH_SIZE_INVALID_ERROR);
            }
        }

        #endregion Public Methods
    }
}
