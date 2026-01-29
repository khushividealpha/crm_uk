using MetaQuotes.MT5CommonAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MT5LIB;

public class CAccountSink : CIMTAccountSink
{
    public override void OnAccountMarginCallEnter(CIMTAccount account, CIMTConGroup group)
    {
       
        base.OnAccountMarginCallEnter(account, group);
    }
    public override void OnAccountMarginCallLeave(CIMTAccount account, CIMTConGroup group)
    {
        base.OnAccountMarginCallLeave(account, group);
    }
    public override void OnAccountStopOutEnter(CIMTAccount account, CIMTConGroup group)
    {
        base.OnAccountStopOutEnter(account, group);
    }
    public override void OnAccountStopOutLeave(CIMTAccount account, CIMTConGroup group)
    {
        base.OnAccountStopOutLeave(account, group);
    }
}
