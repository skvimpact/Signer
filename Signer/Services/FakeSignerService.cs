using StatusGeneric;

namespace Signer.Services
{
    public class FakeSignerService : ISignerService
    {
        public IStatusGeneric FindSignature()
        {
            return new StatusGenericHandler();
        }

        public void SignDocument(string? docFile, string? sigFile)
        {            
            if (docFile == null)
                throw new ArgumentNullException(nameof(docFile));
            if (sigFile == null)
                throw new ArgumentNullException(nameof(sigFile));            
                
            File.Copy(docFile, sigFile);
        }
    }
}    