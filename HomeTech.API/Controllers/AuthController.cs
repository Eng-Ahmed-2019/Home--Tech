using MediatR;
using Microsoft.AspNetCore.Mvc;
using HomeTech.Application.DTOs;
using HomeTech.Application.Common;
using HomeTech.Application.CQRS.Queries;
using Microsoft.AspNetCore.RateLimiting;
using HomeTech.Application.CQRS.Commands;
using Microsoft.AspNetCore.Authorization;

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
        [EnableRateLimiting("register")]
        [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
        public async Task<IActionResult> Register(
            [FromBody] RegisterUserDto dto,
            CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(
                new RegisterUserCommand(dto),
                cancellationToken);

            return Ok(
                ApiResponse<string>.CreateSuccessResponse(
                    result,
                    "تم إنشاء الحساب بنجاح"
                )
            );
        }

        [HttpPost("login")]
        [EnableRateLimiting("login")]
        [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
        public async Task<IActionResult> Login(
            [FromBody] LoginDto loginDto,
            CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(
                new LoginQuery(loginDto),
                cancellationToken);

            return Ok(
                ApiResponse<LoginResponseDto>.CreateSuccessResponse(
                    result,
                    "تم تسجيل الدخول بنجاح"
                )
            );
        }

        [Authorize]
        [HttpPost("logout")]
        public async Task<IActionResult> Logout(
            CancellationToken cancellationToken)
        {
            var sessionIdClaim = User.Claims
                .FirstOrDefault(c => c.Type == "sessionId")?.Value;
            if (sessionIdClaim == null ||
                !Guid.TryParse(sessionIdClaim, out var sessionId))
            {
                return Unauthorized(
                    ApiResponse<string>.CreateErrorResponse(
                        new List<string> { "جلسة غير صالحة" },
                        "فشل تسجيل الخروج"
                    )
                );
            }

            await _mediator.Send(
                new LogoutCommand(sessionId),
                cancellationToken);

            return Ok(
                ApiResponse<string>.CreateSuccessResponse(
                    string.Empty,
                    "تم تسجيل الخروج بنجاح"
                )
            );
        }
    }
}