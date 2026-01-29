using Apmcrm.V1.Msgs;
using Apmcrm.V1.Msgs.Types;
using CLIB.Constants;
using CLIB.Models;
using CRMUKMTPApi;
using CRMUKMTPApi.Data;
using CRMUKMTPApi.Extentions;
using CRMUKMTPApi.Helpers;
using CRMUKMTPApi.Middleware;
using CRMUKMTPApi.Models;
using CRMUKMTPApi.Repositories;
using Google.Protobuf;
using Microsoft.EntityFrameworkCore;
using MT5LIB;
using MT5LIB.Helpers;
using Newtonsoft.Json;
using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
string appdir = builder.Configuration.GetValue<string>("AppDir") ?? throw new ArgumentNullException("AppDir");
var logger = builder.AddSerilogServices(appdir);
string? LaunchUrl = builder.Configuration["LaunchUrl"];

try
{
    string conString = builder.Configuration.GetConnectionString("default") ?? throw new ArgumentNullException(nameof(conString));
    ConfigInfo configInfo = builder.Configuration.GetSection("ConfigInfo").Get<ConfigInfo>() ?? throw new ArgumentNullException(nameof(ConfigInfo));
    var jwt = builder.Configuration.GetSection("jwt").Get<JWTModel>() ?? throw new ArgumentNullException("jwt");

    builder.Services.AddSingleton(configInfo);
    builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));
    builder.Services.AddDbContext<AppDBContext>(cfg => cfg.UseMySQL(conString));
    //builder.Services.AddControllers().AddJsonOptions(x =>
    //{
    //    x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve;
    //});
    builder.Services.AddScoped<ITokenRepository, TokenRepository>();
    builder.Services.AddScoped<IOrderRepository, OrderRepository>();
    builder.Services.AddScoped<IUserRepository, UserRepository>();
    builder.Services.AddScoped<IDealRepository, DealRepository>();
    builder.Services.AddScoped<IPositionRepository, PositionRepository>();
    builder.Services.AddScoped<ISummaryRepository, SummaryRepository>();
    builder.Services.AddScoped<IDailyRepository, DailyRepository>();
    builder.Services.AddScoped<ILastTradeTimeRepository, LastTradeTimeRepository>();
    builder.Services.AddScoped<ITransactionRepository, TransactionRepository>();
    builder.Services.AddSingleton<Manager>();
    builder.Services.AddSingleton<CDealSink>();
    builder.Services.AddSingleton<COrderSink>();
    builder.Services.AddSingleton<CUserSink>();
    builder.Services.AddSingleton<CPositionSink>();
    builder.Services.AddSingleton<CDailySink>();
    builder.Services.AddSingleton<CTickSink>();

    builder.Services.AddSingleton<MT5LIBHelper>();
    builder.Services.AddSingleton<ProcedureHelper>();
    builder.Services.AddSingleton<DealHelper>();
    builder.Services.AddSingleton<OrderHelper>();
    builder.Services.AddSingleton<PositionHelper>();
    builder.Services.AddSingleton<UserHelper>();
    builder.Services.AddSingleton<DailyHelper>();

    builder.Services.AddHostedService<AppHostService>();
    // Add services to the container.
    builder.AddSwaggerGenUI("UKCrm Api");
    



    //builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    //.AddJwtBearer(options =>
    //{
    //    // Firebase will validate this separately, so we don’t need much here.
    //});
    builder.Services.AddSignalR();
    builder.Services.AddAuthorization();

    //builder.Services.AddControllers().AddJsonOptions(options =>
    //{
    //    options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
    //    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    //    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    //});
    builder.Services.AddHttpContextAccessor();
    builder.Services.AddControllers().AddNewtonsoftJson(options =>
    {
        options.SerializerSettings.ContractResolver = new OrderedContractResolver();
        options.SerializerSettings.Formatting = Formatting.Indented; // optional
    });
    // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
    builder.Host.UseWindowsService(options =>
    {
        options.ServiceName = "UKCRMService";
    });

    if (!string.IsNullOrEmpty(LaunchUrl))
        builder.WebHost.UseUrls(LaunchUrl);

    builder.Services.AddCors(o => o.AddPolicy("AllowAll", builder =>
    {
        builder.AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader()
                .WithExposedHeaders("Grpc-Status", "Grpc-Message", "Grpc-Encoding", "Grpc-Accept-Encoding");
    }));

    var app = builder.Build();
    app.UseCors("AllowAll");
    // Configure the HTTP request pipeline.


    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "UKCrm Api");
    });


    app.UseHttpsRedirection();

    app.UseAuthorization();
    app.UseWebSockets();
    app.MapControllers();
    //app.UseMiddleware<OriginMiddleware>();
    app.UseMiddleware<GlobalExceptionHandler>();
    //app.UseMiddleware<FirebaseAuthenticationMiddleware>();

    app.Map("/listener", async context =>
    {
        if (!context.WebSockets.IsWebSocketRequest)
            return;

        var scopeFactory = app.Services.GetRequiredService<IServiceScopeFactory>();
        using var scope = scopeFactory.CreateScope();
        var tokenRepository = scope.ServiceProvider.GetRequiredService<ITokenRepository>();
        var ws = await context.WebSockets.AcceptWebSocketAsync();
        byte[] buffer = new byte[4560];

        try
        {
            while (ws.State == WebSocketState.Open)
            {
                var result = await ws.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                if (result.MessageType == WebSocketMessageType.Close)
                {
                    RemoveWebSocket(ws);
                    break;
                }

                byte[] message = buffer[..result.Count];
                Authorization? auth = result.MessageType switch
                {
                    WebSocketMessageType.Binary => Authorization.Parser.ParseFrom(message),
                    WebSocketMessageType.Text => JsonConvert.DeserializeObject<Authorization>(Encoding.UTF8.GetString(message)),
                    _ => null
                };

                if (auth == null || !await tokenRepository.AuthenticateUser(auth.Token))
                    continue;

                await tokenRepository.SetToken(auth.Token, context);
                string? role = tokenRepository.GetRole();
                if (string.IsNullOrEmpty(role))
                    continue;

                if (role.Equals(AppRoles.Admin, StringComparison.OrdinalIgnoreCase))
                {
                    AddSocketToGroup(0, ws);

                    if (result.MessageType == WebSocketMessageType.Text)
                    {
                        var response = new Response { Result = "Connected", Successful = true };
                        var msg = new ApmCrmMessage
                        {
                            State = MessageState.New,
                            Type = MessageType.Response,
                            Value = response.ToByteString()
                        };
                        await ws.SendAsync(new ArraySegment<byte>(msg.ToByteArray()), WebSocketMessageType.Binary, true, CancellationToken.None);
                    }
                }
                else
                {
                    var logins = tokenRepository.GetLogin();
                    if (logins == null) continue;
                    foreach (var login in logins)
                        AddSocketToGroup(login, ws);
                }
            }
        }
        catch (Exception ex)
        {
            // Optional: log exception
        }
        finally
        {
            RemoveWebSocket(ws);
            await ws.CloseAsync(WebSocketCloseStatus.NormalClosure, "Connection closed", CancellationToken.None);
            ws.Dispose();
        }
    }).AllowAnonymous();


    app.Run();

}
catch (Exception ex)
{
    logger.Error($"{ex.StackTrace}");
}

void RemoveWebSocket(WebSocket ws)
{
    foreach (var key in Globals.ConnectedSockets.Keys)
    {
        if (Globals.ConnectedSockets.TryGetValue(key, out var bag))
        {
            var filtered = new ConcurrentBag<WebSocket>(bag.Where(s => s != ws));
            Globals.ConnectedSockets[key] = filtered;
        }
    }
}
void AddSocketToGroup(ulong login, WebSocket ws)
{
    Globals.ConnectedSockets.AddOrUpdate(
        login,
        _ => new ConcurrentBag<WebSocket> { ws },
        (_, existingBag) =>
        {
            existingBag.Add(ws);
            return existingBag;
        });
}