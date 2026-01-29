using CRMUKMTPApi.Models;
using CRMUKMTPApi.Repositories;
using MediatR;

namespace CRMUKMTPApi.CommandHandler
{
    public class DepositWithdrawalCommand :IRequest<object>
    {
        public ulong Mt5Id { get; set; }
        public double Amount { get; set; }
        public string Comment { get; set; }
        public DepositWithdrawalCommand(ulong mt5Id, double amount, string comment)
        {
            Mt5Id = mt5Id;
            Amount = amount;
            Comment = comment;
        }
    }
    //public class DepositWithdrawalHandler : IRequestHandler<DepositWithdrawalCommand, object>
    //{
    //    private readonly ILogger<DepositWithdrawalHandler> _logger;
    //    private readonly ITransactionRepository _transactionRepository;
    //    public DepositWithdrawalHandler(ILogger<DepositWithdrawalHandler> logger, ITransactionRepository transactionRepository)
    //    {
    //        _logger = logger;
    //        _transactionRepository = transactionRepository;
    //    }
    //    public async Task<object> Handle(DepositWithdrawalCommand request, CancellationToken cancellationToken)
    //    {
    //        try
    //        {
    //            if (request.Amount <= 0)
    //            {
    //                return new ApiResponse
    //                {
    //                    status = "Error",
                        
    //                };
    //            }
    //            if (request.Mt5Id <= 0)
    //            {
    //                return new ApiResponse
    //                {
    //                    status = "Invalid MT5 ID"
    //                };
    //            }
    //            double amount = request.Amount;
    //            var result = await _transactionRepository.DepositWithdrawalAsync(request.Mt5Id, request.Amount, request.Comment, out string respStr);
    //            return new
    //            {
    //                data = result,
    //                status = "Success"
    //            };
    //        }
    //        catch (Exception ex)
    //        {
    //            _logger.LogError(ex, "Error in DepositWithdrawalHandler");
    //            return new
    //            {
    //                data = (object?)null,
    //                status = "Error",
    //                message = ex.Message
    //            };
    //        }
    //    }
    //}
}
