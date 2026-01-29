using MT5LIB.Models;

namespace CRMUKMTPApi.Repositories
{
    public interface ILastTradeTimeRepository
    {
        Task<Dictionary<ulong, DateTime>> GetAsync(List<ulong> loginId);

    }
}
