using CRMUKMTPApi.Models;
using MT5LIB.Models;
using Org.BouncyCastle.Asn1.Ocsp;

namespace CRMUKMTPApi.Repositories
{
    public interface IUserRepository
    {
        Task<bool> AddAsync(IEnumerable<ManagerUser> users);
        Task<bool> AddAsync(ManagerUser user);
        Task<ManagerUser?> GetAsync(ulong loginId);
        Task<List<ManagerUser>?> GetAsync();
        Task<bool> UpdateAsync(ManagerUser user);
        Task<bool> AddOrUpdateUsersAsync(IEnumerable<ManagerUser> users);
        Task<(List<ManagerUser>, int, bool)> GetAsync(ParamModel param);
        Task<List<ManagerUser>?> GetAsync(List<ulong> loginIds);

    }
}