using ComputerShop.DL.Interfaces;
using ComputerShop.Models.MediatR.Commands;
using ComputerShop.Models.Models;
using MediatR;

namespace ComputerShop.BL.CommandHandlers
{
    public class GetAllComputersCommandHandler : IRequestHandler<GetAllComputersCommand, IEnumerable<Computer>>
    {
        private readonly IComputerRepository computerRepository;

        public GetAllComputersCommandHandler(IComputerRepository computerRepository)
        {
            this.computerRepository = computerRepository;
        }
        public async Task<IEnumerable<Computer>> Handle(GetAllComputersCommand request, CancellationToken cancellationToken)
        {
            return await computerRepository.GetAll();
        }
    }
}
