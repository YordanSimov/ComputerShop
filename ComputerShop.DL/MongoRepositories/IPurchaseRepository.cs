using ComputerShop.Models.Models;

namespace ComputerShop.DL.MongoRepositories
{
    public interface IPurchaseRepository
    {
        Task<Purchase> AddPurchase(Purchase purchase);
    }
}
