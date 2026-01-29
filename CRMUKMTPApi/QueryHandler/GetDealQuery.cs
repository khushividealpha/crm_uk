using CRMUKMTPApi.Models;
using CRMUKMTPApi.Repositories;
using MediatR;
using MT5LIB.Models;

namespace CRMUKMTPApi.QueryHandler;

public class GetDealQuery : IRequest<ApiResponse>
{
    public GetDealQuery(ParamModel param)
    {
        Params = param;
    }

    public ParamModel Params { get; set; }
}
public class GetDealHandler : IRequestHandler<GetDealQuery, ApiResponse>
{
    private readonly ILogger<GetDealHandler> _logger;
    private readonly IDealRepository _dealRepository;

    public GetDealHandler(ILogger<GetDealHandler> logger, IDealRepository dealRepository)
    {
        _logger = logger;
        _dealRepository = dealRepository;
    }

    public async Task<ApiResponse> Handle(GetDealQuery request, CancellationToken cancellationToken)
    {
        try
        {

            var data= await _dealRepository.GetAsync(request.Params);
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
            _logger.LogError(ex, "Error on GetDealHandler");
            return new ApiResponse();
        }
    }
}
