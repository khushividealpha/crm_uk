using CRMUKMTPApi.Models;

namespace CRMUKMTPApi.Repositories
{
    public interface ITransactionRepository
    {
         Task<DepositResponse> DepositWithdrawalAsync(ulong mt5Id, double amount, string comment);
        Task<bool> TransferBetweenAccountsAsync(
         ulong fromMt5Id,
         ulong toMt5Id,
         double amount,
         string fromComment,
         string toComment);
    }
}
