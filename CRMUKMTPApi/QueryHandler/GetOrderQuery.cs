using CRMUKMTPApi.Models;
using CRMUKMTPApi.Repositories;
using MediatR;
using Microsoft.EntityFrameworkCore;
using MT5LIB.Models;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace CRMUKMTPApi.QueryHandler;

public class GetOrderQuery : IRequest<ApiResponse>
{
    public GetOrderQuery(ParamModel model)
    {
        Params = model;
    }

    public ParamModel Params { get; set; }
    
}
public class GetOrderHandler : IRequestHandler<GetOrderQuery, ApiResponse>
{
    private readonly ILogger<GetOrderHandler> _logger;
    private readonly IOrderRepository _orderRepository;

    public GetOrderHandler(ILogger<GetOrderHandler> logger, IOrderRepository orderRepository)
    {
        _logger = logger;
        _orderRepository = orderRepository;
    }

    public async Task<ApiResponse> Handle(GetOrderQuery request, CancellationToken cancellationToken)
    {
        try
        {

            bool sort = !request.Params.Sort.StartsWith('-');
            var data = await _orderRepository.GetAsync(request.Params);
            if(data.Item3)
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
                data = new List<ManagerOrder>(),
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
