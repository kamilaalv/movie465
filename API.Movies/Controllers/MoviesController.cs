using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using APP.Movies.Features.Movies;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace API.Movies.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MoviesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public MoviesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // GET: api/Movies
        [HttpGet]
        public async Task<ActionResult<List<MovieQueryResponse>>> Get([FromQuery] MovieQueryRequest request = null)
        {
            if (request == null)
                request = new MovieQueryRequest();

            var response = await _mediator.Send(request);

            if (!response.IsSuccessful)
                return BadRequest(response.Message);

            return Ok(response.Data);
        }

        // GET: api/Movies/5
        [HttpGet("{id}")]
        public async Task<ActionResult<MovieQueryResponse>> Get(int id)
        {
            var request = new MovieQueryRequest();
            var response = await _mediator.Send(request);

            if (!response.IsSuccessful)
                return BadRequest(response.Message);

            var movie = response.Data.Find(m => m.Id == id);

            if (movie == null)
                return NotFound($"Movie with ID {id} not found");

            return Ok(movie);
        }

        // POST: api/Movies
        [HttpPost]
        public async Task<ActionResult<int>> Post([FromBody] MovieCreateRequest request)
        {
            var response = await _mediator.Send(request);

            if (!response.IsSuccessful)
                return BadRequest(response.Message);

            return CreatedAtAction(nameof(Get), new { id = response.Data }, response.Data);
        }

        // PUT: api/Movies/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] MovieUpdateRequest request)
        {
            if (id != request.Id)
                return BadRequest("ID mismatch between URL and request body");

            var response = await _mediator.Send(request);

            if (!response.IsSuccessful)
                return BadRequest(response.Message);

            return NoContent();
        }

        // DELETE: api/Movies/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var request = new MovieDeleteRequest { Id = id };
            var response = await _mediator.Send(request);

            if (!response.IsSuccessful)
                return BadRequest(response.Message);

            return NoContent();
        }
    }
}