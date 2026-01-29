using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MT5LIB.Enums;

public enum TradeReason
{
    Client,
    Expert,
    Dealer,
    SL,
    TP,
    SO,
    Rollower,
    ExternalClient,
    Vmargin,
    Gateway,
    Signal,
    Settlement,
    Transfer,
    Sync,
    ExternalService,
    Migration,
    Mobile,
    Web,
    Split,
}