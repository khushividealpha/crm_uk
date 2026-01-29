using CLIB.Helpers;
using MetaQuotes.MT5CommonAPI;
using MT5LIB.Enums;
using MT5LIB.Helpers;
using MT5LIB.Models;

namespace MT5LIB;


public class CDailySink : CIMTDailySink
{
    public event SinkDelegate<ManagerDailyReport>? DailyUpdate;

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
    public override void OnDailyAdd(CIMTDaily daily)
    {
        var dailyLoad=Utilities.GetDailyLoad(daily);
        DailyUpdate?.Invoke(TradeEvent.Perform, dailyLoad);
        base.OnDailyAdd(daily);
    }
    public override void OnDailyClean(ulong login)
    {
        base.OnDailyClean(login);
    }

    public override void OnDailyUpdate(CIMTDaily daily)
    {
        var dailyLoad = Utilities.GetDailyLoad(daily);
        DailyUpdate?.Invoke(TradeEvent.Perform, dailyLoad);
        base.OnDailyUpdate(daily);
    }
    
}
