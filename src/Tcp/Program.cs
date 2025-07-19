using Tcp.Workers;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<ServerWorker>();
builder.Services.AddHostedService<ClientWorker>();

var host = builder.Build();
host.Run();
