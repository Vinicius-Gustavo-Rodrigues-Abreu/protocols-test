using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Udp.Workers;

public class ClientWorker(ILogger<ClientWorker> logger) : BackgroundService
{
    private readonly ILogger<ClientWorker> _logger = logger;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var endpoint = IPEndPoint.Parse("172.21.148.229:8080");

        using var socket = new Socket(
            AddressFamily.InterNetwork,
            SocketType.Dgram,
            ProtocolType.Udp
        );

        await socket.SendToAsync(Encoding.UTF8.GetBytes("PING"), endpoint, stoppingToken);

        _logger.LogInformation("Sent message: PING");

        var buffer = new byte[1024];

        await socket.ReceiveFromAsync(buffer, endpoint, stoppingToken);

        var message = Encoding.UTF8.GetString(buffer);

        _logger.LogInformation("Received message: {message}", message);

        await Task.Delay(1000, stoppingToken);
    }
}
