using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Ingredients.Protos;
using Orders.Protos;

namespace Orders.Services;

public class OrderService : Protos.OrderService.OrderServiceBase
{
    private readonly IngredientsService.IngredientsServiceClient _ingredients;
    private readonly ILogger<OrderService> _logger;

    public OrderService(IngredientsService.IngredientsServiceClient ingredients, ILogger<OrderService> logger)
    {
        _ingredients = ingredients;
        _logger = logger;
    }

    public override async Task<PlaceOrderResponse> PlaceOrder(PlaceOrderRequest request, ServerCallContext context)
    {
        var decrementToppingsRequest = new DecrementToppingsRequest
        {
            ToppingIds = { request.ToppingIds }
        };
        await _ingredients.DecrementToppingsAsync(decrementToppingsRequest);

        var decrementCrustsRequest = new DecrementCrustsRequest
        {
            CrustId = request.CrustId
        };
        await _ingredients.DecrementCrustsAsync(decrementCrustsRequest);

        return new PlaceOrderResponse
        {
            Time = DateTimeOffset.UtcNow.ToTimestamp()
        };
    }
}