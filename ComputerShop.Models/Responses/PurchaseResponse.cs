using ComputerShop.Models.Models;

namespace ComputerShop.Models.Responses
{
    public class PurchaseResponse : BaseResponse
    {
        public Purchase? Purchase { get; set; }
    }
}
