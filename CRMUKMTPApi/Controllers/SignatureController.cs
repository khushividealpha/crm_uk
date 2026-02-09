using CRMUKMTPApi.Models;
using CRMUKMTPApi.Repositories;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CRMUKMTPApi.Controllers
{
    [Route("api/v1")]
    [ApiController]
    public class SignatureController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ITransactionRepository _repo;

        public SignatureController(IMediator mediator, ITransactionRepository repo)
        {
            _mediator = mediator;
            _repo = repo;
        }
        [HttpPost("internaltransfer")]
        public async Task<IActionResult> InternalTransfer([FromBody] InternalTranReuest request)
        {
            if (request == null)
                return BadRequest("Invalid request");

            var result = await _repo.TransferBetweenAccountsAsync(
                request.From,
                request.To,
                request.Amount,
                request.FromComment,
                request.ToComment
            );

            if (!result)
            {
                return BadRequest(new
                {
                    status = "Failed",
                });
            }

            return Ok(new
            {
                status = "Success",
            });
        }
    }
}
