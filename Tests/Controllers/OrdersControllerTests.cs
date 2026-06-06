using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Interfaces;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Tests.Common;
using Xunit;
using API.Controllers;

namespace Tests.Controllers
{
    public class OrdersControllerTests
    {
        [Fact]
        public async Task GetAll_WhenOrdersExist_ReturnsOkWithOrders()
        {
            // Arrange
            var orders = TestDataFactory.CreateOrders(3).ToList();
            var serviceMock = new Mock<IOrderService>();
            serviceMock.Setup(s => s.GetAllAsync()).ReturnsAsync(orders);
            var loggerMock = new Mock<ILogger<OrdersController>>();

            var controller = new OrdersController(serviceMock.Object, loggerMock.Object);

            // Act
            var result = await controller.GetAll();

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            var ok = result as OkObjectResult;
            ok!.Value.Should().BeEquivalentTo(orders);
            serviceMock.Verify(s => s.GetAllAsync(), Times.Once);
        }

        [Fact]
        public async Task GetAll_WhenNoOrders_ReturnsNotFound()
        {
            // Arrange
            var serviceMock = new Mock<IOrderService>();
            serviceMock.Setup(s => s.GetAllAsync()).ReturnsAsync((IEnumerable<Order>?)null);
            var loggerMock = new Mock<ILogger<OrdersController>>();
            var controller = new OrdersController(serviceMock.Object, loggerMock.Object);

            // Act
            var result = await controller.GetAll();

            // Assert
            result.Should().BeOfType<NotFoundResult>();
            // Verify a warning log containing the expected message was written
            loggerMock.Invocations.Should().Contain(inv =>
                inv.Method.Name == "Log" &&
                inv.Arguments.Count > 2 &&
                inv.Arguments[0] != null && inv.Arguments[0].GetType() == typeof(Microsoft.Extensions.Logging.LogLevel) &&
                (Microsoft.Extensions.Logging.LogLevel)inv.Arguments[0] == Microsoft.Extensions.Logging.LogLevel.Warning &&
                inv.Arguments[2] != null && inv.Arguments[2].ToString().Contains("Orders not found")
            );
        }

        [Fact]
        public async Task GetById_OrderExists_ReturnsOk()
        {
            // Arrange
            var order = TestDataFactory.CreateOrder(1);
            var serviceMock = new Mock<IOrderService>();
            serviceMock.Setup(s => s.GetByIdAsync(1)).ReturnsAsync(order);
            var loggerMock = new Mock<ILogger<OrdersController>>();
            var controller = new OrdersController(serviceMock.Object, loggerMock.Object);

            // Act
            var result = await controller.GetById(1);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            var ok = result as OkObjectResult;
            ok!.Value.Should().BeEquivalentTo(order);
            serviceMock.Verify(s => s.GetByIdAsync(1), Times.Once);
        }

        [Fact]
        public async Task GetById_OrderNotFound_ReturnsNotFound()
        {
            // Arrange
            var serviceMock = new Mock<IOrderService>();
            serviceMock.Setup(s => s.GetByIdAsync(5)).ReturnsAsync((Order?)null);
            var loggerMock = new Mock<ILogger<OrdersController>>();
            var controller = new OrdersController(serviceMock.Object, loggerMock.Object);

            // Act
            var result = await controller.GetById(5);

            // Assert
            result.Should().BeOfType<NotFoundResult>();
            // Verify a warning log containing the expected message was written
            loggerMock.Invocations.Should().Contain(inv =>
                inv.Method.Name == "Log" &&
                inv.Arguments.Count > 2 &&
                inv.Arguments[0] != null && inv.Arguments[0].GetType() == typeof(Microsoft.Extensions.Logging.LogLevel) &&
                (Microsoft.Extensions.Logging.LogLevel)inv.Arguments[0] == Microsoft.Extensions.Logging.LogLevel.Warning &&
                inv.Arguments[2] != null && inv.Arguments[2].ToString().Contains("Order not found")
            );
            serviceMock.Verify(s => s.GetByIdAsync(5), Times.Once);
        }

        [Fact]
        public async Task Create_ValidOrder_ReturnsCreatedAtAction()
        {
            // Arrange
            var order = TestDataFactory.CreateOrder(10);
            var created = TestDataFactory.CreateOrder(10);
            var serviceMock = new Mock<IOrderService>();
            serviceMock.Setup(s => s.CreateAsync(It.IsAny<Order>())).ReturnsAsync(created);
            var loggerMock = new Mock<ILogger<OrdersController>>();
            var controller = new OrdersController(serviceMock.Object, loggerMock.Object);

            // Act
            var result = await controller.Create(order);

            // Assert
            result.Should().BeOfType<CreatedAtActionResult>();
            var createdResult = result as CreatedAtActionResult;
            createdResult!.Value.Should().BeEquivalentTo(created);
            serviceMock.Verify(s => s.CreateAsync(It.Is<Order>(o => o == order)), Times.Once);
        }

        [Fact]
        public async Task Update_WhenUpdateSucceeds_ReturnsNoContent()
        {
            // Arrange
            var order = TestDataFactory.CreateOrder(2);
            var serviceMock = new Mock<IOrderService>();
            serviceMock.Setup(s => s.UpdateAsync(2, It.IsAny<Order>())).ReturnsAsync(true);
            var loggerMock = new Mock<ILogger<OrdersController>>();
            var controller = new OrdersController(serviceMock.Object, loggerMock.Object);

            // Act
            var result = await controller.Update(2, order);

            // Assert
            result.Should().BeOfType<NoContentResult>();
            serviceMock.Verify(s => s.UpdateAsync(2, It.Is<Order>(o => o == order)), Times.Once);
        }

        [Fact]
        public async Task Update_WhenUpdateFails_ReturnsBadRequest()
        {
            // Arrange
            var order = TestDataFactory.CreateOrder(3);
            var serviceMock = new Mock<IOrderService>();
            serviceMock.Setup(s => s.UpdateAsync(3, It.IsAny<Order>())).ReturnsAsync(false);
            var loggerMock = new Mock<ILogger<OrdersController>>();
            var controller = new OrdersController(serviceMock.Object, loggerMock.Object);

            // Act
            var result = await controller.Update(3, order);

            // Assert
            result.Should().Match(r => r is BadRequestObjectResult || r is BadRequestResult);
            serviceMock.Verify(s => s.UpdateAsync(3, It.IsAny<Order>()), Times.Once);
        }

        [Fact]
        public async Task Delete_WhenDeleteSucceeds_ReturnsNoContent()
        {
            // Arrange
            var serviceMock = new Mock<IOrderService>();
            serviceMock.Setup(s => s.DeleteAsync(4)).ReturnsAsync(true);
            var loggerMock = new Mock<ILogger<OrdersController>>();
            var controller = new OrdersController(serviceMock.Object, loggerMock.Object);

            // Act
            var result = await controller.Delete(4);

            // Assert
            result.Should().BeOfType<NoContentResult>();
            serviceMock.Verify(s => s.DeleteAsync(4), Times.Once);
        }

        [Fact]
        public async Task Delete_WhenDeleteFails_ReturnsNotFound()
        {
            // Arrange
            var serviceMock = new Mock<IOrderService>();
            serviceMock.Setup(s => s.DeleteAsync(99)).ReturnsAsync(false);
            var loggerMock = new Mock<ILogger<OrdersController>>();
            var controller = new OrdersController(serviceMock.Object, loggerMock.Object);

            // Act
            var result = await controller.Delete(99);

            // Assert
            result.Should().BeOfType<NotFoundResult>();
            serviceMock.Verify(s => s.DeleteAsync(99), Times.Once);
        }
    }
}
