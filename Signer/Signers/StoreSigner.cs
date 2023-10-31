using System.Security.Cryptography.Pkcs;
using System.Security.Cryptography.X509Certificates;
using Signer.Services;
using Signer.Settings;

namespace Signer.Signers
{
    public class StoreSigner : BaseSigner, ISigner
    {
        private readonly StoreSettings settings = new();  
        private readonly ILogger<StoreSigner> _logger;
        public StoreSigner(
            IConfiguration configuration,
            ILogger<StoreSigner> logger) : base(configuration) {
            _logger = logger;
        }

        public void Sign(UnsignedDocument document)
        {
            configuration?.GetSection(nameof(StoreSettings)).Bind(settings);

            using  (var store = new X509Store(StoreLocation.CurrentUser))
            {
                var contentInfo = new ContentInfo(document.BytesToHash());
                var signedCms = new SignedCms(contentInfo, true);

                store.Open(OpenFlags.OpenExistingOnly | OpenFlags.ReadOnly);
                var certificate = store.Certificates.Find(
                    X509FindType.FindBySerialNumber, 
                    settings.SerialNumber ?? throw new ArgumentNullException(nameof(settings.SerialNumber)),
                    true)[0];
                var cmsSigner = new CmsSigner(certificate);
                // _logger.LogDebug($"{cmsSigner.PrivateKey?.KeySize ?? -1}");
                _logger.LogDebug($"Valid={certificate.Verify()}");
                _logger.LogDebug($"HasPrivateKey={certificate.HasPrivateKey}");
                _logger.LogDebug($"{certificate.SignatureAlgorithm.FriendlyName}");
                signedCms.ComputeSignature(cmsSigner, false);
                document.Signature = signedCms.Encode();                    
            }
        }
    }
} 