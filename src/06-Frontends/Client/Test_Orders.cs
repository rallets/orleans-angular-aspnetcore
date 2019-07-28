using GrainInterfaces.Orders;
using Orleans;
using System;
using System.Collections.Async;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace TestClient
{
    public static class Test_Orders
    {
        public static async Task<OrdersStats> GetOrdersStatsCache(IClusterClient client)
        {
            Stopwatch watch = new Stopwatch();
            watch.Start();

            var g = client.GetGrain<IOrdersStatsCache>(Guid.Empty);
            var response = await g.GetAsync();

            watch.Stop();
            Console.WriteLine($"Found {response.Orders} orders, {response.OrdersNotDispatched} not dispatched - Time elapsed: {watch.Elapsed.TotalMilliseconds} ms");

            return response;
        }

        public static async Task<OrderState[]> GetAll(IClusterClient client)
        {
            Stopwatch watch = new Stopwatch();
            watch.Start();

            var g = client.GetGrain<IOrders>(Guid.Empty);
            var response = await g.GetAll();

            watch.Stop();
            Console.WriteLine($"Found {response.Length} orders - Time elapsed: {watch.Elapsed.TotalMilliseconds} ms");

            return response;
        }

        public static async Task<Guid[]> GetAllNotDispatched(IClusterClient client)
        {
            Stopwatch watch = new Stopwatch();
            watch.Start();

            var g = client.GetGrain<IOrders>(Guid.Empty);
            var response = await g.GetNotDispatched();

            watch.Stop();
            Console.WriteLine($"Found {response.Length} orders not dispatched - Time elapsed: {watch.Elapsed.TotalMilliseconds} ms");

            return response;
        }

        public static async Task WaitForAllDispatched(IClusterClient client)
        {
            Stopwatch watch = new Stopwatch();
            watch.Start();

            Guid[] orders = null;
            do
            {
                if (orders != null)
                {
                    Console.WriteLine($"Not all orders are dispatched - waiting 10000 ms");
                    await Task.Delay(10000);
                }
                orders = await GetAllNotDispatched(client);
            } while (orders.Length != 0);

            watch.Stop();
            Console.WriteLine($"All orders are dispatched - Time elapsed: {watch.Elapsed.TotalMilliseconds} ms");
        }

        public static async Task<Guid> Add(IClusterClient client, string name)
        {
            var qty = Math.Round(Convert.ToDecimal(new Random().NextDouble() * 100), 0);
            if (qty < 1)
            {
                qty = 1;
            }

            var products = await Test_Products.GetAll(client);
            var product = products.OrderBy(x => Guid.NewGuid()).First();

            var g = client.GetGrain<IOrders>(Guid.Empty);
            var response = await g.Add(new OrderCreateRequest
            {
                Date = DateTimeOffset.Now,
                Items = new List<OrderItemCreateRequest>
                {
                    new OrderItemCreateRequest
                    {
                        ProductId = product.Id,
                        Quantity = qty
                    }
                },
                Name = $"Test order {name}",
            });
            Console.WriteLine($"Added order {name} {response.Id}");
            return response.Id;
        }

        public static async Task<IReadOnlyCollection<Guid>> AddBatch(IClusterClient client, int batchSize, int maxDegreeOfParalellism)
        {
            List<int> batch = new List<int>();
            for (int i = 1; i <= batchSize; i++)
            {
                batch.Add(i);
            }
            var result = new ConcurrentBag<Guid>();

            Stopwatch watch = new Stopwatch();
            watch.Start();

            await batch.ParallelForEachAsync(
                async item =>
                {
                    var name = $"{item.ToString("000")}/{batchSize}";
                    var orderGuid = await Add(client, name);
                    result.Add(orderGuid);
                },
                maxDegreeOfParalellism: maxDegreeOfParalellism);

            watch.Stop();
            Console.WriteLine($"Added {result.Count} orders - Time elapsed: {watch.Elapsed.TotalMilliseconds}ms - {(result.Count == 0 ? 0 : watch.Elapsed.TotalMilliseconds / result.Count)} for order");

            return result;
        }
    }
}
