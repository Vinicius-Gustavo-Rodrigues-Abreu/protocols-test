using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Tcp.Server;

public class Worker(ILogger<Worker> logger, IConfiguration configuration) : BackgroundService
{
    private readonly ILogger<Worker> _logger = logger;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var endpoint = IPEndPoint.Parse(configuration["AddressToBind"]!);

        using var socket = new Socket(
            AddressFamily.InterNetwork,
            SocketType.Stream,
            ProtocolType.Tcp
        );

        socket.Bind(endpoint);
        socket.Listen(150);

        _logger.LogInformation("Listening on {endpoint}", endpoint);

        while (!stoppingToken.IsCancellationRequested)
        {
            using var clientSocket = await socket.AcceptAsync(stoppingToken);

            _logger.LogInformation(
                "Accepted connection from {endpoint}",
                clientSocket.RemoteEndPoint
            );

            var buffer = new byte[1024];

            await clientSocket.ReceiveAsync(buffer, stoppingToken);

            var message = Encoding.UTF8.GetString(buffer);

            _logger.LogInformation("Received message: {message}", message);

            await clientSocket.SendAsync(Encoding.UTF8.GetBytes($"PONG"), stoppingToken);

            _logger.LogInformation("Sent message: PONG");
        }
    }
}
