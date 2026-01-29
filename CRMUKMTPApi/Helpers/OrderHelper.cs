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

public class OrderHelper
{
    private readonly ILogger<OrderHelper> _logger;
    private readonly AppQueue<Tuple<TradeEvent, ManagerOrder>> _queue;
    private readonly COrderSink _orderSink;
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly MT5LIBHelper _helper;

    public OrderHelper(ILogger<OrderHelper> logger, COrderSink orderSink,
        MT5LIBHelper helper, IServiceScopeFactory serviceScopeFactory)
    {
        _logger = logger;
        _orderSink = orderSink;
        _helper = helper;
        _serviceScopeFactory = serviceScopeFactory;
        _orderSink.OrderUpdate += OrderUpdate;
        _queue = new AppQueue<Tuple<TradeEvent, ManagerOrder>>(PrcessOrder);
    }

    private void OrderUpdate(TradeEvent tradeEvent, ManagerOrder data)
    {
        _queue.Enqueue(Tuple.Create(tradeEvent, data));
    }

    private async Task PrcessOrder(Tuple<TradeEvent, ManagerOrder> tuple)
    {
        try
        {
            MessageState messageState = MessageState.New;
            var tradeEvent = tuple.Item1;
            var order = tuple.Item2;
            using var scope = _serviceScopeFactory.CreateScope();
            var repository = scope.ServiceProvider.GetRequiredService<IOrderRepository>();
            if (tradeEvent == TradeEvent.Modify)
            {
                await repository.UpdateAsync(order);
                 messageState = MessageState.Update;
            }
            else if (tradeEvent == TradeEvent.Delete)
            {
                await repository.DeleteAsync(order);
                messageState = MessageState.Delete;
            }
            else if (tradeEvent == TradeEvent.Perform)
            {
                await repository.AddAsync(order);
            }
            ByteString stringData = Globals.ConvertToByteString<ManagerOrder>(order);
            await Globals.BroadcastData<ManagerOrder>(order.Login,messageState, MessageType.Order, stringData);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Fail on process deal");
        }
    }
    public async Task<bool> InitializeOrder()
    {
        try
        {
            using var scope = _serviceScopeFactory.CreateScope();
            var dealRepository = scope.ServiceProvider.GetRequiredService<IOrderRepository>();
            if (dealRepository == null) return false;

            DateTime fromDate = await dealRepository.GetMaxTime();
            fromDate = fromDate == DateTime.MinValue ? new DateTime(2015, 01, 01) : fromDate;

            var orders = _helper.GetOrders(fromDate, DateTime.Now.AddDays(1));
            if (orders == null) return false;
          
            await dealRepository.AddOrUpdateUsersAsync(orders);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error on initialize order");
            return false;
        }
    }
}
