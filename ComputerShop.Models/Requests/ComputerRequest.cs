namespace ComputerShop.Models.Requests
{
    public class ComputerRequest
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public int BrandId { get; set; }

        public decimal Price { get; set; }

        public string VideoCard { get; set; }

        public string Processor { get; set; }

        public int RAM { get; set; }
    }
}
