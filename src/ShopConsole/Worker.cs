using Grpc.Core;
using Orders.Protos;

namespace ShopConsole;

public class Worker : BackgroundService
{
    private readonly OrderService.OrderServiceClient _orders;
    private readonly ILogger<Worker> _logger;

    public Worker(OrderService.OrderServiceClient orders, ILogger<Worker> logger)
    {
        _orders = orders;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var call = _orders.Subscribe(new SubscribeRequest(), cancellationToken: stoppingToken);

                await foreach (var response in call.ResponseStream.ReadAllAsync(stoppingToken))
                {
                    Console.WriteLine($"Order received: {response.CrustId}");

                    foreach (var toppingId in response.ToppingIds)
                    {
                        Console.WriteLine($"    {toppingId}");
                    }

                    var dueBy = response.Time.ToDateTimeOffset().AddHours(.5);
                    Console.WriteLine($"Due by: {dueBy:t}");
                    Console.WriteLine();
                }
            }
            catch (OperationCanceledException)
            {
                if (stoppingToken.IsCancellationRequested)
                {
                    break;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error: {Message}", ex.Message);
                await Task.Delay(TimeSpan.FromSeconds(1), stoppingToken);
            }
        }
    }
}
