using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Services;
using FluentAssertions;
using Moq;
using Tests.Common;
using Xunit;
using Application.Interfaces;

namespace Tests.Services
{
    public class OrderServiceTests
    {
        [Fact]
        public async Task GetAllAsync_ReturnsAllOrders()
        {
            // Arrange
            var orders = TestDataFactory.CreateOrders(2).ToList();
            var repoMock = new Mock<IOrderRepository>();
            repoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(orders);
            var svc = new OrderService(repoMock.Object);

            // Act
            var result = await svc.GetAllAsync();

            // Assert
            result.Should().BeEquivalentTo(orders);
            repoMock.Verify(r => r.GetAllAsync(), Times.Once);
        }

        [Fact]
        public async Task GetByIdAsync_OrderExists_ReturnsOrder()
        {
            var order = TestDataFactory.CreateOrder(5);
            var repoMock = new Mock<IOrderRepository>();
            repoMock.Setup(r => r.GetByIdAsync(5)).ReturnsAsync(order);
            var svc = new OrderService(repoMock.Object);

            var result = await svc.GetByIdAsync(5);

            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(order);
            repoMock.Verify(r => r.GetByIdAsync(5), Times.Once);
        }

        [Fact]
        public async Task CreateAsync_CallsRepositoryAndReturnsOrder()
        {
            var order = TestDataFactory.CreateOrder(6);
            var repoMock = new Mock<IOrderRepository>();
            repoMock.Setup(r => r.CreateAsync(It.IsAny<Order>())).Returns(Task.CompletedTask);
            var svc = new OrderService(repoMock.Object);

            var result = await svc.CreateAsync(order);

            result.Should().Be(order);
            repoMock.Verify(r => r.CreateAsync(It.Is<Order>(o => o == order)), Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_WhenOrderExists_ReturnsTrueAndUpdates()
        {
            var existing = TestDataFactory.CreateOrder(7);
            var update = new Order { Id = 7, ProductName = "Updated", Amount = 123m };
            var repoMock = new Mock<IOrderRepository>();
            repoMock.Setup(r => r.GetByIdAsync(7)).ReturnsAsync(existing);
            repoMock.Setup(r => r.UpdateAsync(It.IsAny<Order>())).Returns(Task.CompletedTask);
            var svc = new OrderService(repoMock.Object);

            var result = await svc.UpdateAsync(7, update);

            result.Should().BeTrue();
            repoMock.Verify(r => r.GetByIdAsync(7), Times.Once);
            repoMock.Verify(r => r.UpdateAsync(It.Is<Order>(o => o.Id == 7 && o.ProductName == "Updated")), Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_WhenOrderNotFound_ReturnsFalse()
        {
            var repoMock = new Mock<IOrderRepository>();
            repoMock.Setup(r => r.GetByIdAsync(8)).ReturnsAsync((Order?)null);
            var svc = new OrderService(repoMock.Object);

            var result = await svc.UpdateAsync(8, TestDataFactory.CreateOrder(8));

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
