using ComputerShop.DL.MongoRepositories;
using ComputerShop.Models.MediatR.Commands.UserCommands;
using ComputerShop.Models.Models;
using MediatR;

namespace ComputerShop.BL.CommandHandlers.UserCommandHandlers
{
    public class GetAllUsersCommandHandler : IRequestHandler<GetAllUsersCommand, IEnumerable<User>>
    {
        private readonly IUserRepository userRepository;

        public GetAllUsersCommandHandler(IUserRepository userRepository)
        {
            this.userRepository = userRepository;
        }
        public async Task<IEnumerable<User>> Handle(GetAllUsersCommand request, CancellationToken cancellationToken)
        {
            return await userRepository.GetAllUsers();
        }
    }
}
