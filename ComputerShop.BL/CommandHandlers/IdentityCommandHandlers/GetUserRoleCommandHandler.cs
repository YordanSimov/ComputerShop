using ComputerShop.DL.MongoRepositories;
using ComputerShop.Models.MediatR.Commands.IdentityCommands;
using MediatR;

namespace ComputerShop.BL.CommandHandlers.IdentityCommandHandlers
{
    public class GetUserRoleCommandHandler : IRequestHandler<GetUserRoleCommand, string>
    {
        private readonly IUserRepository userRepository;

        public GetUserRoleCommandHandler(IUserRepository userRepository)
        {
            this.userRepository = userRepository;
        }
        public async Task<string> Handle(GetUserRoleCommand request, CancellationToken cancellationToken)
        {
            var result = await userRepository.GetUserByUserName(request.UserName);
            if (result == null)
            {
                return "User does not exist";
            }
            return result.Role;
        }
    }
}
