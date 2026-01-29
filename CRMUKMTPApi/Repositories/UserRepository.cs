using CRMUKMTPApi.Data;
using CRMUKMTPApi.Helpers;
using CRMUKMTPApi.Models;
using Microsoft.EntityFrameworkCore;
using MT5LIB.Models;
using System.Linq;
using System.Linq.Expressions;

namespace CRMUKMTPApi.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly ILogger<UserRepository> _logger;
        private readonly AppDBContext _dbContext;

        public UserRepository(ILogger<UserRepository> logger, AppDBContext dbContext)
        {
            _logger = logger;
            _dbContext = dbContext;
        }

        public async Task<bool> AddAsync(IEnumerable<ManagerUser> users)
        {
            try
            {
                var incomingUserIds = users.Select(o => o.LoginId).ToList();

                var existingUserIds = new HashSet<ulong>(await _dbContext.Users
                                           .Where(x => incomingUserIds.Contains(x.LoginId))
                                           .Select(x => x.LoginId)
                                           .ToListAsync());

                var newUsers = users.Where(o => !existingUserIds.Contains(o.LoginId)).ToList();
                if (newUsers.Any())
                {
                    await _dbContext.Users.AddRangeAsync(newUsers);
                    return await _dbContext.SaveChangesAsync() > 0;
                }
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error on adding User");
                return false;
            }
        }

        public async Task<bool> AddAsync(ManagerUser user)
        {
            try
            {
                await _dbContext.Users.AddAsync(user);
                return await _dbContext.SaveChangesAsync() > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error on adding User");
                return false;
            }
        }

        public async Task<ManagerUser?> GetAsync(ulong loginId)
        {
            try
            {
                return await _dbContext.Users.FindAsync(loginId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error on find user");
                return null;
            }
        }

        public async Task<List<ManagerUser>?> GetAsync(List<ulong> loginIds)
        {
            try
            {
                var baseQuery = _dbContext.Users.AsNoTracking().AsQueryable();
                if (loginIds.Count > 0)
                    baseQuery = baseQuery.Where(x => loginIds.Contains(x.LoginId));
                var users = await baseQuery
                       .ToListAsync();
                return users;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error on find user");
                return null;
            }
        }

        public async Task<List<ManagerUser>?> GetAsync()
        {
            try
            {
                return await _dbContext.Users.ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error on find user");
                return null;
            }
        }

        public async Task<bool> UpdateAsync(ManagerUser user)
        {
            try
            {
                var existUser = await _dbContext.Users.FindAsync(user.LoginId);
                if (existUser == null) return false;

                _dbContext.Users.Entry(existUser).CurrentValues.SetValues(user);
                return await _dbContext.SaveChangesAsync() > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error on update orders in db");
                return false;
            }
        }

        public async Task<bool> AddOrUpdateUsersAsync(IEnumerable<ManagerUser> users)
        {
            try
            {
                var userIds = users.Select(u => u.LoginId).ToList();

                // Fetch existing users
                var existingUsers = await _dbContext.Users
                    .Where(u => userIds.Contains(u.LoginId))
                    .ToListAsync();

                var existingUserIds = existingUsers.Select(u => u.LoginId).ToHashSet();

                var newUsers = users.Where(u => !existingUserIds.Contains(u.LoginId)).ToList();
                var usersToUpdate = users.Where(u => existingUserIds.Contains(u.LoginId)).ToList();

                if (newUsers.Any())
                    await _dbContext.Users.AddRangeAsync(newUsers);

                foreach (var userToUpdate in usersToUpdate)
                {
                    var existing = existingUsers.First(u => u.LoginId == userToUpdate.LoginId);

                    _dbContext.Users.Entry(existing).CurrentValues.SetValues(userToUpdate);
                }

                return await _dbContext.SaveChangesAsync() > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding/updating users in DB");
                return false;
            }

        }
        public async Task<(List<ManagerUser>, int, bool)> GetAsync(ParamModel param)
        {
            try
            {


                var baseQuery = _dbContext.Users.AsNoTracking().AsQueryable();

                if (!string.IsNullOrWhiteSpace(param.Loginid))
                {
                    var loginIds = Globals.GetLoginId(param.Loginid);
                    if (loginIds.Count > 0)
                        baseQuery = baseQuery.Where(x => loginIds.Contains(x.LoginId));
                    else
                        return (new List<ManagerUser>(), 0, true);
                }


                if (!string.IsNullOrWhiteSpace(param.Filter) && !string.IsNullOrWhiteSpace(param.Filtervalue))
                {
                    Expression<Func<ManagerUser, bool>>? lambda = Globals.FilterByExpression<ManagerUser>(param, _logger);
                    baseQuery = baseQuery.Where(lambda);
                }

                var totalCount = await baseQuery.CountAsync();

                if (!string.IsNullOrEmpty(param.Sort))
                {
                    var ordering = Globals.SortByExpression<ManagerUser>(param, _logger);
                    if (ordering != null)
                    {
                        baseQuery = ordering(baseQuery);
                    }
                }

                var users = await baseQuery
                    .Skip((param.Page - 1) * param.Limit)
                    .Take(param.Limit)
                    .ToListAsync();



                return (users, totalCount, true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error on find order with parameters {@Param}", param);
                return (new List<ManagerUser>(), 0, false);
            }
        }
    }
}
