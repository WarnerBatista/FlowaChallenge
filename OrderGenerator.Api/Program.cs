using OrderGenerator.Api.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddSingleton(new FixInitiatorService("config/initiator.cfg"));

var app = builder.Build();

// Configure the HTTP request pipeline.

var fixService = app.Services.GetRequiredService<FixInitiatorService>();
fixService.Start();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
