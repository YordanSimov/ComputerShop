using ComputerShop.Models.Models;

namespace ComputerShop.DL.MongoRepositories
{
    public interface IReportRepository
    {
        Task<Report> CreateReportByTime(Report report);

        Task<IEnumerable<Report>> GetAllReports(DateTime? time);
    }
}