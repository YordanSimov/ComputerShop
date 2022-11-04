using MessagePack;

namespace ComputerShop.Models.Models
{
    [MessagePackObject]
    public class DeliveryInfo
    {
        [Key(1)]
        public Guid Id { get; set; }

        [Key(2)]
        public string Address { get; set; }

        [Key(3)]
        public Guid PurchaseId { get; set; }
    }
}
