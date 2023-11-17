using Signer;
using Signer.Services;
using Serilog;
using Microsoft.AspNetCore.Hosting;

IHost host = Host.
CreateDefaultBuilder(args)
    .UseSerilog()
    .UseWindowsService()
    .ConfigureAppConfiguration((hostContext, configBuilder) =>
    {
        configBuilder
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
    })
    .ConfigureWebHostDefaults((configure) => 
    {
        configure.UseStartup<Startup>();
    })
    .ConfigureServices((hostContext, services) =>
    {
        services.AddControllers();
        services.AddSingleton<UnsignedDocuments>();
        services.AddSingleton<StoreFinder>();
        services.AddSingleton<TokenFinder>();
        services.AddSingleton<TokenUsbFinder>();        
        services.AddSingleton<ISignerService>(x =>
            new SignerService(new ISignFinder[] {
                x.GetRequiredService<StoreFinder>(),
                x.GetRequiredService<TokenFinder>(),
                x.GetRequiredService<TokenUsbFinder>() }));   
        services.AddSingleton<WorkerTask>();
        services.AddHostedService<Worker>();

        Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(hostContext.Configuration)
            .CreateLogger();
    })
    .Build();

try{
    await host.RunAsync();
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