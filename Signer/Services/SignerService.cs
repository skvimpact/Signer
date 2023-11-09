
using System.Security.Cryptography.Pkcs;
using System.Security.Cryptography.X509Certificates;
using StatusGeneric;

namespace Signer.Services
{
    public class SignerService : ISignerService
    {
        private X509Certificate2? certificate;
        private readonly ISignFinder[] finders;
        public SignerService(ISignFinder[] finders)
        {
            this.finders = finders;
            LibCore.Initializer.Initialize();
        }

        public IStatusGeneric FindSignature()
        {
            var status = new StatusGenericHandler();
            certificate = null;
            foreach(var f in finders)
            {
                try
                {
                    if (f.SignIsFound())
                    {
                        certificate = f.Sign();
                        return status;
                    }
                }
                catch(Exception ex)
                {
                    status.AddError(ex.Message);
                }
            };
            if (certificate == null)
                status.AddError("Valid secret key was not found");
            return status;            
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
                if (certificate == null)
                    throw new ArgumentNullException(nameof(certificate));

                var contentInfo = new ContentInfo(File.ReadAllBytes(docFile));
                var signedCms = new SignedCms(contentInfo, true);
                var cmsSigner = new CmsSigner(certificate);
                signedCms.ComputeSignature(cmsSigner, false);
                File.WriteAllBytes(sigFile, signedCms.Encode());     
            }
            catch(Exception ex)
            {
                status.AddError(ex.Message);
            }  
            return status;                
        }
        public bool SignIsFound()
        {
            return certificate != null;
        }
    }
}