using GrainInterfaces.Warehouses;
using Orleans;
using System;
using System.Collections.Async;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace TestClient
{
    public static class Test_Warehouses
    {
        public static async Task<Warehouse[]> GetAll(IClusterClient client)
        {
            Stopwatch watch = new Stopwatch();
            watch.Start();

            var warehouses = client.GetGrain<IWarehouses>(Guid.Empty);
            var response = await warehouses.GetAll();

            watch.Stop();
            Console.WriteLine($"Found {response.Length} warehouses - Time elapsed: {watch.Elapsed.TotalMilliseconds}");

            return response;
        }

        public static async Task<Guid> Add(IClusterClient client, string name = "")
        {
            var price = Math.Round(Convert.ToDecimal(new Random().NextDouble() * 1000), 2);

            var warehouses = client.GetGrain<IWarehouses>(Guid.Empty);
            var response = await warehouses.Add(new Warehouse
            {
                Code = $"Warehouse {name}",
                CreationDate = DateTimeOffset.Now,
                Description = $"Warehouse description {name}",
                Name = $"Warehouse name {name}",
            });
            Console.WriteLine($"Added warehouse {name} {response.Id}");
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
                    var warehouseGuid = await Add(client);
                    result.Add(warehouseGuid);
                },
                maxDegreeOfParalellism: maxDegreeOfParalellism);

            watch.Stop();
            Console.WriteLine($"Added {result.Count} warehouses - Time elapsed: {watch.Elapsed.TotalMilliseconds}");

            return result;
        }
    }
}
