using System.Net;
using System.Text.Json;
using FluentValidation;
using HomeTech.Application.Common;
using HomeTech.Application.Exceptions;

namespace HomeTech.API.Middleware
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;
        private static readonly JsonSerializerOptions _jsonOptions = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        };

        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (ValidationException ve)
            {
                var errors = ve.Errors
                    .Select(e => $"{e.PropertyName}: {e.ErrorMessage}")
                    .ToList();

                await WriteResponse(
                    context, HttpStatusCode.BadRequest,
                    ApiResponse<string>.CreateErrorResponse(
                        errors,
                        "خطأ في التحقق من البيانات"
                    )
                );
            }
            catch (BaseApplicationException ex)
            {
                await WriteResponse(context,
                    ex.StatusCode,
                    ApiResponse<string>.CreateErrorResponse(
                        new List<string> { ex.Message },
                        "حدث خطأ"
                    )
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled exception");

                await WriteResponse(context,
                    HttpStatusCode.InternalServerError,
                    ApiResponse<string>.CreateErrorResponse(
                        new List<string> { "خطأ غير متوقع" }
                    )
                );
            }
        }

        private async Task WriteResponse
        (
            HttpContext context, HttpStatusCode statusCode, ApiResponse<string> response
        )
        {
            if (context.Response.HasStarted) return;

            context.Response.StatusCode = (int)statusCode;
            context.Response.ContentType = "application/json";

            await context.Response.WriteAsync(
                JsonSerializer.Serialize(response, _jsonOptions)
            );
        }
    }
}