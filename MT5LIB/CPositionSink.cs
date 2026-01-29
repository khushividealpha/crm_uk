using MetaQuotes.MT5CommonAPI;
using MT5LIB.Enums;
using MT5LIB.Helpers;
using MT5LIB.Models;

namespace MT5LIB;

public class CPositionSink:CIMTPositionSink
{
    public event SinkDelegate<ManagerPosition>? PositionUpdate;

    public bool Initialize(ref string error)
    {
        if (Utilities.Manager == null)
        {
            error = string.Format("Master not initialized");
            return false;
        }
        MTRetCode orderRes = RegisterSink();
        if (orderRes == MTRetCode.MT_RET_OK)
        {
            return true;
        }
        error = string.Format("PositionSink: creating position sink failed ({0})", (object)MTRetCodeFormater.Format(orderRes));
        return false;

    }
    public override void OnPositionAdd(CIMTPosition position)
    {
        var pos=Utilities.GetPositions(position);
        PositionUpdate?.Invoke(TradeEvent.Perform, pos);
        base.OnPositionAdd(position);
    }
    public override void OnPositionClean(ulong login)
    {
        base.OnPositionClean(login);
    }
    public override void OnPositionDelete(CIMTPosition position)
    {
        base.OnPositionDelete(position);
    }
    public override void OnPositionUpdate(CIMTPosition position)
    {
        var pos = Utilities.GetPositions(position);
        PositionUpdate?.Invoke(TradeEvent.Modify, pos);
        base.OnPositionUpdate(position);
    }
}
