using AcessoTeste.ThiagoPereira.Web.Application.Interfaces;
using AcessoTeste.ThiagoPereira.Web.Application.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using EasyNetQ;

namespace AcessoTeste.ThiagoPereira.Web.Infra.Repository
{
    public class SendQueueRepository : ISendQueueRepository
    {
        private readonly IConfiguration _configuration;
        private readonly IBus _bus;

        public SendQueueRepository(IConfiguration configuration, IBus bus)
        {
            _configuration = configuration;
            _bus = bus;
        }
        public async Task SendQueue(TransferInfo transferInfo)
        {
           // _bus.Subscribe<TransferInfo>("my_test_subscriptionid", transferInfo);
            //await _bus.PublishAsync<TransferInfo>(transferInfo).;
            await _bus.PublishAsync(transferInfo, "TransferJob");

            //var factory = new ConnectionFactory()
            //{
            //    HostName = _configuration.GetValue<string>("RabbitConfiguration:HostName"),
            //    Port = _configuration.GetValue<int>("RabbitConfiguration:Port"),
            //    UserName = _configuration.GetValue<string>("RabbitConfiguration:UserName"),
            //    Password = _configuration.GetValue<string>("RabbitConfiguration:Password")
            //};

            //using (var connection = factory.CreateConnection())
            //using (var channel = connection.CreateModel())
            //{
            //    channel.QueueDeclare(queue: "TransferJob",
            //                         durable: true,
            //                         exclusive: false,
            //                         autoDelete: false,
            //                         arguments: null);

            //    string message = "Vai";

            //    var body = Encoding.UTF8.GetBytes(message);

            //    channel.BasicPublish(exchange: "",
            //                         routingKey: "TransferJob",
            //                         basicProperties: null,
            //                         body: body);
            //}
        }
    }
}
