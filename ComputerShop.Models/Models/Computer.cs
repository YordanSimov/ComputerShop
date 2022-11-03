namespace ComputerShop.Models.Models
{
    public class Computer
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
