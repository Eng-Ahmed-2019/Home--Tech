using MediatR;
using System.Security.Claims;
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
            if (!result.Any())
            {
                return BadRequest(
                    ApiResponse<string>.CreateErrorResponse(
                        new List<string> { "عملية غير ناجحة" },
                        "فشل انشاء حساب"
                    )
                );
            }

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
            if (result == null)
            {
                return BadRequest(
                    ApiResponse<LoginResponseDto>.CreateErrorResponse(
                        new List<string> { "عملية غير ناجحة" },
                        "فشل تسجيل الدخول"
                    )
                );
            }

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

            var r = await _mediator.Send(
                new LogoutCommand(sessionId),
                cancellationToken);
            if (!r)
            {
                return BadRequest(
                    ApiResponse<bool>.CreateErrorResponse(
                        new List<string> { "عملية غير ناجحة" },
                        "فشل تسجيل الخروج"
                    )
                );
            }

            return Ok(
                ApiResponse<bool>.CreateSuccessResponse(
                    r,
                    "تم تسجيل الخروج بنجاح"
                )
            );
        }

        [Authorize]
        [HttpPut("profile")]
        [EnableRateLimiting("profile")]
        public async Task<IActionResult> Update(
            [FromBody] UpdateUserDto dto,
            CancellationToken cancellationToken
        )
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return Unauthorized(
                    ApiResponse<string>.CreateErrorResponse(
                        new List<string> { "عملية غير ناجحة" },
                        "فشل تعديل البيانات"
                    )
                );
            }

            var r = await _mediator.Send(
                new UpdateUserCommand(userId, dto), cancellationToken
            );
            if (r == null)
            {
                return Unauthorized(
                    ApiResponse<string>.CreateErrorResponse(
                        new List<string> { "عملية غير ناجحة" },
                        "فشل تعديل البيانات"
                    )
                );
            }

            return Ok(
                ApiResponse<string>.CreateSuccessResponse(
                    r,
                    "تم تعديل البيانات بنجاح"
                )
            );
        }

        [Authorize]
        [HttpDelete("account")]
        [EnableRateLimiting("delete")]
        public async Task<IActionResult> Delete(
            CancellationToken cancellationToken
        )
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return Unauthorized(
                    ApiResponse<string>.CreateErrorResponse(
                        new List<string> { "عملية غير ناجحة" },
                        "فشل حذف البيانات"
                    )
                );
            }

            var sessionId = Guid.Parse(
                User.Claims.First(c => c.Type == "sessionId").Value
            );
            if (string.IsNullOrEmpty(sessionId.ToString()))
            {
                return BadRequest(
                    ApiResponse<Guid>.CreateErrorResponse(
                        new List<string> { "عملية غير ناجحة" },
                        "فشل حذف البيانات"
                    )
                );
            }

            var r = await _mediator.Send(
                new DeleteAccountCommand(userId, sessionId), cancellationToken
            );
            if (!r.Any())
            {
                return BadRequest(
                    ApiResponse<string>.CreateErrorResponse(
                        new List<string> { "عملية غير ناجحة" },
                        "فشل حذف البيانات"
                    )
                );
            }

            return Ok(
                ApiResponse<string>.CreateSuccessResponse(
                    r,
                    "تم حذف البيانات بنجاح"
                )
            );
        }

        [Authorize]
        [HttpGet("confirm-email")]
        [EnableRateLimiting("email")]
        public async Task<IActionResult> ConfirmEmail(
        [FromQuery] string email,
        [FromQuery] string token,
        CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(
                new ConfirmEmailCommand(email, token),
                cancellationToken);
            if (!result.Any())
            {
                return BadRequest(
                    ApiResponse<string>.CreateErrorResponse(
                        new List<string> { "عملية غير ناجحة" },
                        "فشل طلب تغيير البريد"
                    )
                );
            }

            return Ok(ApiResponse<string>.CreateSuccessResponse(
                result,
                "تم التأكيد بنجاح"
            ));
        }

        [Authorize]
        [HttpPost("change-email")]
        [EnableRateLimiting("email")]
        public async Task<IActionResult> ChangeEmail(
        [FromBody] ChangeEmailDto dto,
        CancellationToken cancellationToken)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return BadRequest(
                    ApiResponse<string>.CreateErrorResponse(
                        new List<string> { "عملية غير ناجحة" },
                        "لم يتم تغيير البريد"
                    )
                );
            }

            var result = await _mediator.Send(
                new ChangeEmailCommand(userId!, dto.NewEmail),
                cancellationToken);
            if (!result.Any())
            {
                return BadRequest(
                    ApiResponse<string>.CreateErrorResponse(
                        new List<string> { "عملية غير ناجحة" },
                        "لم يتم تغيير البريد"
                    )
                );
            }

            return Ok(ApiResponse<string>.CreateSuccessResponse(
                result,
                "برجاء تأكيد البريد الجديد"
            ));
        }

        [Authorize]
        [HttpPost("reactivate")]
        [EnableRateLimiting("email")]
        public async Task<IActionResult> ReactivateAccount(
        [FromQuery] string email,
        CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(
                new ReactivateAccountCommand(email),
                cancellationToken);
            if (!result.Any())
            {
                return BadRequest(
                    ApiResponse<string>.CreateErrorResponse(
                        new List<string> { "عملية غير ناجحة" },
                        "فشل تفعيل الحساب"
                    )
                );
            }

            return Ok(ApiResponse<string>.CreateSuccessResponse(
                result,
                "تحقق من بريدك الإلكتروني"
            ));
        }

        [Authorize]
        [HttpPost("change-password")]
        [EnableRateLimiting("login")]
        public async Task<IActionResult> ChangePassword(
        [FromBody] ChangePasswordDto dto,
        CancellationToken cancellationToken)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return BadRequest(
                    ApiResponse<string>.CreateErrorResponse(
                        new List<string> { "عملية غير ناجحة" },
                        "فشل تغيير كلمة المرور"
                    )
                );
            }

            var result = await _mediator.Send(
                new ChangePasswordCommand(userId!, dto),
                cancellationToken);
            if (!result.Any())
            {
                return BadRequest(
                    ApiResponse<string>.CreateErrorResponse(
                        new List<string> { "عملية غير ناجحة" },
                        "فشل تغيير كلمة المرور"
                    )
                );
            }

            return Ok(ApiResponse<string>.CreateSuccessResponse(
                result,
                "تم التغيير بنجاح"
            ));
        }
    }
}