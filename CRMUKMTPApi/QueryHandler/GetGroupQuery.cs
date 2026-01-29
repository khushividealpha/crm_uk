//using CRMUKMTPApi.Models;
//using MediatR;

//namespace CRMUKMTPApi.QueryHandler
//{
//    public class GetGroupQuery:IRequest<ApiResponse>
//    {
//        public ParamModel Params { get; set; }
//        public GetGroupQuery(ParamModel @params)
//        {
//            Params = @params;
//        }
//    }
//    public class GetGroupHandler : IRequestHandler<GetGroupQuery, ApiResponse>
//    {
//        private readonly ILogger<GetGroupHandler> _logger;
//        public GetGroupHandler(ILogger<GetGroupHandler> logger)
//        {
//            _logger = logger;
//        }
//        public Task<ApiResponse> Handle(GetGroupQuery request, CancellationToken cancellationToken)
//        {
//            try{

//            }
//            catch(Exception ex)
//            {
//                return new ApiResponse();
//            }
//    }
//}
