namespace ComputerShop.Models.Models
{
    public class Report
    {
        public Guid Id { get; set; }

        public List<Purchase> Purchases { get; set; }

        public DateTime ReportTime { get; set; }
    }
}