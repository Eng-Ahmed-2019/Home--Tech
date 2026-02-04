using MediatR;
using HomeTech.Infrastructure;
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
    [EnableRateLimiting("services")]
    public class HomeTechServicesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public HomeTechServicesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("Create-Service")]
        [Authorize(Roles = Roles.Admin)]
        public async Task<IActionResult> Create(
            [FromBody] CreateServiceDto dto, CancellationToken cancellationToken
        )
        {
            var result = await _mediator.Send(
                new CreateHomeTechServiceCommand(dto), cancellationToken
            );
            if (result == null)
            {
                return NotFound(
                    ApiResponse<string>.CreateErrorResponse(
                        new List<string> { "لم تتم العملية بنجاح" }
                        , "فشل انشاء الخدمة"
                    )
                );
            }

            return Ok(
                ApiResponse<string>.CreateSuccessResponse(
                    result,
                    "تم انشاء الخدمة بنجاح"
                )
            );
        }

        [HttpPut("Update-service")]
        [Authorize(Roles = Roles.Admin)]
        public async Task<IActionResult> Update(
            [FromBody] UpdateServiceDto dto, CancellationToken cancellationToken
        )
        {
            var result = await _mediator.Send(
                new UpdateHomeTechServicesCommand(dto), cancellationToken
            );
            if (!result)
            {
                return NotFound(
                    ApiResponse<bool>.CreateErrorResponse(
                        new List<string> { "لم تتم العملية بنجاح" }
                        , "فشل تحديث الخدمة"
                    )
                );
            }

            return Ok(
                ApiResponse<bool>.CreateSuccessResponse(
                    result,
                    "تم تحديث الخدمة بنجاح"
                )
            );
        }

        [HttpDelete("Delete-Service")]
        [Authorize(Roles = Roles.Admin)]
        public async Task<IActionResult> Delete(
            [FromBody] DeleteServiceDto dto, CancellationToken cancellationToken
        )
        {
            var result = await _mediator.Send(
                new DeleteHomeTechServicesCommand(dto), cancellationToken
            );
            if (!result)
            {
                return NotFound(
                    ApiResponse<bool>.CreateErrorResponse(
                        new List<string> { "لم تتم العملية بنجاح" }
                        , "فشل حذف الخدمة"
                    )
                );
            }

            return Ok(
                ApiResponse<bool>.CreateSuccessResponse(
                    result,
                    "تم حذف الخدمة بنجاح"
                )
            );
        }

        [Authorize]
        [HttpGet("Get-All-Services")]
        public async Task<IActionResult> GetAllServices(
            CancellationToken cancellationToken
        )
        {
            var result = await _mediator.Send(
                new GetAllServicesQuery(), cancellationToken
            );
            if (!result.Any())
            {
                return Ok(
                    ApiResponse<List<ServicesDto>>.CreateErrorResponse(
                        new List<string> { "عملية غير ناجحة" },
                        "لم يتم جلب الخدمات بنجاح, حاول مرة اخري"
                    )
                );
            }

            return Ok(
                ApiResponse<List<ServicesDto>>.CreateSuccessResponse(
                    result,
                    "تم جلب الخدمات بنجاح"
                )
            );
        }
    }
}