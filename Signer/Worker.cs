using Microsoft.Extensions.Options;
using Signer.Services;
using Signer.Settings;

namespace Signer;

public class Worker : BackgroundService
{
    private readonly WorkerSettings settings = new();
    private readonly IConfiguration configuration;    
    private readonly ILogger<Worker> logger;
    private readonly WorkerTask workerTask;
    public Worker(
        IConfiguration configuration,
        ILogger<Worker> logger, 
        WorkerTask workerTask)
    {
        this.configuration = configuration;
        this.logger = logger;
        this.workerTask = workerTask;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            configuration.GetSection(nameof(WorkerSettings)).Bind(settings);
            try
            {
                workerTask.Execute();
            }
            catch(Exception ex)
            {
                logger.LogError(ex.Message);
            }            
            await Task.Delay(settings.Delay, stoppingToken);
        }
    }
}
