using AcessoTeste.ThiagoPereira.Web.Application.Models;
using System.Threading.Tasks;

namespace AcessoTeste.ThiagoPereira.Web.Application.Interfaces
{
    public interface ITransferInfoRepository
    {
        Task AddOrUpdate(TransferInfo transferInfo);
        Task<TransferInfo> GetById(string Id);
        Task CreateTable();

    }
}
