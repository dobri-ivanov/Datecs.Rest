using Datecs.Rest.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddSingleton<FiscalService>();
builder.Services.AddSingleton<IFiscalService, FiscalService>();

builder.Services.AddControllers();

var app = builder.Build();

app.MapControllers();

app.Run("http://localhost:5000");
