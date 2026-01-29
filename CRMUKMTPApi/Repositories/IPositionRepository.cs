using CRMUKMTPApi.Models;
using MT5LIB.Models;

namespace CRMUKMTPApi.Repositories
{
    public interface IPositionRepository
    {
        Task<bool> AddAsync(IEnumerable<ManagerPosition> psitions);
        Task<bool> AddAsync(ManagerPosition position);
        Task<(List<ManagerPosition>, int, bool)> GetAsync(ParamModel param);
        Task<ManagerPosition?> GetAsync(ulong positionId);
        Task<List<ManagerPosition>?> GetByUserAsync(ulong positionId);
        Task<bool> UpdateAsync(ManagerPosition position);
        Task<DateTime> GetMaxTime();
        Task<bool> AddOrUpdateUsersAsync(IEnumerable<ManagerPosition> positions);
    }
}