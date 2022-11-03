using ComputerShop.Models.Models;
using ComputerShop.Models.Requests;

namespace ComputerShop.DL.MongoRepositories
{
    public interface IPurchaseRepository
    {
        Task<Purchase> AddPurchase(PurchaseRequest purchaseRequest);
    }
}
