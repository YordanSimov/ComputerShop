using ComputerShop.DL.MongoRepositories;
using ComputerShop.Models.MediatR.Commands.IdentityCommands;
using ComputerShop.Models.Models;
using MediatR;
using System.Security.Cryptography;
using System.Text;

namespace ComputerShop.BL.CommandHandlers.IdentityCommandHandlers
{
    public class CheckUserAndPasswordCommandHandler : IRequestHandler<CheckUserAndPasswordCommand, User?>
    {
        private readonly IUserRepository userRepository;

        public CheckUserAndPasswordCommandHandler(IUserRepository userRepository)
        {
            this.userRepository = userRepository;
        }
        public async Task<User?> Handle(CheckUserAndPasswordCommand request, CancellationToken cancellationToken)
        {
            var user = await userRepository.GetUserByUserName(request.UserName);

            if (user == null)
                return null;

            var hashedPassword = HashPassword(request.Password);
            if (hashedPassword == user.Password)
            {
                return user;
            }
            return null;
        }

        private string HashPassword(string password)
        {
            using (var sha256Hash = SHA256.Create())
            {
                var bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(password));

                var stringbuilder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    stringbuilder.Append(bytes[i].ToString("x2"));
                }
                return stringbuilder.ToString();
            }
        }
    }
}
