using CRMUKMTPApi.Data;
using CRMUKMTPApi.Helpers;
using CRMUKMTPApi.Models;
using Microsoft.EntityFrameworkCore;
using MT5LIB.Enums;
using MT5LIB.Models;
using MySql.Data.MySqlClient;
using System.Linq;
using System.Linq.Expressions;

namespace CRMUKMTPApi.Repositories;

public class PositionRepository : IPositionRepository
{
    private readonly ILogger<PositionRepository> _logger;
    private readonly AppDBContext _dbContext;

    public PositionRepository(ILogger<PositionRepository> logger, AppDBContext dbContext)
    {
        _logger = logger;
        _dbContext = dbContext;
    }
    public async Task<bool> AddAsync(ManagerPosition position)
    {
        try
        {
            await _dbContext.Positions.AddAsync(position);
            return await _dbContext.SaveChangesAsync() > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error add position in db");
            return false;
        }
    }
    public async Task<bool> AddAsync(IEnumerable<ManagerPosition> positions)
    {
        try
        {
            var incomingPositionNumbers = positions.Select(o => o.PositionId).ToList();

            var existingPositionNumbers = new HashSet<ulong>(await _dbContext.Positions
                                       .Where(x => incomingPositionNumbers.Contains(x.PositionId))
                                       .Select(x => x.PositionId)
                                       .ToListAsync());

            var newPosition = positions.Where(o => !existingPositionNumbers.Contains(o.PositionId)).ToList();
            if (newPosition.Any())
            {
                await _dbContext.Positions.AddRangeAsync(newPosition);
                return await _dbContext.SaveChangesAsync() > 0;
            }
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error add positions in db");
            return false;
        }
    }
    public async Task<bool> UpdateAsync(ManagerPosition position)
    {
        try
        {
            var existPosition = await _dbContext.Positions.FindAsync(position.PositionId);
            if (existPosition == null) return false;

            _dbContext.Positions.Entry(existPosition).CurrentValues.SetValues(position);
            return await _dbContext.SaveChangesAsync() > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error on update position in db");
            return false;
        }
    }
    public async Task<ManagerPosition?> GetAsync(ulong positionId)
    {
        try
        {
            return await _dbContext.Positions.FindAsync(positionId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error on find position");
            return null;
        }
    }
    public async Task<(List<ManagerPosition>, int, bool)> GetAsync(ParamModel param)
    {

        try
        {
            List<ManagerPosition> positions = new List<ManagerPosition>();
            

            using var connection = _dbContext.Database.GetDbConnection() as MySqlConnection;
            if (connection == null)
            {
                _logger.LogError("Database connection is null");
                return (positions, 0, false);
            }
            await connection.OpenAsync();
            using var command = new MySqlCommand("GetManagerPositions", connection);
            command.CommandType = System.Data.CommandType.StoredProcedure;

            command.Parameters.AddWithValue("p_loginids", param.Loginid);
            command.Parameters.AddWithValue("p_Symbols", param.Symbols);
            command.Parameters.AddWithValue("p_SortColumn", param.Sort);
            command.Parameters.AddWithValue("p_FilterColumn", param.Filter);
            command.Parameters.AddWithValue("p_FilterValue", param.Filtervalue);
            command.Parameters.AddWithValue("p_Page", param.Page);
            command.Parameters.AddWithValue("p_Limit", param.Limit);
            var reader = await command.ExecuteReaderAsync();
            if (reader == null || !reader.HasRows)
            {
                _logger.LogInformation("No summaries found for the given parameters");
                return (positions, 0, true);
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
                    var pos = new ManagerPosition
                    {
                        LoginId = Globals.GetColumnValue<ulong>(reader, "LoginId"),
                        Symbol = Globals.GetColumnValue<string>(reader, "Symbol") ?? string.Empty,
                        Volume = Globals.GetColumnValue<double>(reader, "Volume"),
                        Time = Globals.GetColumnValue<DateTime>(reader, "Time"),
                        Type = (TradeType)Globals.GetColumnValue<int>(reader, "Type"),
                        Comment = Globals.GetColumnValue<string>(reader, "Comment") ?? string.Empty,
                        Sl = Globals.GetColumnValue<double>(reader, "Sl"),
                        Tp = Globals.GetColumnValue<double>(reader, "Tp"),
                        Profit = Globals.GetColumnValue<double>(reader, "Profit"),
                        Swap = Globals.GetColumnValue<double>(reader, "Swap"),
                        PositionId = Globals.GetColumnValue<ulong>(reader, "PositionId"),
                        PriceCurr = Globals.GetColumnValue<double>(reader, "PriceCurr"),
                        PriceOpen = Globals.GetColumnValue<double>(reader, "PriceOpen"),
                        Reason = Globals.GetColumnValue<string>(reader, "Reason") ?? string.Empty,
                    };
                    positions.Add(pos);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.Message);
                }
            }
            return (positions, totalCount, true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error on find order with parameters {@Param}", param);
            return (new List<ManagerPosition>(), 0, false);
        }

    }
    //public async Task<(List<ManagerPosition>, int, bool)> GetAsync(ParamModel param)
    //{
    //    try
    //    {


    //        var baseQuery = _dbContext.Positions.AsNoTracking().AsQueryable();

    //        if (!string.IsNullOrWhiteSpace(param.Loginid))
    //        {
    //            var loginIds = Globals.GetLoginId(param.Loginid);
    //            if (loginIds.Count > 0)
    //                baseQuery = baseQuery.Where(x => loginIds.Contains(x.LoginId));
    //            else
    //                return (new List<ManagerPosition>(), 0, true);
    //        }


    //        if (!string.IsNullOrWhiteSpace(param.Filter) && !string.IsNullOrWhiteSpace(param.Filtervalue))
    //        {
    //            Expression<Func<ManagerPosition, bool>>? lambda = Globals.FilterByExpression<ManagerPosition>(param,_logger);
    //            baseQuery = baseQuery.Where(lambda);
    //        }

    //        var totalCount =await baseQuery.CountAsync();

    //        if (!string.IsNullOrEmpty(param.Sort))
    //        {
    //            var ordering = Globals.SortByExpression<ManagerPosition>(param, _logger);
    //            if (ordering != null)
    //            {
    //                baseQuery = ordering(baseQuery);
    //            }
    //        }

    //        var positions =await baseQuery
    //            .Skip((param.Page - 1) * param.Limit)
    //            .Take(param.Limit)
    //            .ToListAsync();



    //        return (positions, totalCount, true);
    //    }
    //    catch (Exception ex)
    //    {
    //        _logger.LogError(ex, "Error on find order with parameters {@Param}", param);
    //        return (new List<ManagerPosition>(), 0, false);
    //    }
    //}

    public async Task<List<ManagerPosition>?> GetByUserAsync(ulong loginId)
    {
        try
        {
            return await _dbContext.Positions.Where(x => x.LoginId == loginId).ToListAsync();
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
    public async Task<bool> AddOrUpdateUsersAsync(IEnumerable<ManagerPosition> positions)
    {
        try
        {
            var positionIds = positions.Select(u => u.PositionId).ToList();

            // Fetch existing users
            var existingPositions = await _dbContext.Positions
                .Where(u => positionIds.Contains(u.PositionId))
                .ToListAsync();

            var existingPositionIds = existingPositions.Select(u => u.PositionId).ToHashSet();

            var newPositions = positions.Where(u => !existingPositionIds.Contains(u.PositionId)).ToList();
            var usersToUpdate = positions.Where(u => existingPositionIds.Contains(u.PositionId)).ToList();

            if (newPositions.Any())
                await _dbContext.Positions.AddRangeAsync(newPositions);

            foreach (var userToUpdate in usersToUpdate)
            {
                var existing = existingPositions.First(u => u.PositionId == userToUpdate.PositionId);

                _dbContext.Positions.Entry(existing).CurrentValues.SetValues(userToUpdate);
            }

            return await _dbContext.SaveChangesAsync() > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding/updating users in DB");
            return false;
        }
    }
}
