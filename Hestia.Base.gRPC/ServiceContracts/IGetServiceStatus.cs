using System.ServiceModel;
using System.Threading.Tasks;
using Hestia.Base.gRPC.Models;
using Hestia.Base.gRPC.ProtoContracts;

namespace Hestia.Base.gRPC.ServiceContracts
{
    /// <summary>
    /// Defines a basic service contract that allows for "pinging" a service
    /// </summary>
    [ServiceContract]
    public interface IGetServiceStatus
    {
        /// <summary>
        /// Requests the current operating status from the implementing endpoint
        /// </summary>
        /// <returns>Operating status</returns>
        [OperationContract]
        ValueTask<ProtoMessage<ServiceStatusMessage>> GetCurrentStatus();
    }
}
