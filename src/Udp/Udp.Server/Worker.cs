using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Udp.Server;

public class Worker(ILogger<Worker> logger, IConfiguration configuration) : BackgroundService
{
    private readonly ILogger<Worker> _logger = logger;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var endpoint = IPEndPoint.Parse(configuration["AddressToBind"]!);

        using var socket = new Socket(
            AddressFamily.InterNetwork,
            SocketType.Dgram,
            ProtocolType.Udp
        );

        socket.Bind(endpoint);

        _logger.LogInformation("Receiving from {endpoint}", endpoint);

        while (!stoppingToken.IsCancellationRequested)
        {
            var buffer = new byte[1024];

            var socketReceiveFromResult = await socket.ReceiveFromAsync(
                buffer,
                endpoint,
                stoppingToken
            );

            var message = Encoding.UTF8.GetString(buffer);

            _logger.LogInformation("Received message: {message}", message);

            await socket.SendToAsync(
                Encoding.UTF8.GetBytes("PONG"),
                socketReceiveFromResult.RemoteEndPoint,
                stoppingToken
            );

            _logger.LogInformation("Sent message: PONG");
        }
    }
}
