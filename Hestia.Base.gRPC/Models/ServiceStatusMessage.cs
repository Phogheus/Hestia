namespace Hestia.Base.gRPC.Models
{
    /// <summary>
    /// Defines a simple message used to get a remote service's current operating status
    /// </summary>
    /// <param name="StatusCode">Current service status code</param>
    public record ServiceStatusMessage(int StatusCode);
}
