using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MediatR;
using System.Threading.Tasks;
using System.Collections.Generic;
using APP.Users.Features.Roles;

namespace API.Users.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class RolesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public RolesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // GET: api/Roles
        [HttpGet]
        public async Task<ActionResult<List<RoleQueryResponse>>> Get([FromQuery] RoleQueryRequest request = null)
        {
            if (request == null)
                request = new RoleQueryRequest();

            var response = await _mediator.Send(request);

            if (!response.IsSuccessful)
                return BadRequest(response.Message);

            return Ok(response.Data);
        }

        // The following endpoints require Admin role

        // POST: api/Roles
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<int>> Post([FromBody] RoleCreateRequest request)
        {
            var response = await _mediator.Send(request);

            if (!response.IsSuccessful)
                return BadRequest(response.Message);

            return CreatedAtAction(nameof(Get), new { id = response.Data }, response.Data);
        }

        // PUT: api/Roles/5
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Put(int id, [FromBody] RoleUpdateRequest request)
        {
            if (id != request.Id)
                return BadRequest("ID mismatch between URL and request body");

            var response = await _mediator.Send(request);

            if (!response.IsSuccessful)
                return BadRequest(response.Message);

            return NoContent();
        }

        // DELETE: api/Roles/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var request = new RoleDeleteRequest { Id = id };
            var response = await _mediator.Send(request);

            if (!response.IsSuccessful)
                return BadRequest(response.Message);

            return NoContent();
        }
    }
}