using Signer.Signers;
using StatusGeneric;

namespace Signer.Services
{
    public class SignerService : ISignerService
    {
        readonly ILogger<SignerService> logger;
        private readonly ISigner signer;
        public SignerService(
            ILogger<SignerService> logger,
            ISigner signer) 
        {
            this.logger = logger;
            this.signer = signer;
            LibCore.Initializer.Initialize();
        }

        public IStatusGeneric<int> SignDocument(UnsignedDocument[] documents)
        {
            var status = new StatusGenericHandler<int>();
            int count = 0;
            documents.ToList().ForEach(document =>
            {
                try
                {
                    signer.Sign(document);
                    document
                        .VerifySignature()
                        .SaveSignatureToFile();
                    count++;
                    logger.LogInformation("\"{Path}\" has been signed", document.Path);
                }
                catch(Exception ex)
                {
                    status.AddError(ex.Message);
                }
            });
            status.SetResult(count);
            return status;                
        }
    }
}