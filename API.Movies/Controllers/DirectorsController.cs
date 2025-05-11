using System.Collections.Generic;
using System.Threading.Tasks;
using APP.Movies.Features.Directors;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace API.Movies.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DirectorsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public DirectorsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // GET: api/Directors
        [HttpGet]
        public async Task<ActionResult<List<DirectorQueryResponse>>> Get([FromQuery] DirectorQueryRequest request = null)
        {
            if (request == null)
                request = new DirectorQueryRequest();

            var response = await _mediator.Send(request);

            if (!response.IsSuccessful)
                return BadRequest(response.Message);

            return Ok(response.Data);
        }

        // GET: api/Directors/5
        [HttpGet("{id}")]
        public async Task<ActionResult<DirectorQueryResponse>> Get(int id)
        {
            var request = new DirectorQueryRequest();
            var response = await _mediator.Send(request);

            if (!response.IsSuccessful)
                return BadRequest(response.Message);

            var director = response.Data.Find(d => d.Id == id);

            if (director == null)
                return NotFound($"Director with ID {id} not found");

            return Ok(director);
        }

        // POST: api/Directors
        [HttpPost]
        public async Task<ActionResult<int>> Post([FromBody] DirectorCreateRequest request)
        {
            var response = await _mediator.Send(request);

            if (!response.IsSuccessful)
                return BadRequest(response.Message);

            return CreatedAtAction(nameof(Get), new { id = response.Data }, response.Data);
        }

        // PUT: api/Directors/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] DirectorUpdateRequest request)
        {
            if (id != request.Id)
                return BadRequest("ID mismatch between URL and request body");

            var response = await _mediator.Send(request);

            if (!response.IsSuccessful)
                return BadRequest(response.Message);

            return NoContent();
        }

        // DELETE: api/Directors/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var request = new DirectorDeleteHandler { Id = id };
            var response = await _mediator.Send(request);

            if (!response.IsSuccessful)
                return BadRequest(response.Message);

            return NoContent();
        }
    }
}