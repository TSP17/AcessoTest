using AcessoTeste.ThiagoPereira.Web.Application.Commands;
using AcessoTeste.ThiagoPereira.Web.Application.Response;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Threading.Tasks;

namespace AcessoTeste.ThiagoPereira.Web.Controllers
{
    [Produces("application/json")]
    [Route("api/[Controller]")]
    public class TransferController : ControllerBase
    {
        private readonly IMediator _mediatr;
        public ILogger<TransferController> _logger;

        public TransferController(IMediator mediatr, ILogger<TransferController> logger)
        {
            _mediatr = mediatr;
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> Transfer([FromBody]TransferCommand request)
        {
            try
            {
                _logger.LogInformation("Transfer - Starting request", request);

                var response = await _mediatr.Send(request);

                if (!response.Erros.Any())
                {
                    return Ok(new {  transactionId = response.TransactionId });
                }

                return BadRequest(ResultResponse.CreateError(response.Erros.FirstOrDefault()));
            }
            catch (System.Exception e)
            {
                _logger.LogError(e.ToString(), request);
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpGet]
        public async Task<IActionResult> TransferStatus([FromQuery]string id)
        {
            try
            {
                _logger.LogInformation("TransferStatus - Starting request", id);

                TransferStatusCommand command = new TransferStatusCommand()
                {
                    TransactionId = id
                };

                var response = await _mediatr.Send(command);

                if (!response.Erros.Any())
                {
                    return Ok(new { Status = response.Status });
                }

                return BadRequest(ResultResponse.CreateError(response.Erros.FirstOrDefault()));

            }
            catch (System.Exception e)
            {
                _logger.LogError($"TransferStatus{e.ToString()}", id);

                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
        }
    }
}
