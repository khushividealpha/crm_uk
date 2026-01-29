using Apmcrm.V1.Msgs;
using CRMUKMTPApi.Data;
using CRMUKMTPApi.Helpers;
using CRMUKMTPApi.Models;
using Microsoft.EntityFrameworkCore;
using MT5LIB.Enums;
using MT5LIB.Models;
using MySql.Data.MySqlClient;
using System.Data;
using System.Data.Common;
using System.Linq.Expressions;
using System.Xml;

namespace CRMUKMTPApi.Repositories;

public class DealRepository : IDealRepository
{
    private readonly ILogger<DealRepository> _logger;
    private readonly AppDBContext _dbContext;

    public DealRepository(ILogger<DealRepository> logger, AppDBContext dbContext)
    {
        _logger = logger;
        _dbContext = dbContext;
    }
    public async Task<bool> AddAsync(ManagerDeal deal)
    {
        try
        {
            await _dbContext.Deals.AddAsync(deal);
            return await _dbContext.SaveChangesAsync() > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error add deal in db");
            return false;
        }
    }
    public async Task<bool> AddAsync(IEnumerable<ManagerDeal> deals)
    {
        try
        {
            var incomingDealNumbers = deals.Select(o => o.DealId).ToList();

            var existingDealNumbers = new HashSet<ulong>(await _dbContext.Deals
                                       .Where(x => incomingDealNumbers.Contains(x.DealId))
                                       .Select(x => x.DealId)
                                       .ToListAsync());

            var newDeals = deals.Where(o => !existingDealNumbers.Contains(o.DealId)).ToList();
            if (newDeals.Any())
            {
                await _dbContext.Deals.AddRangeAsync(newDeals);
                return await _dbContext.SaveChangesAsync() > 0;
            }
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error add deals in db");
            return false;
        }
    }
    public async Task<bool> UpdateAsync(ManagerDeal deal)
    {
        try
        {
            var existDeal = await _dbContext.Deals.FindAsync(deal.DealId);
            if (existDeal == null) return false;

            _dbContext.Deals.Entry(existDeal).CurrentValues.SetValues(deal);
            return await _dbContext.SaveChangesAsync() > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error on update deals in db");
            return false;
        }
    }
    public async Task<ManagerDeal?> GetAsync(ulong dealId)
    {
        try
        {
            return await _dbContext.Deals.FindAsync(dealId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error on find deal");
            return null;
        }
    }
    public async Task<IEnumerable<ManagerDeal>?> GetAsync()
    {
        try
        {
            return await _dbContext.Deals.ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error on get deals");
            return null;
        }
    }

    public async Task<(List<ManagerDeal>, int, bool)> GetAsync(ParamModel param, string symbols = "")
    {

        try
        {
            List<ManagerDeal> deals = new List<ManagerDeal>();
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
                return (deals, 0, false);
            }
            await connection.OpenAsync();
            using var command = new MySqlCommand("GetManagerDeals", connection);
            command.CommandType = System.Data.CommandType.StoredProcedure;

            command.Parameters.AddWithValue("p_loginids", param.Loginid);
            command.Parameters.AddWithValue("p_Symbols", param.Symbols);
            command.Parameters.AddWithValue("p_FromDate", fromDate);
            command.Parameters.AddWithValue("p_ToDate", toDate);
            command.Parameters.AddWithValue("p_SortColumn", param.Sort);
            command.Parameters.AddWithValue("p_FilterColumn", param.Filter);
            command.Parameters.AddWithValue("p_FilterValue", param.Filtervalue);
            command.Parameters.AddWithValue("p_Page", param.Page);
            command.Parameters.AddWithValue("p_Limit", param.Limit);
            var reader = await command.ExecuteReaderAsync();
            if (reader == null || !reader.HasRows)
            {
                _logger.LogInformation("No summaries found for the given parameters");
                return (deals, 0, true);
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
                    var deal = new ManagerDeal
                    {
                        DealId = Globals.GetColumnValue<ulong>(reader, "DealId"),
                        LoginId = Globals.GetColumnValue<ulong>(reader, "LoginId"),
                        Symbol = Globals.GetColumnValue<string>(reader, "Symbol") ?? string.Empty,
                        Volume = Globals.GetColumnValue<double>(reader, "Volume"),
                        Price = Globals.GetColumnValue<double>(reader, "Price"),
                        Time = Globals.GetColumnValue<DateTime>(reader, "Time"),
                        Type = (TradeType)Globals.GetColumnValue<int>(reader, "Type"),
                        Comment = Globals.GetColumnValue<string>(reader, "Comment") ?? string.Empty,
                        Action = Globals.GetColumnValue<uint>(reader, "Action"),
                        Sl = Globals.GetColumnValue<double>(reader, "Sl"),
                        Tp = Globals.GetColumnValue<double>(reader, "Tp"),
                        Profit = Globals.GetColumnValue<double>(reader, "Profit"),
                        Swap = Globals.GetColumnValue<double>(reader, "Swap"),
                        Dealer = Globals.GetColumnValue<ulong>(reader, "Dealer"),
                        Entry = Globals.GetColumnValue<string>(reader, "Entry") ?? string.Empty,
                        OrderId = Globals.GetColumnValue<ulong>(reader, "OrderId"),
                        Reason = Globals.GetColumnValue<string>(reader, "Reason") ?? string.Empty,
                        Demo = Globals.GetColumnValue<bool>(reader, "Demo"),
                        ClientName = Globals.GetColumnValue<string>(reader, "ClientName") ?? string.Empty,
                        CommissionFee = Globals.GetColumnValue<double>(reader, "CommissionFee"),
                        Currency = Globals.GetColumnValue<string>(reader, "Currency") ?? string.Empty,
                        Fee = Globals.GetColumnValue<double>(reader, "Fee"),
                        MarketAsk = Globals.GetColumnValue<double>(reader, "MarketAsk"),
                        MarketBid = Globals.GetColumnValue<double>(reader, "MarketBid"),
                        Position = Globals.GetColumnValue<long>(reader, "Position"),
                    };
                    deals.Add(deal);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.Message);
                }
            }
            return (deals, totalCount, true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error on find order with parameters {@Param}", param);
            return (new List<ManagerDeal>(), 0, false);
        }

    }







    //public async Task<(List<ManagerDeal>, int, bool)> GetAsync(ParamModel param, string symbols = "")
    //{
    //    try
    //    {


    //        var baseQuery = _dbContext.Deals.AsNoTracking().AsQueryable();

    //        if (!string.IsNullOrWhiteSpace(param.Loginid))
    //        {
    //            List<ulong> loginIds = Globals.GetLoginId(param.Loginid);
    //            if (loginIds.Count > 0)
    //                baseQuery = baseQuery.Where(x => loginIds.Contains(x.LoginId));
    //            else
    //                return (new List<ManagerDeal>(), 0, true);
    //        }
    //        if (!string.IsNullOrWhiteSpace(symbols))
    //        {
    //            List<string> lstSymbol = Globals.GetSymbols(symbols);
    //            if (lstSymbol.Count > 0)
    //                baseQuery = baseQuery.Where(x => lstSymbol.Contains(x.Symbol));
    //        }
    //        if (Globals.ConvertDate(param.From, out var from) && from.HasValue && Globals.ConvertDate(param.To, out var to) && to.HasValue)
    //        {
    //            var fromDate = from.Value.Date;
    //            var toDate = to.Value.Date.AddDays(1).AddTicks(-1);
    //            baseQuery = baseQuery.Where(x => x.Time >= fromDate && x.Time <= toDate);
    //        }

    //        if (!string.IsNullOrWhiteSpace(param.Filter) && !string.IsNullOrWhiteSpace(param.Filtervalue))
    //        {
    //            Expression<Func<ManagerDeal, bool>>? lambda = Globals.FilterByExpression<ManagerDeal>(param, _logger);
    //            baseQuery = baseQuery.Where(lambda);
    //        }

    //        var totalCount = await baseQuery.CountAsync();

    //        if (!string.IsNullOrEmpty(param.Sort))
    //        {
    //            var ordering = Globals.SortByExpression<ManagerDeal>(param, _logger);
    //            if (ordering != null)
    //            {
    //                baseQuery = ordering(baseQuery);
    //            }
    //        }

    //        List<ManagerDeal>? ordersTask = null;
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
    //        return (new List<ManagerDeal>(), 0, false);
    //    }
    //}

    public async Task<List<ManagerDeal>?> GetByUserAsync(ulong loginId)
    {
        try
        {
            return await _dbContext.Deals.Where(x => x.LoginId == loginId).ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error on find order");
            return null;
        }
    }
    public async Task<DateTime> GetMaxTime()
    {
        return await _dbContext.Deals
                           .OrderByDescending(d => d.Time)
                           .Select(d => d.Time)
                           .FirstOrDefaultAsync();

    }

    public async Task<bool> AddOrUpdateUsersAsync(IEnumerable<ManagerDeal> deals)
    {
        try
        {
            var dealIds = deals.Select(u => u.DealId).ToList();

            // Fetch existing users
            var existingDeals = await _dbContext.Deals
                .Where(u => dealIds.Contains(u.DealId))
                .ToListAsync();

            var existingDealIds = existingDeals.Select(u => u.DealId).ToHashSet();

            var newDeals = deals.Where(u => !existingDealIds.Contains(u.DealId)).ToList();
            var usersToUpdate = deals.Where(u => existingDealIds.Contains(u.DealId)).ToList();

            if (newDeals.Any())
                await _dbContext.Deals.AddRangeAsync(newDeals);

            foreach (var userToUpdate in usersToUpdate)
            {
                var existing = existingDeals.First(u => u.DealId == userToUpdate.DealId);

                _dbContext.Deals.Entry(existing).CurrentValues.SetValues(userToUpdate);
            }

            return await _dbContext.SaveChangesAsync() > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding/updating users in DB");
            return false;
        }
    }

    public async Task<bool> DeleteAsync(ManagerDeal deal)
    {
        try
        {
            var existDeal = await _dbContext.Deals.FindAsync(deal.DealId);
            if (existDeal == null) return false;
            _dbContext.Deals.Remove(existDeal);
            return await _dbContext.SaveChangesAsync() > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error on remove deal");
            return false;
        }
    }

    public async Task<(List<TradeDataModel>, int, bool)> GetTradeDataAsync(ParamModel param)
    {
        try
        {
            List<TradeDataModel> tradeData = new List<TradeDataModel>();
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
                return (tradeData, 0, false);
            }
            await connection.OpenAsync();
            using var command = new MySqlCommand("GetTradeData", connection);
            command.CommandType = System.Data.CommandType.StoredProcedure;

            command.Parameters.AddWithValue("p_loginids", param.Loginid);
            command.Parameters.AddWithValue("p_Symbols", param.Symbols);
            command.Parameters.AddWithValue("p_FromDate", fromDate);
            command.Parameters.AddWithValue("p_ToDate", toDate);
            command.Parameters.AddWithValue("p_Page", param.Page);
            command.Parameters.AddWithValue("p_Limit", param.Limit);
            var reader = await command.ExecuteReaderAsync();
            if (reader == null || !reader.HasRows)
            {
                _logger.LogInformation("No summaries found for the given parameters");
                return (tradeData, 0, true);
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
                    var deal = new TradeDataModel
                    {
                        Profit = Globals.GetColumnValue<string>(reader, "Profit"),
                        Swap = Globals.GetColumnValue<string>(reader, "Swap"),
                        Login = Globals.GetColumnValue<string>(reader, "LoginId"),
                        Symbol = Globals.GetColumnValue<string>(reader, "Symbol"),
                        LastTrade = Globals.GetColumnValue<string>(reader, "LastTrade"),
                        TotalCount = Globals.GetColumnValue<string>(reader, "TotalCount"),
                        FirstTrade = Globals.GetColumnValue<string>(reader, "FirstTrade"),
                        Email= Globals.GetColumnValue<string>(reader, "Email"),
                        FirstName = Globals.GetColumnValue<string>(reader, "Name"),
                        LastName = Globals.GetColumnValue<string>(reader, "LastName"),
                        Group = Globals.GetColumnValue<string>(reader, "Group"),
                        Phone = Globals.GetColumnValue<string>(reader, "Phone"),
                        
                    };
                    tradeData.Add(deal);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.Message);
                }
            }
            return (tradeData, totalCount, true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error on find order with parameters {@Param}", param);
            return (new List<TradeDataModel>(), 0, false);
        }
    }

    public async Task<(List<TradeSummary>, int, bool)> GetTradeSummaryDataAsync(ParamModel param)
    {
        try
        {
            List<TradeSummary> tradeData = new List<TradeSummary>();
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
                return (tradeData, 0, false);
            }
            await connection.OpenAsync();
            using var command = new MySqlCommand("GetTradeSummaryData", connection);
            command.CommandType = System.Data.CommandType.StoredProcedure;

            command.Parameters.AddWithValue("p_loginids", param.Loginid);
            command.Parameters.AddWithValue("p_FromDate", fromDate);
            command.Parameters.AddWithValue("p_ToDate", toDate);
      
            var reader = await command.ExecuteReaderAsync();
            if (reader == null || !reader.HasRows)
            {
                _logger.LogInformation("No summaries found for the given parameters");
                return (tradeData, 0, true);
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
                    var deal = new TradeSummary
                    {
                        Profit = Globals.GetColumnValue<double>(reader, "Profit"),
                        Login = Globals.GetColumnValue<ulong>(reader, "LoginId"),
                        TotalCount = Globals.GetColumnValue<int>(reader, "TotalCount"),
                        Balance= Globals.GetColumnValue<double>(reader, "Balance"),
                        AUM= Globals.GetColumnValue<double>(reader, "AUM"),
                    };
                    tradeData.Add(deal);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.Message);
                }
            }
            return (tradeData, totalCount, true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error on find order with parameters {@Param}", param);
            return (new List<TradeSummary>(), 0, false);
        }
    }
}
