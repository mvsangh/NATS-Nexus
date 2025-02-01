using NATS_Nexeus_Consumer;
using Common.NATS;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<Worker>();
builder.Services.RegisterNatsService(builder.Configuration);

var host = builder.Build();
await host.RunAsync();
