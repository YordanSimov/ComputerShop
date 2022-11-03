using ComputerShop.DL.Interfaces;
using ComputerShop.DL.Repositories;

namespace ComputerShop.Extensions
{
    public static class ServiceExtensions
    {
        public static IServiceCollection RegisterRepositories(this IServiceCollection services)
        {
            services.AddSingleton<IComputerRepository, ComputerRepository>();
            services.AddSingleton<IBrandRepository, BrandRepository>();

            return services;
        }
    }
}
