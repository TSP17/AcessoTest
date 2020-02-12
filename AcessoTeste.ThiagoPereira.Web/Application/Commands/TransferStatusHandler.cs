using AcessoTeste.ThiagoPereira.Web.Application.Interfaces;
using AcessoTeste.ThiagoPereira.Web.Application.Response;
using AcessoTeste.ThiagoPereira.Web.Infra.Helper;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace AcessoTeste.ThiagoPereira.Web.Application.Commands
{
    public class TransferStatusHandler : IRequestHandler<TransferStatusCommand, TransferStatusResponse>
    {
        private readonly ITransferInfoRepository _transferInfoRepository;

        public TransferStatusHandler(ITransferInfoRepository transferInfoRepository)
        {
            _transferInfoRepository = transferInfoRepository;
        }

        public async Task<TransferStatusResponse> Handle(TransferStatusCommand request, CancellationToken cancellationToken)
        {
            var response = new TransferStatusResponse();
            var transfer = await  _transferInfoRepository.GetById(request.TransactionId);

            if (transfer is null || string.IsNullOrEmpty(transfer.Status))
            {
                response.AddError(Constants.AccountDestinationInvalid);
                return response;
            }

            response.Status = transfer.Status;
            return response;
        }
    }
}
