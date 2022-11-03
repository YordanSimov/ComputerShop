using ComputerShop.DL.Interfaces;
using ComputerShop.Models.MediatR.Commands.BrandCommands;
using ComputerShop.Models.Models;
using MediatR;

namespace ComputerShop.BL.CommandHandlers.BrandCommandHandlers
{
    public class GetByNameBrandCommandHandler : IRequestHandler<GetByNameBrandCommand, Brand?>
    {
        private readonly IBrandRepository brandRepository;

        public GetByNameBrandCommandHandler(IBrandRepository brandRepository)
        {
            this.brandRepository = brandRepository;
        }
        public async Task<Brand?> Handle(GetByNameBrandCommand request, CancellationToken cancellationToken)
        {
            return await brandRepository.GetByName(request.Name);
        }
    }
}
