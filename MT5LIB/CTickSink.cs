using MetaQuotes.MT5CommonAPI;
using Microsoft.Extensions.Logging;

namespace MT5LIB;

public class CTickSink:CIMTTickSink
{
    
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
    public override void OnTick(int feeder, MTTick tick)
    {
        base.OnTick(feeder, tick);
    }
    public override void OnTickStat(MTTickStat tick)
    {
        base.OnTickStat(tick);
    }
    public override void OnTickStat(int feeder, MTTickStat tstat)
    {
        base.OnTickStat(feeder, tstat);
    }
    public override MTRetCode HookTick(int feeder, ref MTTick tick)
    {
        return base.HookTick(feeder, ref tick);
    }
    public override MTRetCode HookTickStat(int feeder, ref MTTickStat tstat)
    {
        return base.HookTickStat(feeder, ref tstat);
    }
    public override void OnTick(string symbol, MTTickShort tick)
    {
        base.OnTick(symbol, tick);
    }
    
}
