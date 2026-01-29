using MetaQuotes.MT5CommonAPI;
using MT5LIB.Enums;
using MT5LIB.Models;
using System;

namespace MT5LIB.Helpers;

public class MT5LIBHelper
{
    public IEnumerable<ManagerUser>? GetUsers()
    {
        if (Utilities.Manager == null) return null;
        var userArray = Utilities.Manager.UserCreateArray();
        if (Utilities.Manager.UserRequestArray("*", userArray) == MTRetCode.MT_RET_OK)
        {

            List<ManagerUser> users = new List<ManagerUser>();
            int count = userArray.ToArray().Count();
            if (count == 0) return Enumerable.Empty<ManagerUser>();
            for (uint i = 0; i < count; i++)
            {
                var user = userArray.Next(i);
                ManagerUser managerUser = Utilities.GetUser(user);
                Utilities.dctUser.TryAdd(managerUser.LoginId, managerUser);
                users.Add(managerUser);
            }
            return users;
        }
        return null;
    }
    public IEnumerable<ManagerOrder>? GetOrders(DateTime from, DateTime to)
    {
        if (Utilities.Manager == null) return null;
        var orderCreateArray = Utilities.Manager.OrderCreateArray();
        if (Utilities.Manager.HistoryRequestByGroup("*", Utilities.ConvertInSeconds(from), Utilities.ConvertInSeconds(to), orderCreateArray) == MTRetCode.MT_RET_OK)
        {
            int count = orderCreateArray.ToArray().Count();
            if (count == 0) return Enumerable.Empty<ManagerOrder>();
            List<ManagerOrder> orders = new List<ManagerOrder>();
            for (uint i = 0; i < count; i++)
            {
                var order = orderCreateArray.Next(i);
                ManagerOrder orderLoad = Utilities.GetOrderLoad(order);
                orders.Add(orderLoad);
            }
            return orders;
        }
        return null;
    }
    public IEnumerable<ManagerDeal>? GetDeals(DateTime from, DateTime to)
    {
        if (Utilities.Manager == null) return null;
        var dealCreateArray = Utilities.Manager.DealCreateArray();
        if (Utilities.Manager.DealRequestByGroup("*", Utilities.ConvertInSeconds(from), Utilities.ConvertInSeconds(to), dealCreateArray) == MTRetCode.MT_RET_OK)
        {
            List<ManagerDeal> deals = new List<ManagerDeal>();
            int count = dealCreateArray.ToArray().Count();
            if (count == 0) return Enumerable.Empty<ManagerDeal>();
            for (uint i = 0; i < count; i++)
            {
                var deal = dealCreateArray.Next(i);
                ManagerDeal dealLoad = Utilities.GetDealLoad(deal);


                deals.Add(dealLoad);
            }
            return deals;
        }
        return null;
    }
    public IEnumerable<ManagerPosition>? GetPositions()
    {
        if (Utilities.Manager == null) return null;
        CIMTPositionArray positionArray = Utilities.Manager.PositionCreateArray();
        if (Utilities.Manager.PositionRequestByGroup("*", positionArray) == MTRetCode.MT_RET_OK)
        {
            List<ManagerPosition> positions = new List<ManagerPosition>();
            int count = positionArray.ToArray().Count();
            if (count == 0) return null;
            for (uint i = 0; i < count; i++)
            {
                CIMTPosition position = positionArray.Next(i);
                ManagerPosition positionLoad = Utilities.GetPositions(position);
                positions.Add(positionLoad);
            }
            return positions;
        }
        return null;

    }
    public ManagerUser? GetUser(ulong login)
    {
        Utilities.dctUser.TryGetValue(login, out var user);
        return user;
    }
    public IEnumerable<ManagerDailyReport>? GetDailySummary(long from, long to)
    {
        if (Utilities.Manager == null) return null;

        CIMTDailyArray dailyArray = Utilities.Manager.DailyCreateArray();
        if (Utilities.Manager.DailyRequestByGroup("*", from, to, dailyArray) == MTRetCode.MT_RET_OK)
        {
            List<ManagerDailyReport> dailySummaries = new List<ManagerDailyReport>();
            int count = dailyArray.ToArray().Count();
            if (count == 0) return null;
            for (uint i = 0; i < count; i++)
            {
                CIMTDaily daily = dailyArray.Next(i);
                ManagerDailyReport managerDailyReport = Utilities.GetDailyLoad(daily);

                dailySummaries.Add(managerDailyReport);
            }
            return dailySummaries;
        }
        return null;

    }
    public void Subscribe(string symbol)
    {
        if (Utilities.Manager == null) return;
        var order = Utilities.Manager.OrderCreate();
        order.Login(1013);
        order.Symbol(symbol);
        order.PriceOrder(25);
        var newOrder = Utilities.Manager.OrderAdd(order);

        MTTick mTTick = new MTTick();
        mTTick.flags = MTTick.EnTickFlags.FLAG_TICK_ALL;
        mTTick.symbol = symbol;

        var result = Utilities.Manager.TickAdd(symbol, mTTick);
    }

}
