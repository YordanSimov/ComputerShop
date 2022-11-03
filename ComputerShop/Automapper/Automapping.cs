using AutoMapper;
using ComputerShop.Models.Models;
using ComputerShop.Models.Requests;

namespace ComputerShop.Automapper
{
    public class Automapping : Profile
    {
        public Automapping()
        {
            CreateMap<ComputerRequest, Computer>();
            CreateMap<BrandRequest, Brand>();
            CreateMap<PurchaseRequest, Purchase>();
        }
    }
}
