using System;
using System.Threading.Tasks;
using Hestia.Base.gRPC.Models;
using Hestia.Base.gRPC.ProtoContracts;
using Hestia.Base.gRPC.ServiceContracts;
using Hestia.Web.Examples.gRPC.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Hestia.Web.Examples.gRPC.Services
{
    public class GetServiceStatusService : IGetServiceStatus
    {
        #region Fields

        private const string METHOD_CALLED_LOG_FORMAT = "Called endpoint '{OperationName}'.";
        private const string METHOD_THREW_EX_LOG_FORMAT = "Call to endpoint '{OperationName}' threw an exception.";
        private const string METHOD_EXITED_LOG_FORMAT = "Exited endpoint '{OperationName}'.";

        private static readonly ProtoMessage<ServiceStatusMessage> GENERIC_ERROR = new ProtoMessage<ServiceStatusMessage>(new ServiceStatusMessage(-99));

        private readonly MyApplicationConfiguration _config;
        private readonly ILogger _logger;

        #endregion Fields

        #region Constructors

        public GetServiceStatusService(IOptions<MyApplicationConfiguration> options, ILogger<GetServiceStatusService> logger)
        {
            _config = options.Value;
            _logger = logger;
        }

        #endregion Constructors

        #region Public Methods

        public async ValueTask<ProtoMessage<ServiceStatusMessage>> GetCurrentStatus()
        {
            ProtoMessage<ServiceStatusMessage> response;

            using (var scope = _logger.BeginScope(METHOD_CALLED_LOG_FORMAT, nameof(GetCurrentStatus)))
            {
                try
                {
                    if (_config.MyOtherConfigValue > 10)
                    {
                        throw new Exception("Something bad happened, oh no!");
                    }

                    var serviceStatus = new ServiceStatusMessage(_config.MyOtherConfigValue);
                    var message = new ProtoMessage<ServiceStatusMessage>(serviceStatus);

                    response = await ValueTask.FromResult(message);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, METHOD_THREW_EX_LOG_FORMAT, nameof(GetCurrentStatus));

                    response = await ValueTask.FromResult(GENERIC_ERROR);
                }
                finally
                {
                    _logger.LogTrace(METHOD_EXITED_LOG_FORMAT, nameof(GetCurrentStatus));
                }
            }

            return response;
        }

        #endregion Public Methods
    }
}
