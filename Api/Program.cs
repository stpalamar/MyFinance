using Api.Middleware;
using ApplicationCore.Interfaces;
using ApplicationCore.Services;
using ApplicationCore.Utilities;
using Infrastructure;
using Microsoft.AspNet.Identity;

const string myAllowSpecificOrigins = "_myAllowSpecificOrigins";
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: myAllowSpecificOrigins,
        policy  =>
        {
            policy.WithOrigins("http://localhost:3000")
                .AllowCredentials()
                .AllowAnyHeader()
                .AllowAnyMethod();
        });
});

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<ApplicationDbContext>();
builder.Services.AddTransient<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddTransient<IJwtUtils, JwtUtils>();
builder.Services.AddTransient<ITransactionService, TransactionService>();
builder.Services.AddTransient<IUserService, UserService>();
builder.Services.AddTransient<IReceiptService, ReceiptService>();
builder.Services.AddTransient<IAccountService, AccountService>();
builder.Services.AddTransient<IPaymentService, PaymentService>();
builder.Services.AddTransient<IImportExportService, ImportExportService>();
builder.Services.AddTransient<IPasswordHasher, PasswordHasher>();
builder.Services.AddLogging();
builder.Services.AddTransient<ErrorHandlingMiddleware>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
// app.UseHttpsRedirection();
app.UseRouting();
app.UseCors(myAllowSpecificOrigins);
app.UseMiddleware<JwtMiddleware>();
app.UseMiddleware<ErrorHandlingMiddleware>();
app.MapControllers();
app.Run();