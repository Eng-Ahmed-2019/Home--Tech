using MediatR;
using Microsoft.AspNetCore.Mvc;
using HomeTech.Application.DTOs;
using HomeTech.Application.CQRS.Commands;

namespace HomeTech.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AuthController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterUserDto dto)
        {
            var result = await _mediator.Send(
                new RegisterUserCommand(dto));

            return Ok(new
            {
                success = true,
                message = result
            });
        }
    }
}