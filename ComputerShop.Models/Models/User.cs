namespace ComputerShop.Models.Models
{
    public class User
    {
        public Guid Id { get; set; }

        public string UserName { get; set; }

        public int Age { get; set; }

        public string Email { get; set; }

        public string Password { get; set; }

        public string Address { get; set; }

        public string Role { get; set; }
    }
}
