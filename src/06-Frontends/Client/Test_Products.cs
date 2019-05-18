using GrainInterfaces.Products;
using Orleans;
using System;
using System.Collections.Async;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;


namespace TestClient
{
    public static class Test_Products
    {
        public static async Task<Product[]> GetAll(IClusterClient client)
        {
            Stopwatch watch = new Stopwatch();
            watch.Start();

            var products = client.GetGrain<IProducts>(Guid.Empty);
            var response = await products.GetAll();

            watch.Stop();
            Console.WriteLine($"Found {response.Length} products - Time elapsed: {watch.Elapsed.TotalMilliseconds}");

            return response;
        }

        public static async Task<Guid> Add(IClusterClient client, string name)
        {
            var price = Math.Round(Convert.ToDecimal(new Random().NextDouble() * 1000), 2);

            var products = client.GetGrain<IProducts>(Guid.Empty);
            var response = await products.Add(new Product
            {
                Code = $"Product {name}",
                CreationDate = DateTimeOffset.Now,
                Description = $"Product description {name}",
                Name = $"Product name {name}",
                Price = price
            });
            Console.WriteLine($"Added product {name} {response.Id}");
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
                    var productGuid = await Add(client, name);
                    result.Add(productGuid);
                },
                maxDegreeOfParalellism: maxDegreeOfParalellism);

            watch.Stop();
            Console.WriteLine($"Added {result.Count} products - Time elapsed: {watch.Elapsed.TotalMilliseconds}");

            return result;
        }
    }
}
