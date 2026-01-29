using Apmcrm.V1.Msgs;
using CRMUKMTPApi.Helpers;
using CRMUKMTPApi.Models;
using CRMUKMTPApi.Repositories;
using MediatR;
using MT5LIB.Enums;
using MT5LIB.Helpers;
using MT5LIB.Models;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace CRMUKMTPApi.QueryHandler;

public class GetTradeDataQuery : IRequest<ApiResponse>
{
    public GetTradeDataQuery(ParamModel param)
    {
        Params = param;


    }

    public ParamModel Params { get; set; }


}

public class GetTradeDataQueryHandler : IRequestHandler<GetTradeDataQuery, ApiResponse>
{
    private readonly ILogger<GetTradeDataQueryHandler> _logger;
    private readonly IDealRepository _dealRepository;
    private readonly IUserRepository _userRepository;

    public GetTradeDataQueryHandler(ILogger<GetTradeDataQueryHandler> logger,
        IDealRepository dealRepository,
        IUserRepository userRepository)
    {
        _logger = logger;
        _dealRepository = dealRepository;
        _userRepository = userRepository;
    }

    public async Task<ApiResponse> Handle(GetTradeDataQuery request, CancellationToken cancellationToken)
    {
        try
        {

            //IEnumerable<ManagerDeal> allDeals = GetDeals(request, fromDate, todate);
            
            
            var tradeDetails = await _dealRepository.GetTradeDataAsync(request.Params);
            if (tradeDetails.Item3)
            {
                var tradeData = tradeDetails.Item1;
                // Get distinct logins and pre-fetch user data
                //var loginIds = tradeData.Select(g => Globals.ConvertNumric<ulong>(g.Login)).Distinct().ToList();
                //var users = await _userRepository.GetAsync(loginIds);
                //Dictionary<string, ManagerUser> dctUser = new Dictionary<string, ManagerUser>();

                //if (users != null)
                //{
                //    dctUser = users.ToDictionary(x => x.LoginId.ToString(), x => x);
                //}

                //foreach (var trade in tradeData)
                //{

                //    var user = dctUser[trade.Login];
                //    if (user != null)
                //    {

                //        trade.Group = user.Group;
                //        trade.FirstName = user.Name;
                //        trade.LastName = user.LastName;
                //        trade.Email = user.Email;
                //        trade.Phone = user.Phone;
                //    };
                //}
                int totalCount = tradeDetails.Item2;
               
                return new ApiResponse
                {
                    data = tradeDetails.Item1,
                    status = "success",
                    totalRecords = totalCount,
                    page = request.Params.Page,
                    pageSize = request.Params.Limit,
                    totalPages = (int)Math.Ceiling(totalCount / (double)request.Params.Limit),
                    result = tradeDetails.Item1.Count
                };
            }


        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in GetTradeDataQueryHandler");

        }
        //try
        //{

        //    //IEnumerable<ManagerDeal> allDeals = GetDeals(request, fromDate, todate);
        //    int page = request.Params.Page;
        //    request.Params.Page = 0;
        //    var dealData = await _dealRepository.GetAsync(request.Params);
        //    if (dealData.Item3)
        //    {
        //        IEnumerable<ManagerDeal> allDeals = dealData.Item1.Where(x=>x.Type==TradeType.Buy|| x.Type == TradeType.Sell);

        //        var groupedDeals = allDeals.GroupBy(x => new { x.Symbol, x.LoginId });

        //        // Get distinct logins and pre-fetch user data
        //        var loginIds = groupedDeals.Select(g => g.Key.LoginId).Distinct().ToList();
        //        var users =await _userRepository.GetAsync(loginIds);
        //        Dictionary<ulong, ManagerUser> dctUser = new Dictionary<ulong, ManagerUser>();

        //        if (users != null)
        //        {
        //            dctUser = users.ToDictionary(x => x.LoginId, x => x);
        //        }


        //        List<TradeDataModel> tradeDetails = new List<TradeDataModel>();
        //        foreach (var deals in groupedDeals)
        //        {

        //            var user = dctUser[deals.Key.LoginId];
        //            var firstDeal = deals.FirstOrDefault();

        //            var tradeData = new TradeDataModel
        //            {
        //                Login = deals.Key.LoginId.ToString(),
        //                Symbol = deals.Key.Symbol,
        //                Group = user?.Group ?? string.Empty,
        //                FirstName = firstDeal?.ClientName ?? string.Empty,
        //                LastName = user?.LastName ?? string.Empty,
        //                TotalCount = deals.Count().ToString(),
        //                LastTrade = deals.Max(x => x.Time).ToString(),
        //                FirstTrade = deals.Min(x => x.Time).ToString(),
        //                Profit = Math.Round(deals.Sum(x => x.Profit), 2).ToString(),
        //                Swap = Math.Round(deals.Sum(x => x.Swap), 2).ToString(),
        //                Email = user?.Email ?? string.Empty,
        //                Phone = user?.Phone ?? string.Empty
        //            };
        //            tradeDetails.Add(tradeData);
        //        }
        //        //var tradeData = allDeals
        //        //    .GroupBy(x => new { symbol = x.Symbol, login = x.LoginId, group = x.Group })
        //        //    .Select(deals => new TradeDataModel
        //        //    {
        //        //        Login = deals.Key.login.ToString(),
        //        //        Symbol = deals.Key.symbol,
        //        //        Group = deals.Key.group,
        //        //        FirstName = deals.FirstOrDefault()?.ClientName ?? string.Empty,
        //        //        LastName = deals.FirstOrDefault()?.LastName ?? string.Empty,
        //        //        TotalCount = deals.Count().ToString(),
        //        //        LastTrade = deals.Max(x => x.Time).ToString(),
        //        //        FirstTrade = deals.Min(x => x.Time).ToString(),
        //        //        Profit = Math.Round(deals.Sum(x => x.Profit), 2).ToString(),
        //        //        Swap = Math.Round(deals.Sum(x => x.Swap), 2).ToString(),

        //        //    }).ToList();
        //        int totalCount = tradeDetails.Count;
        //        tradeDetails = tradeDetails.Skip((page - 1) * request.Params.Limit)
        //           .Take(request.Params.Limit).ToList();
        //        return new ApiResponse
        //        {
        //            data = tradeDetails,
        //            status = "success",
        //            totalRecords = totalCount,
        //            page = page,
        //            pageSize = request.Params.Limit,
        //            totalPages = (int)Math.Ceiling(totalCount / (double)request.Params.Limit),
        //            result = tradeDetails.Count
        //        };
        //    }


        //}
        //catch (Exception ex)
        //{
        //    _logger.LogError(ex, "Error in GetTradeDataQueryHandler");

        //}
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
