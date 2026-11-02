using System.Net;

namespace TheLibrary_PresentationLayer.Middleware
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        //private readonly ILogger<ExceptionMiddleware> _logger;

        public ExceptionHandlingMiddleware(RequestDelegate next)
        {
            _next = next;
            //_logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                // 1. Intercepción de la solicitud (REQUEST)
                await _next(context); // 1. Intercepción
                // 2. Intercepción de la respuesta (RESPONSE)
            }
            catch (Exception ex)
            {
                //_logger.LogError(ex, "Ocurrió un error inesperado."); // 2. Log
                await HandleExceptionAsync(context, ex); // 3. Manejo
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception ex)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            var response = new { statusCode = context.Response.StatusCode, message = ex.Message };
            return context.Response.WriteAsJsonAsync(response);
        }

    }
}
