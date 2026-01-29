using CRMUKMTPApi.Models;
using CRMUKMTPApi.Repositories;
using MediatR;
using MT5LIB.Models;

namespace CRMUKMTPApi.QueryHandler;

public class GetDailyQuery:IRequest<ApiResponse>
{
    public GetDailyQuery(ParamModel @params)
    {
        Params = @params;
    }

    public ParamModel Params { get; set; }
}
public class GetDailyHandler : IRequestHandler<GetDailyQuery, ApiResponse>
{
    private readonly ILogger<GetDailyHandler> _logger;
    private readonly IDailyRepository _repository;

    public GetDailyHandler(ILogger<GetDailyHandler> logger, IDailyRepository repository)
    {
        _logger = logger;
        _repository = repository;
    }

    public async Task<ApiResponse> Handle(GetDailyQuery request, CancellationToken cancellationToken)
    {
        try
        {

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
            _logger.LogError(ex, "Error on Get daily Handler");
            return new ApiResponse();
        }
    }
}
