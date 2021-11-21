﻿using Frontend.Models;
using Microsoft.AspNetCore.Mvc;
using Orders.Protos;

namespace Frontend.Controllers;

[Route("orders")]
public class OrdersController : Controller
{
    private readonly OrderService.OrderServiceClient _orders;
    private readonly ILogger<OrdersController> _log;

    public OrdersController(OrderService.OrderServiceClient orders, ILogger<OrdersController> log)
    {
        _orders = orders;
        _log = log;
    }

    [HttpPost]
    public async Task<IActionResult> Order([FromForm] HomeViewModel viewModel)
    {
        try
        {
            var placeOrder = new PlaceOrderRequest
            {
                CrustId = viewModel.SelectedCrust,
                ToppingIds =
                {
                    viewModel.Toppings.Where(t => t.Selected)
                        .Select(t => t.Id)
                }
            };

            await _orders.PlaceOrderAsync(placeOrder);
        
            return View();
        }
        catch (Exception ex)
        {
            _log.LogError(ex, ex.Message);
            throw;
        }
    }
}