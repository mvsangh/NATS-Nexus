using NATS_Nexus.Émetteur;
using Common.NATS;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.RegisterNatsService(builder.Configuration);
builder.Services.AddHostedService<Worker>();

var host = builder.Build();
host.Run();
