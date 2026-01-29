using CRMUKMTPApi.Models;
using MT5LIB.Models;

namespace CRMUKMTPApi.Repositories
{
    public interface IOrderRepository
    {
        Task<bool> AddAsync(IEnumerable<ManagerOrder> orders);
        Task<bool> AddAsync(ManagerOrder order);
        Task<(List<ManagerOrder>, int, bool)> GetAsync(ParamModel param);
        Task<ManagerOrder?> GetAsync(ulong orderId);
        Task<List<ManagerOrder>?> GetByUserAsync(ulong loginId);
        Task<bool> UpdateAsync(ManagerOrder order);
        Task<DateTime> GetMaxTime();
        Task<bool> DeleteAsync(ManagerOrder order);
        Task<bool> AddOrUpdateUsersAsync(IEnumerable<ManagerOrder> orders);
    }
}