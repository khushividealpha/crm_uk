using CLIB.Constants;
using CRMUKMTPApi.Data;
using Microsoft.EntityFrameworkCore;
using MySql.Data.MySqlClient;

namespace CRMUKMTPApi.Helpers;

public class ProcedureHelper
{
    private readonly ILogger<ProcedureHelper> _logger;
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public ProcedureHelper(ILogger<ProcedureHelper> logger, IServiceScopeFactory serviceScopeFactory)
    {
        _logger = logger;
        _serviceScopeFactory = serviceScopeFactory;
    }
    public async Task CreateTradeDataProcedure()
    {
        try
        {
            using var scope = _serviceScopeFactory.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDBContext>();
            using var connection = dbContext.Database.GetDbConnection() as MySqlConnection;
            if (connection != null)
            {
                await connection.OpenAsync();
                using var command = new MySqlCommand(StoreProcedures.GetTradeData, connection);
                await command.ExecuteNonQueryAsync();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error on CreateProcedure");
        }
    }
    public async Task CreateManagerSummariesProcedure()
    {
        try
        {
            using var scope = _serviceScopeFactory.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDBContext>();
            using var connection = dbContext.Database.GetDbConnection() as MySqlConnection;
            if (connection != null)
            {
                await connection.OpenAsync();
                using var command = new MySqlCommand(StoreProcedures.GetManagerSummaries, connection);
                await command.ExecuteNonQueryAsync();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error on CreateProcedure");
        }
    }
    public async Task CreateManagerDailyProcedure()
    {
        try
        {
            using var scope = _serviceScopeFactory.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDBContext>();
            using var connection = dbContext.Database.GetDbConnection() as MySqlConnection;
            if (connection != null)
            {
                await connection.OpenAsync();
                using var command = new MySqlCommand(StoreProcedures.GetManagerDaily, connection);
                await command.ExecuteNonQueryAsync();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error on CreateProcedure");
        }
    }
    public async Task CreateManagerPositionsProcedure()
    {
        try
        {
            using var scope = _serviceScopeFactory.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDBContext>();
            using var connection = dbContext.Database.GetDbConnection() as MySqlConnection;
            if (connection != null)
            {
                await connection.OpenAsync();
                using var command = new MySqlCommand(StoreProcedures.GetManagerPositions, connection);
                await command.ExecuteNonQueryAsync();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error on CreateProcedure");
        }
    }
    public async Task CreateManagerDealsProcedure()
    {
        try
        {
            using var scope = _serviceScopeFactory.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDBContext>();
            using var connection = dbContext.Database.GetDbConnection() as MySqlConnection;
            if (connection != null)
            {
                await connection.OpenAsync();
                using var command = new MySqlCommand(StoreProcedures.GetManagerDeals, connection);
                await command.ExecuteNonQueryAsync();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error on CreateProcedure");
        }
    }
    public async Task CreateManagerOrdersProcedure()
    {
        try
        {
            using var scope = _serviceScopeFactory.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDBContext>();
            using var connection = dbContext.Database.GetDbConnection() as MySqlConnection;
            if (connection != null)
            {
                await connection.OpenAsync();
                //using var dropCmd = new MySqlCommand("DROP PROCEDURE IF EXISTS GetManagerOrders;", connection);
                //await dropCmd.ExecuteNonQueryAsync();

                using var createCmd = new MySqlCommand(StoreProcedures.GetManagerOrders, connection);
                await createCmd.ExecuteNonQueryAsync();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error on CreateProcedure");
        }
    }

    public async Task CreateGetTradeSummaryDataProcedure()
    {
        try
        {
            using var scope = _serviceScopeFactory.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDBContext>();
            using var connection = dbContext.Database.GetDbConnection() as MySqlConnection;
            if (connection != null)
            {
                await connection.OpenAsync();
                using var command = new MySqlCommand(StoreProcedures.GetTradeSummaryData, connection);
                await command.ExecuteNonQueryAsync();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error on CreateProcedure");
        }
    }

}
