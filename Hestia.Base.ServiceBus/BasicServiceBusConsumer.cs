using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Hestia.Base.ServiceBus
{
    /// <summary>
    /// Defines a simple Azure Service Bus Queue or Topic Consumer
    /// </summary>
    public sealed class BasicServiceBusConsumer : BaseServiceBusClient
    {
        #region Delegates & Events

        /// <summary>
        /// Delegate for processing a received message
        /// </summary>
        /// <param name="receivedMessage">Received message</param>
        public delegate void ProcessReceivedMessage(ServiceBusReceivedMessage receivedMessage);

        /// <summary>
        /// Delegate for processing a received error
        /// </summary>
        /// <param name="arg">Received error arguments</param>
        public delegate void ProcessErrorMessage(ProcessErrorEventArgs arg);

        #endregion Delegates & Events

        #region Fields

        private readonly string? _subscriptionName;

        private ServiceBusProcessor? _processor;

        #endregion Fields

        #region Properties

        /// <summary>
        /// Called when processor receives a message via <see cref="ServiceBusProcessor.ProcessMessageAsync"/>
        /// </summary>
        public ProcessReceivedMessage? OnMessageReceived { get; set; }

        /// <summary>
        /// Called when processor receives an error via <see cref="ServiceBusProcessor.ProcessErrorAsync"/>
        /// </summary>
        public ProcessErrorMessage? OnErrorReceived { get; set; }

        #endregion Properties

        #region Constructors

        /// <summary>
        /// Constructor for creating a new Service Bus Queue Consumer using configured <see cref="IOptions{TOptions}"/>
        /// </summary>
        /// <param name="queueName">Queue to consume from</param>
        /// <param name="options">Options defining connection settings</param>
        /// <param name="connectImmediately">If true, this client will connect immediately before exiting the constructor</param>
        /// <param name="logger">Logger</param>
        public BasicServiceBusConsumer(string queueName, IOptions<ServiceBusSettings> options, bool connectImmediately = true, ILogger? logger = null)
            : base(queueName, options.Value, connectImmediately, logger)
        {
        }

        /// <summary>
        /// Constructor for creating a new Service Bus Queue Consumer
        /// </summary>
        /// <param name="queueName">Queue to consume from</param>
        /// <param name="settings">Connection settings</param>
        /// <param name="connectImmediately">If true, this client will connect immediately before exiting the constructor</param>
        /// <param name="logger">Logger</param>
        public BasicServiceBusConsumer(string queueName, ServiceBusSettings settings, bool connectImmediately = true, ILogger? logger = null)
            : base(queueName, settings, connectImmediately, logger)
        {
        }

        /// <summary>
        /// Constructor for creating a new Service Bus Topic Consumer using configured <see cref="IOptions{TOptions}"/>
        /// </summary>
        /// <param name="topicName">Topic to consume from</param>
        /// <param name="subscriptionName">Subscription name</param>
        /// <param name="options">Options defining connection settings</param>
        /// <param name="connectImmediately">If true, this client will connect immediately before exiting the constructor</param>
        /// <param name="logger">Logger</param>
        public BasicServiceBusConsumer(string topicName, string subscriptionName, IOptions<ServiceBusSettings> options, bool connectImmediately = true, ILogger? logger = null)
            : this(topicName, subscriptionName, options.Value, connectImmediately, logger)
        {
        }

        /// <summary>
        /// Constructor for creating a new Service Bus Topic Consumer
        /// </summary>
        /// <param name="topicName">Topic to consume from</param>
        /// <param name="subscriptionName">Subscription name</param>
        /// <param name="settings">Connection settings</param>
        /// <param name="connectImmediately">If true, this client will connect immediately before exiting the constructor</param>
        /// <param name="logger">Logger</param>
        public BasicServiceBusConsumer(string topicName, string subscriptionName, ServiceBusSettings settings, bool connectImmediately = true, ILogger? logger = null)
            : base(topicName, settings, false, logger)
        {
            _subscriptionName = subscriptionName;

            // We need to set subscription name before creating connectors, so
            // if connect immediately is true, send false to base and call it
            // here instead
            if (connectImmediately)
            {
                _ = Connect();
            }
        }

        #endregion Constructors

        #region Protected Methods

        /// <summary>
        /// Creates the underlying <see cref="ServiceBusProcessor"/> for this consumer
        /// </summary>
        /// <returns>True if operation succeeds</returns>
        protected override bool CreateAdditionalConnectors()
        {
            if (ClientIsConnected)
            {
                // If subscription name is set create topic processor, otherwise make queue processor
                _processor = string.IsNullOrWhiteSpace(_subscriptionName)
                    ? Client!.CreateProcessor(_queueOrTopicName, Settings.ProcessorOptions ?? new ServiceBusProcessorOptions())
                    : Client!.CreateProcessor(_queueOrTopicName, _subscriptionName, Settings.ProcessorOptions ?? new ServiceBusProcessorOptions());

                _processor.ProcessMessageAsync += ProcessMessageAsync;
                _processor.ProcessErrorAsync += ProcessErrorAsync;

                _ = _processor.StartProcessingAsync();

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
            if (_processor != null)
            {
                await _processor.StopProcessingAsync();
                await _processor.DisposeAsync();
            }
        }

        #endregion Protected Methods

        #region Private Methods

        private async Task ProcessErrorAsync(ProcessErrorEventArgs arg)
        {
            if (arg.Exception != null)
            {
                _logger.LogWarning(arg.Exception, "Received error '{ErrorMessage}' from Source '{Source}'.", arg.Exception.Message, arg.ErrorSource);
            }
            else
            {
                _logger.LogWarning("Received unspecified error from Source '{Source}'.", arg.ErrorSource);
            }

            OnErrorReceived?.Invoke(arg);

            await Task.Delay(0);
        }

        private async Task ProcessMessageAsync(ProcessMessageEventArgs arg)
        {
            _logger.LogTrace("Received message on consumer.");

            OnMessageReceived?.Invoke(arg.Message);

            await Task.Delay(0);
        }

        #endregion Private Methods
    }
}
