using CRMUKMTPApi.Models;
using CRMUKMTPApi.Repositories;
using MediatR;
using MetaQuotes.MT5CommonAPI;
using MT5LIB.Helpers;
using MT5LIB.Models;

namespace CRMUKMTPApi.QueryHandler;

public class GetSummaryQuery : IRequest<ApiResponse>
{
    public GetSummaryQuery(ParamModel param)
    {
        Params = param;
    }

    public ParamModel Params { get; set; }

}
public class GetSummaryHandler : IRequestHandler<GetSummaryQuery, ApiResponse>
{
    private readonly ILogger<GetSummaryHandler> _logger;
    private readonly ISummaryRepository _repository;
    private readonly MT5LIBHelper _helper;

    public GetSummaryHandler(ILogger<GetSummaryHandler> logger, ISummaryRepository repository, MT5LIBHelper helper)
    {
        _logger = logger;
        _repository = repository;
        _helper = helper;
    }

    public async Task<ApiResponse> Handle(GetSummaryQuery request, CancellationToken cancellationToken)
    {
        try
        {
            bool sort = !request.Params.Sort.StartsWith('-');
            var data = await _repository.GetAsync(request.Params);
            if (data.Item3)
            {
                return new ApiResponse
                {
                    data = data.Item1,
                    page = request.Params.Page,
                    pageSize = request.Params.Limit,
                    result = data.Item1.Count,
                    status = "success",
                    totalPages = (int)Math.Ceiling(data.Item2 / (double)request.Params.Limit),
                    totalRecords = data.Item2
                };

            }
            return new ApiResponse
            {
                data = new List<ManagerDeal>(),
                page = request.Params.Page,
                pageSize = request.Params.Limit,
                result = 0,
                status = "success",
                totalPages = 0,
                totalRecords = 0
            };
        }
        catch (Exception ex)
        {
            return new ApiResponse();
        }
    }
    //public async Task<ApiResponse> Handle(GetSummaryQuery request, CancellationToken cancellationToken)
    //{
    //    try
    //    {

    //        var deals = await _repository.GetAsync(new ParamModel
    //        {
    //            Loginid = request.Params.Loginid
    //        });

    //        if (deals.Item3)
    //        {

    //            var remainDeals = deals.Item1.ToList();
    //            Dictionary<ulong, ManagerSummaryReport> dctSummary = new Dictionary<ulong, ManagerSummaryReport>();
    //            foreach (var deal in remainDeals)
    //            {

    //                var user = _helper.GetUser(deal.Login);


    //                if (!dctSummary.TryGetValue(deal.Login, out ManagerSummaryReport? summaryReport))
    //                {

    //                    summaryReport = new ManagerSummaryReport
    //                    {
    //                        ClientName = deal.ClientName,
    //                        Currency = deal.Currency,
    //                        LoginId = deal.Login,
    //                        CurrentBalance = user == null ? 0 : user.Balance,
    //                        CurrencyDigits = user == null ? 0 : user.CurrencyDigits
    //                    };
    //                    dctSummary.TryAdd(deal.Login, summaryReport);
    //                }
    //                switch (deal.Action)
    //                {
    //                    case 0:
    //                    case 1:
    //                        summaryReport.Profit = SMTMath.MoneyAdd(summaryReport.Profit, deal.Profit, (byte)summaryReport.CurrencyDigits);
    //                        summaryReport.Swap = SMTMath.MoneyAdd(summaryReport.Swap, deal.Swap, (byte)summaryReport.CurrencyDigits);
    //                        summaryReport.Commission = SMTMath.MoneyAdd(summaryReport.Commission, deal.CommissionFee, (byte)summaryReport.CurrencyDigits);
    //                        summaryReport.Fee = SMTMath.MoneyAdd(summaryReport.Fee, deal.Fee, (byte)summaryReport.CurrencyDigits);
    //                        summaryReport.Volume += deal.Volume;
    //                        break;
    //                    case 2:
    //                        if (deal.Profit >= 0)
    //                            summaryReport.Deposit = SMTMath.MoneyAdd(summaryReport.Deposit, deal.Profit, (byte)summaryReport.CurrencyDigits);
    //                        else
    //                            summaryReport.Withdraw = SMTMath.MoneyAdd(summaryReport.Withdraw, deal.Profit, (byte)summaryReport.CurrencyDigits);
    //                        break;
    //                    case 3:
    //                        summaryReport.Credit = SMTMath.MoneyAdd(summaryReport.Credit, deal.Profit, (byte)summaryReport.CurrencyDigits);
    //                        break;
    //                    case 4:
    //                    case 5:
    //                    case 6:
    //                        summaryReport.Additional = SMTMath.MoneyAdd(summaryReport.Additional, deal.Profit, (byte)summaryReport.CurrencyDigits);
    //                        break;
    //                    case 7:
    //                    case 8:
    //                    case 9:
    //                        summaryReport.Commission = SMTMath.MoneyAdd(summaryReport.Commission, deal.Profit, (byte)summaryReport.CurrencyDigits);
    //                        break;
    //                    case 10:
    //                    case 11:
    //                    case 12:
    //                        summaryReport.Additional = SMTMath.MoneyAdd(summaryReport.Additional, deal.Profit, (byte)summaryReport.CurrencyDigits);
    //                        break;
    //                    case 15:
    //                    case 16:
    //                    case 17:
    //                    case 18:
    //                        summaryReport.Additional = SMTMath.MoneyAdd(summaryReport.Additional, deal.Profit, (byte)summaryReport.CurrencyDigits);
    //                        break;
    //                    default:
    //                        break;
    //                }
    //                summaryReport.InOut = SMTMath.MoneyAdd(summaryReport.Deposit, summaryReport.Withdraw, (byte)summaryReport.CurrencyDigits);


    //            }
    //            return new ApiResponse
    //            {
    //                data = dctSummary.Values,
    //                page = request.Params.Page,
    //                pageSize = request.Params.Limit,
    //                result = dctSummary.Count,
    //                status = "success",
    //                totalPages = request.Params.Page,
    //                totalRecords = dctSummary.Count,
    //            };
    //        }
    //        return new ApiResponse
    //        {
    //            data = new List<ManagerDeal>(),
    //            page = request.Params.Page,
    //            pageSize = request.Params.Limit,
    //            result = 0,
    //            status = "success",
    //            totalPages = 0,
    //            totalRecords = 0
    //        };

    //    }
    //    catch (Exception ex)
    //    {
    //        return new ApiResponse();
    //    }
    //}
}
