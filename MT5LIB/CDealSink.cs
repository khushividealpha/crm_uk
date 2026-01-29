using MetaQuotes.MT5CommonAPI;
using MT5LIB.Enums;
using MT5LIB.Helpers;
using MT5LIB.Models;

namespace MT5LIB;

public delegate void SinkDelegate<T>(TradeEvent tradeEvent, T data);
public class CDealSink : CIMTDealSink
{
    public event SinkDelegate<ManagerDeal>? DealUpdate;

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
    public override void OnDealAdd(CIMTDeal deal)
    {
        ManagerDeal dealLoad = Utilities.GetDealLoad(deal);
        DealUpdate?.Invoke(TradeEvent.Perform, dealLoad);
    }
    public override void OnDealUpdate(CIMTDeal deal)
    {
        ManagerDeal dealLoad = Utilities.GetDealLoad(deal);
        DealUpdate?.Invoke(TradeEvent.Modify, dealLoad);
    }
    public override void OnDealDelete(CIMTDeal deal)
    {
        ManagerDeal dealLoad = Utilities.GetDealLoad(deal);
        DealUpdate?.Invoke(TradeEvent.Delete, dealLoad);
    }
    public override void OnDealPerform(CIMTDeal deal, CIMTAccount account, CIMTPosition position)
    {
        ManagerDeal dealLoad = Utilities.GetDealLoad(deal);
        DealUpdate?.Invoke(TradeEvent.Perform, dealLoad);
    }
}
