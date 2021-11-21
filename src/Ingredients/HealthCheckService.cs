using Grpc.Health.V1;
using Grpc.HealthCheck;
using Ingredients.Data;

namespace Ingredients;

public class HealthCheckService : BackgroundService
{
    private readonly IToppingData _toppingData;
    private readonly HealthServiceImpl _healthService;

    public HealthCheckService(IToppingData toppingData, HealthServiceImpl healthService)
    {
        _toppingData = toppingData;
        _healthService = healthService;
        
        _healthService.SetStatus("Ingredients", HealthCheckResponse.Types.ServingStatus.NotServing);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var _ = await _toppingData.GetAsync(stoppingToken);
                _healthService.SetStatus("Ingredients", HealthCheckResponse.Types.ServingStatus.Serving);
                await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch
            {
                _healthService.SetStatus("Ingredients", HealthCheckResponse.Types.ServingStatus.NotServing);
            }
        }
    }
}