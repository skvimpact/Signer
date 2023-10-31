using StatusGeneric;

namespace Signer.Services
{
    public class FakeSignerService : ISignerService
    {
        public IStatusGeneric FindSignature()
        {
            return new StatusGenericHandler();
        }

        public IStatusGeneric SignDocument(string? docFile, string? sigFile)
        {     
            var status = new StatusGenericHandler();
            try
            {                   
                if (docFile == null)
                    throw new ArgumentNullException(nameof(docFile));
                if (sigFile == null)
                    throw new ArgumentNullException(nameof(sigFile));            
                    
                File.Copy(docFile, sigFile);
            }
            catch(Exception ex)
            {
                status.AddError(ex.Message);
            }  
            return status;             
        }

        public IStatusGeneric<int> SignDocument(UnsignedDocument[] documents)
        {
            throw new NotImplementedException();
        }

        public bool SignIsFound()
        {
            return true;
        }
    }
}    