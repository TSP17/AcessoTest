using AcessoTeste.ThiagoPereira.Web.Application.Commands;
using AcessoTeste.ThiagoPereira.Web.Application.Interfaces;
using AcessoTeste.ThiagoPereira.Web.Application.Models;
using AcessoTeste.ThiagoPereira.Web.Application.Response.Api;
using AcessoTeste.ThiagoPereira.Web.Infra.Helper;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace AcessoTeste.ThiagoPereira.Test.Handlers
{
    public class TransferHandlerTest
    {
        private readonly TransferHandler handler;

        private readonly Mock<IAcessoRepository> _acessoRepository;
        private readonly Mock<ITransferInfoRepository> _transferInfoRepository;
        private readonly Mock<ISendQueueRepository> _sendQueuRepository;

        public AcessoAcountResponse AcessoAcountResponse;
        public AcessoAcountResponse AcessoAcountDestinationResponse;
        public TransferInfo TransferInfo;

        public TransferHandlerTest()
        {
            _acessoRepository = new Mock<IAcessoRepository>();
            _transferInfoRepository = new Mock<ITransferInfoRepository>();
            _sendQueuRepository = new Mock<ISendQueueRepository>();

            handler = new TransferHandler(
                _acessoRepository.Object,
                _transferInfoRepository.Object, 
                _sendQueuRepository.Object);
        }

        [Fact(DisplayName = "Transfer sended to queue to process")]
        public async void TransferSuccess()
        {
            SetAcessoAcountResponse(1, 30000);
            SetTransferInfo("10");

            _acessoRepository.Setup(p => p.GetAccountByNumber(It.IsAny<string>()))
                .Returns(Task.FromResult(AcessoAcountResponse));

            var result = await handler.Handle(
                        new TransferCommand()
                        {
                            AccountOrigin = "123",
                            AccountDestination = "456",
                            Value = 1234
                        }, CancellationToken.None);

            Assert.True(result != null);
            Assert.True(!result.Erros.Any());
            Assert.True(!string.IsNullOrEmpty(result.TransactionId));
        }

        [Fact(DisplayName = "Account origin error.")]
        public async void AccountOriginError()
        {
            SetAcessoAcountResponse(0, 30000);
            SetTransferInfo( "10");

            _transferInfoRepository.Setup(p => p.CreateTable());
            _acessoRepository.Setup(p => p.GetAccountByNumber(It.IsAny<string>()))
                .Returns(Task.FromResult(AcessoAcountResponse));

            var result = await handler.Handle(
                        new TransferCommand()
                        {
                            AccountOrigin = "123",
                            AccountDestination = "456",
                            Value = 1234
                        }, CancellationToken.None);

            Assert.True(result != null);
            Assert.Contains(result.Erros, x => x == Constants.AccountInvalid);
        }

        [Fact(DisplayName = "Value is greater to Balance")]
        public async void TransferValueGreaterThanBalanceError()
        {
            SetAcessoAcountResponse(1, 10);
            SetTransferInfo("1000");

            _transferInfoRepository.Setup(p => p.CreateTable());
            _acessoRepository.Setup(p => p.GetAccountByNumber(It.IsAny<string>()))
                .Returns(Task.FromResult(AcessoAcountResponse));

            var result = await handler.Handle(
                        new TransferCommand()
                        {
                            AccountOrigin = "123",
                            AccountDestination = "456",
                            Value = 1234
                        }, CancellationToken.None);

            Assert.True(result != null);
            Assert.Contains(result.Erros, x => x == Constants.AccountBalanceInsufficient);
        }

        [Fact(DisplayName = "Account destination error.")]
        public async void AccountDestinationtError()
        {
            SetAcessoAcountResponse(1, 30000);
            SetTransferInfo("10");
            SetAcessoAcountDestinationResponse(0, 10);

            _transferInfoRepository.Setup(p => p.CreateTable());
            _acessoRepository.SetupSequence(p => p.GetAccountByNumber(It.IsAny<string>()))
                .Returns(Task.FromResult(AcessoAcountResponse))
                .Returns(Task.FromResult(AcessoAcountDestinationResponse));

            var result = await handler.Handle(
                        new TransferCommand()
                        {
                            AccountOrigin = "123",
                            AccountDestination = "456",
                            Value = 1234
                        }, CancellationToken.None);

            Assert.True(result != null);
            Assert.Contains(result.Erros, x => x == Constants.AccountDestinationInvalid);
        }

        private void SetAcessoAcountResponse(int id, decimal balance)
        {
            AcessoAcountResponse =  new AcessoAcountResponse()
            {
                AccountNumber = "1234",
                Balance = balance,
                Id = id
            };
        }

        private void SetAcessoAcountDestinationResponse(int id, decimal balance)
        {
            AcessoAcountDestinationResponse = new AcessoAcountResponse()
            {
                AccountNumber = "1234",
                Balance = balance,
                Id = id
            };
        }

        private void SetTransferInfo(string value)
        {
            TransferInfo =  new TransferInfo()
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
    }
}
