using AcessoTeste.ThiagoPereira.Web.Application.Interfaces;
using AcessoTeste.ThiagoPereira.Web.Application.Models;
using AcessoTeste.ThiagoPereira.Web.Application.Response.Api;
using AcessoTeste.ThiagoPereira.Web.Infra.Helper;
using AcessoTeste.ThiagoPereira.Web.Service;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Threading.Tasks;
using Xunit;

namespace AcessoTeste.ThiagoPereira.Test.Handlers
{
    public class TransferServiceTest
    {
        private readonly TransferService _transferService;

        private readonly Mock<IAcessoRepository> _acessoRepository;
        private readonly Mock<ITransferInfoRepository> _transferInfoRepository;
        private readonly Mock<ILogger<TransferService>> _logger;

        public TransferInfo TransferInfo;
        public AcessoAcountResponse AcessoAcountResponse;

        public TransferServiceTest()
        {
            _acessoRepository = new Mock<IAcessoRepository>();
            _transferInfoRepository = new Mock<ITransferInfoRepository>();
            _logger = new Mock<ILogger<TransferService>>();

            _transferService = new TransferService(_transferInfoRepository.Object, _acessoRepository.Object, _logger.Object);

        }

        [Fact(DisplayName = "Success")]
        public async void ProcessSuccess()
        {
            SetTransferInfo("10");
            SetAcessoAcountResponse(1, 10000);

            _acessoRepository.Setup(x => x.GetAccountByNumber(It.IsAny<string>()))
                .Returns(Task.FromResult(AcessoAcountResponse));

            var result = await _transferService.Process(TransferInfo);

            Assert.True(result == Constants.TransferStatusConfirmed);
        }

        [Fact(DisplayName = "Error balance Insuficcient")]
        public async void BalanceInsuficcientError()
        {
            SetTransferInfo("1000");
            SetAcessoAcountResponse(1, 1);

            _acessoRepository.Setup(x => x.GetAccountByNumber(It.IsAny<string>()))
                .Returns(Task.FromResult(AcessoAcountResponse));

            var result = await _transferService.Process(TransferInfo);

            Assert.True(result == Constants.AccountBalanceInsufficient);
        }

        private void SetTransferInfo(string value)
        {
            TransferInfo = new TransferInfo()
            {
                AccountDestination = "5678",
                AccountOrigin = "1234",
                DateCreated = DateTime.UtcNow.ToString(),
                DateUpdatedStatus = DateTime.UtcNow.ToString(),
                Id = Guid.NewGuid().ToString(),
                Status = "In Queue",
                Value = value
            };
        }

        private void SetAcessoAcountResponse(int id, decimal balance)
        {
            AcessoAcountResponse = new AcessoAcountResponse()
            {
                AccountNumber = "1234",
                Balance = balance,
                Id = id
            };
        }
    }
}
