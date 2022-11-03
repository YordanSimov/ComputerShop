using System.Net;

namespace ComputerShop.Models.Responses
{
    public class BaseResponse
    {
        public HttpStatusCode HttpStatusCode { get; set; }

        public string Message { get; set; }
    }
}