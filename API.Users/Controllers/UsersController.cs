using System.Collections.Generic;
using System.Threading.Tasks;
using APP.Users.Features.Users;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Users.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UsersController : ControllerBase
    {
        private readonly IMediator _mediator;

        public UsersController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // GET: api/Users
        [HttpGet]
        public async Task<ActionResult<List<UserQueryResponse>>> Get([FromQuery] UserQueryRequest request)
        {
            var response = await _mediator.Send(request);

            if (!response.IsSuccessful)
                return BadRequest(response.Message);

            return Ok(response.Data);
        }

        // GET: api/Users/5
        [HttpGet("{id}")]
        public async Task<ActionResult<UserQueryResponse>> Get(int id)
        {
            var request = new UserQueryRequest();
            var response = await _mediator.Send(request);

            if (!response.IsSuccessful)
                return BadRequest(response.Message);

            var user = response.Data.Find(u => u.Id == id);

            if (user == null)
                return NotFound($"User with ID {id} not found");

            return Ok(user);
        }

        // POST: api/Users
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<int>> Post([FromBody] UserCreateRequest request)
        {
            var response = await _mediator.Send(request);

            if (!response.IsSuccessful)
                return BadRequest(response.Message);

            return CreatedAtAction(nameof(Get), new { id = response.Data }, response.Data);
        }

        // PUT: api/Users/5
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Put(int id, [FromBody] UserUpdateRequest request)
        {
            if (id != request.Id)
                return BadRequest("ID mismatch between URL and request body");

            var response = await _mediator.Send(request);

            if (!response.IsSuccessful)
                return BadRequest(response.Message);

            return NoContent();
        }

        // DELETE: api/Users/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var request = new UserDeleteRequest { Id = id };
            var response = await _mediator.Send(request);

            if (!response.IsSuccessful)
                return BadRequest(response.Message);

            return NoContent();
        }

        // POST: api/Users/Token
        [HttpPost("Token")]
        [AllowAnonymous]
        public async Task<ActionResult<TokenResponse>> Token([FromBody] TokenRequest request)
        {
            var response = await _mediator.Send(request);

            if (!response.IsSuccessful)
                return Unauthorized(response.Message);

            return Ok(response);
        }

        // POST: api/Users/RefreshToken
        [HttpPost("RefreshToken")]
        [AllowAnonymous]
        public async Task<ActionResult<TokenResponse>> RefreshToken([FromBody] RefreshTokenRequest request)
        {
            var response = await _mediator.Send(request);

            if (!response.IsSuccessful)
                return Unauthorized(response.Message);

            return Ok(response);
        }

        // GET: api/Users/Authorize
        [HttpGet("Authorize")]
        public ActionResult Authorize()
        {
            // This endpoint just verifies that the user is authorized
            // It returns 200 OK if the JWT is valid, 401 Unauthorized if not
            return Ok(new { IsAuthorized = true, Message = "User is authorized" });
        }
    }
}