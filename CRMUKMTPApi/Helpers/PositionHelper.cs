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

public class PositionHelper
{
    private readonly ILogger<PositionHelper> _logger;
    private readonly AppQueue<Tuple<TradeEvent, ManagerPosition>> _queue;
    private readonly CPositionSink _positionSink;
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly MT5LIBHelper _helper;

    public PositionHelper(ILogger<PositionHelper> logger, CPositionSink positionSink,
        MT5LIBHelper helper, IServiceScopeFactory serviceScopeFactory)
    {
        _logger = logger;
        _positionSink = positionSink;
        _helper = helper;
        _serviceScopeFactory = serviceScopeFactory;
        _positionSink.PositionUpdate += PositionUpdate;
        _queue = new AppQueue<Tuple<TradeEvent, ManagerPosition>>(PrcessPosition);
    }

    private void PositionUpdate(TradeEvent tradeEvent, ManagerPosition data)
    {
        _queue.Enqueue(Tuple.Create(tradeEvent, data));
    }

    private async Task PrcessPosition(Tuple<TradeEvent, ManagerPosition> tuple)
    {
        try
        {
            MessageState messageState = MessageState.New;
            var tradeEvent = tuple.Item1;
            var position = tuple.Item2;
            using var scope = _serviceScopeFactory.CreateScope();
            var repository = scope.ServiceProvider.GetRequiredService<IPositionRepository>();
            if (tradeEvent == TradeEvent.Modify)
            {
                await repository.UpdateAsync(position);
                messageState = MessageState.Update;
            }
            else if (tradeEvent == TradeEvent.Perform)
            {
                await repository.AddAsync(position);
            }

            ByteString stringData = Globals.ConvertToByteString<ManagerPosition>(position);
            await Globals.BroadcastData<ManagerSummaryReport>(position.LoginId, messageState, MessageType.PositionOpen, stringData);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Fail on process deal");
        }
    }
    public async Task<bool> InitializePosition()
    {
        try
        {
            using var scope = _serviceScopeFactory.CreateScope();
            var repository = scope.ServiceProvider.GetRequiredService<IPositionRepository>();
            if (repository == null) return false;

            DateTime fromDate = await repository.GetMaxTime();
            fromDate = fromDate == DateTime.MinValue ? new DateTime(2015, 01, 01) : fromDate;

            var data = _helper.GetPositions();
            if (data == null) return false;

            await repository.AddOrUpdateUsersAsync(data);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error on initialize positions");
            return false;
        }
    }
}
