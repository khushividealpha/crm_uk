using CRMUKMTPApi.Models;
using MediatR;
using MetaQuotes.MT5CommonAPI;
using Microsoft.Extensions.Logging;
using MT5LIB.Helpers;


public class SignupMt5UserCommand : IRequest<SignupMt5UserResult>
{

    public SignupMt5UserCommand(SignUpMTModel parameters)
    {
        Params = parameters;
    }

    public SignUpMTModel Params { get; }
}
public class SignupMt5UserCommandHandler
    : IRequestHandler<SignupMt5UserCommand, SignupMt5UserResult>
{
    private readonly ILogger<SignupMt5UserCommandHandler> _logger;

    public SignupMt5UserCommandHandler(
        ILogger<SignupMt5UserCommandHandler> logger)
    {
        _logger = logger;
    }

    public Task<SignupMt5UserResult> Handle(
        SignupMt5UserCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            if (Utilities.Manager == null)
            {
                return Task.FromResult(new SignupMt5UserResult
                {
                    status = false,
                });
            }

            var manager = Utilities.Manager;

            var group = manager.GroupCreate();
            var retGroup = manager.GroupGet(request.Params.Group, group);

            if (retGroup != MTRetCode.MT_RET_OK)
            {
                group.Release();
                return Task.FromResult(new SignupMt5UserResult
                {
                    status = false,
                });
            }
            group.Release();

            
            var user = manager.UserCreate();
            if (user == null)
            {
                return Task.FromResult(new SignupMt5UserResult
                {
                    status = false,
                });
            }
            user.Name(request.Params.FullName);
            user.EMail(request.Params.Email);
            user.Group(request.Params.Group);
            user.Rights(CIMTUser.EnUsersRights.USER_RIGHT_DEFAULT);
            ulong login = (ulong)DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            user.Login(login);
            user.Leverage(1);

            var retAdd = manager.UserAdd(
                user,
                request.Params.Password,
                request.Params.Password

            );

            if (retAdd != MTRetCode.MT_RET_OK)
            {
                user.Release();

                return Task.FromResult(new SignupMt5UserResult
                {
                    status = false,
                });
            }

            ulong loginId = user.Login();
            var rights = user.Rights();

            if ((rights & CIMTUser.EnUsersRights.USER_RIGHT_RESET_PASS) != 0)
            {
                rights &= ~CIMTUser.EnUsersRights.USER_RIGHT_RESET_PASS;
                user.Rights(rights);

                manager.UserUpdate(user);
            }

            user.Release();

            return Task.FromResult(new SignupMt5UserResult
            {
                status = true,
                mt5Id = loginId.ToString(),
                email = request.Params.Email
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "MT5 signup failed");

            return Task.FromResult(new SignupMt5UserResult
            {
                status = false,
              
            });
        }
    }

    private static string GetUserAddError(MTRetCode code) =>
        code switch
        {
            MTRetCode.MT_RET_USR_LOGIN_EXHAUSTED => "Login range exhausted.",
            MTRetCode.MT_RET_USR_LOGIN_PROHIBITED => "Login reserved on another server.",
            MTRetCode.MT_RET_USR_LOGIN_EXIST => "Account already exists.",
            MTRetCode.MT_RET_USR_INVALID_PASSWORD => "Invalid password.",
            MTRetCode.MT_RET_USR_LIMIT_REACHED => "User limit reached.",
            _ => "MT5 user creation failed."
        };

  
}
