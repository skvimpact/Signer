
using LibCore.Security.Cryptography;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.Pkcs;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography;
using System.Security;
using System.Threading.Tasks;
using Signer.Settings;
using StatusGeneric;

namespace Signer.Services
{
    public class SignerService : ISignerService
    {
        private readonly CryptoSettings settings = new();        
        private readonly IConfiguration configuration;
        private X509Certificate2? certificate;
        public SignerService(
            IConfiguration configuration)
        {
            this.configuration = configuration;
            LibCore.Initializer.Initialize();
        }

        public IStatusGeneric FindSignature()
        {
            var status = new StatusGenericHandler();
            certificate = null;

            configuration.GetSection(nameof(CryptoSettings)).Bind(settings);

            if ((settings.SerialNumber == null) && (settings.SubjectName == null))
            {
                status.AddError($"{nameof(settings.SerialNumber)} and {nameof(settings.SubjectName)} are both unspecified");
                return status;
            }
            var store = new X509Store(StoreLocation.CurrentUser);
            X509Certificate2Collection? found = null;

            store.Open(OpenFlags.OpenExistingOnly | OpenFlags.ReadOnly);

            if (settings.SerialNumber != null)
                found = store.Certificates.Find(
                    X509FindType.FindBySerialNumber, 
                    settings.SerialNumber,
                    true); 

            if ((found == null) || (found?.Count == 0))
            {
                if (settings.SubjectName != null)
                    found = store.Certificates.Find(
                        X509FindType.FindBySubjectName, 
                        settings.SubjectName,
                        true); 
            }
            
            store.Close();

            if ((found == null) || (found?.Count == 0))
            {
                status.AddError("Secret key was not found");
                return status;
            }
            if (found?.Count > 1)
            {
                status.AddError("There is more than one secret key");
                return status;
            }
            if (found != null)
                certificate = found[0];

            return status;
        }
        public void SignDocument(string? docFile, string? sigFile)
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
    }
}