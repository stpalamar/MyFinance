using System.Text;
using Api.Middleware;
using ApplicationCore.Interfaces;
using ApplicationCore.Services;
using Infrastructure;
using Infrastructure.Data;
using Microsoft.AspNet.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddTransient<DataSeed>();
builder.Services.AddDbContext<ApplicationDbContext>();
builder.Services.AddTransient<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddTransient<ITransactionService, TransactionService>();
builder.Services.AddTransient<IUserService, UserService>();
builder.Services.AddTransient<IReceiptService, ReceiptService>();
builder.Services.AddTransient<IAccountService, AccountService>();
builder.Services.AddTransient<IPaymentService, PaymentService>();
builder.Services.AddTransient<IImportExportService, ImportExportService>();
builder.Services.AddTransient<IPasswordHasher, PasswordHasher>();
builder.Services.AddLogging();
builder.Services.AddTransient<ErrorHandlingMiddleware>();

var secret = Environment.GetEnvironmentVariable("JWT_SECRET");
var issuer = Environment.GetEnvironmentVariable("JWT_ISSUER");

builder.Services.AddAuthentication(opts =>
    {
        opts.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        opts.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(opts =>
    {
        opts.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            ValidIssuer = issuer,
            ValidateAudience = false,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret))
        };
    });

var app = builder.Build();


using (var scope = app.Services.CreateScope())
{
    var scopedProvider = scope.ServiceProvider.GetService<DataSeed>();
    scopedProvider?.Seed();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.UseMiddleware<ErrorHandlingMiddleware>();
app.MapControllers();
app.Run();