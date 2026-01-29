using CLIB.Models;
using MetaQuotes.MT5CommonAPI;
using MetaQuotes.MT5ManagerAPI;
using Microsoft.Extensions.Logging;
using MT5LIB.Helpers;

namespace MT5LIB;

public class Manager : CIMTManagerSink
{


    private readonly ILogger<Manager> _logger;
    private readonly CDealSink _dealSink;
    private readonly COrderSink _orderSink;
    private readonly CTickSink _tickSink;
    private readonly ConfigInfo _configInfo;

    public Manager(ILogger<Manager> logger,
        CDealSink dealSink,
        COrderSink orderSink,
        ConfigInfo configInfo,
        CTickSink cTickSink)
    {
        _logger = logger;
        _dealSink = dealSink;
        _orderSink = orderSink;
        _configInfo = configInfo;
        _tickSink = cTickSink;
    }


    public override void OnConnect()
    {
        _logger.LogInformation("Manager Connected");
        base.OnConnect();
    }
    public override void OnDisconnect()
    {
        _logger.LogInformation("Manager Disconnected");
        base.OnDisconnect();
    }
    public override void Release()
    {
        Stop();
        Shutdown();
        base.Release();
    }
    public bool Initialize(ref string error)
    {
        try
        {
            var regiterSink = RegisterSink();
            if (regiterSink == MTRetCode.MT_RET_OK)
            {

                var initApiFactory = SMTManagerAPIFactory.Initialize(null);
                _logger.LogInformation("Manager API Factory initialized with result: {Result}", MTRetCodeFormater.Format(initApiFactory));
                if (initApiFactory == MTRetCode.MT_RET_OK)
                {
                    MTRetCode versionResponse = SMTManagerAPIFactory.GetVersion(out uint version);
                    if (versionResponse == MTRetCode.MT_RET_OK && version == SMTManagerAPIFactory.ManagerAPIVersion)
                    {
                        if ((Utilities.Manager = SMTManagerAPIFactory.CreateManager(SMTManagerAPIFactory.ManagerAPIVersion, out MTRetCode res)) != null && res == MTRetCode.MT_RET_OK)
                        {
                            if (!_dealSink.Initialize(ref error) ||
                                !_orderSink.Initialize(ref error) ||
                                 !Subscribe(ref error) || !_tickSink.Initialize(ref error))
                            { return false; }

                            if (Utilities.Manager.TickSubscribe(_tickSink) == MTRetCode.MT_RET_OK)
                                return true;
                        }
                        else
                        {
                            error = string.Format("Dealer: creating manager interface failed ({0})", (object)MTRetCodeFormater.Format(res));
                        }
                    }
                    else
                    {
                        error = string.Format("Dealer: getting version failed ({0})", (object)MTRetCodeFormater.Format(versionResponse));
                    }
                }
                else
                {
                    error = string.Format("Loading manager API failed ({0})", MTRetCodeFormater.Format(initApiFactory));
                }
            }
            else
            {
                error = string.Format("Register sink failed ({0})", MTRetCodeFormater.Format(regiterSink));
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.StackTrace);
        }
        return false;

    }
    public bool Subscribe(ref string error)
    {
        try
        {
            if (Utilities.Manager != null)
            {
                
                if (Utilities.Manager.Subscribe(this) != MTRetCode.MT_RET_OK) { error = "Fail on manger subscribe"; return false; }
                if (Utilities.Manager.OrderSubscribe(_orderSink) != MTRetCode.MT_RET_OK) { error = "Fail on order subscribe"; return false; }
                if (Utilities.Manager.DealSubscribe(_dealSink) != MTRetCode.MT_RET_OK) { error = "Fail on deal subscribe"; return false; }
                var modes = CIMTManagerAPI.EnPumpModes.PUMP_MODE_USERS | CIMTManagerAPI.EnPumpModes.PUMP_MODE_ORDERS | CIMTManagerAPI.EnPumpModes.PUMP_MODE_POSITIONS | CIMTManagerAPI.EnPumpModes.PUMP_MODE_GROUPS | CIMTManagerAPI.EnPumpModes.PUMP_MODE_SYMBOLS | CIMTManagerAPI.EnPumpModes.PUMP_MODE_TIME;
                var isConnect = Utilities.Manager.Connect(_configInfo.Server, ulong.Parse(_configInfo.Username), _configInfo.Password, null, modes, 7000);
                if (isConnect != MTRetCode.MT_RET_OK)
                {
                    error = $"Fail on manger subscribe {isConnect}";
                    return false;
                }
                return true;
            }
            error = "Manager not initialize";
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "On Connect Manager");
            error = ex.Message;
            return false;
        }
    }
    private void Stop()
    {
        try
        {
            if (Utilities.Manager == null) return;
            MTRetCode code = Utilities.Manager.DealUnsubscribe(_dealSink);
            code = Utilities.Manager.OrderUnsubscribe(_orderSink);
            Utilities.Manager.Disconnect();
            code = Utilities.Manager.Unsubscribe(this);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error on Manager Stop");
        }
    }
    private void Shutdown()
    {
        try
        {
            _orderSink.Dispose();
            _dealSink.Dispose();
            if (Utilities.Manager == null) return;
            Utilities.Manager.Dispose();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error on Manager shutdown");
        }
    }
}
