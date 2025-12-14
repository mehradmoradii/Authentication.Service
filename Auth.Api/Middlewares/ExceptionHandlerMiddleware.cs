using Auth.Infrastracture.ErrorHandler;

namespace Auth.Api.Middlewares
{
    public class ExceptionHandlerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlerMiddleware> _logger;

        public ExceptionHandlerMiddleware(RequestDelegate next, ILogger<ExceptionHandlerMiddleware> logger)
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
            catch (AppException appEx)
            {
                // Log custom error
                _logger.LogWarning(appEx, appEx.Message);

                context.Response.StatusCode = 400;
                context.Response.ContentType = "application/json";

                await context.Response.WriteAsJsonAsync(new
                {
                    status = 400,
                    error = appEx.ErrorMessage
                });
            }
            catch (Exception ex)
            {
                // Log internal server error
                _logger.LogError(ex, ex.Message);

                context.Response.StatusCode = 500;
                context.Response.ContentType = "application/json";

                await context.Response.WriteAsJsonAsync(new
                {
                    status = 500,
                    error = "Internal server error",
                    detail = ex.Message
                });
            }
        }
    }
}
