using CRMUKMTPApi.Data;
using CRMUKMTPApi.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CRMUKMTPApi.CommandHandler
{
    public class FirstDepositRequestCommand : IRequest<FirstDepositResponse>
    {
        public FirstDepositRequestModel Params { get; }

        public FirstDepositRequestCommand(FirstDepositRequestModel parameters)
        {
            Params = parameters;
        }
    }

    public class FirstDepositRequestCommandHandler
      : IRequestHandler<FirstDepositRequestCommand, FirstDepositResponse>
    {
        private readonly ILogger<FirstDepositRequestCommandHandler> _logger;
        private readonly AppDBContext _dbContext;

        public FirstDepositRequestCommandHandler(
            ILogger<FirstDepositRequestCommandHandler> logger,
            AppDBContext dbContext)
        {
            _logger = logger;
            _dbContext = dbContext;
        }

        public async Task<FirstDepositResponse> Handle(
            FirstDepositRequestCommand request,
            CancellationToken cancellationToken)
        {
            var response = new FirstDepositResponse();

            try
            {
                var mt5Ids = request.Params.mt5Id;

                // First Deposit (oldest deal)
                var firstDeposits = await _dbContext.Deals
                    .Where(d => mt5Ids.Contains(d.LoginId))
                    .GroupBy(d => d.LoginId)
                    .Select(g => new
                    {
                        LoginId = g.Key,
                        FirstDepositTime = g.Min(x => x.Time),

                        FirstDepositAmount = g.Where(x=>x.Entry=="IN")
                            .OrderBy(x => x.Time)
                            .Select(x => x.Profit)

                            .FirstOrDefault(),
                        Method = "Deposit"
                    })
                    .ToListAsync(cancellationToken);

                // Last Trade
                var lastTrades = await _dbContext.Deals
                    .Where(d => mt5Ids.Contains(d.LoginId))
                    .GroupBy(d => d.LoginId)
                    .Select(g => new
                    {
                        LoginId = g.Key,
                        LastTradeTime = g.Max(x => x.Time)
                    })
                    .ToDictionaryAsync(x => x.LoginId, x => x.LastTradeTime, cancellationToken);

                // User Info
                var users = await _dbContext.Users
                    .Where(u => mt5Ids.Contains(u.LoginId))
                    .ToDictionaryAsync(x => x.LoginId, cancellationToken);

                foreach (var fd in firstDeposits)
                {
                    var user = users.GetValueOrDefault(fd.LoginId);

                    response.Data[fd.LoginId] = new FirstDepositData
                    {
                       AccountCreatedTime = user?.Created ?? DateTime.MinValue,
                        FirstDepositAmount = fd.FirstDepositAmount,
                        FirstDepositMethod = fd.Method,
                        FirstDepositTime = fd.FirstDepositTime,
                       FirstName = user?.Name ?? "",
                        LastName = user?.LastName ?? "",
                        LastTradeTime = lastTrades.GetValueOrDefault(fd.LoginId),
                        Mt5Id = fd.LoginId
                    };
                }

                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in FirstDepositRequest");
                throw;
            }
        }
    }



}

