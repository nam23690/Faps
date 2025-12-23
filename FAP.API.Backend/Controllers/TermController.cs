using FAP.API.Backend.Models;
using FAP.Common.Application.Features.Term.Commands;
using FAP.Common.Application.Features.Term.Queries;
using FAP.Common.Application.Features.User.Queries;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FAP.API.Controllers
{
    [Authorize(Roles = "admin")]
    [ApiController]
    [Route("api/[controller]")]
    public class TermController : ControllerBase
    {
        private readonly IMediator _mediator;
        public TermController(IMediator mediator)
        {
            _mediator = mediator;
        }


        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet("ping")]
        public IActionResult Ping()
        {
            return Ok("pong");
        }
        [Authorize(Roles = "admin")]
        [HttpGet]
        public async Task<IActionResult> GetUsers(CancellationToken ct)
        {
            var result = await _mediator.Send(new GetTermQuery.GetTermsQuery(), ct);
            return Ok(result);
        }
        [Authorize(Roles = "admin")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(short id)
        {
            var user = await _mediator.Send(new GetTermByIdQuery { Id = id });
            return user == null ? NotFound() : Ok(user);
        }
        [Authorize(Roles = "admin")]
        [HttpPost]

       
        public async Task<IActionResult> Create([FromBody] CreateTermCommand command)
        {
            var id = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetById), new { id }, command);
        }
        [Authorize(Roles = "admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, UpdateTermCommand command)
        {
            command.Id =(short) id;
            await _mediator.Send(command);
            return NoContent();
        }
        [Authorize(Roles = "admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _mediator.Send(new DeleteTermCommand { Id = id });
            return NoContent();
        }
    }


}
