using System;

namespace Tests.Common
{
    public static class TestDataFactory
    {
        public static Order CreateOrder(int id = 1)
        {
            return new Order
            {
                Id = id,
                ProductName = $"Product-{id}",
                Amount = id * 10m
            };
        }

        public static Order[] CreateOrders(int count)
        {
            var arr = new Order[count];
            for (int i = 0; i < count; i++)
            {
                arr[i] = CreateOrder(i + 1);
            }
            return arr;
        }
    }
}
