using OrderGenerator.Api.Services;
using OrderGenerator.Api.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddSingleton<OrderGeneratorApplication>();
builder.Services.AddSingleton<IFixInitiatorService>(sp =>
    {
        var application = sp.GetRequiredService<OrderGeneratorApplication>();
        return new FixInitiatorService("config/initiator.cfg", application);
    });

builder.Services.AddControllers();

var app = builder.Build();

// Configure the HTTP request pipeline.

var fixService = app.Services.GetRequiredService<IFixInitiatorService>();
fixService.Start();

app.UseAuthorization();

app.MapControllers();

app.Run();
