using CRMUKMTPApi.Models;
using CRMUKMTPApi.Repositories;
using MediatR;
using MT5LIB.Models;

namespace CRMUKMTPApi.QueryHandler;

public class GetPositionrQuery : IRequest<ApiResponse>
{
    public GetPositionrQuery(ParamModel param)
    {
        Params = param;
    }

    public ParamModel Params { get; set; }

}
public class GetPositionrHandler : IRequestHandler<GetPositionrQuery, ApiResponse>
{
    private readonly ILogger<GetPositionrHandler> _logger;
    private readonly IPositionRepository _repository;

    public GetPositionrHandler(ILogger<GetPositionrHandler> logger, IPositionRepository repository)
    {
        _logger = logger;
        _repository = repository;
    }

    public async Task<ApiResponse> Handle(GetPositionrQuery request, CancellationToken cancellationToken)
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
}
