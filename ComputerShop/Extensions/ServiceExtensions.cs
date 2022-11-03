using ComputerShop.BL.Kafka;
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

        public static IServiceCollection RegisterKafka<TKey,TValue>(this IServiceCollection services)
        {
            services.AddSingleton<IKafkaProducerService<TKey,TValue>, KafkaProducerService<TKey,TValue>>();

            return services;
        }
    }
}
