using Amazon.Lambda.AspNetCoreServer;

namespace Integrador
{
    public class ServerlessEntryPoint : APIGatewayHttpApiV2ProxyFunction
    {
        protected override void Init(IWebHostBuilder builder)
        {
            builder.UseStartup<Startup>();
        }
    }
}