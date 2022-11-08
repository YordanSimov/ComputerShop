using ComputerShop.Models.Models;

namespace ComputerShop.DL.MongoRepositories
{
    public interface IPurchaseRepository
    {
        Task<Purchase> AddPurchase(Purchase purchase);

        Task<IEnumerable<Purchase>> GetPurchasesByTime(DateTime time);

        Task<Purchase> GetPurchaseById(Guid id);

        Task UpdatePurchase(Purchase purchase);

        Task<IEnumerable<Purchase>> GetUserPurchases(Guid userId);
    }
}
