using AutoMapper;
using ComputerShop.DL.Interfaces;
using ComputerShop.Models.MediatR.Commands.BrandCommands;
using ComputerShop.Models.Models;
using ComputerShop.Models.Responses;
using MediatR;
using System.Net;

namespace ComputerShop.BL.CommandHandlers.BrandCommandHandlers
{
    public class AddBrandCommandHandler : IRequestHandler<AddBrandCommand, BrandResponse>
    {
        private readonly IBrandRepository brandRepository;
        private readonly IMapper mapper;

        public AddBrandCommandHandler(IBrandRepository brandRepository,IMapper mapper)
        {
            this.brandRepository = brandRepository;
            this.mapper = mapper;
        }
        public async Task<BrandResponse> Handle(AddBrandCommand request, CancellationToken cancellationToken)
        {
            var brandCheck = await brandRepository.GetByName(request.Brand.Name);
            if (brandCheck != null)
            {
                return new BrandResponse()
                {
                    HttpStatusCode = HttpStatusCode.BadRequest,
                    Message = "Brand already exists",
                };
            }

            var brand = mapper.Map<Brand>(request.Brand);
            var result = await brandRepository.Add(brand);

            return new BrandResponse()
            {
                HttpStatusCode = HttpStatusCode.OK,
                Message="Successfully added brand",
                Brand = result,
            };
        }
    }
}
