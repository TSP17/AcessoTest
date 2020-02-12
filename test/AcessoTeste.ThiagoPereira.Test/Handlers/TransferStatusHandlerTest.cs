using AcessoTeste.ThiagoPereira.Web.Application.Commands;
using AcessoTeste.ThiagoPereira.Web.Application.Interfaces;
using AcessoTeste.ThiagoPereira.Web.Application.Models;
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
    public class TransferStatusHandlerTest
    {
        private readonly TransferStatusHandler handler;

        private readonly Mock<ITransferInfoRepository> _transferInfoRepository;
        private TransferInfo TransferInfo;

        public TransferStatusHandlerTest()
        {
            _transferInfoRepository = new Mock<ITransferInfoRepository>();

            handler = new TransferStatusHandler(_transferInfoRepository.Object);

        }

        [Fact(DisplayName = ("Get information with Success"))]
        public async void Success()
        {
            SetTransferInfo("Confirmed");

            _transferInfoRepository.Setup(x => x.GetById(It.IsAny<string>()))
                .Returns(Task.FromResult(TransferInfo));

           var result = await handler.Handle(
               new TransferStatusCommand()
               {
                   TransactionId = "1"
               },
               CancellationToken.None);


            Assert.True(result != null);
            Assert.True(!result.Erros.Any());
        }

        [Fact(DisplayName = ("Get information with Error"))]
        public async void StatusNotFoundError()
        {
            SetTransferInfo(string.Empty);

            _transferInfoRepository.Setup(x => x.GetById(It.IsAny<string>()))
                .Returns(Task.FromResult(TransferInfo));

            var result = await handler.Handle(
                new TransferStatusCommand()
                {
                    TransactionId = "1"
                },
                CancellationToken.None);


            Assert.True(result != null);
            Assert.Contains(result.Erros, x => x == Constants.AccountDestinationInvalid);
        }

        private void SetTransferInfo(string status)
        {
            TransferInfo = new TransferInfo()
            {
                AccountDestination = "1234",
                AccountOrigin = "5678",
                DateCreated = DateTime.UtcNow.ToString(),
                DateUpdatedStatus = DateTime.UtcNow.ToString(),
                Id = "1",
                Status = status,
                Value = "100"
            };
        }
    }
}
