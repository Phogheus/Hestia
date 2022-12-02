using System;
using System.Threading.Tasks;
using Hestia.Base.gRPC.ServiceContracts;
using ProtoBuf.Grpc.Client;

namespace Hestia.Web.Examples.gRPC.Client
{
    internal class Program
    {
        private static async Task Main(string[] args)
        {
            using (var channel = Grpc.Net.Client.GrpcChannel.ForAddress("https://localhost:7222/"))
            {
                var serviceProxy = channel.CreateGrpcService<IGetServiceStatus>();
                var response = await serviceProxy.GetCurrentStatus();
                var message = response.GetMessage();

                if (message != null)
                {
                    Console.WriteLine("Response Code from server: {0}", message.StatusCode);
                }
                else
                {
                    Console.WriteLine("Response from server failed!");
                }
            }

            _ = Console.ReadKey(true);
        }
    }
}
