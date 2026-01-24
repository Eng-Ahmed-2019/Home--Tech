using System.Net;
using System.Text.Json;
using FluentValidation;
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
                await WriteResponse(
                    context, HttpStatusCode.BadRequest,
                    new
                    {
                        success = false,
                        errors = ve.Errors.Select(e => new
                        {
                            field = e.PropertyName,
                            error = e.ErrorMessage
                        })
                    }
                );
            }
            catch (BaseApplicationException ex)
            {
                await WriteResponse(context,
                    ex.StatusCode,
                    new
                    {
                        success = false,
                        message = ex.Message
                    }
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled exception");

                await WriteResponse(context,
                    HttpStatusCode.InternalServerError,
                    new
                    {
                        success = false,
                        message = "حدث خطأ غير متوقع",
                        traceId = context.TraceIdentifier
                    }
                );
            }
        }

        private async Task WriteResponse
        (
            HttpContext context, HttpStatusCode statusCode, object response
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