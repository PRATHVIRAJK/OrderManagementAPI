using Application.DTOs;
using Application.Interfaces;
using Application.Services;
using FluentAssertions;
using Moq;
using Tests.Common;
using Xunit;

namespace Tests.Services
{
    public class OrderServiceTests
    {
        [Fact]
        public async Task GetAllAsync_ReturnsAllOrders()
        {
            var orders = TestDataFactory.CreateOrders(2).ToList();
            var repoMock = new Mock<IOrderRepository>();
            repoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(orders);
            var svc = new OrderService(repoMock.Object);

            var result = (await svc.GetAllAsync()).ToList();

            result.Should().HaveCount(2);
            result.Should().BeEquivalentTo(orders, opts => opts.ExcludingMissingMembers());
            repoMock.Verify(r => r.GetAllAsync(), Times.Once);
        }

        [Fact]
        public async Task GetByIdAsync_OrderExists_ReturnsDto()
        {
            var order = TestDataFactory.CreateOrder(5);
            var repoMock = new Mock<IOrderRepository>();
            repoMock.Setup(r => r.GetByIdAsync(5)).ReturnsAsync(order);
            var svc = new OrderService(repoMock.Object);

            var result = await svc.GetByIdAsync(5);

            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(order, opts => opts.ExcludingMissingMembers());
            repoMock.Verify(r => r.GetByIdAsync(5), Times.Once);
        }

        [Fact]
        public async Task GetByIdAsync_OrderNotFound_ReturnsNull()
        {
            var repoMock = new Mock<IOrderRepository>();
            repoMock.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((Order?)null);
            var svc = new OrderService(repoMock.Object);

            var result = await svc.GetByIdAsync(99);

            result.Should().BeNull();
        }

        [Fact]
        public async Task CreateAsync_CallsRepositoryAndReturnsDto()
        {
            var dto = TestDataFactory.CreateOrderDto(6);
            var repoMock = new Mock<IOrderRepository>();
            repoMock.Setup(r => r.CreateAsync(It.IsAny<Order>())).Returns(Task.CompletedTask);
            var svc = new OrderService(repoMock.Object);

            var result = await svc.CreateAsync(dto);

            result.Should().NotBeNull();
            result.ProductName.Should().Be(dto.ProductName);
            result.Amount.Should().Be(dto.Amount);
            repoMock.Verify(
                r => r.CreateAsync(It.Is<Order>(o => o.ProductName == dto.ProductName && o.Amount == dto.Amount)),
                Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_WhenOrderExists_ReturnsTrueAndUpdates()
        {
            var existing = TestDataFactory.CreateOrder(7);
            var updateDto = new UpdateOrderDto { ProductName = "Updated", Amount = 123m };
            var repoMock = new Mock<IOrderRepository>();
            repoMock.Setup(r => r.GetByIdAsync(7)).ReturnsAsync(existing);
            repoMock.Setup(r => r.UpdateAsync(It.IsAny<Order>())).Returns(Task.CompletedTask);
            var svc = new OrderService(repoMock.Object);

            var result = await svc.UpdateAsync(7, updateDto);

            result.Should().BeTrue();
            repoMock.Verify(r => r.GetByIdAsync(7), Times.Once);
            repoMock.Verify(
                r => r.UpdateAsync(It.Is<Order>(o => o.Id == 7 && o.ProductName == "Updated" && o.Amount == 123m)),
                Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_WhenOrderNotFound_ReturnsFalse()
        {
            var repoMock = new Mock<IOrderRepository>();
            repoMock.Setup(r => r.GetByIdAsync(8)).ReturnsAsync((Order?)null);
            var svc = new OrderService(repoMock.Object);

            var result = await svc.UpdateAsync(8, new UpdateOrderDto { ProductName = "X", Amount = 1m });

            result.Should().BeFalse();
            repoMock.Verify(r => r.GetByIdAsync(8), Times.Once);
            repoMock.Verify(r => r.UpdateAsync(It.IsAny<Order>()), Times.Never);
        }

        [Fact]
        public async Task DeleteAsync_WhenOrderExists_ReturnsTrueAndDeletes()
        {
            var existing = TestDataFactory.CreateOrder(9);
            var repoMock = new Mock<IOrderRepository>();
            repoMock.Setup(r => r.GetByIdAsync(9)).ReturnsAsync(existing);
            repoMock.Setup(r => r.DeleteAsync(9)).Returns(Task.CompletedTask);
            var svc = new OrderService(repoMock.Object);

            var result = await svc.DeleteAsync(9);

            result.Should().BeTrue();
            repoMock.Verify(r => r.GetByIdAsync(9), Times.Once);
            repoMock.Verify(r => r.DeleteAsync(9), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_WhenOrderNotFound_ReturnsFalse()
        {
            var repoMock = new Mock<IOrderRepository>();
            repoMock.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((Order?)null);
            var svc = new OrderService(repoMock.Object);

            var result = await svc.DeleteAsync(99);

            result.Should().BeFalse();
            repoMock.Verify(r => r.GetByIdAsync(99), Times.Once);
            repoMock.Verify(r => r.DeleteAsync(It.IsAny<int>()), Times.Never);
        }
    }
}
