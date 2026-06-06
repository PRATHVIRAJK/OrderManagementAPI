using Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;

        public OrderService(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }
        public async Task<IEnumerable<Order>> GetAllAsync()
        {
            return await _orderRepository.GetAllAsync();
        }

        public async Task<Order?> GetByIdAsync(int id)
        {
            return await _orderRepository.GetByIdAsync(id);
        }

        public async Task<Order> CreateAsync(Order order)
        {
            await _orderRepository.CreateAsync(order);
            return order;
        }

        public async Task<bool> UpdateAsync(int id, Order order)
        {
            var existingOrder = await _orderRepository.GetByIdAsync(id);

            if (existingOrder == null)
                return false;

            existingOrder.ProductName = order.ProductName;
            existingOrder.Amount = order.Amount;

            await _orderRepository.UpdateAsync(existingOrder);

            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var existingOrder = await _orderRepository.GetByIdAsync(id);

            if (existingOrder == null)
                return false;

            await _orderRepository.DeleteAsync(id);

            return true;
        }
    }
}
