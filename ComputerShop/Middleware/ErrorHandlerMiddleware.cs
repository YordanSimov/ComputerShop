using Newtonsoft.Json;
using System.Net;

namespace ComputerShop.Middleware
{
    public class ErrorHandlerMiddleware
    {
        private readonly RequestDelegate next;
        private readonly ILogger<ErrorHandlerMiddleware> logger;

        public ErrorHandlerMiddleware(RequestDelegate next, ILogger<ErrorHandlerMiddleware> logger)
        {
            this.next = next;
            this.logger = logger;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            try
            {
                await next(httpContext);
            }
            catch (Exception error)
            {
                var response = httpContext.Response;
                response.ContentType = "application/json";
                response.StatusCode = error switch
                {
                    AppException ex => (int)HttpStatusCode.BadRequest,//custom application error
                    KeyNotFoundException ex => (int)HttpStatusCode.NotFound,//not found error
                    _ => (int)HttpStatusCode.InternalServerError,//unhandled error
                };
                var result = JsonConvert.SerializeObject(new { message = error.Message });
                logger.LogError(result);
                await response.WriteAsync(result);
            }
        }
    }
}
