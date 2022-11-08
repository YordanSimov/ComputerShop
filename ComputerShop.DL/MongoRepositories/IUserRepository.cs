using ComputerShop.Models.Models;
using Microsoft.AspNetCore.Identity;

namespace ComputerShop.DL.MongoRepositories
{
    public interface IUserRepository
    {
        Task<IdentityResult> CreateUser(User user,CancellationToken cancellationToken);

        Task<User?> GetUserById(Guid userId);

        Task<User?> GetUserByUserName(string username);

        Task<IEnumerable<User>> GetAllUsers();
    }
}