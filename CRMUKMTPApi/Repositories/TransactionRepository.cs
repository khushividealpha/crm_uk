using MetaQuotes.MT5ManagerAPI;
using MetaQuotes.MT5CommonAPI;
using MT5LIB.Helpers;
using CRMUKMTPApi.Models;


namespace CRMUKMTPApi.Repositories
{
    public class TransactionRepository : ITransactionRepository
    {
        private readonly ILogger<TransactionRepository> _logger;

        public TransactionRepository(ILogger<TransactionRepository> logger)
        {
            _logger = logger;
        }
    
        public async Task<DepositResponse> DepositWithdrawalAsync(
    ulong mt5Id,
    double amount,
    string comment
    )
        {
           string respStr = string.Empty;

            if (Utilities.Manager == null)
            {
                respStr = "MT5 Manager not connected";
                return new DepositResponse
                {
                    Success = false,
                    Message = respStr,
                    DealId = 0
                };
            }

            try
            {
                var manager = Utilities.Manager;

                var user = manager.UserCreate();
                var result = manager.UserGet(mt5Id, user);

                if (result != MTRetCode.MT_RET_OK)
                {
                    respStr = "MT5 User Not Found";
                    return new DepositResponse
                    {
                        Success = false,
                        Message = respStr,
                        DealId = 0
                    };

                }

                if (amount < 0)
                {
                    double currentBalance = user.Balance();
                    double withdrawal =amount;

                    if (withdrawal > currentBalance)
                    {
                        respStr = "Insufficient balance";
                        user.Release();
                        return new DepositResponse
                        {
                            Success = false,
                            Message = respStr,
                            DealId = 0
                        };

                    }
                }

                ulong dealId = 0;
                string traceId = comment;

                var dealerResult = manager.DealerBalanceRaw(
                    mt5Id,
                    amount,
(uint)CIMTDeal.EnDealAction.DEAL_BALANCE,
                    traceId,
                    out dealId
                );

                if (dealerResult == MTRetCode.MT_RET_REQUEST_DONE)
                {
                    manager.UserUpdate(user);
                    user.Release();

                    string successMessage = amount >= 0 ? "Deposit successful" : "Withdrawal successful";

                    return new DepositResponse
                    {
                        Success = true,
                        Message = successMessage,
                        DealId = dealId
                    };
                }


                respStr = "Unable to process transaction";
                return new DepositResponse
                {
                    Success = false,
                    Message = respStr,
                    DealId = 0
                };
            }
            catch (Exception ex)
            {
                respStr = "Internal Server Error";
                return new DepositResponse
                {
                    Success = false,
                    Message = respStr,
                    DealId = 0
                };
            }
        }

    }
}
