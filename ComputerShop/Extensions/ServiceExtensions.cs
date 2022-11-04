using ComputerShop.BL.Dataflow;
using ComputerShop.BL.Kafka;
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

            return services;
        }

        public static IServiceCollection RegisterKafka<TKey, TValue>(this IServiceCollection services)
        {
            services.AddSingleton<IKafkaProducerService<TKey, TValue>, KafkaProducerService<TKey, TValue>>();

            return services;
        }
        public static IServiceCollection RegisterHostedService(this IServiceCollection services)
        {
            services.AddHostedService<PurchaseDataflow>();

            return services;
        }
    }
}
