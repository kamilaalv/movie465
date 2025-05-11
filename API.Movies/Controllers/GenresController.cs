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
        public async Task<ActionResult<List<GenreQueryResponse>>> Get([FromQuery] GenreQueryRequest request)
        {
            var response = await _mediator.Send(request ?? new GenreQueryRequest());

            if (!response.IsSuccessful)
                return BadRequest(response.Message);

            return Ok(response.Data);
        }

        // GET: api/Genres/5
        [HttpGet("{id}")]
        public async Task<ActionResult<GenreQueryResponse>> Get(int id)
        {
            var queryRequest = new GenreQueryRequest();
            var queryResponse = await _mediator.Send(queryRequest);

            if (!queryResponse.IsSuccessful)
                return BadRequest(queryResponse.Message);

            var genre = queryResponse.Data.Find(g => g.Id == id);

            if (genre == null)
                return NotFound($"Genre with ID {id} not found");

            return Ok(genre);
        }

        // POST: api/Genres
        [HttpPost]
        public async Task<ActionResult<int>> Post([FromBody] GenreCreateRequest request)
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
            var deleteRequest = new GenreDeleteRequest { Id = id };
            var deleteResponse = await _mediator.Send(deleteRequest);

            if (!deleteResponse.IsSuccessful)
                return BadRequest(deleteResponse.Message);

            return NoContent();
        }
    }
}
