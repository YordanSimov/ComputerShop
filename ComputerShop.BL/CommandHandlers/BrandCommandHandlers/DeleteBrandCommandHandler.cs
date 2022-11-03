using ComputerShop.DL.Interfaces;
using ComputerShop.Models.MediatR.Commands.BrandCommands;
using ComputerShop.Models.Responses;
using MediatR;
using System.Net;

namespace ComputerShop.BL.CommandHandlers.BrandCommandHandlers
{
    public class DeleteBrandCommandHandler : IRequestHandler<DeleteBrandCommand, BrandResponse>
    {
        private readonly IBrandRepository brandRepository;

        public DeleteBrandCommandHandler(IBrandRepository brandRepository)
        {
            this.brandRepository = brandRepository;
        }
        public async Task<BrandResponse> Handle(DeleteBrandCommand request, CancellationToken cancellationToken)
        {
            var brandCheck = await brandRepository.GetById(request.BrandId);
            if (brandCheck == null)
            {
                return new BrandResponse()
                {
                    HttpStatusCode = HttpStatusCode.NotFound,
                    Message = "Brand to delete not found",
                };
            }
            var deletedBrand = await brandRepository.Delete(request.BrandId);

            return new BrandResponse()
            {
                HttpStatusCode = HttpStatusCode.OK,
                Message = "Successfullt deleted brand",
                Brand = deletedBrand,
            };
        }
    }
}
