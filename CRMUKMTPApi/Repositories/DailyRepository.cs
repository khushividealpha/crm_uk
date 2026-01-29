using CRMUKMTPApi.Data;
using CRMUKMTPApi.Helpers;
using CRMUKMTPApi.Models;
using Microsoft.EntityFrameworkCore;
using MT5LIB.Enums;
using MT5LIB.Models;
using MySql.Data.MySqlClient;
using System.Linq.Expressions;

namespace CRMUKMTPApi.Repositories;

public class DailyRepository : IDailyRepository
{
    private readonly ILogger<DailyRepository> _logger;
    private readonly AppDBContext _dbContext;

    public DailyRepository(ILogger<DailyRepository> logger, AppDBContext dbContext)
    {
        _logger = logger;
        _dbContext = dbContext;
    }
    public async Task<bool> AddAsync(ManagerDailyReport daily)
    {
        try
        {
            await _dbContext.Daily.AddAsync(daily);
            return await _dbContext.SaveChangesAsync() > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error add DailyReport in db");
            return false;
        }
    }
    public async Task<bool> AddAsync(IEnumerable<ManagerDailyReport> dailyReports)
    {
        try
        {
            await _dbContext.Daily.AddRangeAsync(dailyReports);
            return await _dbContext.SaveChangesAsync() > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error add DailyReports in db");
            return false;
        }
    }
    public async Task<bool> UpdateAsync(ManagerDailyReport daily)
    {
        try
        {
            var existDeal = await _dbContext.Deals.FindAsync(daily.id);
            if (existDeal == null) return false;

            _dbContext.Deals.Entry(existDeal).CurrentValues.SetValues(daily);
            return await _dbContext.SaveChangesAsync() > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error on update DailyReport in db");
            return false;
        }
    }
    public async Task<ManagerDailyReport?> GetAsync(int id)
    {
        try
        {
            return await _dbContext.Daily.FindAsync(id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error on find DailyReport");
            return null;
        }
    }
    public async Task<IEnumerable<ManagerDailyReport>?> GetAsync()
    {
        try
        {
            return await _dbContext.Daily.ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error on get DailyReports");
            return null;
        }
    }
    public async Task<(List<ManagerDailyReport>, int, bool)> GetAsync(ParamModel param)
    {

        try
        {
            List<ManagerDailyReport> dailyReports = new List<ManagerDailyReport>();
            DateTime? fromDate = null;
            DateTime? toDate = null;
            if (Globals.ConvertDate(param.From, out var from) && from.HasValue && Globals.ConvertDate(param.To, out var to) && to.HasValue)
            {
                fromDate = from.Value.Date;
                toDate = to.Value.Date.AddDays(1).AddTicks(-1);

            }

            using var connection = _dbContext.Database.GetDbConnection() as MySqlConnection;
            if (connection == null)
            {
                _logger.LogError("Database connection is null");
                return (dailyReports, 0, false);
            }
            await connection.OpenAsync();
            using var command = new MySqlCommand("GetManagerDaily", connection);
            command.CommandType = System.Data.CommandType.StoredProcedure;

            command.Parameters.AddWithValue("p_loginids", param.Loginid);
            command.Parameters.AddWithValue("p_SortColumn", param.Sort);
            command.Parameters.AddWithValue("p_FilterColumn", param.Filter);
            command.Parameters.AddWithValue("p_FilterValue", param.Filtervalue);
            command.Parameters.AddWithValue("p_Page", param.Page);
            command.Parameters.AddWithValue("p_Limit", param.Limit);
            command.Parameters.AddWithValue("p_FromDate", fromDate);
            command.Parameters.AddWithValue("p_ToDate", toDate);
            var reader = await command.ExecuteReaderAsync();
            if (reader == null || !reader.HasRows)
            {
                _logger.LogInformation("No summaries found for the given parameters");
                return (dailyReports, 0, true);
            }
            int totalCount = 0;
            if (await reader.ReadAsync())
            {
                totalCount = reader.GetInt32(reader.GetOrdinal("TotalCount"));
            }
            await reader.NextResultAsync();
            while (await reader.ReadAsync())
            {

                try
                {
                    var daily = new ManagerDailyReport
                    {
                        LoginId = Globals.GetColumnValue<ulong>(reader, "LoginId"),
                        Balance = Globals.GetColumnValue<ulong>(reader, "Balance"),
                        ClientName = Globals.GetColumnValue<string>(reader, "ClientName"),
                        ClosedPL = Globals.GetColumnValue<double>(reader, "ClosedPL"),
                        Credit = Globals.GetColumnValue<double>(reader, "Credit"),
                        Currency = Globals.GetColumnValue<string>(reader, "Currency"),
                        Date = Globals.GetColumnValue<DateTime>(reader, "Date"),
                        Demo = Globals.GetColumnValue<bool>(reader, "Demo"),
                        Deposit = Globals.GetColumnValue<double>(reader, "Deposit"),
                        Email = Globals.GetColumnValue<string>(reader, "Email"),
                        Equity = Globals.GetColumnValue<double>(reader, "Equity"),
                        FloatingPL = Globals.GetColumnValue<double>(reader, "FloatingPL"),
                        Group = Globals.GetColumnValue<string>(reader, "Group"),
                        Margin = Globals.GetColumnValue<double>(reader, "Margin"),
                        MarginFree = Globals.GetColumnValue<double>(reader, "MarginFree"),
                        PrevBalance = Globals.GetColumnValue<double>(reader, "PrevBalance"),
                    };
                    dailyReports.Add(daily);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.Message);
                }
            }
            return (dailyReports, totalCount, true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error on find order with parameters {@Param}", param);
            return (new List<ManagerDailyReport>(), 0, false);
        }

    }
    //public async Task<(List<ManagerDailyReport>, int, bool)> GetAsync(ParamModel param)
    //{

    //    try
    //    {

    //        var baseQuery = _dbContext.Daily.AsNoTracking().AsQueryable();

    //        if (!string.IsNullOrWhiteSpace(param.Loginid))
    //        {
    //            var loginIds = Globals.GetLoginId(param.Loginid);
    //            if (loginIds.Count > 0)
    //                baseQuery = baseQuery.Where(x => loginIds.Contains(x.LoginId));
    //            else
    //                return (new List<ManagerDailyReport>(), 0, true);
    //        }

    //        if (Globals.ConvertDate(param.From, out var from) && from.HasValue && Globals.ConvertDate(param.To, out var to) && to.HasValue)
    //        {
    //            var fromDate = from.Value.Date;
    //            var toDate = to.Value.Date.AddDays(1).AddTicks(-1);
    //            baseQuery = baseQuery.Where(x => x.Date >= fromDate && x.Date <= toDate);
    //        }

    //        if (!string.IsNullOrWhiteSpace(param.Filter) && !string.IsNullOrWhiteSpace(param.Filtervalue))
    //        {
    //            Expression<Func<ManagerDailyReport, bool>>? lambda = Globals.FilterByExpression<ManagerDailyReport>(param, _logger);
    //            baseQuery = baseQuery.Where(lambda);
    //        }

    //        var totalCount = await baseQuery.CountAsync();

    //        if (!string.IsNullOrEmpty(param.Sort))
    //        {
    //            var ordering = Globals.SortByExpression<ManagerDailyReport>(param, _logger);
    //            if (ordering != null)
    //            {
    //                baseQuery = ordering(baseQuery);
    //            }
    //        }

    //        List<ManagerDailyReport>? ordersTask = null;
    //        if (param.Page > 0)
    //        {
    //            ordersTask = await baseQuery
    //               .Skip((param.Page - 1) * param.Limit)
    //               .Take(param.Limit)
    //               .ToListAsync();
    //        }
    //        else
    //        {
    //            ordersTask = await baseQuery
    //                .ToListAsync();
    //        }



    //        return (ordersTask, totalCount, true);
    //    }
    //    catch (Exception ex)
    //    {
    //        _logger.LogError(ex, "Error on find order with parameters {@Param}", param);
    //        return (new List<ManagerDailyReport>(), 0, false);
    //    }
    //}

    public async Task<List<ManagerDailyReport>?> GetByUserAsync(ulong loginId)
    {
        try
        {
            return await _dbContext.Daily.Where(x => x.LoginId == loginId).ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error on find DailyReports");
            return null;
        }
    }
    public async Task<DateTime> GetMaxTime()
    {
        return await _dbContext.Daily
                           .OrderByDescending(d => d.Date)
                           .Select(d => d.Date)
                           .FirstOrDefaultAsync();

    }

    public async Task<bool> DeleteAsync(ManagerDailyReport daily)
    {
        try
        {
            // var existDeal=await  _dbContext.Deals.FindAsync(daily.DealId);
            //if (existDeal == null) return false;
            //_dbContext.Deals.Remove(existDeal);
            //return await _dbContext.SaveChangesAsync() > 0;
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error on remove DailyReport");
            return false;
        }
    }
}
