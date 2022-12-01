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

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    
}
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

app.UseRouting();
app.UseAuthorization();

app.MapControllers();
app.MapControllerRoute(
    name: "default",
      pattern: "{controller=Home}/{action=CreateQrCode}/{id?}"
    );

app.Run();
