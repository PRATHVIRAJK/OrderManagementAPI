using Application.DTOs;

namespace Tests.Common
{
    public static class TestDataFactory
    {
        public static Order CreateOrder(int id = 1) => new()
        {
            Id = id,
            ProductName = $"Product-{id}",
            Amount = id * 10m
        };

        public static Order[] CreateOrders(int count)
        {
            var arr = new Order[count];
            for (int i = 0; i < count; i++)
                arr[i] = CreateOrder(i + 1);
            return arr;
        }

        public static CreateOrderDto CreateOrderDto(int seed = 1) => new()
        {
            ProductName = $"Product-{seed}",
            Amount = seed * 10m
        };

        public static UpdateOrderDto CreateUpdateOrderDto(int seed = 1) => new()
        {
            ProductName = $"Product-{seed}",
            Amount = seed * 10m
        };

        public static OrderResponseDto CreateOrderResponseDto(int id = 1) => new()
        {
            Id = id,
            ProductName = $"Product-{id}",
            Amount = id * 10m
        };

        public static OrderResponseDto[] CreateOrderResponseDtos(int count)
        {
            var arr = new OrderResponseDto[count];
            for (int i = 0; i < count; i++)
                arr[i] = CreateOrderResponseDto(i + 1);
            return arr;
        }
    }
}
