using Apmcrm.V1.Msgs;
using Apmcrm.V1.Msgs.Types;
using CLIB.Helpers;
using CRMUKMTPApi.Repositories;
using Google.Protobuf;
using MT5LIB;
using MT5LIB.Enums;
using MT5LIB.Helpers;
using MT5LIB.Models;

namespace CRMUKMTPApi.Helpers;

public class DailyHelper
{
    private readonly ILogger<DailyHelper> _logger;
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly MT5LIBHelper _helper;
    private readonly CDailySink _dailySink;
    private readonly AppQueue<Tuple<TradeEvent, ManagerDailyReport>> _queue;

    public DailyHelper(ILogger<DailyHelper> logger, CDailySink cDailySink,
        MT5LIBHelper helper, IServiceScopeFactory serviceScopeFactory)
    {
        _logger = logger;
        _helper = helper;
        _dailySink = cDailySink;
        _serviceScopeFactory = serviceScopeFactory;
        _dailySink.DailyUpdate += DailyUpdate;
        _queue = new AppQueue<Tuple<TradeEvent, ManagerDailyReport>>(ProcessDaily);
    }
    private void DailyUpdate(TradeEvent tradeEvent, ManagerDailyReport data)
    {
        _queue.Enqueue(Tuple.Create(tradeEvent, data));
    }
    private async Task ProcessDaily(Tuple<TradeEvent, ManagerDailyReport> item)
    {
        try
        {
            using var scope = _serviceScopeFactory.CreateScope();
            var repository = scope.ServiceProvider.GetRequiredService<IDailyRepository>();
            if (repository == null) return;
            if (item.Item1 == TradeEvent.Perform)
            {
                await repository.AddAsync(item.Item2);
            }
            else if (item.Item1 == TradeEvent.Modify)
            {
                await repository.UpdateAsync(item.Item2);
            }
          
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error on process daily {ex.Message}");
        }
    }



    public async Task<bool> InitializeDaily()
    {
        try
        {
            using var scope = _serviceScopeFactory.CreateScope();
            var repository = scope.ServiceProvider.GetRequiredService<IDailyRepository>();
            if (repository == null) return false;

            DateTime fromDate = await repository.GetMaxTime();
            fromDate = fromDate == DateTime.MinValue ? new DateTime(2015, 01, 01) : fromDate;

            var dailySummaries = _helper.GetDailySummary(new DateTimeOffset(fromDate).ToUnixTimeSeconds(), new DateTimeOffset(DateTime.Now.AddDays(1)).ToUnixTimeSeconds());
            if (dailySummaries == null) return false;

            await repository.AddAsync(dailySummaries);

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error on initialize order");
            return false;
        }
    }

}
