using eventRegistration;
using MedcorSL.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using System.Text.Json;
using System.Text.Json.Serialization;
using Hangfire;
using Hangfire.SqlServer;
using System.Configuration;
using Microsoft.AspNetCore.Mvc.Filters;
using Hangfire.Dashboard;
using Hangfire.Dashboard.BasicAuthorization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddControllersWithViews();


builder.Services.AddSingleton<IGuest, guestRepo>();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<IEmailService, EmailService>();
builder.Services.AddSingleton(resolver => resolver.GetRequiredService<IOptions<EmailConfig>>().Value);
builder.Services.Configure<EmailConfig>(options => builder.Configuration.Bind("EmailConfig", options));
builder.Services.Configure<JsonOptions>(options =>
{
    options.SerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
});

builder.Services.AddDbContext<GContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

var options = new JsonSerializerOptions
{
    WriteIndented = true,
    MaxDepth = 6 // Fixed
};

builder.Services.AddCors(c =>
{
    c.AddPolicy("allowAll", p =>
    {
        p.AllowAnyHeader();
        p.AllowAnyMethod();
        p.AllowAnyOrigin();
    });
});



// Add Hangfire services.
builder.Services.AddHangfire(configuration => configuration
    .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
    .UseSimpleAssemblyNameTypeSerializer()
    .UseRecommendedSerializerSettings()
    .UseSqlServerStorage(builder.Configuration.GetConnectionString("DefaultConnection"), new SqlServerStorageOptions
    {
        CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
        SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
        QueuePollInterval = TimeSpan.Zero,
        UseRecommendedIsolationLevel = true,
        DisableGlobalLocks = true
    }));

// Add the processing server as IHostedService
builder.Services.AddHangfireServer(options =>
{
    options.WorkerCount = 1;
    options.SchedulePollingInterval = TimeSpan.FromSeconds(1);
    //options.ServerCheckInterval= TimeSpan.FromSeconds(120);
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    //app.UseSwagger();
    //app.UseSwaggerUI();
}

app.UseSwagger();
app.UseSwaggerUI();

//app.UseEndpoints(endpoints =>
//{
//    endpoints.MapControllerRoute(
//        name:"default",
//        pattern: "{controller=Home}/{action=CreateQrCode}/{id?}"
//       );
//});
app.UseCors("allowAll");
app.UseHttpsRedirection();

app.UseHangfireDashboard("/hangfire", new DashboardOptions
{
    Authorization = new[] { new BasicAuthAuthorizationFilter(new BasicAuthAuthorizationFilterOptions
        {
            RequireSsl = false,
            SslRedirect = false,
            LoginCaseSensitive = true,
            Users = new []
            {
                new BasicAuthAuthorizationUser
                {
                    Login = "hani",
                    PasswordClear =  "200hia11"
                }
            }

        }) }
});

app.UseRouting();
app.UseAuthorization();

app.MapControllers();
app.MapControllerRoute(
    name: "default",
      pattern: "{controller=Home}/{action=CreateQrCode}/{id?}"
    );

app.UseEndpoints(endpoints =>
{
    endpoints.MapHangfireDashboard();
});

app.Run();
