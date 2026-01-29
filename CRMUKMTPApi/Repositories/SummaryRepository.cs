using CRMUKMTPApi.Data;
using CRMUKMTPApi.Helpers;
using CRMUKMTPApi.Models;
using Microsoft.EntityFrameworkCore;
using MT5LIB.Models;
using MySql.Data.MySqlClient;
using System.Collections.Immutable;
using System.Data;
using System.Linq.Expressions;

namespace CRMUKMTPApi.Repositories;

public class SummaryRepository : ISummaryRepository
{
    private readonly ILogger<SummaryRepository> _logger;
    private readonly AppDBContext _dbContext;

    public SummaryRepository(ILogger<SummaryRepository> logger, AppDBContext dbContext)
    {
        _logger = logger;
        _dbContext = dbContext;
    }
    public async Task<bool> AddAsync(ManagerSummaryReport summary)
    {
        try
        {
            await _dbContext.SummaryReports.AddAsync(summary);
            return await _dbContext.SaveChangesAsync() > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error add summary in db");
            return false;
        }
    }
    public async Task<bool> AddAsync(IEnumerable<ManagerSummaryReport> summaries)
    {
        try
        {
            var incomingPositionNumbers = summaries.Select(o => o.LoginId).ToList();

            var existingSummaryNumbers = new HashSet<ulong>(await _dbContext.SummaryReports
                                       .Where(x => incomingPositionNumbers.Contains(x.LoginId))
                                       .Select(x => x.LoginId)
                                       .ToListAsync());

            var newSummary = summaries.Where(o => !existingSummaryNumbers.Contains(o.LoginId)).ToList();
            if (newSummary.Any())
            {
                await _dbContext.SummaryReports.AddRangeAsync(newSummary);
                return await _dbContext.SaveChangesAsync() > 0;
            }
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error add summaries in db");
            return false;
        }
    }
    public async Task<bool> UpdateAsync(ManagerSummaryReport summary)
    {
        try
        {
            var existing = await _dbContext.SummaryReports.FindAsync(summary.LoginId);
            if (existing == null) return false;

            _dbContext.SummaryReports.Entry(existing).CurrentValues.SetValues(summary);
            return await _dbContext.SaveChangesAsync() > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error on update summary in db");
            return false;
        }
    }
    public async Task<ManagerSummaryReport?> GetAsync(int loginId)
    {
        try
        {
            return await _dbContext.SummaryReports.FindAsync(loginId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error on find position");
            return null;
        }
    }

    public async Task<(List<ManagerSummaryReport>, int, bool)> GetAsync(ParamModel param)
    {
        try
        {
            using var connection = _dbContext.Database.GetDbConnection() as MySqlConnection;
            if (connection == null)
            {
                _logger.LogError("Database connection is null");
                return (new List<ManagerSummaryReport>(), 0, false);
            }
            await connection.OpenAsync();
            using var command = new MySqlCommand("GetManagerSummaries", connection);
            command.CommandType = System.Data.CommandType.StoredProcedure;
            command.Parameters.AddWithValue("p_SortDirection", "ASC");     //KHUSHI
            command.Parameters.AddWithValue("p_loginIds", param.Loginid);  //KHUSHI
            command.Parameters.AddWithValue("p_SortColumn", param.Sort);
            command.Parameters.AddWithValue("p_FilterColumn", param.Filter);
            command.Parameters.AddWithValue("p_FilterValue", param.Filtervalue);
            var reader = await command.ExecuteReaderAsync();
            var dt = new DataTable();
            dt.Load(reader);
            if(dt.Rows.Count == 0)
            {
                _logger.LogInformation("No summaries found for the given parameters");
                return (new List<ManagerSummaryReport>(), 0, true);
            }
            var summaries = dt.AsEnumerable().Select(row => new ManagerSummaryReport
            {
                LoginId = (ulong)row.Field<int>("LoginId"),
                Additional= row.Field<double>("Additional"),
                ClientName = row.Field<string>("ClientName")??string.Empty,
                Commission = row.Field<double>("Commission"),
                Credit = row.Field<double>("Credit"),
                Currency = row.Field<string>("Currency") ?? string.Empty,
                CurrencyDigits = row.Field<uint>("CurrencyDigits"),
                CurrentBalance = row.Field<double>("CurrentBalance"),
                Demo = row.Field<string>("Demo")??string.Empty,
                Deposit = row.Field<double>("Deposit"),
                Fee = row.Field<double>("Fee"),
                InOut = row.Field<double>("InOut"),
                Profit = row.Field<double>("Profit"),
                Swap = row.Field<double>("Swap"),
                Volume = row.Field<double>("Volume"),
                Withdraw = row.Field<double>("Withdraw"),
            }).ToList();

            var totalCount = summaries.Count();

            return (summaries.Skip((param.Page - 1) * param.Limit).Take(param.Limit).ToList(), totalCount, true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error on find summaries with parameters {@Param}", param);
            return (new List<ManagerSummaryReport>(), 0, false);
        }
    }
    //public async Task<(List<ManagerSummaryReport>, int, bool)> GetAsync(ParamModel param)
    //{
    //    try
    //    {


    //        var baseQuery = _dbContext.SummaryReports.AsNoTracking().AsQueryable();

    //        if (!string.IsNullOrWhiteSpace(param.Loginid))
    //        {
    //            var loginIds = Globals.GetLoginId(param.Loginid);
    //            if (loginIds.Count > 0)
    //                baseQuery = baseQuery.Where(x => loginIds.Contains(x.LoginId));
    //            else
    //                return (new List<ManagerSummaryReport>(), 0, true);
    //        }


    //        if (!string.IsNullOrWhiteSpace(param.Filter) && !string.IsNullOrWhiteSpace(param.Filtervalue))
    //        {
    //            Expression<Func<ManagerSummaryReport, bool>>? lambda = Globals.FilterByExpression<ManagerSummaryReport>(param, _logger);
    //            baseQuery = baseQuery.Where(lambda);
    //        }

    //        var totalCount =await baseQuery.CountAsync();

    //        if (!string.IsNullOrEmpty(param.Sort))
    //        {
    //            var ordering = Globals.SortByExpression<ManagerSummaryReport>(param, _logger);
    //            if (ordering != null)
    //            {
    //                baseQuery = ordering(baseQuery);
    //            }
    //        }

    //        var summaries =await baseQuery
    //            .Skip((param.Page - 1) * param.Limit)
    //            .Take(param.Limit)
    //            .ToListAsync();



    //        return (summaries,totalCount, true);
    //    }
    //    catch (Exception ex)
    //    {
    //        _logger.LogError(ex, "Error on find summaries with parameters {@Param}", param);
    //        return (new List<ManagerSummaryReport>(), 0, false);
    //    }
    //}

    public async Task<List<ManagerSummaryReport>?> GetByUserAsync(ulong loginId)
    {
        try
        {
            return await _dbContext.SummaryReports.Where(x => x.LoginId == loginId).ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error on find order");
            return null;
        }
    }
    public async Task<DateTime> GetMaxTime()
    {
        return await _dbContext.Positions
                           .OrderByDescending(d => d.Time)
                           .Select(d => d.Time)
                           .FirstOrDefaultAsync();

    }
    public async Task<bool> AddOrUpdateUsersAsync(IEnumerable<ManagerSummaryReport> summaries)
    {
        try
        {
            var summaryIds = summaries.Select(u => u.LoginId).ToList();

            // Fetch existing users
            var existingSummaries = await _dbContext.SummaryReports
                .Where(u => summaryIds.Contains(u.LoginId))
                .ToListAsync();

            var existingSummaryIds = existingSummaries.Select(u => u.LoginId).ToHashSet();

            var newSummaries = summaries.Where(u => !existingSummaryIds.Contains(u.LoginId)).ToList();
            var usersToUpdate = summaries.Where(u => existingSummaryIds.Contains(u.LoginId)).ToList();

            if (newSummaries.Any())
                await _dbContext.SummaryReports.AddRangeAsync(newSummaries);

            foreach (var userToUpdate in usersToUpdate)
            {
                var existing = existingSummaries.First(u => u.LoginId == userToUpdate.LoginId);

                _dbContext.SummaryReports.Entry(existing).CurrentValues.SetValues(userToUpdate);
            }

            return await _dbContext.SaveChangesAsync() > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding/updating summary in DB");
            return false;
        }
    }

    public async Task<List<ManagerSummaryReport>?> GetAsync()
    {
        try
        {
            return await _dbContext.SummaryReports.ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error on getting SummaryReports");
            return null;
        }
    }
}
