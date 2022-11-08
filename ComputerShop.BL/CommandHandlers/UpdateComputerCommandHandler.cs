using AutoMapper;
using ComputerShop.DL.Interfaces;
using ComputerShop.Models.MediatR.Commands;
using ComputerShop.Models.Models;
using ComputerShop.Models.Responses;
using MediatR;
using System.Net;

namespace ComputerShop.BL.CommandHandlers
{
    public class UpdateComputerCommandHandler : IRequestHandler<UpdateComputerCommand, ComputerResponse>
    {
        private readonly IMapper mapper;
        private readonly IComputerRepository computerRepository;
        private readonly IBrandRepository brandRepository;

        public UpdateComputerCommandHandler(IMapper mapper,
            IComputerRepository computerRepository,
            IBrandRepository brandRepository)
        {
            this.mapper = mapper;
            this.computerRepository = computerRepository;
            this.brandRepository = brandRepository;
        }
        public async Task<ComputerResponse> Handle(UpdateComputerCommand request, CancellationToken cancellationToken)
        {
            var computerCheck = await computerRepository.GetByName(request.Computer.Name);
            if (computerCheck == null) return new ComputerResponse()
            {
                HttpStatusCode = HttpStatusCode.NotFound,
                Message = "Computer to update does not exist.",
            };

            var brandCheck = await brandRepository.GetById(request.Computer.BrandId);
            if (brandCheck == null) return new ComputerResponse()
            {
                HttpStatusCode = HttpStatusCode.NotFound,
                Message = "Brand id to update does not exist.",
            };

            var computer = mapper.Map<Computer>(request.Computer);
            await computerRepository.Update(computer);

            return new ComputerResponse()
            {
                HttpStatusCode = HttpStatusCode.OK,
                Message = "Successfully updated computer",
                Computer = computer,
            };
        }
    }
}
