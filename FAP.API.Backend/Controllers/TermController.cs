using MediatR;
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
       
    }


}
