using CRMUKMTPApi.Models;
using MetaQuotes.MT5CommonAPI;
using MT5LIB.Helpers;


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

        public async Task<bool> TransferBetweenAccountsAsync(
       ulong fromMt5Id,
       ulong toMt5Id,
       double amount,
       string fromComment,
       string toComment)
        {
            try
            {
                if (Utilities.Manager == null)
                {
                    _logger.LogError("MT5 Manager not connected");
                    return false;
                }

                if (amount <= 0)
                {
                    _logger.LogError("Transfer amount must be greater than zero");
                    return false;
                }

                var manager = Utilities.Manager;

                // Load sender
                var fromUser = manager.UserCreate();
                var fromResult = manager.UserGet(fromMt5Id, fromUser);

                if (fromResult != MTRetCode.MT_RET_OK)
                {
                    _logger.LogError($"Sender MT5 user not found: {fromMt5Id}");
                    fromUser.Release();
                    return false;
                }

                // Load receiver
                var toUser = manager.UserCreate();
                var toResult = manager.UserGet(toMt5Id, toUser);

                if (toResult != MTRetCode.MT_RET_OK)
                {
                    _logger.LogError($"Receiver MT5 user not found: {toMt5Id}");
                    fromUser.Release();
                    toUser.Release();
                    return false;
                }

                // Check sender balance
                double senderBalance = fromUser.Balance();
                if (senderBalance < amount)
                {
                    _logger.LogError($"Insufficient funds. Sender Balance={senderBalance}, Amount={amount}");
                    fromUser.Release();
                    toUser.Release();
                    return false;
                }

                ulong withdrawDealId = 0;
                ulong depositDealId = 0;

            

                // Step 1 — Withdraw from sender
                var withdrawResult = manager.DealerBalanceRaw(
                    fromMt5Id,
                    -amount,
                    (uint)CIMTDeal.EnDealAction.DEAL_BALANCE,
                    fromComment,
                    out withdrawDealId
                );

                if (withdrawResult != MTRetCode.MT_RET_REQUEST_DONE)
                {
                    _logger.LogError("Withdrawal failed from sender");
                    fromUser.Release();
                    toUser.Release();
                    return false;
                }

                // Step 2 — Deposit to receiver
                var depositResult = manager.DealerBalanceRaw(
                    toMt5Id,
                    amount,
                    (uint)CIMTDeal.EnDealAction.DEAL_BALANCE,
                    toComment,
                    out depositDealId
                );

                if (depositResult != MTRetCode.MT_RET_REQUEST_DONE)
                {
                    _logger.LogError("Deposit failed. Rolling back withdrawal...");

                    // Rollback — return money to sender
                    manager.DealerBalanceRaw(
                        fromMt5Id,
                        amount,
                        (uint)CIMTDeal.EnDealAction.DEAL_BALANCE,
                $"{fromComment}-ROLLBACK",
                        out _
                    );

                    fromUser.Release();
                    toUser.Release();
                    return false;
                }

                // Update users
                manager.UserUpdate(fromUser);
                manager.UserUpdate(toUser);

                fromUser.Release();
                toUser.Release();

                _logger.LogInformation($"Transfer successful. WithdrawDeal={withdrawDealId}, DepositDeal={depositDealId}");

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in TransferBetweenAccountsAsync");
                return false;
            }
        }

    }
}
