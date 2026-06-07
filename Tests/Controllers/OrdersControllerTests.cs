using Application.DTOs;
using Application.Interfaces;
using API.Controllers;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Tests.Common;
using Xunit;

namespace Tests.Controllers
{
    public class OrdersControllerTests
    {
        [Fact]
        public async Task GetAll_WhenOrdersExist_ReturnsOkWithOrders()
        {
            var orders = TestDataFactory.CreateOrderResponseDtos(3).ToList();
            var serviceMock = new Mock<IOrderService>();
            serviceMock.Setup(s => s.GetAllAsync()).ReturnsAsync(orders);
            var loggerMock = new Mock<ILogger<OrdersController>>();
            var controller = new OrdersController(serviceMock.Object, loggerMock.Object);

            var result = await controller.GetAll();

            result.Should().BeOfType<OkObjectResult>();
            var ok = (OkObjectResult)result;
            ok.Value.Should().BeEquivalentTo(orders);
            serviceMock.Verify(s => s.GetAllAsync(), Times.Once);
        }

        [Fact]
        public async Task GetAll_WhenNoOrders_ReturnsOkWithEmptyList()
        {
            var serviceMock = new Mock<IOrderService>();
            serviceMock.Setup(s => s.GetAllAsync()).ReturnsAsync([]);
            var loggerMock = new Mock<ILogger<OrdersController>>();
            var controller = new OrdersController(serviceMock.Object, loggerMock.Object);

            var result = await controller.GetAll();

            result.Should().BeOfType<OkObjectResult>();
            var ok = (OkObjectResult)result;
            ((List<OrderResponseDto>)ok.Value!).Should().BeEmpty();
            serviceMock.Verify(s => s.GetAllAsync(), Times.Once);
        }

        [Fact]
        public async Task GetById_OrderExists_ReturnsOk()
        {
            var orderDto = TestDataFactory.CreateOrderResponseDto(1);
            var serviceMock = new Mock<IOrderService>();
            serviceMock.Setup(s => s.GetByIdAsync(1)).ReturnsAsync(orderDto);
            var loggerMock = new Mock<ILogger<OrdersController>>();
            var controller = new OrdersController(serviceMock.Object, loggerMock.Object);

            var result = await controller.GetById(1);

            result.Should().BeOfType<OkObjectResult>();
            var ok = (OkObjectResult)result;
            ok.Value.Should().BeEquivalentTo(orderDto);
            serviceMock.Verify(s => s.GetByIdAsync(1), Times.Once);
        }

        [Fact]
        public async Task GetById_OrderNotFound_ReturnsNotFound()
        {
            var serviceMock = new Mock<IOrderService>();
            serviceMock.Setup(s => s.GetByIdAsync(5)).ReturnsAsync((OrderResponseDto?)null);
            var loggerMock = new Mock<ILogger<OrdersController>>();
            var controller = new OrdersController(serviceMock.Object, loggerMock.Object);

            var result = await controller.GetById(5);

            result.Should().BeOfType<NotFoundResult>();
            serviceMock.Verify(s => s.GetByIdAsync(5), Times.Once);
        }

        [Fact]
        public async Task Create_ValidDto_ReturnsCreatedAtAction()
        {
            var dto = TestDataFactory.CreateOrderDto(10);
            var created = TestDataFactory.CreateOrderResponseDto(10);
            var serviceMock = new Mock<IOrderService>();
            serviceMock.Setup(s => s.CreateAsync(It.IsAny<CreateOrderDto>())).ReturnsAsync(created);
            var loggerMock = new Mock<ILogger<OrdersController>>();
            var controller = new OrdersController(serviceMock.Object, loggerMock.Object);

            var result = await controller.Create(dto);

            result.Should().BeOfType<CreatedAtActionResult>();
            var createdResult = (CreatedAtActionResult)result;
            createdResult.Value.Should().BeEquivalentTo(created);
            serviceMock.Verify(
                s => s.CreateAsync(It.Is<CreateOrderDto>(d => d.ProductName == dto.ProductName && d.Amount == dto.Amount)),
                Times.Once);
        }

        [Fact]
        public async Task Update_WhenUpdateSucceeds_ReturnsNoContent()
        {
            var dto = TestDataFactory.CreateUpdateOrderDto(2);
            var serviceMock = new Mock<IOrderService>();
            serviceMock.Setup(s => s.UpdateAsync(2, It.IsAny<UpdateOrderDto>())).ReturnsAsync(true);
            var loggerMock = new Mock<ILogger<OrdersController>>();
            var controller = new OrdersController(serviceMock.Object, loggerMock.Object);

            var result = await controller.Update(2, dto);

            result.Should().BeOfType<NoContentResult>();
            serviceMock.Verify(s => s.UpdateAsync(2, It.IsAny<UpdateOrderDto>()), Times.Once);
        }

        [Fact]
        public async Task Update_WhenOrderNotFound_ReturnsNotFound()
        {
            var dto = TestDataFactory.CreateUpdateOrderDto(3);
            var serviceMock = new Mock<IOrderService>();
            serviceMock.Setup(s => s.UpdateAsync(3, It.IsAny<UpdateOrderDto>())).ReturnsAsync(false);
            var loggerMock = new Mock<ILogger<OrdersController>>();
            var controller = new OrdersController(serviceMock.Object, loggerMock.Object);

            var result = await controller.Update(3, dto);

            result.Should().BeOfType<NotFoundResult>();
            serviceMock.Verify(s => s.UpdateAsync(3, It.IsAny<UpdateOrderDto>()), Times.Once);
        }

        [Fact]
        public async Task Delete_WhenDeleteSucceeds_ReturnsNoContent()
        {
            var serviceMock = new Mock<IOrderService>();
            serviceMock.Setup(s => s.DeleteAsync(4)).ReturnsAsync(true);
            var loggerMock = new Mock<ILogger<OrdersController>>();
            var controller = new OrdersController(serviceMock.Object, loggerMock.Object);

            var result = await controller.Delete(4);

            result.Should().BeOfType<NoContentResult>();
            serviceMock.Verify(s => s.DeleteAsync(4), Times.Once);
        }

        [Fact]
        public async Task Delete_WhenOrderNotFound_ReturnsNotFound()
        {
            var serviceMock = new Mock<IOrderService>();
            serviceMock.Setup(s => s.DeleteAsync(99)).ReturnsAsync(false);
            var loggerMock = new Mock<ILogger<OrdersController>>();
            var controller = new OrdersController(serviceMock.Object, loggerMock.Object);

            var result = await controller.Delete(99);

            result.Should().BeOfType<NotFoundResult>();
            serviceMock.Verify(s => s.DeleteAsync(99), Times.Once);
        }
    }
}
