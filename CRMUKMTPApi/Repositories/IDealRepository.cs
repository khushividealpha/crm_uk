using CRMUKMTPApi.Models;
using MT5LIB.Models;

namespace CRMUKMTPApi.Repositories
{
    public interface IDealRepository
    {
        Task<bool> AddAsync(IEnumerable<ManagerDeal> deals);
        Task<bool> AddAsync(ManagerDeal deal);
        Task<(List<ManagerDeal>, int, bool)> GetAsync(ParamModel param,string symbols="");
        Task<ManagerDeal?> GetAsync(ulong dealId);
        Task<List<ManagerDeal>?> GetByUserAsync(ulong dealId);
        Task<bool> UpdateAsync(ManagerDeal deal);
        Task<DateTime> GetMaxTime();
        Task<bool> AddOrUpdateUsersAsync(IEnumerable<ManagerDeal> deals);
        Task<bool> DeleteAsync(ManagerDeal deal);
        Task<IEnumerable<ManagerDeal>?> GetAsync();
        Task<(List<TradeDataModel>, int, bool)> GetTradeDataAsync(ParamModel @params);
        Task<(List<TradeSummary>, int, bool)> GetTradeSummaryDataAsync(ParamModel param);
    }
}