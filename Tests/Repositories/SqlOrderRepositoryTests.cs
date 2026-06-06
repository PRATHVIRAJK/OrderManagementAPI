using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Infrastructure.Persistence;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Tests.Common;
using Xunit;

namespace Tests.Repositories
{
    public class SqlOrderRepositoryTests : BaseTestFixture
    {
        [Fact]
        public async Task GetAllAsync_ReturnsSeededOrders()
        {
            var options = CreateInMemoryOptions("GetAll_DB");
            using var context = new AppDbContext(options);
            context.Orders.AddRange(TestDataFactory.CreateOrders(3));
            await context.SaveChangesAsync();

            var repo = new SqlOrderRepository(context);

            var result = await repo.GetAllAsync();

            result.Should().HaveCount(3);
        }

        [Fact]
        public async Task GetByIdAsync_OrderExists_ReturnsOrder()
        {
            var options = CreateInMemoryOptions("GetById_DB");
            using var context = new AppDbContext(options);
            var seed = TestDataFactory.CreateOrder(42);
            await context.Orders.AddAsync(seed);
            await context.SaveChangesAsync();

            var repo = new SqlOrderRepository(context);

            var result = await repo.GetByIdAsync(42);

            result.Should().NotBeNull();
            result!.Id.Should().Be(42);
        }

        [Fact]
        public async Task CreateAsync_PersistsOrder()
        {
            var options = CreateInMemoryOptions("Create_DB");
            using (var context = new AppDbContext(options))
            {
                var repo = new SqlOrderRepository(context);
                var order = TestDataFactory.CreateOrder(7);

                await repo.CreateAsync(order);
            }

            using (var verifyContext = new AppDbContext(options))
            {
                var saved = await verifyContext.Orders.FindAsync(7);
                saved.Should().NotBeNull();
                saved!.ProductName.Should().Be("Product-7");
            }
        }

        [Fact]
        public async Task UpdateAsync_ModifiesAndPersists()
        {
            var options = CreateInMemoryOptions("Update_DB");
            using (var context = new AppDbContext(options))
            {
                context.Orders.Add(TestDataFactory.CreateOrder(8));
                await context.SaveChangesAsync();
            }

            using (var context = new AppDbContext(options))
            {
                var repo = new SqlOrderRepository(context);
                var toUpdate = await context.Orders.FindAsync(8);
                toUpdate!.ProductName = "UpdatedName";

                await repo.UpdateAsync(toUpdate);
            }

            using (var verify = new AppDbContext(options))
            {
                var saved = await verify.Orders.FindAsync(8);
                saved!.ProductName.Should().Be("UpdatedName");
            }
        }

        [Fact]
        public async Task DeleteAsync_RemovesOrder()
        {
            var options = CreateInMemoryOptions("Delete_DB");
            using (var context = new AppDbContext(options))
            {
                context.Orders.Add(TestDataFactory.CreateOrder(9));
                await context.SaveChangesAsync();
            }

            using (var context = new AppDbContext(options))
            {
                var repo = new SqlOrderRepository(context);
                await repo.DeleteAsync(9);
            }

            using (var verify = new AppDbContext(options))
            {
                var saved = await verify.Orders.FindAsync(9);
                saved.Should().BeNull();
            }
        }

        [Fact]
        public async Task GetByIdAsync_NonExistingId_ReturnsNull()
        {
            var options = CreateInMemoryOptions("NonExisting_DB");
            using var context = new AppDbContext(options);
            var repo = new SqlOrderRepository(context);

            var result = await repo.GetByIdAsync(9999);

            result.Should().BeNull();
        }
    }
}
