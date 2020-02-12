using AcessoTeste.ThiagoPereira.Web.Application.Interfaces;
using AcessoTeste.ThiagoPereira.Web.Application.Models;
using EasyNetQ;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace AcessoTeste.ThiagoPereira.Web.Service
{
    public class TransferHostedService : IHostedService, IDisposable
    {
        private readonly IConfiguration _configuration;
        private readonly IBus _bus;
        private readonly ITransferService _transferService;
        private readonly ILogger<TransferHostedService> _logger;

        public TransferHostedService(
            IConfiguration configuration, 
            IBus bus, 
            ITransferService transferSerivce, 
            ILogger<TransferHostedService> logger)
        {
            _configuration = configuration;
            _bus = bus;
            _transferService = transferSerivce;
            _logger = logger;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("StartAsync TransferJob");

                if (Convert.ToBoolean(_configuration["QueueTransferStart"]))
                {
                    _bus.SubscribeAsync<TransferInfo>($"Subscription-{Guid.NewGuid()}", HandleTransferService,
                        x => x.WithQueueName("TransferJob"));
                }
            }
            catch (Exception e)
            {
                _logger.LogError("Erro ao iniciar a fila.", e.ToString());
            }
        }


        private async Task HandleTransferService(TransferInfo transfer)
        {
            _transferService.Process(transfer);
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
