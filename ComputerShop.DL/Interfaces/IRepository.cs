namespace ComputerShop.DL.Interfaces
{
    public interface IRepository<T> where T : class
    {
        Task<IEnumerable<T>> GetAll();

        Task<T?> GetById(int id);

        Task<T> Add(T input);

        Task Update(T input);

        Task<T?> Delete(int id);

        Task<T?> GetByName(string name);
    }
}
