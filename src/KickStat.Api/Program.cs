using FarPlan.Json;
using KickStat;
using KickStat.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MultiVod.UI.SiteApi.Framework.Config;
using NLog.Extensions.Logging;
using System.IO.Compression;
using System.Text.Json.Serialization;
using System.Text.Json;
using KickStat.Data.Domain.Identity;
using Microsoft.AspNetCore.Identity;

var builder = WebApplication.CreateBuilder(args);
builder.WebHost.ConfigureKestrel(options => options.AddServerHeader = false);

builder.Configuration
    .AddJsonFile("Config/AppSettings.json", true, true)
    .AddJsonFile("Config/ConnectionStrings.json", true, true);

builder.Logging
    .ClearProviders()
    .SetMinimumLevel(LogLevel.Information)
    .AddNLog("config/nlog.config");
// Add services to the container.

builder.Services.AddMemoryCache();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddHttpContextAccessor();

builder.Services.Configure<AppSettings>(builder.Configuration.GetSection("AppSettings"));
builder.Services.Configure<JwtAuthSettings>(builder.Configuration.GetSection("JwtAuthentication"));

builder.Services.AddDbContext<KickStatDbContext>(opt =>
    opt.UseNpgsql(builder.Configuration.GetConnectionString("KickStat")));
builder.Services.AddKickStatIdentity();

builder.Services.AddResponseCompression(options =>
    {
        options.Providers.Add<BrotliCompressionProvider>();
        options.Providers.Add<GzipCompressionProvider>();
        options.EnableForHttps = true;
    }
); 
builder.Services.Configure<GzipCompressionProviderOptions>(options => { options.Level = CompressionLevel.Optimal; });
builder.Services.Configure<BrotliCompressionProviderOptions>(options => { options.Level = CompressionLevel.Optimal; });

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = false;
        //options.Audience = Configuration.GetValue<string>("JwtAuthentication::Audience");
        options.TokenValidationParameters = new TokenValidationParameters
        {
            IssuerSigningKey =
                JwtAuthSettings.GetSymmetricSecurityKey(builder.Configuration["JwtAuthentication:SecretKey"]!),
            ValidateIssuerSigningKey = true,

            ValidateIssuer = true,
            ValidIssuer = builder.Configuration["JwtAuthentication:Issuer"],

            ValidateLifetime = true,

            ValidateAudience = true,
            ValidAudience = builder.Configuration["JwtAuthentication:Audience"]
        };
    });

builder.Services.AddCors(x => x.AddPolicy("DebugPolicy", policyBuilder =>
{
    policyBuilder.AllowAnyOrigin();
    policyBuilder.AllowAnyHeader();
    policyBuilder.AllowAnyMethod();
}));

builder.Services.AddControllers()
    .AddMvcOptions(options => options.Filters.Add(new HandleAndLogErrorAttribute()))
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        options.JsonSerializerOptions.Converters.Add(new TimeSpanWithoutSecondsJsonConverter());
    });


// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// ����� �� �������� � net.core 6 � Datetime � Npgsql, ��� ����� �������� �������� ��� � Timestamp with zone, ���� � �� ���� ������� without
// �� stackoverflow ������� ������������ ����� ���������
AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);


var app = builder.Build();

app.UseResponseCompression();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.UseCors("DebugPolicy");

app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    await scope.ServiceProvider.GetRequiredService<KickStatDbContext>()
        .EnsureAdminCreated(
            scope.ServiceProvider.GetRequiredService<UserManager<KickStatUser>>(),
            scope.ServiceProvider.GetRequiredService<RoleManager<KickStatRole>>()
        );

}

Console.WriteLine("APP START!");

app.Run();