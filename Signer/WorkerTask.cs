using Signer.Services;
using Signer.Settings;

namespace Signer;
public class WorkerTask
{
    private readonly ILogger<WorkerTask> logger;    
    private readonly ISignerService signerService;
    private readonly UnsignedDocuments documents;
    public WorkerTask(
        ILogger<WorkerTask> logger,
        ISignerService signerService,
        UnsignedDocuments documents) {
        this.logger = logger;
        this.signerService = signerService;
        this.documents = documents;
    }
    public void Execute()
    {
        var updatingResult = documents.UpdateList();
        if (!updatingResult.HasErrors)
        {
            if (documents.Count > 0)
            {
                logger.LogInformation($"There are {documents.Count} documents for signing");
                var signingResult = signerService.SignDocument(documents.ToArray());
                if (signingResult.HasErrors)
                    logger.LogError(signingResult.GetAllErrors());
            }  
        }        
        else
            logger.LogError(updatingResult.GetAllErrors());              
    }
}