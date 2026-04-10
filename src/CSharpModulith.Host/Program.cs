using App.Capability.Order.Infrastructure;
using App.Capability.Payment.Infrastructure;
using App.Shared;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddSingleton(new AppConfig(builder.Environment.EnvironmentName));
builder.Services.AddOrderCapability();
builder.Services.AddPaymentCapability();

var app = builder.Build();

app.MapDefaultEndpoints();

if (!app.Environment.IsDevelopment())
{
    app.MapHealthChecks("/health");
    app.MapHealthChecks("/alive", new HealthCheckOptions
    {
        Predicate = r => r.Tags.Contains("live")
    });
}

app.MapGet("/", () => Results.Ok(new { name = "CSharpModulith" }));

app.Run();
