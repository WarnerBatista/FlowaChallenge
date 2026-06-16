using Microsoft.AspNetCore.Mvc;
using OrderGenerator.Api.Dtos;
using OrderGenerator.Api.Services.Interfaces;

namespace OrderGenerator.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class OrdersController : ControllerBase
{
    private readonly IFixInitiatorService _fixService;

    public OrdersController(IFixInitiatorService fixService)
    {
        _fixService = fixService ?? throw new ArgumentNullException(nameof(fixService));
    }

    [HttpPost("send")]
    public IActionResult SendOrder(OrderDto order)
    {
        _fixService.SendOrder(order);
        return Accepted("Ordem enviada via FIX");
    }
}
