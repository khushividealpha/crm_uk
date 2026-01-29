using CRMUKMTPApi.Data;
using Microsoft.EntityFrameworkCore;
using MT5LIB.Models;

namespace CRMUKMTPApi.Repositories
{
    public class LastTradeTimeRepository : ILastTradeTimeRepository
    {
        private readonly ILogger<LastTradeTimeRepository> _logger;
        private readonly AppDBContext _dbContext;

        public LastTradeTimeRepository(ILogger<LastTradeTimeRepository> logger, AppDBContext dbContext)
        {
            _logger = logger;
            _dbContext = dbContext;
        }

        public async Task<Dictionary<ulong,DateTime>> GetAsync(List<ulong> loginId)
        {
            try
            {
                var result = await _dbContext.Deals
                .Where(d => loginId.Contains(d.LoginId))
                .GroupBy(d => d.LoginId)
                .Select(g => new
                {
                    LoginId = g.Key,
                    OldestTime = g.Min(x => x.Time)
                })
                .ToDictionaryAsync(x => x.LoginId, x => x.OldestTime);
                return result;
            }
            catch(Exception ex)
            {
                return null;
            }
        }
    }
}
