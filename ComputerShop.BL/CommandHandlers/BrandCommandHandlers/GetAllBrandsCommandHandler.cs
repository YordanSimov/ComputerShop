using ComputerShop.DL.Interfaces;
using ComputerShop.Models.MediatR.Commands.BrandCommands;
using ComputerShop.Models.Models;
using MediatR;

namespace ComputerShop.BL.CommandHandlers.BrandCommandHandlers
{
    public class GetAllBrandsCommandHandler : IRequestHandler<GetAllBrandsCommand, IEnumerable<Brand>>
    {
        private readonly IBrandRepository brandRepository;

        public GetAllBrandsCommandHandler(IBrandRepository brandRepository)
        {
            this.brandRepository = brandRepository;
        }
        public async Task<IEnumerable<Brand>> Handle(GetAllBrandsCommand request, CancellationToken cancellationToken)
        {
            return await brandRepository.GetAll();
        }
    }
}
