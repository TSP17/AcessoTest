using AcessoTeste.ThiagoPereira.Web.Application.Interfaces;
using AcessoTeste.ThiagoPereira.Web.Application.Response;
using AcessoTeste.ThiagoPereira.Web.Infra.Helper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AcessoTeste.ThiagoPereira.Web.Application.Commands
{
    public class TransferHandler : IRequestHandler<TransferCommand, TransferResponse>
    {
        private readonly IAcessoRepository _acessoRepository;
        private readonly ITransferInfoRepository _transferInfoRepository;
        private readonly ISendQueueRepository _sendQueuRepository;

        public TransferHandler(IAcessoRepository acessoRepository,
            ITransferInfoRepository transferInfoRepository,
            ISendQueueRepository sendQueuRepository)
        {
            _acessoRepository = acessoRepository;
            _transferInfoRepository = transferInfoRepository;
            _sendQueuRepository = sendQueuRepository;
        }

        public async Task<TransferResponse> Handle(TransferCommand request, CancellationToken cancellationToken)
        {
            await _transferInfoRepository.CreateTable();
            TransferResponse response = new TransferResponse();

            //1 - Consultar se existe conta de origem.
           var accountOrigin = await _acessoRepository.GetAccountByNumber(request.AccountOrigin);
            if (accountOrigin is null || accountOrigin.Id == 0)
            {
                response.AddError(Constants.AccountInvalid);
                return response;
            }

            // 2 - Validar saldo da conta de origem.
            if (accountOrigin.Balance < request.Value)
            {
                response.AddError(Constants.AccountBalanceInsufficient);
                return response;
            }

            // 3 - Consultar se existe conta de destino.
            var accountDestination = await _acessoRepository.GetAccountByNumber(request.AccountDestination);
            if (accountDestination is null || accountDestination.Id == 0)
            {
                response.AddError(Constants.AccountDestinationInvalid);
                return response;
            }

            response.TransactionId = Guid.NewGuid().ToString();
            // 4 - Gravar operação no DynamoDb.

            var transferInfo = new Models.TransferInfo()
            {
                Id = response.TransactionId,
                AccountDestination = request.AccountDestination,
                AccountOrigin = request.AccountOrigin,
                DateCreated = DateTime.UtcNow.ToString(),
                DateUpdatedStatus = DateTime.UtcNow.ToString(),
                Status = Constants.TransferStatusInQueue,
                Value = request.Value.ToString()
            };

            await _transferInfoRepository.Put(transferInfo);

            // 5 - Enviar para Fila.
            await _sendQueuRepository.SendQueue(transferInfo);

            return response;
        }
    }
}
