using Autofac.Core;
using CloudinaryDotNet;
using Eigakan.Application.Helper;
using Eigakan.Application.Helper.Logging;
using Eigakan.Application.Interface;
using Eigakan.Application.Interface.IRepository;
using Eigakan.Application.Service;
using Eigakan.Infractructure.Repositories.AuthRepositories;
using Eigakan.Infractructure.Repositories.MovieRepositories;
using Eigakan.Infractructure.Repositories.ContractRepositories;
using Eigakan.Infractructure.Repositories.RoleRepositories;
using Eigakan.Infractructure.Repositories.UserRegisterRepositories;
using Eigakan.Infractructure.Repositories.UserRepositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;
using System.Text;
using static Eigakan.Application.Helper.EmailSetting;
using Eigakan.Infractructure.Repositories.GerneRepositories;
using Eigakan.Infractructure.Repositories.PersonRepositories;
using Eigakan.Infractructure.Repositories.MoviePersonRepositories;
using Eigakan.Infractructure.Repositories.MovieGenreRepositories;
using Eigakan.Infractructure.Repositories.MediaRepositories;
using Eigakan.Infractructure.Repositories.GenericRepositories;
using Eigakan.Infractructure.Repositories.NewsRepositories;
using Eigakan.Infractructure.Repositories.SubscriptionPackageRepositories;
using Eigakan.Infractructure.Repositories.SubscriptionPurchaseRepositories;
using Eigakan.Infractructure.Repositories.CommentRepositories;
using Amazon.S3;
using Amazon;
using Eigakan.Infractructure.Repositories.MovieRatingRepositories;
using Eigakan.Infractructure.Repositories.AdPackageRepositories;
using Eigakan.Application.Helper.Configuration;
using StackExchange.Redis;
using Eigakan.Infractructure.Base;
using Eigakan.Infractructure.Repositories.AdMediaRepositories;
using Eigakan.Infractructure.Repositories.RoomRepositories;
using Eigakan.DomainMongoDB.Base;
using Eigakan.Infractructure.Repositories.UserRoomRepositories;
using Eigakan.Application.Helper.SignalR;
using Eigakan.Infractructure.Repositories.MovieHistoryRepositories;
using Eigakan.Infractructure.Repositories.MovieCountRepositories;
using Eigakan.Infractructure.Repositories.ViewPaymentPolicyRepositories;

using Eigakan.Infractructure.Repositories.UserEariningRepositories;
using Eigakan.Infractructure.Repositories.MovieEarningRepositories;
using Eigakan.Infractructure.Repositories.UserWalletRepositories;
using Eigakan.Infractructure.Repositories.WalletTransactionRepositories;
using Eigakan.Infractructure.Repositories.AdPurchaseItemRepositories;
using Eigakan.Infractructure.Repositories.AdPurchaseTransactionRepositories;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
	options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
	{
		Description = "Standard Authorization header using the Bearer scheme (\"bearer {token}\")",
		In = ParameterLocation.Header,
		Name = "Authorization",
		Type = SecuritySchemeType.ApiKey
	});

	options.OperationFilter<SecurityRequirementsOperationFilter>();
});


// Đọc cấu hình từ appsettings.json
var configuration = builder.Configuration;

// Đăng ký DbContext
builder.Services.AddDbContext<EigakanDbContext>(options =>
	options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

// Mongodb
builder.Services.AddSingleton<MongoDbContext>();


//Cors
builder.Services.AddCors(options =>
{
	options.AddPolicy("AllowAll", policy =>
	{
		policy.SetIsOriginAllowed(_ => true) 
			  .AllowAnyHeader()
			  .AllowAnyMethod()
			  .AllowCredentials();
	});
});


//Authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
	.AddJwtBearer(options =>
	{
		options.TokenValidationParameters = new TokenValidationParameters
		{
			ValidateIssuerSigningKey = true,
			IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8
				.GetBytes(builder.Configuration.GetSection("AppSettings:Token").Value)),
			ValidateIssuer = false,
			ValidateAudience = false
		};
	});

// Load Cloudinary settings from appsettings.json
builder.Services.Configure<CloudinarySettings>(builder.Configuration.GetSection("Cloudinary"));

//DI
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IAuthRepository, AuthRepository>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IMoviesRepository, MovieRepository>();
builder.Services.AddScoped<IMovieService, MovieService>();
builder.Services.AddScoped<IUserRegisterRepository, UserRegisterRepository>();
builder.Services.AddScoped<IUserRegisterService, UserRegisterService>();
builder.Services.AddScoped<IRoleRepository, RoleRepository>();
builder.Services.AddScoped<IRoleService, RoleService>();
builder.Services.AddScoped<IGenreRepository, GerneRepository>();
builder.Services.AddScoped<IGenreService, GenreService>();
builder.Services.AddScoped<Webhook>();
builder.Services.AddScoped<NotificationService>();
builder.Services.AddScoped<IContractRepository, ContractRepository>();
builder.Services.AddScoped<IContractService, ContractService>();
builder.Services.AddScoped<IPersonRepository, PersonRepository>();
builder.Services.AddScoped<IMoviePersonRepository, MoviePersonRepository>();
builder.Services.AddScoped<IMovieGenreRepository, MovieGenreRepository>();
builder.Services.AddScoped<IPersonService, PersonService>();
builder.Services.AddScoped<IMediaRepository, MediaRepository>();
builder.Services.AddScoped<IMediaService,MediaService>();
builder.Services.AddScoped<IUploadService, UploadService>();
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
builder.Services.AddScoped<INewsRepository, NewsRepository>();
builder.Services.AddScoped<INewsService, NewsService>();
builder.Services.AddScoped<ISubscriptionPackageRepository, SubscriptionPackagRepository>();
builder.Services.AddScoped<ISubscriptionPackageService, SubscriptionPackageService>();
builder.Services.AddScoped<ISubscriptionPurchaseService, SubscriptionPurchaseService>();
builder.Services.AddScoped<ISubscriptionPurchaseRepository, SubscriptionPurchaseRepository>();
builder.Services.AddScoped<IVnPayService, VnPayService>();
builder.Services.AddScoped<ICommentRepository, CommentRepository>();
builder.Services.AddScoped<ICommentService, CommentService>();
builder.Services.AddScoped<IMovieRatingRepository, MovieRatingRepository>();
builder.Services.AddScoped<IMovieRatingService, MovieRatingService>();
builder.Services.AddScoped<IAdPackageRepository, AdPackageRepository>();
builder.Services.AddScoped<IAdPackageService, AdPackageService>();
builder.Services.AddScoped<IAdMediaRepository, AdMediaRepository>();
builder.Services.AddScoped<IAdMediaService, AdMediaService>();
builder.Services.AddScoped<IRoomRepository, RoomRepository>();
builder.Services.AddScoped<IUserRoomRepository, UserRoomRepository>();
builder.Services.AddScoped<IRoomService ,RoomService>();
builder.Services.AddScoped<IMovieHistoryRepository, MovieHistoryRepository>();
builder.Services.AddScoped<IMovieHistoryService, MovieHistoryService>();
builder.Services.AddScoped<IMovieCountRepository, MovieCountRepository>();
builder.Services.AddScoped<IMovieCountService, MovieCountService>();
builder.Services.AddScoped<IPayOSService, PayOSService>();
builder.Services.AddScoped<IViewPaymentPolicyRepository, ViewPaymentPolicyRepository>();
builder.Services.AddScoped<IViewPaymentPolicyService, ViewPaymentPolicyService>();
builder.Services.AddScoped<IAdMediaCountRepository, AdMediaCountRepository>();
builder.Services.AddScoped<IAdMediaCountService, AdMediaCountService>();
builder.Services.AddScoped<IUserEarningRepository, UserEarningRepository>();
builder.Services.AddScoped<IUserEarningService, UserEarningService>();
builder.Services.AddScoped<IMovieEarningRepository, MovieEarningRepository>();
builder.Services.AddScoped<IMovieEarningService, MovieEarningService>();
builder.Services.AddScoped<IUserWalletRepository, UserWalletRepository>();
builder.Services.AddScoped<IUserWalletService, UserWalletService>();
builder.Services.AddScoped<IWalletTransactionRepository, WalletTransactionRepository>();
builder.Services.AddScoped<IWalletTransactionService, WalletTransactionService>();
builder.Services.AddScoped<IAdPurchaseItemRepository, AdPurchaseItemRepository>();
builder.Services.AddScoped<IAdPurchaseItemService, AdPurchaseItemService>();
builder.Services.AddScoped<IAdPurchaseTransactionRepository, AdPurchaseTransactionRepository>();
builder.Services.AddScoped<IAdPurchaseTransactionService, AdPurchaseTranasctionService>();
builder.Services.AddScoped<IAdminDashboardService, AdminDashboardService>();

//Background service
builder.Services.AddHostedService<RoomCleanupService>();
builder.Services.AddHostedService<SubscriptionStatusBackgroundService>();
builder.Services.AddHostedService<ContractExpirationNotificationService>();
builder.Services.AddHostedService<ContractExpirationStatusUpdateService>();
builder.Services.AddHostedService<WalletTransactionTimeoutService>();
builder.Services.AddHostedService<RefundRemainingViewsService>();
builder.Services.AddHostedService<EarningCalculationBackgroundService>();


builder.Services.AddSignalR();


builder.Services.AddSingleton(new Lazy<IAmazonS3>(() =>
{
    var accessKey = configuration["AWS:AccessKey"];
    var secretKey = configuration["AWS:SecretKey"];
    var region = RegionEndpoint.GetBySystemName(configuration["AWS:Region"]);
    return new AmazonS3Client(accessKey, secretKey, region);
}));


//BunnyCDN
builder.Services.Configure<BunnyStreamSettings>(builder.Configuration.GetSection("BunnyStream"));

builder.Services.AddHttpClient<BunnyStreamUploadService>()
	.ConfigureHttpClient(client => client.Timeout = TimeSpan.FromMinutes(15)); 


//Aws
builder.Services.AddSingleton<AwsS3Service>();

//Email
builder.Services.AddTransient<IEmailService, EmailService>();
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));

//Mapper
builder.Services.AddAutoMapper(typeof(AutoMapperProfile));

//Cloudinary
builder.Services.AddSingleton(serviceProvider =>
{
	var cloudinarySettings = serviceProvider.GetRequiredService<IOptions<CloudinarySettings>>().Value;
	return new Cloudinary(new Account(cloudinarySettings.CloudName, cloudinarySettings.ApiKey, cloudinarySettings.ApiSecret));
});

//Redis
var redisConfig = builder.Configuration.GetSection("RedisConfiguration").Get<RedisConfiguration>();

 if (redisConfig != null && redisConfig.Enable && !string.IsNullOrEmpty(redisConfig.ConnectionString))
	{
		var redis = ConnectionMultiplexer.Connect(redisConfig.ConnectionString);
		builder.Services.AddSingleton(redis);
	}
 else
	{
		Console.WriteLine("Redis is disabled or misconfigured.");
	}


builder.Services.Configure<RedisConfiguration>(builder.Configuration.GetSection("RedisConfiguration"));
builder.Services.AddScoped<ICacheService, CacheService>();


// Cấu hình các dịch vụ DI
builder.Services.Configure<DiscordWebhookUrls>(builder.Configuration.GetSection("DiscordWebhookUrls"));
builder.Services.AddSingleton<HttpClient>(); 
builder.Services.AddScoped<Logger>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddSingleton<PasswordSettings>();


//builder.Services.AddCors(options =>
//{
//    options.AddPolicy("AllowAll",
//        policy =>
//        {
//            policy.WithOrigins("http://localhost:5173", "https://eigakan-fe.vercel.app", "http://192.168.1.20:5173", "https://deploy-eigakan.vercel.app", "https://eigakan-fe-git-quan-micharel09s-projects.vercel.app") 
//                  .AllowAnyHeader()
//                  .AllowAnyMethod()
//                  .AllowCredentials();
//        });
//});

var app = builder.Build();


app.UseSwagger();
app.UseSwaggerUI();

if (app.Environment.IsDevelopment())
{

}

app.UseHttpsRedirection();

app.UseCors("AllowAll");
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
    endpoints.MapHub<RoomHub>("/roomHub");

});

app.Run();
