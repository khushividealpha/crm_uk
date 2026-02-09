using CRMUKMTPApi.CommandHandler;
using CRMUKMTPApi.Models;
using CRMUKMTPApi.QueryHandler;
using CRMUKMTPApi.Repositories;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CRMUKMTPApi.Controllers;
[ApiController]
[Route("api/v1")]

public class HomeController : Controller
{
    private readonly IMediator _mediator;
    private readonly ITransactionRepository _repo;
    public HomeController(IMediator mediator, ITransactionRepository repo)
    {
        _mediator = mediator;
        _repo = repo;
    }
    [HttpGet]
    [Route("info")]
    public  string GetInfo()
    {

        return "{\r\n    \"status\": \"success\"\r\n}";
    }

    [HttpGet]
    [Route("orders")]
    public async Task<ApiResponse> GetOrder(int page = 1, int limit = 25,
        string loginid = "", string sort = "",
        string filter = "", string filtervalue = "",
        string from = "", string to = "", bool topdf = false)
    {
   
        ParamModel model = new ParamModel
        {
            Filter = filter,
            Filtervalue = filtervalue,
            From = from,
            To = to,
            Limit = limit,
            Sort = sort,
            Loginid = loginid,
            Page = page,
            ToPdf = topdf
        };

        GetOrderQuery query = new GetOrderQuery
            
            
            
            (model);
        return await _mediator.Send(query);
    }

    [HttpGet]
    [Route("deals")]
    public async Task<ApiResponse> GetDeal(int page = 1, int limit = 25,
        string loginid = "", string sort = "", string filter = "",
        string filtervalue = "", string from = "",string to = "", bool topdf = false, bool tocsv = false)
    {
        ParamModel model = new ParamModel
        {
            Filter = filter,
            Filtervalue = filtervalue,
            From = from,
            To = to,
            Limit = limit,
            Sort = sort,
            Loginid = loginid,
            Page = page,
            ToCSV = tocsv,
            ToPdf = topdf
        };
        GetDealQuery query = new GetDealQuery(model);
        return await _mediator.Send(query);
    }

    [HttpGet]
    [Route("positions/open")]
    public async Task<ApiResponse> GetPosition(int page = 1, int limit = 25, string sort = "", string loginid = "", string filter = "", string filtervalue = "")
    {
        ParamModel model = new ParamModel
        {
            Filter = filter,
            Filtervalue = filtervalue,
            Limit = limit,
            Sort = sort,
            Loginid = loginid,
            Page = page
        };
        GetPositionrQuery query = new GetPositionrQuery(model);
        return await _mediator.Send(query);
    }

    [HttpGet]
    [Route("summary")]
    public async Task<ApiResponse> GetSummary(int page= 1,int limit = 25, string loginid = "", string sort = "",string filter = "" ,string filtervalue = "" ,bool topdf = false ,bool tocsv = false)
    {
        ParamModel model = new ParamModel
        {
            Filter = filter,
            Filtervalue = filtervalue,
            Limit = limit,
            Sort = sort,
            Loginid = loginid,
            Page = page,
            ToPdf = topdf
        };
        GetSummaryQuery query = new GetSummaryQuery(model);
        return await _mediator.Send(query);
    }

    [HttpGet]
    [Route("trans")]
    public async Task<ApiResponse> GetTransaction(int page= 1 ,int limit = 25 ,string loginid = "" ,string sort = "" ,
        string filter = "" ,string filtervalue = "" ,string from = "" ,string to = "" ,bool topdf = false ,bool tocsv = false)
    {
        ParamModel model = new ParamModel
        {
            Filter = filter,
            Filtervalue = filtervalue,
            Limit = limit,
            Sort = sort,
            Loginid = loginid,
            Page = page,
            ToPdf = topdf,
            From = from,
            To = to,
            ToCSV=tocsv,
        };
        GetTransactionQuery query = new GetTransactionQuery(model);
        return await _mediator.Send(query);
    }
   
    [HttpGet]
    [Route("users")]
    public async Task<ApiResponse> GetUsers(int page = 1, int limit = 25, string loginid = "", string sort = "", string filter = "", string filtervalue = "", bool topdf = false)
    {
        ParamModel model = new ParamModel
        {
            Filter = filter,
            Filtervalue = filtervalue,
            Limit = limit,
            Sort = sort,
            Loginid = loginid,
            Page = page,
            ToPdf = topdf
        };
        GetUserQuery query = new GetUserQuery(model);
        return await _mediator.Send(query);
    }
    
    [HttpGet]
    [Route("daily")]
    public async Task<ApiResponse> GetDaily(int page = 1, int limit = 25, string loginid = "", string sort = "", string filter = "", string filtervalue = "", bool topdf = false, string from = "", string to = "")
    {
        ParamModel model = new ParamModel
        {
            Filter = filter,
            Filtervalue = filtervalue,
            Limit = limit,
            Sort = sort,
            Loginid = loginid,
            Page = page,
            ToPdf = topdf,
            From = from,
            To = to,
            
        };
        GetDailyQuery query = new GetDailyQuery(model);
        return await _mediator.Send(query);
    }
   
    [HttpGet]
    [Route("tradeData")]
    public async Task<ApiResponse> GetTradeData(string from, string to,int page = 1, int limit = 25, string symbol = "", string login = "")
    {
        ParamModel model = new ParamModel
        {
            Limit = limit,
            Loginid = login,
            Page = page,
            From = from,
            To = to,
            Symbols=symbol
        };
        GetTradeDataQuery query = new GetTradeDataQuery(model);
        return await _mediator.Send(query);
    }

    [HttpGet]
    [Route("tradeSummary")]
    public async Task<ApiResponse> GetTradeSummary(string from, string to, string login = "")
    {
        ParamModel model = new ParamModel
        {
            Loginid = login,
            From = from,
            To = to,
        };
        GetTradeSummaryQuery query = new GetTradeSummaryQuery(model);
        return await _mediator.Send(query);
    }
   
    [HttpPost]
    [Route("lasttradetime")]
    public async Task<object> GetLastTradeTime([FromBody] GetLastTradeTimeRequest req)
    {
        GetLastTradeTimeQuery query = new GetLastTradeTimeQuery(req.Mt5Ids);
        return await _mediator.Send(query);
    }
    [HttpPost("deposit")]
    public async Task<IActionResult> Deposit([FromBody] TransactionRequest request)
    {
        if (request == null)
            return BadRequest("Invalid request");

        var result = await _repo.DepositWithdrawalAsync(
            request.Mt5Id,
            request.Amount,
            request.Remarks
        );

        if (!result.Success)
        {
            return BadRequest(new
            {
                status = "Failed",
                message = result.Message
            });
        }

        return Ok(new
        {
            status = "Success",
            dealId = result.DealId,
            message = result.Message
        });
    }
    [HttpPost("withdrwal")]
    public async Task<IActionResult> Withdrawal([FromBody] TransactionRequest request)
    {
        if (request == null)
            return BadRequest("Invalid request");

        var result = await _repo.DepositWithdrawalAsync(
            request.Mt5Id,
            -request.Amount,
            request.Remarks
        );

        if (!result.Success)
        {
            return BadRequest(new
            {
                status = "Failed",
                message = result.Message
            });
        }

        return Ok(new
        {
            status = "Success",
            dealId = result.DealId,
            message = result.Message
        });
    }
    [HttpPost("internaltransfer")]
    public async Task<IActionResult> InternalTransfer([FromBody] InternalTranReuest request)
    {
        if (request == null)
            return BadRequest("Invalid request");

        var result = await _repo.TransferBetweenAccountsAsync(
            request.From,
            request.To,
            request.Amount,
            request.FromComment,
            request.ToComment


        );

        if (!result)
        {
            return BadRequest(new
            {
                status = "Failed",
            });
        }

        return Ok(new
        {
            status = "Success",
        });
    }
    [HttpPost("signupmt")]
    public async Task<object> SignUpMT([FromBody] SignUpMTModel request)
    {
        SignupMt5UserCommand query = new SignupMt5UserCommand(request);
        return await _mediator.Send(query);
    }

    [HttpPost("createclient")]
    public async Task<object> CreateClient([FromBody] CreateClientRequest client )
    {
        CreateClientCommand query = new CreateClientCommand(client);
        return await _mediator.Send(query);
    }
    [HttpGet("firstDeposit")]
    public async Task<object> FirstDeposit([FromBody] FirstDepositRequestModel client)
    {
        FirstDepositRequestCommand query = new FirstDepositRequestCommand(client);
        return await _mediator.Send(query);
    }

    [HttpPost("updateUser")]
    public async Task<IActionResult> UpdateUsers(
        [FromBody] UpdateUsersRequest request)
    {
        if (request.Users == null || !request.Users.Any())
            return BadRequest("Users list is empty");

        var command = new UpdateUsersCommand(request.Users);

        var result = await _mediator.Send(command);

        return Ok(result);
    }
}

