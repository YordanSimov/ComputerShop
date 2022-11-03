using ComputerShop.DL.Interfaces;
using ComputerShop.Models.MediatR.Commands;
using ComputerShop.Models.Responses;
using MediatR;
using System.Net;

namespace ComputerShop.BL.CommandHandlers
{
    public class DeleteComputerCommandHandler : IRequestHandler<DeleteComputerCommand, ComputerResponse>
    {
        private readonly IComputerRepository computerRepository;

        public DeleteComputerCommandHandler(IComputerRepository computerRepository)
        {
            this.computerRepository = computerRepository;
        }
        public async Task<ComputerResponse> Handle(DeleteComputerCommand request, CancellationToken cancellationToken)
        {
            var computerCheck = await computerRepository.GetById(request.ComputerId);
            if (computerCheck == null)
            {
                return new ComputerResponse()
                {
                    HttpStatusCode = HttpStatusCode.NotFound,
                    Message = "Computer to delete not found",
                };
            }
            var deletedComputer = await computerRepository.Delete(request.ComputerId);

            return new ComputerResponse()
            {
                HttpStatusCode = HttpStatusCode.OK,
                Message = "Successfullt deleted computer",
                Computer = deletedComputer,
            };
        }
    }
}
