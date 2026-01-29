using CRMUKMTPApi.Models;
using CRMUKMTPApi.Repositories;
using MediatR;

namespace CRMUKMTPApi.QueryHandler
{
    public class GetLastTradeTimeQuery:IRequest<object>
    {
        public List<ulong> Ids { get; set; }
        public GetLastTradeTimeQuery(List<ulong> ids)
        {
            Ids = ids;
        }
    }
    public class GetLastTradeTimeHandler : IRequestHandler<GetLastTradeTimeQuery, object>
    {
        private readonly ILogger<GetLastTradeTimeHandler> _logger;
        private readonly ILastTradeTimeRepository _repository;
        public GetLastTradeTimeHandler(ILogger<GetLastTradeTimeHandler> logger, ILastTradeTimeRepository repository)
        {
            _logger = logger;
            _repository = repository;
        }
        public async Task<object> Handle(GetLastTradeTimeQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var result = await _repository.GetAsync(request.Ids);
                return  new
                {
                    data = result,                
                    status = "Success"
                    
                };

            }
            catch (Exception ex)
            {
                return null;

            }
        }
    }
}
