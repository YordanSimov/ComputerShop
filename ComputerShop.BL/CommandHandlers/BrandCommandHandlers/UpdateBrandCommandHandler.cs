using AutoMapper;
using ComputerShop.DL.Interfaces;
using ComputerShop.Models.MediatR.Commands.BrandCommands;
using ComputerShop.Models.Models;
using ComputerShop.Models.Responses;
using MediatR;
using System.Net;

namespace ComputerShop.BL.CommandHandlers.BrandCommandHandlers
{
    public class UpdateBrandCommandHandler : IRequestHandler<UpdateBrandCommand, BrandResponse>
    {
        private readonly IMapper mapper;
        private readonly IBrandRepository brandRepository;

        public UpdateBrandCommandHandler(IMapper mapper,IBrandRepository brandRepository)
        {
            this.mapper = mapper;
            this.brandRepository = brandRepository;
        }
        public async Task<BrandResponse> Handle(UpdateBrandCommand request, CancellationToken cancellationToken)
        {
            var brandCheck = await brandRepository.GetById(request.Brand.Id);
            if (brandCheck == null) return new BrandResponse()
            {
                HttpStatusCode = HttpStatusCode.NotFound,
                Message = "Brand to update does not exist.",
            };

            var brand = mapper.Map<Brand>(request.Brand);
            await brandRepository.Update(brand);

            return new BrandResponse()
            {
                HttpStatusCode = HttpStatusCode.OK,
                Message = "Successfully updated brand",
                Brand = brand,
            };
        }
    }
}
