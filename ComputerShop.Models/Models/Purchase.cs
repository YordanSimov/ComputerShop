using MessagePack;

namespace ComputerShop.Models.Models
{
    [MessagePackObject]
    public class Purchase
    {
        [Key(0)]
        public Guid Id { get; set; }

        [Key(1)]
        public int ComputerId { get; set; }

        [Key(2)]
        public int UserId { get; set; }

        [Key(3)]
        public decimal ComputerPrice { get; set; }

        [Key(4)]
        public string ComputerName { get; set; }
    }
}
