using AcessoTeste.ThiagoPereira.Web.Application.Interfaces;
using AcessoTeste.ThiagoPereira.Web.Application.Models;
using AcessoTeste.ThiagoPereira.Web.Infra.Helper;
using System;
using System.Threading.Tasks;

namespace AcessoTeste.ThiagoPereira.Web.Service
{
    public class TransferService : ITransferService
    {
        private readonly ITransferInfoRepository _transferInfoRepository;
        private readonly IAcessoRepository _acessoRepository;
        public TransferService(ITransferInfoRepository transferInfoRepository,
                               IAcessoRepository acessoRepository)
        {
            _transferInfoRepository = transferInfoRepository;
            _acessoRepository = acessoRepository;
        }

        public async Task<string> Process(TransferInfo transfer)
        {
            transfer.Status = Constants.TransferStatusProcessing;
            await _transferInfoRepository.Put(transfer);

           // 1 - Consultar se existe conta de origem.
            var accountOrigin = await _acessoRepository.GetAccountByNumber(transfer.AccountOrigin);
            if (accountOrigin.Balance < Convert.ToDecimal(transfer.Value))
            {
                transfer.Status = Constants.TransferStatusError;
                await _transferInfoRepository.Put(transfer);
                return Constants.AccountBalanceInsufficient;
            }

            bool resultOperation = await _acessoRepository.Transfer(
                new AcessoTransferRequest()
                {
                    AccountNumber = transfer.AccountDestination,
                    Type = Constants.AcessoTransferTypeCredit,
                    Value = Convert.ToDecimal(transfer.Value)
                });

            transfer.Status = resultOperation ? Constants.TransferStatusConfirmed : Constants.TransferStatusError;
            await _transferInfoRepository.Put(transfer);

            return Constants.TransferStatusConfirmed;
        }
    }
}
