using System.Data;
using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyBGList.Attributes;
using MyBGList.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers(options => options.ModelBindingMessageProvider.SetAttemptedValueIsInvalidAccessor((x, y) => $"***** The value '{x}' is not valid for {y}. *****"));

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options => {
    options.ParameterFilter<SortColumnFilter>();
    options.ParameterFilter<SortOrderFilter>();
});

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(cfg =>
    {
        cfg.WithOrigins(builder.Configuration["AllowedOrigins"]);
        cfg.AllowAnyHeader();
        cfg.AllowAnyMethod();
    });
    options.AddPolicy(name: "AnyOrigin", cfg =>
    {
        cfg.AllowAnyOrigin();
        cfg.AllowAnyHeader();
        cfg.AllowAnyMethod();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.

if (app.Configuration.GetValue<bool>("UseSwagger"))
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

if (app.Configuration.GetValue<bool>("UseDeveloperExceptionPage"))
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/error");
}

app.UseHttpsRedirection();

app.UseCors();

app.UseAuthorization();

app.MapGet(
    "/error",
    [EnableCors("AnyOrigin")] [ResponseCache(NoStore = true)] () => Results.Problem());

app.MapGet(
    "/error/test",
    [EnableCors("AnyOrigin")] [ResponseCache(NoStore = true)] () => { throw new Exception("test"); });

app.MapGet(
    "/cod/test",
    [EnableCors("AnyOrigin")] [ResponseCache(NoStore = true)] () =>
        Results.Text("<script>" +
                     "window.alert('Your client supports JavaScript!" +
                     "\\r\\n\\r\\n" +
                     $"Server time (UTC): {DateTime.UtcNow.ToString("o")}" +
                     "\\r\\n" +
                     "Client time (UTC): ' + new Date().toISOString());" +
                     "</script>" +
                     "<noscript>Your client does not support JavaScript</noscript>",
            "text/html"));

app.MapControllers();

app.Run();
