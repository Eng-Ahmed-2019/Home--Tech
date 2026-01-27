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
    [Authorize(Roles = Roles.Admin)]
    public class LocationsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public LocationsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [Authorize]
        [HttpGet("main-locations")]
        public async Task<IActionResult> GetMain(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            CancellationToken ct = default)
        {
            var result = await _mediator.Send(
                new GetMainLocationsQuery(page, pageSize), ct);

            return Ok(ApiResponse<PagedResult<LocationDto>>
                .CreateSuccessResponse(result));
        }

        [Authorize]
        [HttpGet("{parentId}/sub-locations")]
        public async Task<IActionResult> GetSub(
            int parentId,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            CancellationToken ct = default)
        {
            var result = await _mediator.Send(
                new GetSubLocationsQuery(parentId, page, pageSize), ct);

            return Ok(ApiResponse<PagedResult<LocationDto>>
                .CreateSuccessResponse(result));
        }

        [HttpPost("create-locations")]
        public async Task<IActionResult> Create([FromBody] CreateLocationDto dto, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new CreateLocationCommand(dto), cancellationToken);
            if (result == null)
            {
                return NotFound(
                    ApiResponse<string>.CreateErrorResponse(
                        new List<string> { "لم تتم العملية بنجاح" }
                        , "فشل انشاء الموقع"
                    )
                );
            }

            return Ok(
                ApiResponse<string>.CreateSuccessResponse(
                    result,
                    "تم انشاء الموقع بنجاح"
                )
            );
        }

        [HttpPut("update-location")]
        public async Task<IActionResult> Update([FromBody] UpdateLocationDto dto, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new UpdateLocationCommand(dto), cancellationToken);
            if (!result)
            {
                return NotFound(
                    ApiResponse<bool>.CreateErrorResponse(
                        new List<string> { "لم تتم العملية بنجاح" },
                        "فشل تحديث الموقع"
                    )
                );
            }

            return Ok(
                ApiResponse<bool>.CreateSuccessResponse(
                    result,
                    "تم تحديث الموقع بنجاح"
                )
            );
        }

        [HttpDelete("Delete-location")]
        public async Task<IActionResult> Delete([FromBody] DeleteLocationDto dto, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new DeleteLocationCommand(dto), cancellationToken);
            if (!result)
            {
                return NotFound(
                    ApiResponse<bool>.CreateErrorResponse(
                        new List<string> { "عملية فاشلة" },
                        "لم يتم حذف الموقع بنجاح"
                    )
                );
            }
            return Ok(
                ApiResponse<bool>.CreateSuccessResponse(
                    result,
                    "تم حذف الموقع بنجاح"
                )
            );
        }

        [AllowAnonymous]
        [HttpGet("Get-All-Locations")]
        public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new GetAllLocationsQuery(), cancellationToken);
            if (result == null)
            {
                return NotFound(
                    ApiResponse<string>.CreateErrorResponse(
                        new List<string> { "عملية فاشلة" },
                        "لم يتم جلب المواقع بنجاح, حاول مرة اخري"
                    )
                );
            }

            return Ok(
                ApiResponse<List<LocationDto>>.CreateSuccessResponse(
                    result,
                    "تم جلب المواقع بنجاح"
                )
            );
        }
    }
}