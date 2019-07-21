using GrainInterfaces.Serialization.ProtobufNet;
using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Configuration;
using Orleans.Hosting;
using Orleans.Providers.Streams.AzureQueue;
using OrleansSilo.Products;
using ProtoBuf.Meta;
using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

// TODO: use user-secrets - from https://www.twilio.com/blog/2018/05/user-secrets-in-a-net-core-console-app.html
namespace OrleansSilo
{
    public class Program
    {
        static readonly ManualResetEvent _siloStopped = new ManualResetEvent(false);

        static ISiloHost silo;
        static bool siloStopping = false;
        static readonly object syncLock = new object();

        public static int Main(string[] args)
        {
            return RunMainAsync().Result;
        }

        private static async Task<int> RunMainAsync()
        {
            try
            {
                SetupApplicationShutdown();

                silo = CreateSilo();
                await silo.StartAsync();

                Console.WriteLine("\n\n Press Enter to terminate...\n\n");
                Console.ReadLine();

                // Wait for the silo to completely shutdown before exiting. 
                _siloStopped.WaitOne();

                return 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return 1;
            }
        }

        static void SetupApplicationShutdown()
        {
            /// Capture the user pressing Ctrl+C
            Console.CancelKeyPress += (s, a) => {
                /// Prevent the application from crashing ungracefully.
                a.Cancel = true;
                /// Don't allow the following code to repeat if the user presses Ctrl+C repeatedly.
                lock (syncLock)
                {
                    if (!siloStopping)
                    {
                        siloStopping = true;
                        Task.Run(StopSilo).Ignore();
                    }
                }
                /// Event handler execution exits immediately, leaving the silo shutdown running on a background thread,
                /// but the app doesn't crash because a.Cancel has been set = true
            };
        }

        private static ISiloHost CreateSilo()
        {
            // define the cluster configuration
            return new SiloHostBuilder()
                .UseLocalhostClustering()
                .Configure<SiloMessagingOptions>(options =>
                {
                    // Needed by Reactive pool: reduced message timeout to ease promise break testing
                    options.ResponseTimeout = TimeSpan.FromSeconds(30 * 60); // was 10
                    options.ResponseTimeoutWithDebugger = TimeSpan.FromSeconds(30 * 60); // was 10
                })
                .Configure<ClientMessagingOptions>(options =>
                {
                    // Needed by Reactive pool: reduced message timeout to ease promise break testing
                    options.ResponseTimeout = TimeSpan.FromSeconds(10);
                    options.ResponseTimeoutWithDebugger = TimeSpan.FromSeconds(10);
                })
                .Configure<SerializationProviderOptions>(_ =>
                {
                    _.SerializationProviders.Add(typeof(Orleans.Serialization.ProtobufNet.ProtobufNetSerializer).GetTypeInfo());

                    RuntimeTypeModel.Default.Add(typeof(DateTimeOffset), false).SetSurrogate(typeof(DateTimeOffsetSurrogate));
                })
                .Configure<ClusterOptions>(options =>
                {
                    options.ClusterId = "dev";
                    options.ServiceId = "OrleansTest";
                })
                .ConfigureApplicationParts(parts => parts.AddApplicationPart(typeof(ProductsGrain).Assembly).WithReferences())
                .ConfigureLogging(logging => logging.AddConsole())

                // .UseDashboard(options => { })
                // .UsePerfCounterEnvironmentStatistics()

                .AddAzureTableGrainStorage("TableStore", options => options.ConnectionString = "UseDevelopmentStorage=true")
                .AddAzureBlobGrainStorage("BlobStore", options => options.ConnectionString = "UseDevelopmentStorage=true")

                .UseAzureTableReminderService(options => options.ConnectionString = "UseDevelopmentStorage=true")

                // .AddSimpleMessageStreamProvider("SMSProvider")
                // .AddMemoryGrainStorage("PubSubStore")
                .AddAzureQueueStreams<AzureQueueDataAdapterV2>("AzureQueueProvider", optionsBuilder => optionsBuilder.Configure(options => { options.ConnectionString = "UseDevelopmentStorage=true"; }))
                .AddAzureTableGrainStorage("PubSubStore", options => { options.ConnectionString = "UseDevelopmentStorage=true"; })
                // .AddMemoryGrainStorage("PubSubStore")

            .Build();
        }

        static async Task StopSilo()
        {
            await silo.StopAsync();
            _siloStopped.Set();
        }
    }
}
