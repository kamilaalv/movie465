#nullable disable
using Microsoft.EntityFrameworkCore;
using MediatR;
using CORE.APP.Features;
using APP.Projects.Features.Projects;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

//Generated from Custom Template.
namespace API.Projects.Controllers
{
    /// <summary>
    /// API controller for managing projects.
    /// Provides endpoints to query, retrieve, and create project entities.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ProjectsController : ControllerBase
    {
        private readonly ILogger<ProjectsController> _logger;
        private readonly IMediator _mediator;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProjectsController"/> class.
        /// </summary>
        /// <param name="logger">Logger instance for logging operations.</param>
        /// <param name="mediator">Mediator instance for handling requests.</param>
        public ProjectsController(ILogger<ProjectsController> logger, IMediator mediator)
        {
            _logger = logger;
            _mediator = mediator;
        }

        /// <summary>
        /// Retrieves a list of all projects.
        /// </summary>
        /// <returns>A list of project query responses.</returns>
        [HttpGet, AllowAnonymous]
        public async Task<IActionResult> Get()
        {
            // Optional: Create a cancellation token to control request lifetime.
            var cancellationTokenSource = new CancellationTokenSource();
            var cancellationToken = cancellationTokenSource.Token;

            try
            {
                var response = await _mediator.Send(new ProjectQueryRequest(), cancellationToken);
                var list = await response.ToListAsync();

                // Optional: Cancel the task if necessary, for other cancel operations check other Cancel methods of cancellationTokenSource
                // cancellationTokenSource.Cancel();

                if (list.Any())
                    return Ok(list);

                return NoContent();
            }
            catch (Exception exception)
            {
                _logger.LogError($"ProjectsGet Exception: {exception.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new CommandResponse(false, "An exception occurred during ProjectsGet."));
            }
        }

        /// <summary>
        /// Retrieves a specific project by its ID.
        /// </summary>
        /// <param name="id">The ID of the project.</param>
        /// <returns>The project details if found; otherwise, a NoContent response.</returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            try
            {
                var response = await _mediator.Send(new ProjectQueryRequest());
                var item = await response.SingleOrDefaultAsync(r => r.Id == id);

                if (item is not null)
                    return Ok(item);

                return NoContent();
            }
            catch (Exception exception)
            {
                _logger.LogError($"ProjectsGetById Exception: {exception.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new CommandResponse(false, "An exception occurred during ProjectsGetById."));
            }
        }

        /// <summary>
        /// Creates a new project.
        /// </summary>
        /// <param name="request">The project creation request containing necessary details.</param>
        /// <returns>The result of the creation operation.</returns>
        [HttpPost, Authorize(Roles = "Admin")]
        public async Task<IActionResult> Post(ProjectCreateRequest request)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var response = await _mediator.Send(request);

                    if (response.IsSuccessful)
                    {
                        // Optionally, CreatedAtAction(nameof(Get), new { id = response.Id }, response) can be returned.
                        return Ok(response);
                    }

                    ModelState.AddModelError("ProjectsPost", response.Message);
                }

                return BadRequest(new CommandResponse(false,
                    string.Join("|", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage))));
            }
            catch (Exception exception)
            {
                _logger.LogError($"ProjectsPost Exception: {exception.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new CommandResponse(false, "An exception occurred during ProjectsPost."));
            }
        }

        /// <summary>
        /// Updates an existing project.
        /// </summary>
        /// <param name="request">The request containing the project update details.</param>
        /// <returns>A response indicating the success or failure of the update operation.</returns>
        [HttpPut, Authorize(Roles = "Admin")]
        public async Task<IActionResult> Put(ProjectUpdateRequest request)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var response = await _mediator.Send(request);
                    if (response.IsSuccessful)
                    {
                        // Optionally, NoContent() can be returned.
                        return Ok(response);
                    }
                    ModelState.AddModelError("ProjectsPut", response.Message);
                }

                return BadRequest(new CommandResponse(false,
                    string.Join("|", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage))));
            }
            catch (Exception exception)
            {
                _logger.LogError($"ProjectsPut Exception: {exception.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new CommandResponse(false, "An exception occurred during ProjectsPut."));
            }
        }

        /// <summary>
        /// Deletes a project by its ID.
        /// </summary>
        /// <param name="id">The ID of the project to be deleted.</param>
        /// <returns>A response indicating the success or failure of the deletion operation.</returns>
        [HttpDelete("{id}"), Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var response = await _mediator.Send(new ProjectDeleteRequest { Id = id });
                if (response.IsSuccessful)
                {
                    // Optionally, NoContent() can be returned.
                    return Ok(response);
                }

                ModelState.AddModelError("ProjectsDelete", response.Message);
                return BadRequest(new CommandResponse(false,
                    string.Join("|", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage))));
            }
            catch (Exception exception)
            {
                _logger.LogError($"ProjectsDelete Exception: {exception.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new CommandResponse(false, "An exception occurred during ProjectsDelete."));
            }
        }
    }
}