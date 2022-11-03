using AutoMapper;
using ComputerShop.DL.Interfaces;
using ComputerShop.Models.MediatR.Commands;
using ComputerShop.Models.Models;
using ComputerShop.Models.Responses;
using MediatR;
using System.Net;

namespace ComputerShop.BL.CommandHandlers
{
    public class AddComputerCommandHandler : IRequestHandler<AddComputerCommand, ComputerResponse>
    {
        private readonly IComputerRepository computerRepository;
        private readonly IMapper mapper;
        private readonly IBrandRepository brandRepository;

        public AddComputerCommandHandler(IComputerRepository computerRepository, IMapper mapper,IBrandRepository brandRepository)
        {
            this.computerRepository = computerRepository;
            this.mapper = mapper;
            this.brandRepository = brandRepository;
        }
        public async Task<ComputerResponse> Handle(AddComputerCommand request, CancellationToken cancellationToken)
        {
            var computerCheck = await computerRepository.GetByName(request.Computer.Name);
            var brand = await brandRepository.GetById(request.Computer.BrandId);
            if (computerCheck != null)
            {
                return new ComputerResponse()
                {
                    HttpStatusCode = HttpStatusCode.BadRequest,
                    Message = "Computer already exists",
                };
            }

            if (brand == null)
            {
                return new ComputerResponse()
                {
                    HttpStatusCode = HttpStatusCode.NotFound,
                    Message = "Brand id not found",
                };
            }

            var computer = mapper.Map<Computer>(request.Computer);
            var result = await computerRepository.Add(computer);

            return new ComputerResponse()
            {
                HttpStatusCode = HttpStatusCode.OK,
                Message = "Successfully added computer",
                Computer = result,
            };
        }
    }
}
