using Signer.Services;
using Signer.Settings;

namespace Signer;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly SignerService _signer;
    private readonly UnsignedDocuments _documents;
    private readonly int _delay;
    public Worker(
        ILogger<Worker> logger, 
        SignerService signer,
        UnsignedDocuments documents,
        WorkerSettings workerSettings)
    {
        _logger = logger;
        _signer = signer;
        _documents = documents;
        _delay = workerSettings.Delay;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            _documents.Reset();
            if (_documents.Count == 0)
                _logger.LogInformation("No new documents for signing");
            _documents.ForEach(document => {
                _signer.SignDocument(document);
                _logger.LogInformation($"\"{document}\" has been signed");});
            
            await Task.Delay(_delay, stoppingToken);
        }
    }
}
