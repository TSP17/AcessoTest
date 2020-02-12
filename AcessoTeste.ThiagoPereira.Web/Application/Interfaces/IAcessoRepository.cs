using AcessoTeste.ThiagoPereira.Web.Application.Models;
using AcessoTeste.ThiagoPereira.Web.Application.Response.Api;
using System.Threading.Tasks;

namespace AcessoTeste.ThiagoPereira.Web.Application.Interfaces
{
    public interface IAcessoRepository
    {
        Task<AcessoAcountResponse> GetAccountByNumber(string AccountNumber);
        Task<bool> Transfer(AcessoTransferRequest request);
    }
}
