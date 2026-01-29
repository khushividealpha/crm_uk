
using CRMUKMTPApi.Helpers;
using MT5LIB;
using MT5LIB.Enums;
using System.Configuration;

namespace CRMUKMTPApi;

public class AppHostService : IHostedService
{
    private readonly ILogger<AppHostService> _logger;
    private readonly Manager _manager;
    private readonly DealHelper _dealHelper;
    private readonly OrderHelper _orderHelper;
    private readonly PositionHelper _positionHelper;
    private readonly UserHelper _userHelper;
    private readonly DailyHelper _dailyHelper;
    private readonly string _envPath;
    private readonly ProcedureHelper _procedureHelper;
    public AppHostService(ILogger<AppHostService> logger, Manager manager,
        DealHelper dealHelper,OrderHelper orderHelper,
        PositionHelper positionHelper, UserHelper userHelper, DailyHelper dailyHelper,
        IConfiguration configuration, ProcedureHelper procedureHelper)
    {
        _logger = logger;
        _manager = manager;
        _dealHelper = dealHelper;
        _orderHelper = orderHelper;
        _positionHelper = positionHelper;
        _userHelper = userHelper;
        _dailyHelper = dailyHelper;
        _envPath = configuration.GetValue<string>("EnvPath") ?? throw new ArgumentNullException(nameof(_envPath));
        _procedureHelper = procedureHelper;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        SetEnvironment();
        string error = string.Empty;
        if (!_manager.Initialize(ref error))
        {
            _logger.LogError(error);
            return;
        }
        _logger.LogInformation("Manager initialize success fully");
        //await _procedureHelper.CreateBulkUpsertManagerOrdersProcedure();
        await _userHelper.InitializeUser();
        await _orderHelper.InitializeOrder();
        await _dealHelper.InitializeDeal();
        await _positionHelper.InitializePosition();
        await _dailyHelper.InitializeDaily();
        await _procedureHelper.CreateManagerOrdersProcedure();
        await _procedureHelper.CreateTradeDataProcedure();
        await _procedureHelper.CreateManagerDealsProcedure();
        await _procedureHelper.CreateManagerSummariesProcedure();
        await _procedureHelper.CreateManagerDailyProcedure();
        await _procedureHelper.CreateManagerPositionsProcedure();
        await _procedureHelper.CreateGetTradeSummaryDataProcedure();
    }
    public Task StopAsync(CancellationToken cancellationToken)
    {
       return Task.CompletedTask;
    }
    private void SetEnvironment()
    {
        if (System.Environment.GetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS") != null &&
            string.Compare(System.Environment.GetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS"), _envPath) != 0)
        {
            System.Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", _envPath);
        }
        if (System.Environment.GetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS") == null)
        {
            System.Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", _envPath);
        }
    }
}
