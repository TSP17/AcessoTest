using AcessoTeste.ThiagoPereira.Web.Application.Models;
using System.Threading.Tasks;

namespace AcessoTeste.ThiagoPereira.Web.Application.Interfaces
{
    public interface ISendQueueRepository
    {
        Task SendQueue(TransferInfo transferInfo);
    }
}
