using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MediatR;
using System.Threading.Tasks;
using System.Collections.Generic;
using APP.Users.Features.Skills;

namespace API.Users.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class SkillsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public SkillsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // GET: api/Skills
        [HttpGet]
        public async Task<ActionResult<List<SkillQueryResponse>>> Get([FromQuery] SkillQueryRequest request = null)
        {
            if (request == null)
                request = new SkillQueryRequest();

            var response = await _mediator.Send(request);

            if (!response.IsSuccessful)
                return BadRequest(response.Message);

            return Ok(response.Data);
        }

        // The following endpoints require Admin role

        // POST: api/Skills
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<int>> Post([FromBody] SkillCreateRequest request)
        {
            var response = await _mediator.Send(request);

            if (!response.IsSuccessful)
                return BadRequest(response.Message);

            return CreatedAtAction(nameof(Get), new { id = response.Data }, response.Data);
        }

        // PUT: api/Skills/5
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Put(int id, [FromBody] SkillUpdateRequest request)
        {
            if (id != request.Id)
                return BadRequest("ID mismatch between URL and request body");

            var response = await _mediator.Send(request);

            if (!response.IsSuccessful)
                return BadRequest(response.Message);

            return NoContent();
        }

        // DELETE: api/Skills/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var request = new SkillDeleteRequest { Id = id };
            var response = await _mediator.Send(request);

            if (!response.IsSuccessful)
                return BadRequest(response.Message);

            return NoContent();
        }
    }
}