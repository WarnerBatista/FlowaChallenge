using OrderGenerator.Api.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddSingleton<OrderGeneratorApplication>();
builder.Services.AddSingleton(sp =>
    {
        var application = sp.GetRequiredService<OrderGeneratorApplication>();
        return new FixInitiatorService("config/initiator.cfg", application);
    });

var app = builder.Build();

// Configure the HTTP request pipeline.

var fixService = app.Services.GetRequiredService<FixInitiatorService>();
fixService.Start();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
