using CRMUKMTPApi.Models;
using MT5LIB.Models;

namespace CRMUKMTPApi.Repositories
{
    public interface ISummaryRepository
    {
        Task<bool> AddAsync(IEnumerable<ManagerSummaryReport> summaries);
        Task<bool> AddAsync(ManagerSummaryReport summary);
        Task<ManagerSummaryReport?> GetAsync(int loginId);
        Task<(List<ManagerSummaryReport>, int, bool)> GetAsync(ParamModel param);
        Task<List<ManagerSummaryReport>?> GetByUserAsync(ulong loginId);
        Task<DateTime> GetMaxTime();
        Task<bool> UpdateAsync(ManagerSummaryReport summary);
        Task<bool> AddOrUpdateUsersAsync(IEnumerable<ManagerSummaryReport> summaries);
        Task<List<ManagerSummaryReport>?> GetAsync();
    }
}