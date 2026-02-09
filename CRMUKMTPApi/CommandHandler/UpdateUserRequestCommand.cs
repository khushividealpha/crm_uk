namespace CRMUKMTPApi.CommandHandler
{
    using CRMUKMTPApi.Models;
    using MediatR;
    using MetaQuotes.MT5CommonAPI;
    using MT5LIB;
    using MT5LIB.Helpers;
    using MT5LIB.Models;

    public class UpdateUsersCommand : IRequest<UpdateUsersResult>
    {
        public IReadOnlyList<UpdateUserDto> Users { get; }

        public UpdateUsersCommand(IReadOnlyList<UpdateUserDto> users)
        {
            Users = users;
        }
    }


public class UpdateUsersCommandHandler
    : IRequestHandler<UpdateUsersCommand, UpdateUsersResult>
    {
       

        public async Task<UpdateUsersResult> Handle(
            UpdateUsersCommand request,
            CancellationToken cancellationToken)
        {
            var result = new UpdateUsersResult();

            if (Utilities.Manager == null)
            {
                return null;
            }

            var _manager = Utilities.Manager;
            foreach (var item in request.Users)
            {
                var userResult = new UpdateUserResult
                {
                    Mt5Id = item.Mt5Id
                };

                if (_manager == null)
                {
                    userResult.Success = false;
                    userResult.Message = "ERROR: Manager not connected";
                    result.Results.Add(userResult);
                    continue;
                }

                var user = _manager.UserCreate();
                if (user == null)
                {
                    userResult.Success = false;
                    userResult.Message = "ERROR: Cannot create IMTUser object";
                    result.Results.Add(userResult);
                    continue;
                }

                var res = _manager.UserGet(item.Mt5Id, user);
                if (res != MTRetCode.MT_RET_OK)
                {
                    userResult.Success = false;
                    userResult.BadRequest = true;
                    userResult.Message = $"Login {item.Mt5Id} not found";
                    user.Release();
                    result.Results.Add(userResult);
                    continue;
                }

                var handlers = new Dictionary<string, Action<string>>(StringComparer.OrdinalIgnoreCase)
                {
                    ["name"] = v => user.Name(v),
                    ["email"] = v => user.EMail(v),
                    ["phone"] = v => user.Phone(v),
                    ["address"] = v => user.Address(v),
                    ["group"] = v => user.Group(v),
                    ["city"] = v => user.City(v),
                    ["state"] = v => user.State(v),
                    ["country"] = v => user.Country(v),
                    ["zipcode"] = v => user.ZIPCode(v)
                };

                bool invalidField = false;

                foreach (var field in item.Fields)
                {
                    if (!handlers.TryGetValue(field.Key, out var handler))
                    {
                        userResult.Success = false;
                        userResult.BadRequest = true;
                        userResult.Message = $"Unsupported field: {field.Key}";
                        invalidField = true;
                        break;
                    }

                    handler(field.Value ?? string.Empty);
                }

                if (invalidField)
                {
                    user.Release();
                    result.Results.Add(userResult);
                    continue;
                }

                res = _manager.UserUpdate(user);
                if (res != MTRetCode.MT_RET_OK)
                {
                    userResult.Success = false;
                    userResult.Message =
                        $"UserUpdate failed for {item.Mt5Id} (err {(int)res})";
                    user.Release();
                    result.Results.Add(userResult);
                    continue;
                }

                userResult.Success = true;
                userResult.Message = $"User updated for login {item.Mt5Id}";
                var updatedUser = _manager.UserCreate();
                var getRes = _manager.UserGet(item.Mt5Id, updatedUser);

                if (getRes == MTRetCode.MT_RET_OK)
                {
                    var managerUser = Utilities.GetUser(updatedUser.Login());

                    if (managerUser != null)
                    {
                        Utilities.dctUser.AddOrUpdate(
                            managerUser.LoginId,
                            managerUser,
                            (k, v) => managerUser
                        );

                        UserEvents.RaiseUserUpdated(managerUser);
                    }
                }
                user.Release();
                result.Results.Add(userResult);
            }

            return result;
        }
        public static class UserEvents
        {
            public static event SinkDelegate<ManagerUser>? UserUpdated;

            public static void RaiseUserUpdated(ManagerUser user)
            {
                UserUpdated?.Invoke(MT5LIB.Enums.TradeEvent.Modify, user);
            }
        }
    }
    }


