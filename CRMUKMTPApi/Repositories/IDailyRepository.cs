using CRMUKMTPApi.Models;
using MT5LIB.Models;

namespace CRMUKMTPApi.Repositories
{
    public interface IDailyRepository
    {
        Task<bool> AddAsync(IEnumerable<ManagerDailyReport> dailyReports);
        Task<bool> AddAsync(ManagerDailyReport daily);
        Task<bool> DeleteAsync(ManagerDailyReport deal);
        Task<IEnumerable<ManagerDailyReport>?> GetAsync();
        Task<ManagerDailyReport?> GetAsync(int id);
        Task<(List<ManagerDailyReport>, int, bool)> GetAsync(ParamModel param);
        Task<List<ManagerDailyReport>?> GetByUserAsync(ulong loginId);
        Task<DateTime> GetMaxTime();
        Task<bool> UpdateAsync(ManagerDailyReport daily);
    }
}