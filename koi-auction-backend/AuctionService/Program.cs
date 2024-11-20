
using AuctionService.Controller;
using AuctionService.IRepository;
using AuctionService.Repository;
using AuctionService.Middlewares;
using Microsoft.EntityFrameworkCore;
using AuctionService.Data;
using AuctionService.IServices;
using AuctionService.Services;
using AuctionService.HandleMethod;
using AuctionService.Dto.UserConnection;
using AuctionService.Hubs;
using AuctionService.Helper;
using AuctionService.Dto.User;

var builder = WebApplication.CreateBuilder(args);
// // Đăng ký các cấu hình từ appsettings.json
// builder.Services.Configure<BiddingServiceOptionDtos>(
//     builder.Configuration.GetSection("BiddingService"));

builder.Configuration.AddEnvironmentVariables()
                     .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                     .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnectionStringDB");
var allowedOrigins = builder.Configuration.GetSection("AllowedOrigins").Get<string[]>();
allowedOrigins = allowedOrigins == null ? new string[] { "http://localhost:5173", "http://localhost:3000" } : allowedOrigins;


//log allow origin to azure app service

System.Console.WriteLine("Allowed Origins: ");
if (allowedOrigins != null)
{
    foreach (var origin in allowedOrigins)
    {
        System.Console.WriteLine(origin);
    }
}



builder.Services.AddControllers();
builder.Services.AddControllers().AddNewtonsoftJson(options =>
{
    options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<AuctionManagementDbContext>(option =>
{
    option.UseSqlServer(connectionString);
});
builder.Services.AddScoped<IBidStrategy, FixedPriceBidStrategy>();
builder.Services.AddScoped<IBidStrategy, SealedBidStrategy>();
builder.Services.AddScoped<IBidStrategy, AscendingBidStrategy>();
builder.Services.AddScoped<IBidStrategy, DescendingBidStrategy>();

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<ILotRepository, LotRepository>();
builder.Services.AddScoped<IKoiFishRepository, KoiFishRepository>();
builder.Services.AddScoped<IAuctionMethodRepository, AuctionMethodRepository>();
builder.Services.AddScoped<ILotStatusRepository, LotStatusRepository>();
builder.Services.AddScoped<IAuctionRepository, AuctionRepository>();
builder.Services.AddScoped<IAuctionLotRepository, AuctionLotRepository>();
builder.Services.AddScoped<IBidLogRepository, BidLogRepository>();
builder.Services.AddScoped<ISoldLotRepository, SoldLotRepository>();
builder.Services.AddScoped<IAuctionDepositRepository, AuctionDepositRepository>();
builder.Services.AddScoped<IAuctionDepositService, AuctionDepositService>();

builder.Services.AddHttpClient<WalletService>();
builder.Services.AddScoped<ISoldLotService, SoldLotService>();
builder.Services.AddScoped<IBidLogService, BidLogService>();
builder.Services.AddScoped<BidService>();
builder.Services.AddScoped<WalletService>();
builder.Services.AddScoped<MailService>();
builder.Services.AddScoped<ILotService, LotService>();

builder.Services.AddSingleton<BidManagementService>();
builder.Services.AddScoped<IAuctionService, AuctionService.Services.AuctionService>();
builder.Services.AddScoped<IAuctionLotService, AuctionLotService>();

builder.Services.AddSingleton<ITaskSchedulerService, TaskSchedulerService>();

builder.Services.AddScoped<UserSevice>();
builder.Services.AddScoped<BreederDetailService>();

builder.Services.AddScoped<IAuctionStatusRepository, AuctionStatusRepository>();
builder.Services.AddScoped<IAuctionLotStatusRepository, AuctionLotStatusRepository>();

builder.Services.AddSingleton<IDictionary<string, UserConnectionDto>>(_ => new Dictionary<string, UserConnectionDto>());
builder.Services.AddSignalR();
builder.Services.AddMemoryCache();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigins", policy =>
    {
        policy.WithOrigins(allowedOrigins!)
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();

    });
});
builder.Services.AddHttpContextAccessor();
builder.Services.AddHttpClient();

var app = builder.Build();

app.UseMiddleware<AuthorizationMiddleware>();
app.UseMiddleware<ExceptionMiddleware>();
app.UseCors("AllowSpecificOrigins");

app.UseSwagger();
app.UseSwaggerUI();
app.MapGet("/", context =>
{
    context.Response.Redirect("/swagger");
    return Task.CompletedTask;
});

app.MapHub<BidHub>("/hub");
app.UseHttpsRedirection();
app.MapControllers();

app.Run();
