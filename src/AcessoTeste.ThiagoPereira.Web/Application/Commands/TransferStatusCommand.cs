using AcessoTeste.ThiagoPereira.Web.Application.Response;
using MediatR;

namespace AcessoTeste.ThiagoPereira.Web.Application.Commands
{
    public class TransferStatusCommand : IRequest<TransferStatusResponse>
    {
        public string TransactionId { get; set; }
    }
}
