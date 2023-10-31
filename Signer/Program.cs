using Signer;
using Signer.Settings;
using Signer.Services;
using Serilog;
using Microsoft.AspNetCore.Builder;

var builder = WebApplication.CreateBuilder(args);
builder.Host.UseSerilog();
builder.Host.UseWindowsService();
  
var cs = new CryptoSettings();
builder.Configuration.GetSection(nameof(CryptoSettings)).Bind(cs);
builder.Services.AddSingleton(cs);

var fs = new FileSettings();
builder.Configuration.GetSection(nameof(FileSettings)).Bind(fs);
builder.Services.AddSingleton(fs);

var ws = new WorkerSettings();
builder.Configuration.GetSection(nameof(WorkerSettings)).Bind(ws);
builder.Services.AddSingleton(ws);

builder.Services.AddSingleton<UnsignedDocuments>();
builder.Services.AddSingleton<SignerService>();
builder.Services.AddHostedService<Worker>();

var app = builder.Build();

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .CreateLogger();

try{
    app.Run();
    return 0;
}
catch (Exception ex)
{
    Log.Fatal(ex.Message);
    return 1;
}
finally
{
    Log.CloseAndFlush();
}