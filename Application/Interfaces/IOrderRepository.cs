using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Interfaces
{
    public interface IOrderRepository
    {
        Task<IEnumerable<Order>> GetAllAsync();
        Task<Order?> GetByIdAsync(int id);
        Task CreateAsync(Order order);
        Task UpdateAsync(Order order);
        Task DeleteAsync(int id);
    }
}
