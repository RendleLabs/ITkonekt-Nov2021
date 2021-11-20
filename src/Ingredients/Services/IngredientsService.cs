using Grpc.Core;
using Ingredients.Data;
using Ingredients.Protos;

namespace Ingredients.Services;

internal class IngredientsService : Protos.IngredientsService.IngredientsServiceBase
{
    private readonly IToppingData _toppingData;
    private readonly ILogger<IngredientsService> _logger;

    public IngredientsService(IToppingData toppingData, ILogger<IngredientsService> logger)
    {
        _toppingData = toppingData;
        _logger = logger;
    }

    public override async Task<GetToppingsResponse> GetToppings(GetToppingsRequest request, ServerCallContext context)
    {
        _logger.LogInformation("Getting toppings");

        try
        {
            var toppings = await _toppingData.GetAsync(context.CancellationToken);
            var response = new GetToppingsResponse
            {
                Toppings =
                {
                    toppings.Select(t => new Topping
                    {
                        Id = t.Id,
                        Name = t.Name,
                        Price = t.Price
                    })
                }
            };

            return response;
        }
        catch (OperationCanceledException)
        {
            _logger.LogWarning("Operation was cancelled");
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error: {ex.Message}");
            throw;
        }
    }
}