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
    [EnableRateLimiting("admin")]
    public class CategoriesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public CategoriesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("create-category")]
        [Authorize(Roles = Roles.Admin)]
        public async Task<IActionResult> Create(
            [FromBody] CreateCategoryDto dto, CancellationToken cancellationToken
        )
        {
            var result = await _mediator.Send(
                new CreateCategoryCommand(dto),
                cancellationToken
            );
            if (result == null)
            {
                return NotFound(
                    ApiResponse<string>.CreateErrorResponse(
                        new List<string> { "لم تتم العملية بنجاح" }
                        , "فشل انشاء الفئة"
                    )
                );
            }

            return Ok(
                ApiResponse<string>.CreateSuccessResponse(
                    result,
                    "تم انشاء الفئة بنجاح"
                )
            );
        }

        [HttpPut("update-category")]
        [Authorize(Roles = Roles.Admin)]
        public async Task<IActionResult> Update(
            [FromBody] UpdateCategoryDto dto, CancellationToken cancellationToken
        )
        {
            var result = await _mediator.Send(
                new UpdateCategoryCommand(dto),
                cancellationToken
            );
            if (!result)
            {
                return NotFound(
                    ApiResponse<bool>.CreateErrorResponse(
                        new List<string> { "لم تتم العملية بنجاح" },
                        "فشل تحديث الفئة"
                    )
                );
            }

            return Ok(
                ApiResponse<bool>.CreateSuccessResponse(
                    result,
                    "تم تحديث الفئة بنجاح"
                )
            );
        }

        [HttpDelete("Delete-Category")]
        [Authorize(Roles = Roles.Admin)]
        public async Task<IActionResult> Delete(
            [FromBody] DeleteCategoryDto dto, CancellationToken cancellationToken
        )
        {
            var result = await _mediator.Send(
                new DeleteCategoryCommand(dto), cancellationToken
            );
            if (!result)
            {
                return NotFound(
                    ApiResponse<bool>.CreateErrorResponse(
                        new List<string> { "عملية فاشلة" },
                        "لم يتم حذف الفئة بنجاح"
                    )
                );
            }

            return Ok(
                ApiResponse<bool>.CreateSuccessResponse(
                    result,
                    "تم حذف الفئة بنجاح"
                )
            );
        }

        [AllowAnonymous]
        [HttpGet("Get-All-Categories")]
        public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(
                new GetAllCategoriesQuery(), cancellationToken
            );
            if (!result.Any())
            {
                return NotFound(
                    ApiResponse<string>.CreateErrorResponse(
                        new List<string> { "عملية فاشلة" },
                        "لم يتم جلب الفئات بنجاح, حاول مرة اخري"
                    )
                );
            }

            return Ok(
                ApiResponse<List<CategoryDto>>.CreateSuccessResponse(
                    result,
                    "تم جلب الفئات بنجاح"
                )
            );
        }
    }
}