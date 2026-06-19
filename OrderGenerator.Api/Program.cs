using OrderGenerator.Api.Hubs;
using OrderGenerator.Api.Services;
using OrderGenerator.Api.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddSingleton<ISessionIdProvider, SessionIdProvider>();
builder.Services.AddSingleton<IOrderQueue, OrderQueue>();
builder.Services.AddHostedService<OrderWorker>();
builder.Services.AddSingleton<OrderGeneratorApplication>();
builder.Services.AddSingleton<IFixInitiatorService>(sp =>
    {
        var application = sp.GetRequiredService<OrderGeneratorApplication>();
        var queue = sp.GetRequiredService<IOrderQueue>();
        return new FixInitiatorService("config/initiator.cfg", application, queue);
    });

builder.Services.AddSignalR();
builder.Services.AddControllers();
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(builder =>
    {
        builder.AllowAnyHeader()
               .AllowAnyMethod()
               .AllowCredentials()
               .SetIsOriginAllowed(origin => true);
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.

var fixService = app.Services.GetRequiredService<IFixInitiatorService>();
fixService.Start();

app.UseAuthorization();

app.MapControllers();
app.UseCors();

app.MapHub<OrderHub>("/orderHub");

app.Run();
