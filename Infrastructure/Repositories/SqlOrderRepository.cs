using Application.Interfaces;
using Infrastructure.Persistence;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class SqlOrderRepository : IOrderRepository
    {
        private readonly AppDbContext _context;

        public SqlOrderRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Order>> GetAllAsync()
        {
            return await _context.Orders.ToListAsync();
        }

        public async Task<Order?> GetByIdAsync(int id)
        {
            return await _context.Orders
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task CreateAsync(Order order)
        {
            await _context.Orders.AddAsync(order);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Order order)
        {
            _context.Orders.Update(order);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var order = await _context.Orders.FindAsync(id);

            if (order != null)
            {
                _context.Orders.Remove(order);
                await _context.SaveChangesAsync();
            }
        }
    }
}
