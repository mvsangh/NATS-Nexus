namespace NATS_Nexus.Api.Endpoints;

public static class EndpointRegistrationHelper
{
    public static void AddNatsNexusApiEndpoints(this WebApplication app)
    {
        app.MapGroup("/config").AddConfigurationEndpoints();
    }
}

public static class ServiceRegistrationHelper
{
    public static void AddNatsNexusApiServices(this IServiceCollection services)
    {
        services.AddConfigurationServices();
    }
}
