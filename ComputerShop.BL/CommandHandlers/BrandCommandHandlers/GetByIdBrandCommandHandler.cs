using ComputerShop.DL.Interfaces;
using ComputerShop.Models.MediatR.Commands.BrandCommands;
using ComputerShop.Models.Models;
using MediatR;

namespace ComputerShop.BL.CommandHandlers.BrandCommandHandlers
{
    public class GetByIdBrandCommandHandler : IRequestHandler<GetByIdBrandCommand, Brand?>
    {
        private readonly IBrandRepository brandRepository;

        public GetByIdBrandCommandHandler(IBrandRepository brandRepository)
        {
            this.brandRepository = brandRepository;
        }
        public async Task<Brand?> Handle(GetByIdBrandCommand request, CancellationToken cancellationToken)
        {
            return await brandRepository.GetById(request.BrandId);
        }
    }
}
