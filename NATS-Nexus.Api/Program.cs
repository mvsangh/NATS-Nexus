using NATS_Nexus.Api.Endpoints;
using Common.NATS;

var builder = WebApplication.CreateSlimBuilder(args);

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.TypeInfoResolverChain.Insert(0, AppJsonSerializerContext.Default);
});

// Add services to the container.
builder.Services.AddNatsNexusApiServices();

// Register NATS service to DI container
builder.Services.RegisterNatsService(builder.Configuration);

var app = builder.Build();

app.MapGet("/", () => "NATS-Nexus API Welcome !");
app.AddNatsNexusApiEndpoints();

await app.RunAsync();