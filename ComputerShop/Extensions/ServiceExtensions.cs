using ComputerShop.BL.Dataflow;
using ComputerShop.DL.Interfaces;
using ComputerShop.DL.MongoRepositories;
using ComputerShop.DL.Repositories;

namespace ComputerShop.Extensions
{
    public static class ServiceExtensions
    {
        public static IServiceCollection RegisterRepositories(this IServiceCollection services)
        {
            services.AddSingleton<IComputerRepository, ComputerRepository>();
            services.AddSingleton<IBrandRepository, BrandRepository>();
            services.AddSingleton<IPurchaseRepository, PurchaseRepository>();
            services.AddSingleton<IReportRepository, ReportRepository>();

            return services;
        }

        public static IServiceCollection RegisterHostedService(this IServiceCollection services)
        {
            services.AddHostedService<PurchaseDataflow>();
            services.AddHostedService<DeliveryInfoDataflow>();

            return services;
        }
    }
}
