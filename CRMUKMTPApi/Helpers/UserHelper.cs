using Apmcrm.V1.Msgs.Types;
using CLIB.Helpers;
using CRMUKMTPApi.Repositories;
using Google.Protobuf;
using MT5LIB;
using MT5LIB.Enums;
using MT5LIB.Helpers;
using MT5LIB.Models;

namespace CRMUKMTPApi.Helpers;

public class UserHelper
{
    private readonly ILogger<UserHelper> _logger;
    private readonly AppQueue<Tuple<TradeEvent, ManagerUser>> _queue;
    private readonly CUserSink _userSink;
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly MT5LIBHelper _helper;

    public UserHelper(ILogger<UserHelper> logger, CUserSink userSink,
        MT5LIBHelper helper, IServiceScopeFactory serviceScopeFactory)
    {
        _logger = logger;
        _userSink = userSink;
        _helper = helper;
        _serviceScopeFactory = serviceScopeFactory;
        _userSink.UserUpdate += UserUpdate;
        _queue = new AppQueue<Tuple<TradeEvent, ManagerUser>>(PrcessUser);
    }

    private void UserUpdate(TradeEvent tradeEvent, ManagerUser data)
    {
        _queue.Enqueue(Tuple.Create(tradeEvent, data));
    }

    private async Task PrcessUser(Tuple<TradeEvent, ManagerUser> tuple)
    {
        try
        {
            MessageState messageState = MessageState.New;
            var tradeEvent = tuple.Item1;
            var user = tuple.Item2;
            using var scope = _serviceScopeFactory.CreateScope();
            var repository = scope.ServiceProvider.GetRequiredService<IUserRepository>();
            if (tradeEvent == TradeEvent.Modify)
            {
                await repository.UpdateAsync(user);
               messageState = MessageState.Update;
            }
            
            else if (tradeEvent == TradeEvent.Perform)
            {
                await repository.AddAsync(user);
            }
            ByteString stringData = Globals.ConvertToByteString<ManagerUser>(user);
            await Globals.BroadcastData<ManagerUser>(user.LoginId, messageState, MessageType.User, stringData);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Fail on process deal");
        }
    }
    public async Task<bool> InitializeUser()
    {
        try
        {
            using var scope = _serviceScopeFactory.CreateScope();
            var repository = scope.ServiceProvider.GetRequiredService<IUserRepository>();
            if (repository == null) return false;

           

            var users = _helper.GetUsers();
            if (users == null) return false;

            await repository.AddOrUpdateUsersAsync(users);
            
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error on initialize order");
            return false;
        }
    }
}
