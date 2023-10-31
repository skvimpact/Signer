using Signer.Services;

namespace Signer;
public class WorkerTask
{
    private readonly ILogger<WorkerTask> logger;    
    private readonly ISignerService signer;
    private readonly UnsignedDocuments documents;
    public WorkerTask(
        ILogger<WorkerTask> logger,
        ISignerService signer,
        UnsignedDocuments documents) {
        this.logger = logger;
        this.signer = signer;
        this.documents = documents;
    }
    public void Execute()
    {
        documents.Reset();
        if (documents.Count > 0)
        {
            logger.LogInformation($"There are {documents.Count} documents for signing");
            var findResult = signer.FindSignature();
            if (findResult.HasErrors)
            {
                foreach(var error in findResult.Errors)
                {
                    logger.LogError(error.ToString());
                }
            }
            else
            {           
                //if (documents.Count == 0)
                //    logger.LogInformation("There are no new documents for signing");
                documents.ForEach(document => {
                    signer.SignDocument(document.Path, document.SignPath);
                    logger.LogInformation("\"{document}\" has been signed", document.Path);
                });        
            }
        }        
    }
}