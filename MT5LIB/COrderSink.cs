using CLIB.Helpers;
using MetaQuotes.MT5CommonAPI;
using MT5LIB.Enums;
using MT5LIB.Helpers;
using MT5LIB.Models;
using System.Collections.Concurrent;

namespace MT5LIB;

public class COrderSink : CIMTOrderSink
{
    private ConcurrentDictionary<ulong, string> dctOrder = new();

    public event SinkDelegate<ManagerOrder>? OrderUpdate;

    
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
        error = string.Format("OrderSink: creating order sink failed ({0})", (object)MTRetCodeFormater.Format(orderRes));
        return false;

    }
    public override void OnOrderAdd(CIMTOrder order)
    {
        ManagerOrder orderLoad = Utilities.GetOrderLoad(order);
        OrderUpdate?.Invoke(TradeEvent.Place, orderLoad);
    }
    public override void OnOrderUpdate(CIMTOrder order)
    {
        ManagerOrder orderLoad = Utilities.GetOrderLoad(order);
        OrderUpdate?.Invoke(TradeEvent.Modify, orderLoad);
    }
    public override void OnOrderDelete(CIMTOrder order)
    {
        ManagerOrder orderLoad = Utilities.GetOrderLoad(order);
        OrderUpdate?.Invoke(TradeEvent.Delete, orderLoad);
    }
  
}
