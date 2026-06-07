using Microsoft.AspNetCore.Mvc;
using Application.Interfaces;
using Application.DTOs;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrdersController(IOrderService service, ILogger<OrdersController> logger) : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var orders = await service.GetAllAsync();
            var orderList = orders.ToList();
            if (logger.IsEnabled(LogLevel.Information))
                logger.LogInformation("Orders found. Count: {Count}", orderList.Count);
            return Ok(orderList);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var order = await service.GetByIdAsync(id);
            if (order is null)
            {
                logger.LogWarning("Order not found {OrderId}", id);
                return NotFound();
            }
            if (logger.IsEnabled(LogLevel.Information))
                logger.LogInformation("Order found {OrderId}", order.Id);
            return Ok(order);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateOrderDto dto)
        {
            if (!ModelState.IsValid)
            {
                logger.LogWarning("Invalid order model. ModelState entries: {Count}", ModelState.Count);
                return BadRequest(ModelState);
            }

            var created = await service.CreateAsync(dto);

            if (logger.IsEnabled(LogLevel.Information))
                logger.LogInformation("Order {OrderId} created for {Amount}", created.Id, created.Amount);

            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateOrderDto dto)
        {
            if (!ModelState.IsValid)
            {
                logger.LogWarning("Invalid order model. ModelState entries: {Count}", ModelState.Count);
                return BadRequest(ModelState);
            }

            var result = await service.UpdateAsync(id, dto);

            if (!result)
                return NotFound();

            if (logger.IsEnabled(LogLevel.Information))
                logger.LogInformation("Order {OrderId} updated successfully", id);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await service.DeleteAsync(id);

            if (!result)
            {
                logger.LogWarning("Order not found for {OrderId}", id);
                return NotFound();
            }

            if (logger.IsEnabled(LogLevel.Information))
                logger.LogInformation("Order deleted for {OrderId}", id);
            return NoContent();
        }
    }
}
