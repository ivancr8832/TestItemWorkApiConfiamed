using ItemWorks.Api.Application.Core.Application.ItemWorks.Command;
using ItemWorks.Api.Application.Core.Application.ItemWorks.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ItemWorks.Api.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ItemWorkController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ItemWorkController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> GetListPending()
        {
            var res = await _mediator.Send(new ItemWorkGetListQry());
            return StatusCode((int)res.Code, res);
        }

        [HttpGet("find")]
        public async Task<IActionResult> GetFindPending([FromQuery] ItemWorkFindQry query)
        {
            var res = await _mediator.Send(query);
            return StatusCode((int)res.Code, res);
        }

        [HttpPost]
        public async Task<IActionResult> CreateItemWork([FromBody] ItemWorkCreateCmd command)
        {
            var res = await _mediator.Send(command);
            return StatusCode((int)res.Code, res);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateItemWork([FromRoute] int id, [FromBody] ItemWorkUpdateCmd command)
        {
            command.Id = id;
            var res = await _mediator.Send(command);
            return StatusCode((int)res.Code, res);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteItemWork([FromRoute] int id)
        {
            var res = await _mediator.Send(new ItemWorkDeleteCmd { Id = id });
            return StatusCode((int)res.Code, res);
        }
    }
}
