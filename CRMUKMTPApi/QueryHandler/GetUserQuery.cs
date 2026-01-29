using CRMUKMTPApi.Models;
using CRMUKMTPApi.Repositories;
using MediatR;
using MT5LIB.Models;

namespace CRMUKMTPApi.QueryHandler;

public class GetUserQuery : IRequest<ApiResponse>
{
    public GetUserQuery(ParamModel param)
    {
        Params = param;
    }

    public ParamModel Params { get; set; }
}
public class GetUserHandler : IRequestHandler<GetUserQuery, ApiResponse>
{
    private readonly ILogger<GetUserHandler> _logger;
    private readonly IUserRepository _repository;

    public GetUserHandler(ILogger<GetUserHandler> logger, IUserRepository repository)
    {
        _logger = logger;
        _repository = repository;
    }

    public async Task<ApiResponse> Handle(GetUserQuery request, CancellationToken cancellationToken)
    {
        try
        {

            bool sort = !request.Params.Sort.StartsWith('-');
            var data= await _repository.GetAsync(request.Params);
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
            _logger.LogError(ex, "Error on GetUserHandler");
            return new ApiResponse();
        }
    }
}
