using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Application.Interfaces;
namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _service;

        private readonly ILogger<OrdersController> _logger;
        public OrdersController(IOrderService service, ILogger<OrdersController> logger)
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
            _logger.LogInformation("Orders found. Count: {Count}", orders?.Count() ?? 0);
            return Ok(orders);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var order = await _service.GetByIdAsync(id);
            if (order == null)
            {
                _logger.LogWarning("Order not found {OrderId}", id);
                return NotFound();
            }
            _logger.LogInformation("Order found {OrderId}", order.Id);
            return Ok(order);
        }
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Order order)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid order model. ModelState entries: {Count}", ModelState.Count);
                return BadRequest(ModelState);
            }

            var createdOrder = await _service.CreateAsync(order);

            _logger.LogInformation("Order {OrderId} created for {Amount}", createdOrder.Id, createdOrder.Amount);

            return CreatedAtAction(
                nameof(GetById),
                new { id = createdOrder.Id },
                createdOrder);
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] Order order)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid order model. ModelState entries: {Count}", ModelState.Count);
                return BadRequest(ModelState);
            }

            if (order == null)
            {
                _logger.LogWarning("Update called with null order for {Id}", id);
                return BadRequest();
            }

            if (id != order.Id)
            {
                _logger.LogWarning("Route id {RouteId} does not match body id {BodyId}", id, order.Id);
                return BadRequest("Id in route does not match id in body.");
            }

            var result = await _service.UpdateAsync(id, order);

            if (!result)
                return BadRequest();

            _logger.LogInformation("Order {OrderId} updated successfully", order.Id);
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
