using MetaQuotes.MT5CommonAPI;
using Microsoft.Extensions.Logging;
using MT5LIB.Helpers;
using MT5LIB.Models;

namespace MT5LIB;

public class CUserSink : CIMTUserSink
{
    public event SinkDelegate<ManagerUser>? UserUpdate;
    private readonly ILogger<CUserSink> _logger;
    public CUserSink(ILogger<CUserSink> logger)
    {
        _logger = logger;
    }
    public override void OnUserAdd(CIMTUser user)
    {
        var userLoad = Utilities.GetUser(user);
        if (userLoad != null)
        {
            Utilities.dctUser.AddOrUpdate(userLoad.LoginId, userLoad, (k, v) => userLoad);
            UserUpdate?.Invoke(Enums.TradeEvent.Perform, userLoad);
        }
        base.OnUserAdd(user);
    }
    public override void OnUserDelete(CIMTUser user)
    {
        var userLoad = Utilities.GetUser(user);
        UserUpdate?.Invoke(Enums.TradeEvent.Delete, userLoad);
        base.OnUserDelete(user);
    }
    public override void OnUserLogin(string ip, CIMTUser user, CIMTUser.EnUsersConnectionTypes type)
    {
        var userLoad = Utilities.GetUser(user);
        UserUpdate?.Invoke(Enums.TradeEvent.Open, userLoad);
        base.OnUserLogin(ip, user, type);
    }
    public override void OnUserUpdate(CIMTUser user)
    {
        var userLoad = Utilities.GetUser(user);
        if (userLoad != null)
        {
            Utilities.dctUser.AddOrUpdate(userLoad.LoginId, userLoad, (k, v) => userLoad);
            UserUpdate?.Invoke(Enums.TradeEvent.Modify, userLoad);
        }
      
        base.OnUserUpdate(user);
    }
    

    public bool Initialize(ref string error)
    {
        MTRetCode dealRes = RegisterSink();
        if (dealRes == MTRetCode.MT_RET_OK)
        {
            return true;
        }
        error = string.Format("OrderSink: creating order sink failed ({0})", (object)MTRetCodeFormater.Format(dealRes));
        return false;

    }
}
