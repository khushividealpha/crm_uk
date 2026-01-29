using MetaQuotes.MT5CommonAPI;
using MetaQuotes.MT5ManagerAPI;
using MT5LIB.Enums;
using MT5LIB.Models;
using System.Collections.Concurrent;
using System.Xml.Linq;


namespace MT5LIB.Helpers;

public class Utilities
{
    internal static ConcurrentDictionary<ulong, ManagerUser> dctUser = new();
    internal static ConcurrentDictionary<ulong, ManagerSummaryReport> dctSummary = new();
    internal static ConcurrentDictionary<string, string> dctSymbol = new();
    private static readonly DateTime UnixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
    public static CIMTManagerAPI? Manager = null;
    public static DateTime DateTimeFromUnixTimestampSeconds(long seconds) => UnixEpoch.AddSeconds((double)seconds);
    public static DateTime DateTimeFromUnixTimestampMillis(long millis) => UnixEpoch.AddMilliseconds((double)millis);
    public static TradeType GetOrderType(CIMTOrder order)
    {

        switch (order.Type())
        {
            case 2:
                return TradeType.BuyLimit;
            case 3:
                return TradeType.SellLimit;
            case 4:
                return TradeType.BuyStop;
            case 5:
                return TradeType.SellStop;
            case 6:
                return TradeType.BuyStopLimit;
            case 7:
                return TradeType.SellStopLimit;
            default:
                return TradeType.None;

        }
    }
    public static TradeType GetDealType(uint action, double profit)
    {
        switch (action)
        {
            case 0:
                return TradeType.Buy;
            case 1:
                return TradeType.Sell;
            case 2:
                return profit >= 0.0 ? TradeType.Deposit : TradeType.Withdrawal;
            case 3:
                return profit >= 0.0 ? TradeType.CreditIn : TradeType.CreditOut;
            case 4:
                return profit >= 0.0 ? TradeType.ChargeIn : TradeType.ChargeOut;
            case 5:
                return profit >= 0.0 ? TradeType.CorrectionIn : TradeType.CorrectionOut;
            case 6:
                return profit >= 0.0 ? TradeType.BonusIn : TradeType.BonusOut;
            case 7:
                return TradeType.Commission;
            case 8:
                return TradeType.CommissionDaily;
            case 9:
                return TradeType.CommissionMonthly;
            case 10:
                return TradeType.AgentDaily;
            case 11:
                return TradeType.AgentMonthly;
            case 12:
                return TradeType.Interestrate;
            case 13:
                return TradeType.BuyCanceled;
            case 14:
                return TradeType.SellCanceled;
            case 15:
                return TradeType.Divident;
            case 16:
                return TradeType.DividentFranked;
            case 17:
                return TradeType.Tax;
            case 18:
                return TradeType.Agent;
            case 19:
                return TradeType.SoCompensation;
            case 20:
                return TradeType.SOCompensationCredit;
            default:
                return TradeType.None;

        }
    }
    public static TradeEvent GetDealEventType(CIMTDeal deal)
    {
        TradeEvent tradeEvent = TradeEvent.None;
        CIMTDeal.EnDealAction dealAction = (CIMTDeal.EnDealAction)deal.Action();
        var dealEntry = (CIMTDeal.EnEntryFlag)deal.Entry();
        switch (dealAction)
        {
            case CIMTDeal.EnDealAction.DEAL_BUY:
            case CIMTDeal.EnDealAction.DEAL_SELL:
                switch (dealEntry)
                {
                    case CIMTDeal.EnEntryFlag.ENTRY_IN:
                        tradeEvent = TradeEvent.Open;
                        break;
                    case CIMTDeal.EnEntryFlag.ENTRY_OUT:
                    case CIMTDeal.EnEntryFlag.ENTRY_INOUT:
                    case CIMTDeal.EnEntryFlag.ENTRY_OUT_BY:
                        tradeEvent = TradeEvent.Close;
                        break;
                }
                break;
        }
        return tradeEvent;
    }
    public static void PrintSuccess(string message)
    {
        if (Manager == null) return;
        Manager.LoggerOut(EnMTLogCode.MTLogOK, message);
    }
    public static void PrintError(string message)
    {
        if (Manager == null) return;
        Manager.LoggerOut(EnMTLogCode.MTLogErr, message);
    }
    public static void PrintWarning(string message)
    {
        if (Manager == null) return;
        Manager.LoggerOut(EnMTLogCode.MTLogWarn, message);
    }
    public static long ConvertInSeconds(DateTime dateTime)
    {
        return (long)(dateTime - UnixEpoch).TotalSeconds;
    }
    public static string GetTranstype(uint type)
    {
        string s_type = "";
        switch (type)
        {
            case 0:
                s_type = "Buy";
                break;
            case 1:
                s_type = "Sell";
                break;
            case 2:
                s_type = "Buy Limit";
                break;
            case 3:
                s_type = "Sell Limit";
                break;
            case 4:
                s_type = "Buy Stop";
                break;
            case 5:
                s_type = "Sell Stop";
                break;
            case 6:
                s_type = "Buy Stop Limit";
                break;
            case 7:
                s_type = "Sell Stop Limit";
                break;
            case 8:
                s_type = "Close By";
                break;
            default:
                s_type = "";
                break;
        }
        return s_type;
    }
    public static string GetReason(uint reason)
    {
        string s_reason = "";
        switch (reason)
        {
            case 0:
                s_reason = "Client";
                break;
            case 1:
                s_reason = "Expert";
                break;
            case 2:
                s_reason = "Dealer";
                break;
            case 3:
                s_reason = "SL";
                break;
            case 4:
                s_reason = "TP";
                break;
            case 5:
                s_reason = "SO";
                break;
            case 6:
                s_reason = "Rollover";
                break;
            case 7:
                s_reason = "External Client";
                break;
            case 8:
                s_reason = "Vmargin";
                break;
            case 9:
                s_reason = "Gateway";
                break;
            case 10:
                s_reason = "Signal";
                break;
            case 11:
                s_reason = "Settlement";
                break;
            case 12:
                s_reason = "Transfer";
                break;
            case 13:
                s_reason = "Sync";
                break;
            case 14:
                s_reason = "External Service";
                break;
            case 0xF:
                s_reason = "Migration";
                break;
            case 0x10:
                s_reason = "Mobile";
                break;
            case 17:
                s_reason = "Web";
                break;
            case 18:
                s_reason = "Split";
                break;
            default:
                s_reason = "";
                break;
        }
        return s_reason;
    }
    internal static string GetEntry(CIMTDeal deal)
    {
        string entry = "";
        switch (deal.Entry())
        {
            case 0:
                entry = "In";
                break;
            case 1:
                entry = "Out";
                break;
            case 2:
                entry = "In/Out";
                break;
            case 3:
                entry = "Out By";
                break;
            default:
                entry = "";
                break;
        }
        return entry;
    }
    internal static string GetState(CIMTOrder order)
    {
        string s_state = "";
        switch (order.State())
        {
            case 0:
                s_state = "Started";
                break;
            case 1:
                s_state = "Placed";
                break;
            case 2:
                s_state = "Canceled";
                break;
            case 3:
                s_state = "Partial";
                break;
            case 4:
                s_state = "Filled";
                break;
            case 5:
                s_state = "Rejected";
                break;
            case 6:
                s_state = "Expired";
                break;
            case 7:
                s_state = "Request Add";
                break;
            case 8:
                s_state = "Request Modify";
                break;
            case 9:
                s_state = "Request Cancel";
                break;
            default:
                s_state = "";
                break;
        }
        return s_state;
    }
    public static ManagerOrder GetOrderLoad(CIMTOrder order)
    {


        var loginId = order.Login();
        var user = GetUser(loginId);
        var date = Utilities.DateTimeFromUnixTimestampMillis(order.TimeDoneMsc());
        var d = order.VolumeCurrentExt();
        var c = order.VolumeInitialExt();
        var v = order.VolumeInitial();
        return new ManagerOrder
        {
            OrderId = order.Order(),
            ClientName = user == null ? "Not Found" : user.Name,
            Login = loginId,
            Tp = order.PriceTP(),
            Comment = order.Comment(),
            DoneTime = date/*.ToString("yyyy-MM-dd HH:mm:ss.fff")*/,
            Position = order.PositionID(),
            Price = order.PriceOrder(),
            Reason = Utilities.GetReason(order.Reason()),
            SetupTime = Utilities.DateTimeFromUnixTimestampMillis(order.TimeSetupMsc())/*.ToString("yyyy-MM-dd HH:mm:ss.fff")*/,
            Sl = order.PriceSL(),
            State = GetState(order),
            Symbol = order.Symbol(),
            Type = GetOrderType(order),
            VolumeFilled = order.VolumeInitial() / 10000.0,
            VolumeTotal = (order.VolumeInitial() - order.VolumeCurrent()) / 10000.0,
            Demo = user == null ? false : user.Demo,


        };
    }
    internal static ManagerDeal GetDealLoad(CIMTDeal deal)
    {

        var loginId = deal.Login();
        var user = GetUser(loginId);

        //string currency = GetCurrency(deal);

        DateTime date = Utilities.DateTimeFromUnixTimestampMillis(deal.TimeMsc());

        return new ManagerDeal
        {
            DealId = deal.Deal(),
            LoginId = loginId,
            Comment = deal.Comment(),
            OrderId = deal.Order(),
            Position = deal.PositionID(),
            CommissionFee = deal.Commission(),
            ClientName = user == null ? string.Empty : user.Name,
            //LastName = user == null ? string.Empty : user.LastName,
            Dealer = deal.Dealer(),
            Entry = GetEntry(deal),
            Volume = deal.Volume() / 10000.0,
            Price = deal.Price(),
            Profit = deal.Profit(),
            Sl = deal.PriceSL(),
            Reason = GetReason(deal.Reason()),
            Symbol = deal.Symbol(),
            Type = Utilities.GetDealType(deal.Action(), deal.Profit()),
            Tp = deal.PriceTP(),
            Time = date,
            Swap = deal.Storage(),
            MarketAsk = deal.MarketAsk(),
            MarketBid = deal.MarketBid(),
            Currency = user == null ? string.Empty : user.GroupCurrency,
            Demo = user == null ? false : user.Demo,
            Action = deal.Action(),
            //Group = user == null ? string.Empty : user.Group,
        };
    }
    public static ManagerUser? GetUser(ulong loginId)
    {
        if (Utilities.dctUser.TryGetValue(loginId, out var user))
        { return user; }
        return null;
    }
    internal static ManagerUser GetUser(CIMTUser user)
    {
        var loginId = user.Login();
        string currency = string.Empty;
        uint cDigit = 0;
        if (Manager != null)
        {
            var group = Manager.GroupCreate();
            Utilities.Manager.GroupGet(user.Group(), group);
            currency = group.Currency();
            cDigit = group.CurrencyDigits();

        }
        string name = user.FirstName().Replace("\n", "");
        string groupName = user.Group().Replace("\n", ""); ;
        string lastName = user.LastName().Replace("\n", "");
       
        return new ManagerUser
        {

            LoginId = loginId,
            Group = groupName,
            LastName = lastName,
            Name = name,
            Client = user.ClientID(),
            Address = user.Address(),
            Balance = user.Balance(),
            City = user.City(),
            Comment = user.Comment(),
            Country = user.Country(),
            Created = Utilities.DateTimeFromUnixTimestampSeconds(user.Registration()),
            Credit = user.Credit(),
            Email = user.EMail(),
            Equity = user.EquityPrevDay(),
            LastAccess = Utilities.DateTimeFromUnixTimestampSeconds(user.LastAccess()).ToString(),
            Phone = user.Phone(),
            Zipcode = user.ZIPCode(),
            Status = user.Status(),
            Leverage = user.Leverage(),
            Demo = user.Group().ToLower().Contains("demo") ? true : false,
            GroupCurrency = currency,
            CurrencyDigits = cDigit,
            Enabled = (uint)user.Rights() > 0 ? true : false,
            
        };
    }
    internal static ManagerPosition GetPositions(CIMTPosition position)
    {
        return new ManagerPosition
        {
            Comment = position.Comment(),
            Id = position.ExpertID(),
            LoginId = position.Login(),
            PositionId = position.Position(),
            PriceCurr = position.PriceCurrent(),
            PriceOpen = position.PriceOpen(),
            Profit = position.Profit(),
            Reason = Utilities.GetReason(position.Reason()),
            Sl = position.PriceSL(),
            Swap = position.Storage(),
            Symbol = position.Symbol(),
            Time = Utilities.DateTimeFromUnixTimestampSeconds(position.TimeCreate()),
            Tp = position.PriceTP(),
            Type = Utilities.GetDealType(position.Action(), position.Profit()),
            Volume = position.Volume() / 10000.0,
        };
    }
    internal static ManagerDailyReport GetDailyLoad(CIMTDaily daily)
    {
        try
        {
            var user = Utilities.GetUser(daily.Login());

            double closedPL = daily.DailyProfit();
            closedPL = SMTMath.MoneyAdd(closedPL, daily.DailyStorage(), (byte)daily.CurrencyDigits());
            closedPL = SMTMath.MoneyAdd(closedPL, daily.DailyCommInstant(), (byte)daily.CurrencyDigits());

            double floatingPL = daily.Profit();
            floatingPL = SMTMath.MoneyAdd(floatingPL, daily.ProfitStorage(), (byte)daily.CurrencyDigits());

            return new ManagerDailyReport
            {
                Balance = daily.Balance(),
                ClientName = user == null ? string.Empty : user.Name,
                Currency = user == null ? string.Empty : user.GroupCurrency,
                Date = Utilities.DateTimeFromUnixTimestampSeconds(daily.Datetime()),
                PrevBalance = daily.BalancePrevDay(),
                Deposit = daily.DailyBalance(),
                FloatingPL = floatingPL,
                ClosedPL = closedPL,
                Credit = daily.Credit(),
                Equity = daily.ProfitEquity(),
                Margin = daily.Margin(),
                MarginFree = daily.MarginFree(),
                Demo = user == null ? false : user.Demo,
                Email = user == null ? string.Empty : user.Email,
                Group = user == null ? string.Empty : user.Group,
                LoginId = daily.Login(),
            };
        }
        catch (Exception)
        {

            throw;
        }
    }
}
