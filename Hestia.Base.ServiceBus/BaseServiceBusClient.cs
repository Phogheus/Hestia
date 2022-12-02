using System;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;

namespace Hestia.Base.ServiceBus
{
    /// <summary>
    /// Defines a base Azure Service Bus client for creating a consumer or producer 
    /// </summary>
    public abstract class BaseServiceBusClient : IAsyncDisposable
    {
        #region Fields

        /// <summary>
        /// Returns the Queue or Topic name this client connects to
        /// </summary>
        protected readonly string _queueOrTopicName;

        /// <summary>
        /// Logger
        /// </summary>
        protected readonly ILogger _logger;

        private bool _isDisposed;

        #endregion Fields

        #region Properties

        /// <summary>
        /// Returns connection settings
        /// </summary>
        protected ServiceBusSettings Settings { get; }

        /// <summary>
        /// Returns the underlying client if created
        /// </summary>
        protected ServiceBusClient? Client { get; private set; }

        /// <summary>
        /// Returns true if the underlying client is set and not closed
        /// </summary>
        protected bool ClientIsConnected => !_isDisposed && Client != null && !Client.IsClosed;

        #endregion Properties

        #region Constructors

        /// <summary>
        /// Constructor for creating a new Service Bus Queue or Topic Client using configured <see cref="IOptions{TOptions}"/>
        /// </summary>
        /// <param name="queueOrTopicName">Queue or Topic to consume from</param>
        /// <param name="options">Options defining connection settings</param>
        /// <param name="connectImmediately">If true, this client will connect immediately before exiting the constructor</param>
        /// <param name="logger">Logger</param>
        protected BaseServiceBusClient(string queueOrTopicName, IOptions<ServiceBusSettings> options, bool connectImmediately = true, ILogger? logger = null)
            : this(queueOrTopicName, options.Value, connectImmediately, logger)
        {
        }

        /// <summary>
        /// Constructor for creating a new Service Bus Queue or Topic Client
        /// </summary>
        /// <param name="queueOrTopicName">Queue or Topic to consume from</param>
        /// <param name="settings">Connection settings</param>
        /// <param name="connectImmediately">If true, this client will connect immediately before exiting the constructor</param>
        /// <param name="logger">Logger</param>
        protected BaseServiceBusClient(string queueOrTopicName, ServiceBusSettings settings, bool connectImmediately = true, ILogger? logger = null)
        {
            _queueOrTopicName = queueOrTopicName ?? throw new ArgumentNullException(nameof(queueOrTopicName));

            Settings = settings ?? throw new ArgumentNullException(nameof(settings));
            Settings.ValidateOrThrow();

            _logger = logger ?? NullLogger.Instance;

            if (connectImmediately)
            {
                _ = Connect();
            }
        }

        #endregion Constructors

        #region Public Methods

        /// <summary>
        /// Creates the underlying <see cref="ServiceBusClient"/> connection and returns true if connection operations succeed
        /// </summary>
        /// <remarks>If client is already connected, this method just returns true</remarks>
        /// <returns>Success of connecting to Azure Service Bus</returns>
        public bool Connect()
        {
            // If already connected, return true
            if (ClientIsConnected) // ThrowIfClientIsDisposed is called here
            {
                return true;
            }

            Client = new ServiceBusClient(Settings.PrimaryOrSecondaryConnectionString);

            return ClientIsConnected && CreateAdditionalConnectors();
        }

        /// <summary>
        /// Disposes of this client and all resources
        /// </summary>
        /// <returns>Awaitable task representing the operation</returns>
        public async ValueTask DisposeAsync()
        {
            await Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion Public Methods

        #region Protected Methods

        /// <summary>
        /// Create any additional connectors after <see cref="Client"/> has been set
        /// </summary>
        /// <returns>True if operation succeeds</returns>
        protected abstract bool CreateAdditionalConnectors();

        /// <summary>
        /// Dispose task called prior to <see cref="Client"/> being disposed, if set
        /// </summary>
        /// <returns>Awaitable task representing the operation</returns>
        protected abstract Task OnClientBeingDisposed();

        /// <summary>
        /// Throws an <see cref="ObjectDisposedException"/> if <see cref="_isDisposed"/> is true
        /// </summary>
        /// <exception cref="ObjectDisposedException">Thrown if <see cref="_isDisposed"/> is true</exception>
        protected void ThrowIfClientIsDisposed()
        {
            if (_isDisposed)
            {
                throw new ObjectDisposedException(nameof(BaseServiceBusClient));
            }
        }

        #endregion Protected Methods

        #region Private Methods

        private async Task Dispose(bool disposeManaged)
        {
            if (!_isDisposed)
            {
                if (disposeManaged)
                {
                    await OnClientBeingDisposed();

                    if (Client != null)
                    {
                        await Client.DisposeAsync();
                    }
                }

                _isDisposed = true;
            }
        }

        #endregion Private Methods
    }
}
