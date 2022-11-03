using ComputerShop.DL.Interfaces;
using ComputerShop.Models.MediatR.Commands;
using ComputerShop.Models.Models;
using MediatR;

namespace ComputerShop.BL.CommandHandlers
{
    public class GetByNameComputerCommandHandler : IRequestHandler<GetByNameComputerCommand,Computer?>
    {
        private readonly IComputerRepository computerRepository;

        public GetByNameComputerCommandHandler(IComputerRepository computerRepository)
        {
            this.computerRepository = computerRepository;
        }
        public async Task<Computer?> Handle(GetByNameComputerCommand request, CancellationToken cancellationToken)
        {
            return await computerRepository.GetByName(request.Name);
        }
    }
}
