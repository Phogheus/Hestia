using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Hestia.Base.ServiceBus
{
    /// <summary>
    /// Defines a simple Azure Service Bus Queue or Topic Producer
    /// </summary>
    public sealed class BasicServiceBusProducer : BaseServiceBusClient
    {
        #region Fields

        private readonly CreateMessageBatchOptions _defaultBatchOptions;

        private ServiceBusSender? _sender;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Constructor for creating a new Service Bus Queue or Topic Producer using configured <see cref="IOptions{TOptions}"/>
        /// </summary>
        /// <param name="queueOrTopicName">Queue or Topic to consume from</param>
        /// <param name="options">Options defining connection settings</param>
        /// <param name="connectImmediately">If true, this client will connect immediately before exiting the constructor</param>
        /// <param name="logger">Logger</param>
        public BasicServiceBusProducer(string queueOrTopicName, IOptions<ServiceBusSettings> options, bool connectImmediately = true, ILogger? logger = null)
            : this(queueOrTopicName, options.Value, connectImmediately, logger)
        {
        }

        /// <summary>
        /// Constructor for creating a new Service Bus Queue or Topic Producer
        /// </summary>
        /// <param name="queueOrTopicName">Queue or Topic to consume from</param>
        /// <param name="settings">Connection settings</param>
        /// <param name="connectImmediately">If true, this client will connect immediately before exiting the constructor</param>
        /// <param name="logger">Logger</param>
        public BasicServiceBusProducer(string queueOrTopicName, ServiceBusSettings settings, bool connectImmediately = true, ILogger? logger = null)
            : base(queueOrTopicName, settings, connectImmediately, logger)
        {
            _defaultBatchOptions = new CreateMessageBatchOptions
            {
                MaxSizeInBytes = Settings.DefaultMaxSizeInBytes
            };
        }

        #endregion Constructors

        #region Public Methods

        /// <summary>
        /// Sends a single message and returns the success result
        /// </summary>
        /// <param name="message">Message to send</param>
        /// <returns>True if successful</returns>
        public async Task<bool> SendMessageAsync(string message)
        {
            return !string.IsNullOrWhiteSpace(message) && await SendMessageAsync(new ServiceBusMessage(message));
        }

        /// <summary>
        /// Sends a single message and returns the success result
        /// </summary>
        /// <param name="message">Message to send</param>
        /// <returns>True if successful</returns>
        public async Task<bool> SendMessageAsync(ServiceBusMessage message)
        {
            ThrowIfClientIsDisposed();

            if (IsSenderValid() && ClientIsConnected && message != null)
            {
                await _sender!.SendMessageAsync(message);

                return true;
            }

            return false;
        }

        /// <summary>
        /// Sends the given messages as a batched operation, returning how many messages were successfully batched and sent
        /// </summary>
        /// <param name="messages">Messages to send</param>
        /// <param name="options">Optional batch settings overriding <see cref="_defaultBatchOptions"/></param>
        /// <returns>Number of messages successfully batched and sent</returns>
        public async Task<int> SendMessageBatchAsync(IEnumerable<string> messages, CreateMessageBatchOptions? options = null)
        {
            var validMessages = messages?.Where(x => !string.IsNullOrWhiteSpace(x))
                                         .Select(x => new ServiceBusMessage(x));

            return validMessages?.Any() ?? false
                ? await SendMessageBatchAsync(validMessages, options)
                : await Task.FromResult(0);
        }

        /// <summary>
        /// Sends the given messages as a batched operation, returning how many messages were successfully batched and sent
        /// </summary>
        /// <param name="messages">Messages to send</param>
        /// <param name="options">Optional batch settings overriding <see cref="_defaultBatchOptions"/></param>
        /// <returns>Number of messages successfully batched and sent</returns>
        public async Task<int> SendMessageBatchAsync(IEnumerable<ServiceBusMessage> messages, CreateMessageBatchOptions? options = null)
        {
            ThrowIfClientIsDisposed();

            if (!IsSenderValid() || !ClientIsConnected)
            {
                return 0;
            }

            var messagesToSend = messages?.Where(x => x != null).ToArray() ?? Array.Empty<ServiceBusMessage>();
            var messageCountToSend = messagesToSend.Length;

            if (messageCountToSend == 0)
            {
                return 0;
            }

            var messagesProcessed = 0; // Number of messages processed so far
            var messagesBatched = 0; // Number of messages batched successfully

            var batch = await _sender!.CreateMessageBatchAsync(options ?? _defaultBatchOptions);

            for (var i = 0; i < messageCountToSend; i++)
            {
                // Attempt to add current message to batch
                // This will fail if the batch is full, or the message is too large
                if (!batch.TryAddMessage(messagesToSend[i]))
                {
                    // If we failed to batch the message, and the current batch size is 0
                    // that means the message is too large. Mark this message as processed
                    // and move on.
                    if (batch.SizeInBytes == 0)
                    {
                        messagesProcessed++;
                        continue;
                    }

                    // If we failed to batch the current message and we have bytes in the batch
                    // the batch may be full, so send the current batch and try again.
                    await _sender.SendMessagesAsync(batch);

                    // Dispose of batch and create new
                    batch.Dispose();
                    batch = await _sender.CreateMessageBatchAsync(options ?? _defaultBatchOptions);

                    // Repeat message attempt
                    i--;
                    continue;
                }

                messagesProcessed++;
                messagesBatched++;
            }

            // If any messages are remaining, send them
            if (batch.Count > 0)
            {
                await _sender.SendMessagesAsync(batch);
                batch.Dispose();
            }

            return messagesBatched;
        }

        #endregion Public Methods

        #region Protected Methods

        /// <summary>
        /// Creates the underlying <see cref="ServiceBusSender"/> for this producer
        /// </summary>
        /// <returns>True if operation succeeds</returns>
        protected override bool CreateAdditionalConnectors()
        {
            if (ClientIsConnected)
            {
                _sender = Client!.CreateSender(_queueOrTopicName);

                return true;
            }

            return false;
        }

        /// <summary>
        /// Disposes of the underlying <see cref="ServiceBusProcessor"/>
        /// </summary>
        /// <returns>Awaitable task representing the operation</returns>
        protected override async Task OnClientBeingDisposed()
        {
            if (_sender != null)
            {
                await _sender.DisposeAsync();
            }
        }

        #endregion Protected Methods

        #region Private Methods

        private bool IsSenderValid()
        {
            return _sender != null && _sender.IsClosed;
        }

        #endregion Private Methods
    }
}
