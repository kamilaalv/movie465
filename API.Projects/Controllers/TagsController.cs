using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using APP.Projects.Features.Tags;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace API.Projects.Controllers
{
    /// <summary>
    /// API controller for handling tag-related requests.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class TagsController : ControllerBase
    {
        private readonly IMediator _mediator;

        /// <summary>
        /// Initializes a new instance of the <see cref="TagsController"/> class with the specified mediator.
        /// </summary>
        /// <param name="mediator">The mediator to be used for sending requests.</param>
        public TagsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Handles GET requests to retrieve all tags.
        /// </summary>
        /// <returns>A list of tags if available; otherwise, NoContent.</returns>
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var response = await _mediator.Send(new TagQueryRequest());
            var list = await response.ToListAsync();
            if (list.Any()) // Check if the list is not empty.
                return Ok(list);
            return NoContent();
        }

        /// <summary>
        /// Handles GET requests to retrieve a specific tag by ID.
        /// </summary>
        /// <param name="id">The ID of the tag to retrieve.</param>
        /// <returns>The tag if found; otherwise, NoContent.</returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var response = await _mediator.Send(new TagQueryRequest());
            var item = await response.SingleOrDefaultAsync(i => i.Id == id);
            if (item is not null) // if (item != null)
                return Ok(item);
            return NoContent();
        }

        /// <summary>
        /// Handles POST requests to insert a specific tag.
        /// </summary>
        /// <param name="request">The tag request to insert.</param>
        /// <returns>Success if operation succeeds; otherwise, BadRequest.</returns>
        [HttpPost]
        public async Task<IActionResult> Post(TagCreateRequest request)
        {
            if (ModelState.IsValid)
            {
                var response = await _mediator.Send(request);

                if (response.IsSuccessful)
                    return Ok(response); // 200 Http Status Code

                return BadRequest(response); // 400 Http Status Code
            }
            return BadRequest(ModelState);
        }

        /// <summary>
        /// Handles PUT requests to update a specific tag.
        /// </summary>
        /// <param name="request">The tag request to update.</param>
        /// <returns>Success if operation succeeds; otherwise, BadRequest.</returns>
        [HttpPut]
        public async Task<IActionResult> Put(TagUpdateRequest request)
        {
            if (ModelState.IsValid)
            {
                var response = await _mediator.Send(request);

                if (response.IsSuccessful)
                    return Ok(response);

                return BadRequest(response);
            }
            return BadRequest(ModelState);
        }

        /// <summary>
        /// Handles DELETE requests to delete a specific tag by ID.
        /// </summary>
        /// <param name="id">The tag ID to delete.</param>
        /// <returns>Success if operation succeeds; otherwise, BadRequest.</returns>
        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            var response = await _mediator.Send(new TagDeleteRequest() { Id = id });

            if (response.IsSuccessful)
                return Ok(response);

            return BadRequest(response);
        }
    }
}