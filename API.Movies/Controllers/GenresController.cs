using System.Collections.Generic;
using System.Threading.Tasks;
using APP.Movies.Features.Genres;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace API.Movies.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GenresController : ControllerBase
    {
        private readonly IMediator _mediator;

        public GenresController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // GET: api/Genres
        [HttpGet]
        public async Task<ActionResult<List<GenreQueryResponse>>> Get([FromQuery] GenreQueryHandler request = null)
        {
            if (request == null)
                request = new GenreQueryHandler();

            var response = await _mediator.Send(request);

            if (!response.IsSuccessful)
                return BadRequest(response.Message);

            return Ok(response.Data);
        }

        // GET: api/Genres/5
        [HttpGet("{id}")]
        public async Task<ActionResult<GenreQueryResponse>> Get(int id)
        {
            var request = new GenreQueryHandler();
            var response = await _mediator.Send(request);

            if (!response.IsSuccessful)
                return BadRequest(response.Message);

            var genre = response.Data.Find(g => g.Id == id);

            if (genre == null)
                return NotFound($"Genre with ID {id} not found");

            return Ok(genre);
        }

        // POST: api/Genres
        [HttpPost]
        public async Task<ActionResult<int>> Post([FromBody] GenreCreateHandler request)
        {
            var response = await _mediator.Send(request);

            if (!response.IsSuccessful)
                return BadRequest(response.Message);

            return CreatedAtAction(nameof(Get), new { id = response.Data }, response.Data);
        }

        // PUT: api/Genres/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] GenreUpdateRequest request)
        {
            if (id != request.Id)
                return BadRequest("ID mismatch between URL and request body");

            var response = await _mediator.Send(request);

            if (!response.IsSuccessful)
                return BadRequest(response.Message);

            return NoContent();
        }

        // DELETE: api/Genres/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var request = new GenreDeleteHandler { Id = id };
            var response = await _mediator.Send(request);

            if (!response.IsSuccessful)
                return BadRequest(response.Message);

            return NoContent();
        }
    }
}