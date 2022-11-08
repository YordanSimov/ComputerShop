namespace ComputerShop.Models.Requests
{
    public class PurchaseRequest
    {
        public int ComputerId { get; set; }

        public Guid UserId { get; set; }
    }
}
