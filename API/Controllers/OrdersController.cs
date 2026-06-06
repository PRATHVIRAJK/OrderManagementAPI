using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Application.Interfaces;
namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrdersController  : ControllerBase
    {
        private readonly IOrderService _service;

        private readonly ILogger<OrdersController > _logger;
        public OrdersController (IOrderService service, ILogger<OrdersController > logger)
        {
            _service = service;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var orders = await _service.GetAllAsync();
            if(orders==null)
            {
                _logger.LogWarning("Orders not found");
                return NotFound();
            }
            _logger.LogInformation("Orders found {@orders}", orders);
            return Ok(orders);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var order = await _service.GetByIdAsync(id);
            if (order == null)
            {
                _logger.LogWarning("Orders not found {id}", id);
                return NotFound();
            }
            _logger.LogInformation("Order is found {@order}", order);
            return Ok(order);
        }
        [HttpPost]
        public async Task<IActionResult> Create(Order order)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning($"Invalid order model {ModelState.Count}{order}");
            }

            var createdOrder = await _service.CreateAsync(order);

            _logger.LogInformation( "Order {OrderId} created for {Amount}",order.Id,order.Amount);

            return CreatedAtAction(
                nameof(GetById),
                new { id = createdOrder.Id },
                createdOrder);
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, Order order)
        {
            if(!ModelState.IsValid)
            {
                _logger.LogWarning($"Invalid order model {ModelState.Count}{order}");
            }
            var result = await _service.UpdateAsync(id, order);

            if (!result)
                return BadRequest();

            _logger.LogInformation("Order is updated succesfully {@order}", order);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _service.DeleteAsync(id);

            if (!result)
            {
                _logger.LogWarning("Orders not found for {id}", id);
                return NotFound();
            }
            _logger.LogInformation("Order is deleted for {id}", id);
            return NoContent();
        }
    }
}
