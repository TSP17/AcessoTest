using AcessoTeste.ThiagoPereira.Web.Application.Interfaces;
using AcessoTeste.ThiagoPereira.Web.Application.Models;
using AcessoTeste.ThiagoPereira.Web.Infra.Helper;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace AcessoTeste.ThiagoPereira.Web.Service
{
    public class TransferService : ITransferService
    {
        private readonly ITransferInfoRepository _transferInfoRepository;
        private readonly IAcessoRepository _acessoRepository;

        public ILogger<TransferService> _logger;

        public TransferService(ITransferInfoRepository transferInfoRepository,
                               IAcessoRepository acessoRepository,
                               ILogger<TransferService> logger)
        {
            _transferInfoRepository = transferInfoRepository;
            _acessoRepository = acessoRepository;
            _logger = logger;
        }

        public async Task<string> Process(TransferInfo transfer)
        {
            try
            {
                _logger.LogInformation("Queue Process - Starting request", transfer);

                // 1 - Atualiza DynamoDb - Processsando
                transfer.Status = Constants.TransferStatusProcessing;
                await _transferInfoRepository.AddOrUpdate(transfer);

                // 2 - Consultar se cliente possue saldo.
                var accountOrigin = await _acessoRepository.GetAccountByNumber(transfer.AccountOrigin);
                if (accountOrigin.Balance < Convert.ToDecimal(transfer.Value))
                {
                    transfer.Status = Constants.TransferStatusError;
                    await _transferInfoRepository.AddOrUpdate(transfer);
                    return Constants.AccountBalanceInsufficient;
                }

                // 3 - Insere saldo para conta de destino.
                bool resultOperationDestination = await _acessoRepository.Transfer(
                    new AcessoTransferRequest()
                    {
                        AccountNumber = transfer.AccountDestination,
                        Type = Constants.AcessoTransferTypeCredit,
                        Value = Convert.ToDecimal(transfer.Value)
                    });

                // 4 - Retira saldo da conta da conta de origem.
                bool resultOperationOrigin = await _acessoRepository.Transfer(
                    new AcessoTransferRequest()
                    {
                        AccountNumber = transfer.AccountOrigin,
                        Type = Constants.AcessoTransferTypeDebit,
                        Value = Convert.ToDecimal(transfer.Value)
                    });

                // 5 - Atualiza operação no DynamoDb - Confirmado.
                transfer.Status = resultOperationDestination ? Constants.TransferStatusConfirmed : Constants.TransferStatusError;
                _logger.LogInformation("Queue Process - Success", transfer);
            }
            catch (Exception ex)
            {
                transfer.Status = Constants.TransferStatusError;
                _logger.LogError(ex.ToString(), transfer);

                return Constants.TransferStatusError;
            }
            finally
            {
                await _transferInfoRepository.AddOrUpdate(transfer);
            }

            return Constants.TransferStatusConfirmed;
        }
    }
}
