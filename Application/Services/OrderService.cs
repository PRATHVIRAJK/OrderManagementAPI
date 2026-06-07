using Application.DTOs;
using Application.Interfaces;

namespace Application.Services
{
    public class OrderService(IOrderRepository orderRepository) : IOrderService
    {
        public async Task<IEnumerable<OrderResponseDto>> GetAllAsync()
        {
            var orders = await orderRepository.GetAllAsync();
            return orders.Select(ToDto);
        }

        public async Task<OrderResponseDto?> GetByIdAsync(int id)
        {
            var order = await orderRepository.GetByIdAsync(id);
            return order is null ? null : ToDto(order);
        }

        public async Task<OrderResponseDto> CreateAsync(CreateOrderDto dto)
        {
            var order = new Order
            {
                ProductName = dto.ProductName,
                Amount = dto.Amount
            };
            await orderRepository.CreateAsync(order);
            return ToDto(order);
        }

        public async Task<bool> UpdateAsync(int id, UpdateOrderDto dto)
        {
            var existing = await orderRepository.GetByIdAsync(id);
            if (existing is null)
                return false;

            existing.ProductName = dto.ProductName;
            existing.Amount = dto.Amount;

            await orderRepository.UpdateAsync(existing);
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var existing = await orderRepository.GetByIdAsync(id);
            if (existing is null)
                return false;

            await orderRepository.DeleteAsync(id);
            return true;
        }

        private static OrderResponseDto ToDto(Order order) => new()
        {
            Id = order.Id,
            ProductName = order.ProductName,
            Amount = order.Amount
        };
    }
}
