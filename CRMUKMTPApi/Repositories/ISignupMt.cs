using CRMUKMTPApi.Models;

namespace CRMUKMTPApi.Repositories
{
    public interface ISignupMt
    {
         Task<bool> SignupMt5User(SignUpMTModel model);
    }
}
