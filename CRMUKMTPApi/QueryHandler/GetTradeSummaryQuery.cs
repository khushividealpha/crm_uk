using Apmcrm.V1.Msgs;
using CRMUKMTPApi.Helpers;
using CRMUKMTPApi.Models;
using CRMUKMTPApi.Repositories;
using MediatR;
using MT5LIB.Helpers;
using MT5LIB.Models;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace CRMUKMTPApi.QueryHandler;

public class GetTradeSummaryQuery : IRequest<ApiResponse>
{
    public GetTradeSummaryQuery(ParamModel param)
    {
        Params = param;
    }

    public ParamModel Params { get; set; }
    

}

public class GetTradeSummaryQueryHandler : IRequestHandler<GetTradeSummaryQuery, ApiResponse>
{
    private readonly ILogger<GetTradeSummaryQueryHandler> _logger;
    private readonly IDealRepository _dealRepository;
    private readonly IUserRepository _userRepository;

    public GetTradeSummaryQueryHandler(ILogger<GetTradeSummaryQueryHandler> logger,
        IDealRepository dealRepository,
        IUserRepository userRepository)
    {
        _logger = logger;
        _dealRepository = dealRepository;
        _userRepository = userRepository;
    }

    public async Task<ApiResponse> Handle(GetTradeSummaryQuery request, CancellationToken cancellationToken)
    {
        try
        {

            var tradeDetails = await _dealRepository.GetTradeSummaryDataAsync(request.Params);
            if (tradeDetails.Item3)
            {
                var tradeData = tradeDetails.Item1;
                // Get distinct logins and pre-fetch user data
                //var loginIds = tradeData.Select(g => Globals.ConvertNumric<ulong>(g.Login)).Distinct().ToList();
                //var users = await _userRepository.GetAsync(loginIds);
                //Dictionary<ulong, ManagerUser> dctUser = new Dictionary<ulong, ManagerUser>();

                //if (users != null)
                //{
                //    dctUser = users.ToDictionary(x => x.LoginId, x => x);
                //}
             
               //var tradeSummary= tradeData.Select(x => new
               // {
               //     login = x.Login.ToString(),
               //     totalCount = x.TotalCount.ToString(),
               //     balance= dctUser.ContainsKey(x.Login)? dctUser[x.Login].Balance:0,
               //     profit= x.Profit.ToString(),
               //    aum = dctUser.ContainsKey(x.Login) ? $"{dctUser[x.Login].Balance + x.Profit}" : x.Profit.ToString(),
               //});

                
                int totalCount = tradeDetails.Item2;

                return new ApiResponse
                {
                    data = tradeData,
                    status = "success",
                    totalRecords = totalCount,
                    page = request.Params.Page,
                    pageSize = request.Params.Limit,
                    totalPages = (int)Math.Ceiling(totalCount / (double)request.Params.Limit),
                    result = tradeData.Count()
                };
            }


        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in GetTradeDataQueryHandler");

        }
        return new ApiResponse();

    }



    //private IEnumerable<ManagerDeal> GetDeals(GetTradeDataQuery request, DateTime fromDate, DateTime todate)
    //{
    //    IEnumerable<ManagerDeal>? deals = null;

    //    if (string.IsNullOrEmpty(request.Symbol) && string.IsNullOrEmpty(request.LoginId))
    //    {
    //        deals = _mT5LIBHelper.GetDeals(fromDate, todate);
    //    }
    //    else if (!string.IsNullOrEmpty(request.Symbol) && string.IsNullOrEmpty(request.LoginId))
    //    {
    //        deals = _mT5LIBHelper.GetDeals("*", request.Symbol, fromDate, todate);
    //    }
    //    else if (string.IsNullOrEmpty(request.Symbol) && !string.IsNullOrEmpty(request.LoginId))
    //    {
    //        if (request.LoginId.Contains(','))
    //        {
    //            string[] loginIds = request.LoginId.Split(',');
    //            ulong[] logins = loginIds.Select(ulong.Parse).ToArray();
    //            deals = _mT5LIBHelper.GetDeals(logins, fromDate, todate);
    //        }
    //        else if (ulong.TryParse(request.LoginId, out ulong loginId))
    //        {
    //            deals = _mT5LIBHelper.GetDeals(loginId, fromDate, todate);
    //        }
    //    }
    //    else if (!string.IsNullOrEmpty(request.Symbol) && !string.IsNullOrEmpty(request.LoginId))
    //    {
    //        if (request.LoginId.Contains(','))
    //        {
    //            string[] loginIds = request.LoginId.Split(',');
    //            ulong[] logins = loginIds.Select(ulong.Parse).ToArray();
    //            deals = _mT5LIBHelper.GetDeals(logins, request.Symbol, fromDate, todate);
    //        }
    //        else if (ulong.TryParse(request.LoginId, out ulong loginId))
    //        {
    //            deals = _mT5LIBHelper.GetDeals(new ulong[] { loginId }, request.Symbol, fromDate, todate);
    //        }
    //    }
    //    return deals == null ? Enumerable.Empty<ManagerDeal>():deals.Where(x=>x.Type==MT5LIB.Enums.TradeType.Buy||x.Type==MT5LIB.Enums.TradeType.Sell);
    //}
}
