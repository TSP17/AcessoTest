using AcessoTeste.ThiagoPereira.Web.Application.Response;
using MediatR;

namespace AcessoTeste.ThiagoPereira.Web.Application.Commands
{
    public class TransferCommand : IRequest<TransferResponse>
    {
        public string AccountOrigin { get; set; }
        public string AccountDestination { get; set; }
        public decimal Value { get; set; }
    }
}
