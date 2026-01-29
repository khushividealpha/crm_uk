using Apmcrm.V1.Msgs;
using Apmcrm.V1.Msgs.Types;
using CLIB.Helpers;
using CRMUKMTPApi.Repositories;
using Google.Protobuf;
using MetaQuotes.MT5CommonAPI;
using MT5LIB;
using MT5LIB.Enums;
using MT5LIB.Helpers;
using MT5LIB.Models;
using MySqlX.XDevAPI;
using System.Collections.Concurrent;
using System.Net.WebSockets;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace CRMUKMTPApi.Helpers;

public class DealHelper
{
    private readonly ILogger<DealHelper> _logger;
    private readonly AppQueue<Tuple<TradeEvent, ManagerDeal>> _queue;
    private readonly CDealSink _dealSink;
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly MT5LIBHelper _helper;
    private ConcurrentDictionary<ulong, ManagerSummaryReport> dctSummary = new();

    public DealHelper(ILogger<DealHelper> logger, CDealSink dealSink,
        MT5LIBHelper helper, IServiceScopeFactory serviceScopeFactory)
    {
        _logger = logger;
        _dealSink = dealSink;
        _helper = helper;
        _serviceScopeFactory = serviceScopeFactory;
        _dealSink.DealUpdate += DealUpdate;
        _queue = new AppQueue<Tuple<TradeEvent, ManagerDeal>>(PrcessDeal);
    }

    private void DealUpdate(TradeEvent tradeEvent, ManagerDeal data)
    {
        _queue.Enqueue(Tuple.Create(tradeEvent, data));
    }

    private async Task PrcessDeal(Tuple<TradeEvent, ManagerDeal> tuple)
    {
        try
        {
            MessageState messageState = MessageState.New;
            var tradeEvent = tuple.Item1;
            var deal = tuple.Item2;
            using var scope = _serviceScopeFactory.CreateScope();
            var repository = scope.ServiceProvider.GetRequiredService<IDealRepository>();

            if (tradeEvent == TradeEvent.Modify)
            {
                await repository.UpdateAsync(deal);
                messageState = MessageState.Update;
            }
            else if (tradeEvent == TradeEvent.Delete)
            {
                await repository.DeleteAsync(deal);
                messageState = MessageState.Delete;
            }
            else if (tradeEvent == TradeEvent.Perform)
            {
                await repository.AddAsync(deal);
                await InitializeSummaryAsync(deal);
                await SendTransaction(deal);
            }
            ByteString stringData = Globals.ConvertToByteString<ManagerDeal>(deal);
            await Globals.BroadcastData<ManagerDeal>(deal.LoginId, messageState, MessageType.Deal, stringData);


        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Fail on process deal");
        }
    }
    public async Task<bool> InitializeDeal()
    {
        try
        {
            using var scope = _serviceScopeFactory.CreateScope();
            var dealRepository = scope.ServiceProvider.GetRequiredService<IDealRepository>();
            if (dealRepository == null) return false;

            DateTime fromDate = await dealRepository.GetMaxTime();
            fromDate = fromDate == DateTime.MinValue ? new DateTime(2015, 01, 01) : fromDate;

            var deals = _helper.GetDeals(fromDate, DateTime.Now.AddDays(1));
            if (deals == null) return false;

            await dealRepository.AddOrUpdateUsersAsync(deals);
            var allDeals = await dealRepository.GetAsync();
            await InitializeSummaryAsync(allDeals);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error on initialize order");
            return false;
        }
    }
    private async Task InitializeSummaryAsync(ManagerDeal deal)
    {
        try
        {

            if (!dctSummary.TryGetValue(deal.LoginId, out ManagerSummaryReport? summaryReport))
            {
                var user = _helper.GetUser(deal.LoginId);
                summaryReport = new ManagerSummaryReport
                {
                    ClientName = deal.ClientName,
                    Currency = deal.Currency,
                    LoginId = deal.LoginId,
                    CurrentBalance = user == null ? 0 : user.Balance,
                    CurrencyDigits = user == null ? 0 : user.CurrencyDigits
                };
                dctSummary.TryAdd(deal.LoginId, summaryReport);

            }
            switch (deal.Action)
            {
                case 0:
                case 1:
                    summaryReport.Profit = SMTMath.MoneyAdd(summaryReport.Profit, deal.Profit, (byte)summaryReport.CurrencyDigits);
                    summaryReport.Swap = SMTMath.MoneyAdd(summaryReport.Swap, deal.Swap, (byte)summaryReport.CurrencyDigits);
                    summaryReport.Commission = SMTMath.MoneyAdd(summaryReport.Commission, deal.CommissionFee, (byte)summaryReport.CurrencyDigits);
                    summaryReport.Fee = SMTMath.MoneyAdd(summaryReport.Fee, deal.Fee, (byte)summaryReport.CurrencyDigits);
                    summaryReport.Volume += deal.Volume;
                    break;
                case 2:
                    if (deal.Profit >= 0)
                        summaryReport.Deposit = SMTMath.MoneyAdd(summaryReport.Deposit, deal.Profit, (byte)summaryReport.CurrencyDigits);
                    else
                        summaryReport.Withdraw = SMTMath.MoneyAdd(summaryReport.Withdraw, deal.Profit, (byte)summaryReport.CurrencyDigits);
                    break;
                case 3:
                    summaryReport.Credit = SMTMath.MoneyAdd(summaryReport.Credit, deal.Profit, (byte)summaryReport.CurrencyDigits);
                    break;
                case 4:
                case 5:
                case 6:
                    summaryReport.Additional = SMTMath.MoneyAdd(summaryReport.Additional, deal.Profit, (byte)summaryReport.CurrencyDigits);
                    break;
                case 7:
                case 8:
                case 9:
                    summaryReport.Commission = SMTMath.MoneyAdd(summaryReport.Commission, deal.Profit, (byte)summaryReport.CurrencyDigits);
                    break;
                case 10:
                case 11:
                case 12:
                    summaryReport.Additional = SMTMath.MoneyAdd(summaryReport.Additional, deal.Profit, (byte)summaryReport.CurrencyDigits);
                    break;
                case 15:
                case 16:
                case 17:
                case 18:
                    summaryReport.Additional = SMTMath.MoneyAdd(summaryReport.Additional, deal.Profit, (byte)summaryReport.CurrencyDigits);
                    break;
                default:
                    break;
            }
            summaryReport.InOut = SMTMath.MoneyAdd(summaryReport.Deposit, summaryReport.Withdraw, (byte)summaryReport.CurrencyDigits);
            using var scope = _serviceScopeFactory.CreateScope();
            var repository = scope.ServiceProvider.GetRequiredService<ISummaryRepository>();
            await repository.AddOrUpdateUsersAsync(new List<ManagerSummaryReport> { summaryReport });

            ByteString stringData = Globals.ConvertToByteString<ManagerSummaryReport>(summaryReport);
            await Globals.BroadcastData<ManagerSummaryReport>(summaryReport.LoginId, MessageState.New, MessageType.Summary, stringData);
        }
        catch (Exception ex) { }
    }
    private async Task InitializeSummaryAsync(IEnumerable<ManagerDeal>? allDeals)
    {
        try
        {
            if (allDeals == null)
            {
                _logger.LogInformation("Deals not found for create summary");
                return;
            }
            using var scope = _serviceScopeFactory.CreateScope();
            var repository = scope.ServiceProvider.GetRequiredService<ISummaryRepository>();
            var summaries = await repository.GetAsync();

            if (summaries == null) return;

            dctSummary = new(summaries.ToDictionary(x => x.LoginId, x => x));

            foreach (var deal in allDeals)
            {

                if (!dctSummary.TryGetValue(deal.LoginId, out ManagerSummaryReport? summaryReport))
                {
                    var user = _helper.GetUser(deal.LoginId);
                    summaryReport = new ManagerSummaryReport
                    {
                        ClientName = deal.ClientName,
                        Currency = deal.Currency,
                        LoginId = deal.LoginId,
                        CurrentBalance = user == null ? 0 : user.Balance,
                        CurrencyDigits = user == null ? 0 : user.CurrencyDigits
                    };
                    dctSummary.TryAdd(deal.LoginId, summaryReport);
                }
                switch (deal.Action)
                {
                    case 0:
                    case 1:
                        summaryReport.Profit = SMTMath.MoneyAdd(summaryReport.Profit, deal.Profit, (byte)summaryReport.CurrencyDigits);
                        summaryReport.Swap = SMTMath.MoneyAdd(summaryReport.Swap, deal.Swap, (byte)summaryReport.CurrencyDigits);
                        summaryReport.Commission = SMTMath.MoneyAdd(summaryReport.Commission, deal.CommissionFee, (byte)summaryReport.CurrencyDigits);
                        summaryReport.Fee = SMTMath.MoneyAdd(summaryReport.Fee, deal.Fee, (byte)summaryReport.CurrencyDigits);
                        summaryReport.Volume += deal.Volume;
                        break;
                    case 2:
                        if (deal.Profit >= 0)
                            summaryReport.Deposit = SMTMath.MoneyAdd(summaryReport.Deposit, deal.Profit, (byte)summaryReport.CurrencyDigits);
                        else
                            summaryReport.Withdraw = SMTMath.MoneyAdd(summaryReport.Withdraw, deal.Profit, (byte)summaryReport.CurrencyDigits);
                        break;
                    case 3:
                        summaryReport.Credit = SMTMath.MoneyAdd(summaryReport.Credit, deal.Profit, (byte)summaryReport.CurrencyDigits);
                        break;
                    case 4:
                    case 5:
                    case 6:
                        summaryReport.Additional = SMTMath.MoneyAdd(summaryReport.Additional, deal.Profit, (byte)summaryReport.CurrencyDigits);
                        break;
                    case 7:
                    case 8:
                    case 9:
                        summaryReport.Commission = SMTMath.MoneyAdd(summaryReport.Commission, deal.Profit, (byte)summaryReport.CurrencyDigits);
                        break;
                    case 10:
                    case 11:
                    case 12:
                        summaryReport.Additional = SMTMath.MoneyAdd(summaryReport.Additional, deal.Profit, (byte)summaryReport.CurrencyDigits);
                        break;
                    case 15:
                    case 16:
                    case 17:
                    case 18:
                        summaryReport.Additional = SMTMath.MoneyAdd(summaryReport.Additional, deal.Profit, (byte)summaryReport.CurrencyDigits);
                        break;
                    default:
                        break;
                }
                summaryReport.InOut = SMTMath.MoneyAdd(summaryReport.Deposit, summaryReport.Withdraw, (byte)summaryReport.CurrencyDigits);


            }

            if (dctSummary.Any())
            {


                await repository.AddOrUpdateUsersAsync(dctSummary.Values);
            }

        }
        catch (Exception ex) { }
    }
    public async Task SendTransaction(ManagerDeal data)
    {
        
        var trans = new ManagerTransaction
        {
            Amount = data.Profit,
            Comment = data.Comment,
            Currency = data.Currency,
            DealId = data.DealId,
            DealTime = data.Time,
            Demo = data.Demo,
            Login = data.LoginId,
            Name = data.ClientName,
            Type = data.Type,

        };
        ByteString stringData = Globals.ConvertToByteString<ManagerTransaction>(trans);
        await Globals.BroadcastData<ManagerDeal>(trans.Login, MessageState.New, MessageType.Transaction, stringData);
    }
}
