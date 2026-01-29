using CRMUKMTPApi.Models;
using CRMUKMTPApi.Repositories;
using MediatR;
using MT5LIB.Models;

namespace CRMUKMTPApi.QueryHandler;

public class GetTransactionQuery : IRequest<ApiResponse>
{
    public GetTransactionQuery(ParamModel @params)
    {
        Params = @params;
    }

    public ParamModel Params { get; set; }
}
public class GetTransactionHandler : IRequestHandler<GetTransactionQuery, ApiResponse>
{
    private readonly ILogger<GetTransactionHandler> _logger;
    private readonly IDealRepository _dealRepository;

    public GetTransactionHandler(ILogger<GetTransactionHandler> logger, IDealRepository dealRepository)
    {
        _logger = logger;
        _dealRepository = dealRepository;
    }

    public async Task<ApiResponse> Handle(GetTransactionQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var data = await _dealRepository.GetAsync(new ParamModel
            {
                Loginid = request.Params.Loginid,
                From = request.Params.From,
                Filter = request.Params.Filter,
                Filtervalue = request.Params.Filtervalue,
                Sort = request.Params.Sort,
                To = request.Params.To,
            });
            if (data.Item3)
            {
                int totalCount = data.Item1.Where(x => x.Type != MT5LIB.Enums.TradeType.Buy && x.Type != MT5LIB.Enums.TradeType.Sell).Count();

                var transactions = data.Item1.Where(x => x.Type != MT5LIB.Enums.TradeType.Buy && x.Type != MT5LIB.Enums.TradeType.Sell)
                     .Skip((request.Params.Page - 1) * request.Params.Limit)
                     .Take(request.Params.Limit)
                     .Select(x=>new TransactionModel
                     {
                         amount=x.Profit,
                         comment=x.Comment,
                         currency=x.Currency,
                         dealid =x.DealId,
                         dealtime=x.Time,
                         demo=x.Demo,
                         login=x.LoginId,
                         name=x.ClientName,
                         type=x.Type,

                     }).ToList();

                return new ApiResponse
                {
                    data = transactions,
                    page = request.Params.Page,
                    pageSize = request.Params.Limit,
                    result = transactions.Count,
                    status = "success",
                    totalPages = (int)Math.Ceiling(totalCount / (double)request.Params.Limit),
                    totalRecords = totalCount
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
}
