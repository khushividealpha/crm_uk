
using CRMUKMTPApi.Models;
using MetaQuotes.MT5CommonAPI;
using MT5LIB.Helpers;

namespace CRMUKMTPApi.Repositories
{
    public class SignUpMT : ISignupMt
    {
        private readonly ILogger<SignUpMT> _logger;

        public SignUpMT(ILogger<SignUpMT> logger)
        {
            _logger = logger;
        }

        public async Task<bool> SignupMt5User(SignUpMTModel model)
        
        {
            try
            {
                if (Utilities.Manager == null)
                    throw new InvalidOperationException("MT5 Manager API is not initialized.");

                var manager = Utilities.Manager;

                
                var group = manager.GroupCreate();
                var retGroup = manager.GroupGet(model.Group, group);

                if (retGroup != MTRetCode.MT_RET_OK)
                {
                    group.Release();
                    return false;
                }
                group.Release();

                //  Create User Object
                var user = manager.UserCreate();
                if (user == null)
                    return false;

                user.Name(model.FullName);
                user.EMail(model.Email);
                user.Group(model.Group);
        
         
                var retUserAdd = manager.UserAdd(
                    user,
                    model.Password,
                    model.Password // investor password (same as C++)
                );

                if (retUserAdd != MTRetCode.MT_RET_OK)
                {
                    string reason = retUserAdd switch
                    {
                        MTRetCode.MT_RET_USR_LOGIN_EXHAUSTED => "The range of logins has been exhausted.",
                        MTRetCode.MT_RET_USR_LOGIN_PROHIBITED => "The login is reserved on another server.",
                        MTRetCode.MT_RET_USR_LOGIN_EXIST => "The account already exists.",
                        MTRetCode.MT_RET_USR_SUICIDE => "An attempt of self-deletion.",
                        MTRetCode.MT_RET_USR_INVALID_PASSWORD => "Incorrect account password.",
                        MTRetCode.MT_RET_USR_LIMIT_REACHED => "Reached the limit on the number of users.",
                        _ => "Other reason."
                    };

                    user.Release();
                    return false;
                }

                // --------------------------------------------------
                // 6️⃣ Post-create Rights Fix (RESET_PASS)
                // --------------------------------------------------
                ulong loginId = user.Login();
                var rights = user.Rights();

                if ((rights & CIMTUser.EnUsersRights.USER_RIGHT_RESET_PASS) != 0)
                {
                    rights &= ~CIMTUser.EnUsersRights.USER_RIGHT_RESET_PASS;
                    user.Rights(rights);

                    var retUpdate = manager.UserUpdate(user);
                    if (retUpdate != MTRetCode.MT_RET_OK)
                    {
                        _logger.LogWarning(
                            "Failed to update rights for new MT5 user {LoginId}, code {Code}",
                            loginId, retUpdate
                        );
                    }
                }

                user.Release();

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while signing up MT5 user.");
                throw;
            }
        }

    }
}
