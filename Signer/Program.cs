using Signer;
using Signer.Services;
using Signer.Settings;
using Serilog;
using Microsoft.AspNetCore.Hosting;
using Signer.Signers;

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
        /*
        services.AddSingleton<StoreFinder>();
        services.AddSingleton<TokenFinder>();
        services.AddSingleton<TokenUsbFinder>();        
        services.AddSingleton<ISignerService>(x =>
            new SignerService(new ISignFinder[] {
                x.GetRequiredService<StoreFinder>(),
                x.GetRequiredService<TokenFinder>(),
                x.GetRequiredService<TokenUsbFinder>() }));   
        */           
        //services.AddSingleton<ISigner, StoreSigner>();
        //if (hostContext.HostingEnvironment.IsDevelopment())
            services.AddSingleton<ISigner, StoreSigner>();
        //if (hostContext.HostingEnvironment.IsStaging())
        //    services.AddSingleton<ISigner, PfxSigner>();
        //if (hostContext.HostingEnvironment.IsProduction())            
        //    services.AddSingleton<ISigner, TokenSigner>();
        services.AddSingleton<ISignerService, SignerService>();
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