using AcessoTeste.ThiagoPereira.Web.Application.Interfaces;
using AcessoTeste.ThiagoPereira.Web.Application.Models;
using EasyNetQ;
using System.Threading.Tasks;

namespace AcessoTeste.ThiagoPereira.Web.Infra.Repository
{
    public class SendQueueRepository : ISendQueueRepository
    {
        private readonly IBus _bus;

        public SendQueueRepository(IBus bus)
        {
            _bus = bus;
        }
        public async Task SendQueue(TransferInfo transferInfo)
        {
            await _bus.PublishAsync(transferInfo, "TransferJob");
        }
    }
}
