namespace NATS_Nexus.Api.Endpoints.Configuration;

public static class ConfigurationRegistrationHelper
{
    public static RouteGroupBuilder AddConfigurationEndpoints(this RouteGroupBuilder groupBuilder)
    {
        groupBuilder.MapGet("/publisher", static () => "publisher");
        groupBuilder.MapGet("/consumer", static () => "consumer");
        groupBuilder.MapPost("/publisher/{publishRate}", async (AddUpdatePublisherConfiguration handler, int publishRate) => await handler.HandleAsync(publishRate));
        groupBuilder.MapPost("/consumer/{noOfConsumers}", async (AddUpdateConsumerConfiguration handler, int noOfConsumers) => await handler.HandleAsync(noOfConsumers));
        return groupBuilder;
    }
    public static void AddConfigurationServices(this IServiceCollection services)
    {
        services.AddTransient<AddUpdatePublisherConfiguration>();
        services.AddTransient<AddUpdateConsumerConfiguration>();
    }
}
