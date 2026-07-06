using Microsoft.AspNetCore.Mvc;
using SALT.Service;

namespace SALT.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrdersController : ControllerBase
{
    private readonly OrderService _orderService;

    public OrdersController(OrderService orderService)
    {
        _orderService = orderService;
    }

    // 1. Ruta za dobijanje svih sastojaka: GET /api/orders/ingredients
    [HttpGet("ingredients")]
    public async Task<IActionResult> GetIngredients()
    {
        var result = await _orderService.GetIngredientsAsync();
        return Ok(result);
    }

    // 2. Ruta za dobijanje originalnih kolača: GET /api/orders/original-cakes
    [HttpGet("original-cakes")]
    public async Task<IActionResult> GetOriginalCakes()
    {
        var result = await _orderService.GetOriginalCakesAsync();
        return Ok(result);
    }

    // 3. Ruta za kreiranje nove porudžbine: POST /api/orders/create
    [HttpPost("create")]
    public async Task<IActionResult> CreateOrder([FromQuery] int customerId, [FromBody] List<OrderCakeDto> items)
    {
        try
        {
            var success = await _orderService.CreateOrderAsync(customerId, items);
            if (success)
            {
                return Ok(new { message = "Porudžbina je uspešno sačuvana u bazi!" });
            }
            return BadRequest(new { message = "Došlo je do greške prilikom čuvanja porudžbine." });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Unutrašnja greška servera.", detail = ex.Message });
        }
    }

    [HttpPut("cancel/{orderId}")]
    public async Task<IActionResult> CancelOrder(int orderId)
    {
        try
        {
            var success = await _orderService.CancelOrderAsync(orderId);
            if (success)
            {
                return Ok(new { message = $"Porudžbina {orderId} je uspešno otkazana." });
            }
            return BadRequest(new { message = "Došlo je do greške pri otkazivanju porudžbine." });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Greška na serveru.", detail = ex.Message });
        }
    }
}