using WebApplication6.Models;

namespace WebApplication6.Middleware
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;

        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
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
                _logger.LogError(ex, ex.Message);
                await HandleExceptionAsync(context, ex);
            }
        }
        private async Task HandleExceptionAsync(HttpContext context, Exception ex)
        {
            var statusCode = ex switch
            {
                KeyNotFoundException => 404,
                ArgumentException => 400,
                UnauthorizedAccessException => 401,
                _ => 500
            };

            context.Response.StatusCode = statusCode;
            // временно — показвай пълната грешка включително InnerException
            var fullMessage = ex.InnerException != null
                ? $"{ex.Message} | INNER: {ex.InnerException.Message}"
                : ex.Message;

            var message = Uri.EscapeDataString(fullMessage);
            var path = Uri.EscapeDataString(context.Request.Path);
            context.Response.Redirect($"/Home/Error?status={statusCode}&message={message}&path={path}");
        }

    }
}
