#nullable disable
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MediatR;
using CORE.APP.Features;
using APP.Projects.Features.Works;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;

//Generated from Custom Template.
namespace API.Projects.Controllers
{
    /// <summary>
    /// Controller for managing work items.
    /// Provides endpoints to get, create, update, and delete work items.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class WorksController : ControllerBase
    {
        private readonly ILogger<WorksController> _logger;
        private readonly IMediator _mediator;

        /// <summary>
        /// Initializes a new instance of the <see cref="WorksController"/> class.
        /// </summary>
        /// <param name="logger">Logger for logging errors and info.</param>
        /// <param name="mediator">Mediator for handling requests.</param>
        public WorksController(ILogger<WorksController> logger, IMediator mediator)
        {
            _logger = logger;
            _mediator = mediator;
        }

        /// <summary>
        /// Retrieves all work items.
        /// </summary>
        /// <returns>A list of work items or NoContent if none exist.</returns>
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            try
            {
                var response = await _mediator.Send(new WorkQueryRequest());
                var list = await response.ToListAsync();
                if (list.Any())
                    return Ok(list);
                return NoContent();
            }
            catch (Exception exception)
            {
                _logger.LogError("WorksGet Exception: " + exception.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, new CommandResponse(false, "An exception occurred during WorksGet."));
            }
        }

        /// <summary>
        /// Retrieves a specific work item by ID.
        /// </summary>
        /// <param name="id">The ID of the work item.</param>
        /// <returns>The requested work item or NoContent if not found.</returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            try
            {
                var response = await _mediator.Send(new WorkQueryRequest());
                var item = await response.SingleOrDefaultAsync(r => r.Id == id);
                if (item is not null)
                    return Ok(item);
                return NoContent();
            }
            catch (Exception exception)
            {
                _logger.LogError("WorksGetById Exception: " + exception.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, new CommandResponse(false, "An exception occurred during WorksGetById."));
            }
        }

        /// <summary>
        /// Retrieves work items based on filter criteria.
        /// </summary>
        /// <param name="filter">Filter criteria for querying work items.</param>
        /// <returns>A filtered list of work items or NotFound if none match.</returns>
        [HttpPost("[action]")]
        public async Task<IActionResult> GetWorksByFilter(WorkQueryRequest filter)
        {
            var response = await _mediator.Send(filter);
            var list = await response.ToListAsync();
            if (list.Any())
                return Ok(list);
            return NotFound(); // 404
        }

        /// <summary>
        /// Creates a new work item.
        /// </summary>
        /// <param name="request">The work item creation request.</param>
        /// <returns>The created work item or BadRequest if validation fails.</returns>
        [HttpPost, Authorize(Roles = "Admin")]
        public async Task<IActionResult> Post(WorkCreateRequest request)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var response = await _mediator.Send(request);
                    if (response.IsSuccessful)
                    {
                        return Ok(response);
                    }
                    ModelState.AddModelError("WorksPost", response.Message);
                }
                return BadRequest(new CommandResponse(false, string.Join("|", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage))));
            }
            catch (Exception exception)
            {
                _logger.LogError("WorksPost Exception: " + exception.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, new CommandResponse(false, "An exception occurred during WorksPost."));
            }
        }

        /// <summary>
        /// Updates an existing work item.
        /// </summary>
        /// <param name="request">The work item update request.</param>
        /// <returns>The updated work item or BadRequest if validation fails.</returns>
        [HttpPut, Authorize(Roles = "Admin")]
        public async Task<IActionResult> Put(WorkUpdateRequest request)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var response = await _mediator.Send(request);
                    if (response.IsSuccessful)
                    {
                        return Ok(response);
                    }
                    ModelState.AddModelError("WorksPut", response.Message);
                }
                return BadRequest(new CommandResponse(false, string.Join("|", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage))));
            }
            catch (Exception exception)
            {
                _logger.LogError("WorksPut Exception: " + exception.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, new CommandResponse(false, "An exception occurred during WorksPut."));
            }
        }

        /// <summary>
        /// Deletes a specific work item by ID.
        /// </summary>
        /// <param name="id">The ID of the work item to delete.</param>
        /// <returns>Success response if deleted or BadRequest if an error occurs.</returns>
        [HttpDelete("{id}"), Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var response = await _mediator.Send(new WorkDeleteRequest() { Id = id });
                if (response.IsSuccessful)
                {
                    return Ok(response);
                }
                ModelState.AddModelError("WorksDelete", response.Message);
                return BadRequest(new CommandResponse(false, string.Join("|", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage))));
            }
            catch (Exception exception)
            {
                _logger.LogError("WorksDelete Exception: " + exception.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, new CommandResponse(false, "An exception occurred during WorksDelete."));
            }
        }
    }
}