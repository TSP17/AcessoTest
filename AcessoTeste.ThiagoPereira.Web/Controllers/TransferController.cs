using AcessoTeste.ThiagoPereira.Web.Application.Commands;
using AcessoTeste.ThiagoPereira.Web.Application.Response;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

namespace AcessoTeste.ThiagoPereira.Web.Controllers
{
    [Produces("application/json")]
    [Route("api/[Controller]")]
    public class TransferController : ControllerBase
    {
        private readonly IMediator _mediatr;

        public TransferController(IMediator mediatr)
        {
            _mediatr = mediatr;
        }

        [HttpPost]
        public async Task<IActionResult> Transfer([FromBody]TransferCommand request)
        {
            try
            {
                var response = await _mediatr.Send(request);

                if (!response.Erros.Any())
                {
                    return Ok(new {  transactionId = response.TransactionId });
                }

                return BadRequest(ResultResponse.CreateError(response.Erros.FirstOrDefault()));
            }
            catch (System.Exception e)
            {

                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpGet]
        public async Task<IActionResult> TransferStatus([FromQuery]string id)
        {
            try
            {
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
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
        }
    }
}
