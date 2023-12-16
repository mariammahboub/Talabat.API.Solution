using System.Net;
using System.Text.Json;
using Talabat.API.Errors;

namespace Talabat.API.Middlewares
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;
        private readonly IHostEnvironment _env;

        public ExceptionMiddleware(RequestDelegate next , ILogger<ExceptionMiddleware> logger , IHostEnvironment env)
        {
            _next = next;
            _logger = logger;
            _env = env;
        }
        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next.Invoke(context);
            }
            catch(Exception ex) 
            {
                _logger.LogError(ex , ex.Message);
                // Log Exception in db [production]
                context.Response.ContentType = "application/json";            // Header of Response
                context.Response.StatusCode = (int) HttpStatusCode.InternalServerError;    // Header of Response

                var response = _env.IsDevelopment() ? new ApiExceptionResponse((int)HttpStatusCode.InternalServerError, ex.Message, ex.StackTrace.ToString()) 
                    : new ApiExceptionResponse((int)HttpStatusCode.InternalServerError);
                var options = new JsonSerializerOptions() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
                var json = JsonSerializer.Serialize(response, options);
                await context.Response.WriteAsync(json);    
            }
        }
    }
}
