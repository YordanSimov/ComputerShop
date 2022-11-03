using ComputerShop.DL.Interfaces;
using ComputerShop.Models.MediatR.Commands;
using ComputerShop.Models.Models;
using MediatR;

namespace ComputerShop.BL.CommandHandlers
{
    public class GetByIdComputerCommandHandler : IRequestHandler<GetByIdComputerCommand, Computer?>
    {
        private readonly IComputerRepository computerRepository;

        public GetByIdComputerCommandHandler(IComputerRepository computerRepository)
        {
            this.computerRepository = computerRepository;
        }
        public async Task<Computer?> Handle(GetByIdComputerCommand request, CancellationToken cancellationToken)
        {
            return await computerRepository.GetById(request.ComputerId);
        }
    }
}
