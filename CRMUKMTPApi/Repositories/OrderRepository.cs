using CRMUKMTPApi.Data;
using CRMUKMTPApi.Helpers;
using CRMUKMTPApi.Models;
using Microsoft.EntityFrameworkCore;
using MT5LIB.Enums;
using MT5LIB.Models;
using MySql.Data.MySqlClient;


namespace CRMUKMTPApi.Repositories;

public class OrderRepository : IOrderRepository
{
    private readonly ILogger<OrderRepository> _logger;
    private readonly AppDBContext _dbContext;

    public OrderRepository(ILogger<OrderRepository> logger, AppDBContext dbContext)
    {
        _logger = logger;
        _dbContext = dbContext;
    }
    public async Task<bool> AddAsync(ManagerOrder order)
    {
        try
        {
            await _dbContext.Orders.AddAsync(order);
            return await _dbContext.SaveChangesAsync() > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error add order in db");
            return false;
        }
    }
    public async Task<bool> AddAsync(IEnumerable<ManagerOrder> orders)
    {
        try
        {
            var incomingOrderNumbers = orders.Select(o => o.OrderId).ToList();

            var existingOrderNumbers = new HashSet<ulong>(await _dbContext.Orders
                                       .Where(x => incomingOrderNumbers.Contains(x.OrderId))
                                       .Select(x => x.OrderId)
                                       .ToListAsync());

            var newOrders = orders.Where(o => !existingOrderNumbers.Contains(o.OrderId)).ToList();
            if (newOrders.Any())
            {
                await _dbContext.Orders.AddRangeAsync(newOrders);
                return await _dbContext.SaveChangesAsync() > 0;
            }
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error add orders in db");
            return false;
        }
    }
    public async Task<bool> UpdateAsync(ManagerOrder order)
    {
        try
        {
            var existOrder = await _dbContext.Orders.FindAsync(order.OrderId);
            if (existOrder == null) return false;

            _dbContext.Orders.Entry(existOrder).CurrentValues.SetValues(order);
            return await _dbContext.SaveChangesAsync() > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error on update orders in db");
            return false;
        }
    }
    public async Task<ManagerOrder?> GetAsync(ulong orderId)
    {
        try
        {
            return await _dbContext.Orders.FindAsync(orderId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error on find order");
            return null;
        }
    }
    public async Task<(List<ManagerOrder>, int, bool)> GetAsync(ParamModel param)
    {
        try
        {
            List<ManagerOrder> orders = new List<ManagerOrder>();
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
                return (orders, 0, false);
            }
            await connection.OpenAsync();
            using var command = new MySqlCommand("GetManagerOrders", connection);
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
                return (orders, 0, true);
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
                    var order = new ManagerOrder
                    {
                        ClientName = Globals.GetColumnValue<string>(reader, "ClientName"),
                        Comment = Globals.GetColumnValue<string>(reader, "Comment"),
                        Demo = Globals.GetColumnValue<bool>(reader, "Demo"),
                        DoneTime = Globals.GetColumnValue<DateTime>(reader, "DoneTime"),
                        Login = Globals.GetColumnValue<ulong>(reader, "Login"),
                        OrderId = Globals.GetColumnValue<ulong>(reader, "OrderId"),
                        Position = Globals.GetColumnValue<ulong>(reader, "Position"),
                        Price = Globals.GetColumnValue<double>(reader, "Price"),
                        Reason = Globals.GetColumnValue<string>(reader, "Reason"),
                        SetupTime = Globals.GetColumnValue<DateTime>(reader, "SetupTime"),
                        Sl = Globals.GetColumnValue<double>(reader, "Sl"),
                        State = Globals.GetColumnValue<string>(reader, "State"),
                        Symbol = Globals.GetColumnValue<string>(reader, "Symbol"),
                        Tp = Globals.GetColumnValue<double>(reader, "Tp"),
                        Type = (TradeType)Globals.GetColumnValue<int>(reader, "Type"),
                        VolumeFilled = Globals.GetColumnValue<double>(reader, "VolumeFilled"),
                        VolumeTotal = Globals.GetColumnValue<double>(reader, "VolumeTotal"),

                    };
                    orders.Add(order);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.Message);
                }
            }
            return (orders, totalCount, true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error on find order with parameters {@Param}", param);
            return (new List<ManagerOrder>(), 0, false);
        }
    }
    //public async Task<(List<ManagerOrder>, int, bool)> GetAsync(ParamModel param)
    //{
    //    try
    //    {


    //        var baseQuery = _dbContext.Orders.AsNoTracking().AsQueryable();

    //        if (!string.IsNullOrWhiteSpace(param.Loginid))
    //        {
    //            var loginIds = Globals.GetLoginId(param.Loginid);
    //            if (loginIds.Count > 0)
    //                baseQuery = baseQuery.Where(x => loginIds.Contains(x.Login));
    //            else
    //                return (new List<ManagerOrder>(), 0, true);
    //        }

    //        if (Globals.ConvertDate(param.From, out var from) && from.HasValue && Globals.ConvertDate(param.To, out var to) && to.HasValue)
    //        {
    //            var fromDate = from.Value.Date;
    //            var toDate = to.Value.Date.AddDays(1).AddTicks(-1);
    //            baseQuery = baseQuery.Where(x => x.DoneTime >= fromDate && x.DoneTime <= toDate);
    //        }

    //        if (!string.IsNullOrWhiteSpace(param.Filter) && !string.IsNullOrWhiteSpace(param.Filtervalue))
    //        {
    //            Expression<Func<ManagerOrder, bool>>? lambda = Globals.FilterByExpression<ManagerOrder>(param, _logger);
    //            baseQuery = baseQuery.Where(lambda);
    //        }

    //        var totalCount =await baseQuery.CountAsync();

    //        if (!string.IsNullOrEmpty(param.Sort))
    //        {
    //            var ordering = Globals.SortByExpression<ManagerOrder>(param, _logger);
    //            if (ordering != null)
    //            {
    //                baseQuery = ordering(baseQuery);
    //            }
    //        }

    //        var orders =await baseQuery
    //            .Skip((param.Page - 1) * param.Limit)
    //            .Take(param.Limit)
    //            .ToListAsync();



    //        return (orders, totalCount, true);
    //    }
    //    catch (Exception ex)
    //    {
    //        _logger.LogError(ex, "Error on find order with parameters {@Param}", param);
    //        return (new List<ManagerOrder>(), 0, false);
    //    }
    //}
    public async Task<List<ManagerOrder>?> GetByUserAsync(ulong loginId)
    {
        try
        {
            return await _dbContext.Orders.Where(x => x.Login == loginId).ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error on find order");
            return null;
        }
    }
    public async Task<DateTime> GetMaxTime()
    {
        return await _dbContext.Orders
                           .OrderByDescending(d => d.SetupTime)
                           .Select(d => d.SetupTime)
                           .FirstOrDefaultAsync();

    }
    //public async Task<bool> AddOrUpdateUsersAsync(IEnumerable<ManagerOrder> orders)
    //{
    //    try
    //    {

    //        // var json =Newtonsoft.Json.JsonConvert.SerializeObject(orders);

    //        // Call the stored procedure
    //        foreach (var chunk in orders.Chunk(1000))
    //        {
    //            var json = Newtonsoft.Json.JsonConvert.SerializeObject(chunk);

    //            try
    //            {
    //                await _dbContext.Database.ExecuteSqlRawAsync(
    //                    "CALL BulkUpsertManagerOrders({0})",
    //                    json);
    //            }
    //            catch (Exception ex)
    //            {
    //                _logger.LogError(ex, "Error processing chunk of size {ChunkSize}", chunk.Count());
    //                return false;
    //            }
    //        }

    //        return true;

    //    }
    //    catch (Exception ex)
    //    {
    //        _logger.LogError(ex, "Error adding/updating users in DB");
    //        return false;
    //    }
    //}
    public async Task<bool> AddOrUpdateUsersAsync(IEnumerable<ManagerOrder> orders)
    {
        try
        {
            var orderIds = orders.Select(u => u.OrderId).ToList();

            // Fetch existing users
            var existingOrders = await _dbContext.Orders
                .Where(u => orderIds.Contains(u.OrderId))
                .ToListAsync();

            var existingOrderIds = existingOrders.Select(u => u.OrderId).ToHashSet();

            var newOrders = orders.Where(u => !existingOrderIds.Contains(u.OrderId)).ToList();
            var usersToUpdate = orders.Where(u => existingOrderIds.Contains(u.OrderId)).ToList();

            if (newOrders.Any())
            {
                const int batchSize = 1000;
                for (int i = 0; i < newOrders.Count; i += batchSize)
                {
                    var batch = newOrders.Skip(i).Take(batchSize);
                    await _dbContext.Orders.AddRangeAsync(batch);
                    await _dbContext.SaveChangesAsync();
                }
            }

            if (usersToUpdate.Any())
            {
                foreach (var userToUpdate in usersToUpdate)
                {
                    var existing = existingOrders.First(u => u.OrderId == userToUpdate.OrderId);

                    _dbContext.Orders.Entry(existing).CurrentValues.SetValues(userToUpdate);
                }

                await _dbContext.SaveChangesAsync();
            }
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding/updating users in DB");
            return false;
        }
    }
    public async Task<bool> DeleteAsync(ManagerOrder order)
    {
        try
        {
            var existOrder = await _dbContext.Orders.FindAsync(order.OrderId);
            if (existOrder == null) return false;
            _dbContext.Orders.Remove(existOrder);
            return await _dbContext.SaveChangesAsync() > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error on remove deal");
            return false;
        }
    }
}
