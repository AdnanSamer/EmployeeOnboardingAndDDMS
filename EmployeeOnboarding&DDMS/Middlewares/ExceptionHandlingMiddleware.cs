using EmployeeOnboarding_DDMS.Aplication.Wrappers;
using System.Net;
using System.Text.Json;

namespace EmployeeOnboarding_DDMS.Middlewares
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unhandled exception occurred.");
                await HandleExceptionAsync(context, ex);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            var code = HttpStatusCode.InternalServerError;
            var result = string.Empty;

            switch (exception)
            {
                case FileNotFoundException:
                    code = HttpStatusCode.NotFound;
                    result = JsonSerializer.Serialize(new Response<object>
                    {
                        Succeeded = false,
                        Message = exception.Message
                    });
                    break;
                case UnauthorizedAccessException:
                    code = HttpStatusCode.Unauthorized;
                    result = JsonSerializer.Serialize(new Response<object>
                    {
                        Succeeded = false,
                        Message = "Unauthorized access."
                    });
                    break;
                default:
                    // Include inner exception details for database errors
                    var errorMessages = new List<string> { exception.Message };
                    if (exception.InnerException != null)
                    {
                        errorMessages.Add($"Inner Exception: {exception.InnerException.Message}");
                    }
                    
                    // Check for DbUpdateException (Entity Framework errors)
                    if (exception is Microsoft.EntityFrameworkCore.DbUpdateException dbEx)
                    {
                        errorMessages.Add($"Database Error: {dbEx.InnerException?.Message ?? dbEx.Message}");
                    }

                    result = JsonSerializer.Serialize(new Response<object>
                    {
                        Succeeded = false,
                        Message = "An error occurred while processing your request.",
                        Errors = errorMessages
                    });
                    break;
            }

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)code;
            return context.Response.WriteAsync(result);
        }
    }
}

