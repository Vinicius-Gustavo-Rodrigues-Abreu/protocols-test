using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Tcp.Client;

public class Worker(ILogger<Worker> logger, IConfiguration configuration) : BackgroundService
{
    private readonly ILogger<Worker> _logger = logger;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var endpoint = IPEndPoint.Parse(configuration["AddressToConnect"]!);

        using var socket = new Socket(
            AddressFamily.InterNetwork,
            SocketType.Stream,
            ProtocolType.Tcp
        );

        await socket.ConnectAsync(endpoint);

        _logger.LogInformation("Connecting on {endpoint}", endpoint);

        await socket.SendAsync(Encoding.UTF8.GetBytes("PING"), SocketFlags.None, stoppingToken);

        _logger.LogInformation("Sent message: PING");

        var buffer = new byte[1024];

        await socket.ReceiveAsync(buffer, SocketFlags.None, stoppingToken);

        var message = Encoding.UTF8.GetString(buffer);

        _logger.LogInformation("Received message: {message}", message);

        await Task.Delay(1000, stoppingToken);
    }
}
