﻿using GrainInterfaces;
using GrainInterfaces.Orders;
using GrainInterfaces.Products;
using GrainInterfaces.Scheduled;
using GrainInterfaces.Serialization.ProtobufNet;
using GrainInterfaces.Warehouses;
using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Configuration;
using ProtoBuf.Meta;
using System;
using System.Collections.Async;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using TestClient;

namespace OrleansSilo
{
    public class Program
    {
        static int Main(string[] args)
        {
            return RunMainAsync().Result;
        }

        private static async Task<int> RunMainAsync()
        {
            try
            {
                using (var client = await ConnectClient())
                {
                    // await RunClientTests(client);
                    await RunClientTestsSetup(client);
                    Console.ReadKey();
                }

                return 0;
            }
            catch (Exception e)
            {
                Console.WriteLine($"\nException while trying to run client: {e.Message}");
                Console.WriteLine("Make sure the silo the client is trying to connect to is running.");
                Console.WriteLine("\nPress any key to exit.");
                Console.ReadKey();
                return 1;
            }
        }

        private static async Task<IClusterClient> ConnectClient()
        {
            IClusterClient client;
            client = new ClientBuilder()
                .UseLocalhostClustering()
                .Configure<SerializationProviderOptions>(_ =>
                {
                    _.SerializationProviders.Add(typeof(Orleans.Serialization.ProtobufNet.ProtobufNetSerializer).GetTypeInfo());
                    RuntimeTypeModel.Default.Add(typeof(DateTimeOffset), false).SetSurrogate(typeof(DateTimeOffsetSurrogate));
                })
                .Configure<ClusterOptions>(options =>
                {
                    options.ClusterId = "dev";
                    options.ServiceId = "TestInventoryService";
                })
                .Configure<ProcessExitHandlingOptions>(options => options.FastKillOnProcessExit = false)
                .ConfigureLogging(logging => logging.AddConsole())
                .Build();

            await client.Connect();
            Console.WriteLine("Client successfully connected to silo host");
            return client;
        }

        private static async Task StartReminders(IClusterClient client)
        {
            // startup the global reminders
            {
                var g = client.GetGrain<IInventoryAutoSupplying>(Guid.Empty);
                await g.Start();
            }
            {
                var g = client.GetGrain<IOrderScheduledProcessing>(Guid.Empty);
                await g.Start();
            }
        }

        private static async Task RunClientTests(IClusterClient client)
        {
            await Test_Products.GetAll(client);
            await Test_Products.AddBatch(client, 100, 100);

            await Test_Orders.WaitForAllDispatched(client);
            await Test_Orders.AddBatch(client, 100, 10);
            await Test_Orders.WaitForAllDispatched(client);
        }

        private static async Task RunClientTestsSetup(IClusterClient client)
        {
            int numWarehouses = 2;
            int numProducts = 100;
            int numOrders = 100;

            var warehouses = await Test_Warehouses.GetAll(client);
            for(int n = warehouses.Length; n < numWarehouses; n++)
            {
                var name = (n + 1).ToString("000");
                await Test_Warehouses.Add(client, name);
            }

            var products = await Test_Products.GetAll(client);
            if (products.Length < numProducts)
            { 
                await Test_Products.AddBatch(client, numProducts - products.Length, 100);
            }

            products = await Test_Products.GetAll(client);

            var totMsExists = 0d;
            var totMsCreated = 0d;
            for (int n = 0; n < 10; n++)
            {
                totMsExists += await Test_Products.ExistsBatch(client, numProducts, 100, products);
                totMsCreated += await Test_Products.CreatedBatch(client, numProducts, 100, products);
            }
            Console.WriteLine($"Check for exists for products - Time elapsed: {totMsExists} ms - {(products.Length == 0 ? 0 : totMsExists / products.Length)} for product");
            Console.WriteLine($"Check for created for products - Time elapsed: {totMsCreated} ms - {(products.Length == 0 ? 0 : totMsCreated / products.Length)} for product");

            if (products.Length < numProducts)
            {
                var x = "wait for streaming";
            }

            await Test_Orders.GetOrdersStatsCache(client);

            var orders = await Test_Orders.GetAll(client);
            if (orders.Length < numOrders)
            {
                await Test_Orders.AddBatch(client, numOrders - orders.Length, 100);

                await StartReminders(client);

                await Test_Orders.WaitForAllDispatched(client);
            }

            await Test_Orders.GetOrdersStatsCache(client);
        }
    }
}
