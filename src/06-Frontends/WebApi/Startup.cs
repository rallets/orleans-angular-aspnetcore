using GrainInterfaces.Scheduled;
using GrainInterfaces.Serialization.ProtobufNet;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Configuration;
using Orleans.Hosting;
using ProtoBuf.Meta;
using System;
using System.Reflection;
using System.Threading.Tasks;

namespace WebApi
{
    public class Startup
    {
        readonly string _allowFrontendOrigin = "_allowFrontendOrigin";

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy(_allowFrontendOrigin,
                builder =>
                {
                    builder.AllowAnyOrigin() // .WithOrigins("http://localhost:4200")
                        .AllowAnyHeader()
                        .AllowAnyMethod();
                });
            });

            var orleansClient = CreateOrleansClient();
            services.AddSingleton<IClusterClient>(orleansClient);

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, Microsoft.AspNetCore.Hosting.IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseCors(_allowFrontendOrigin);

            app.UseHttpsRedirection();
            app.UseMvc();
        }

        private IClusterClient CreateOrleansClient()
        {
            var clientBuilder = new ClientBuilder()
                .UseLocalhostClustering()
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
                    options.ServiceId = "TestInventoryService";
                })
                .Configure<ProcessExitHandlingOptions>(options => options.FastKillOnProcessExit = false)
                .ConfigureLogging(logging => logging.AddConsole())

                .AddSimpleMessageStreamProvider("SMSProvider");

            var client = clientBuilder.Build();

            client.Connect(async ex =>
            {  // replace Console with actual logging
                Console.WriteLine(ex);
                Console.WriteLine("Retrying...");
                await Task.Delay(3000);
                return true;
            }).Wait();

            // startup the global reminders
            {
                var g = client.GetGrain<IInventoryAutoSupplying>(Guid.Empty);
                g.Start();
            }
            {
                var g = client.GetGrain<IOrderScheduledProcessing>(Guid.Empty);
                g.Start();
            }
            return client;
        }
    }
}
