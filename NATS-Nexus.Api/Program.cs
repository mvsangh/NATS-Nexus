using NATS_Nexus.Api.Endpoints;

var builder = WebApplication.CreateSlimBuilder(args);

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.TypeInfoResolverChain.Insert(0, AppJsonSerializerContext.Default);
});
builder.Services.AddNatsNexusApiServices();

var app = builder.Build();

app.MapGet("/", () => "NATS-Nexus API Welcome !");
app.AddNatsNexusApiEndpoints();
app.Run();