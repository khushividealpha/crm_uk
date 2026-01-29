using CRMUKMTPApi.Models;

namespace CRMUKMTPApi.Repositories
{
    public interface ITransactionRepository
    {
         Task<DepositResponse> DepositWithdrawalAsync(ulong mt5Id, double amount, string comment);
    }
}
