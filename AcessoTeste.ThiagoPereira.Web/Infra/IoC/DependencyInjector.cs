using AcessoTeste.ThiagoPereira.Web.Application.Interfaces;
using AcessoTeste.ThiagoPereira.Web.Infra.Repository;
using AcessoTeste.ThiagoPereira.Web.Service;
using Amazon.DynamoDBv2;
using Amazon.S3;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.IO;

namespace AcessoTeste.ThiagoPereira.Web.Infra.IoC
{
    public static class DependencyInjector
    {
        private static IServiceProvider _serviceProvider;
        private static IServiceCollection _services;

        public static T GetService<T>()
        {
            _services = _services ?? RegisterServices();
            _serviceProvider = _serviceProvider ?? _services.BuildServiceProvider();
            return _serviceProvider.GetService<T>();
        }

        public static IServiceCollection RegisterServices()
        {
            IConfigurationBuilder builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json");
            IConfiguration configuration = builder.Build();

            return RegisterServices(new ServiceCollection(), configuration);
        }

        public static IServiceCollection RegisterServices(IServiceCollection services, IConfiguration configuration)
        {
            _services = services;

            services.AddScoped<IMediator, Mediator>();

            // Dynamo Configuration
            Environment.SetEnvironmentVariable("AWS_ACCESS_KEY_ID", "AWS_ACCESS_KEY_ID");
            Environment.SetEnvironmentVariable("AWS_SECRET_ACCESS_KEY", "AWS_SECRET_ACCESS_KEY");
            Environment.SetEnvironmentVariable("AWS_REGION", "us-west-2");

            var dynamoDbConfig = configuration.GetSection("DynamoDb");

            var options = configuration.GetAWSOptions();
            IAmazonS3 client = options.CreateServiceClient<IAmazonS3>();

            var clientConfig2 = new AmazonDynamoDBConfig { ServiceURL = dynamoDbConfig.GetValue<string>("LocalServiceUrl") };

            services.AddSingleton<IAmazonDynamoDB>(sp =>
            {
                var clientConfig = new AmazonDynamoDBConfig { ServiceURL = dynamoDbConfig.GetValue<string>("LocalServiceUrl") };
                return new AmazonDynamoDBClient(clientConfig);
            });

            services.RegisterEasyNetQ("host=localhost:5672;username=guest;password=guest");
            //services.AddSingleton<IManagementClient>(p => new ManagementClient("localhost:5672", "guest", "guest"));

            services.AddMediatR();

            services.AddTransient<ITransferService, TransferService>();

            services.AddTransient<IAcessoRepository, AcessoRepository>();
            services.AddTransient<ITransferInfoRepository, TransferInfoRepository>();
            services.AddScoped<ISendQueueRepository, SendQueueRepository>();
            services.AddSingleton<IHostedService, TransferHostedService>();

            return services;
        }
    }
}
