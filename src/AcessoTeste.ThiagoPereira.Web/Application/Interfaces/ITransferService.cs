using AcessoTeste.ThiagoPereira.Web.Application.Models;
using System.Threading.Tasks;

namespace AcessoTeste.ThiagoPereira.Web.Application.Interfaces
{
    public interface ITransferService
    {
        Task<string> Process(TransferInfo transfer);
    }
}
