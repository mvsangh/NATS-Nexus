using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NATS.Client.Core;
using NATS.Client.Hosting;

namespace Common.NATS;

public static class DependencyInjection
{
    public static IServiceCollection RegisterNatsService(this IServiceCollection services, IConfiguration configuration)
    {
        var url = Environment.GetEnvironmentVariable("NATS_URL") ?? "127.0.0.1:4222";
        var opts = new NatsOpts
        {
            Url = url,
            Name = "NATS.KV",
            Verbose = true
        };
        services.AddNats(configureOpts: _ => opts);

        return services;
    }

}
