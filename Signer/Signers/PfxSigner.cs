using System.Security.Cryptography.Pkcs;
using System.Security.Cryptography.X509Certificates;
using Signer.Services;
using Signer.Settings;

namespace Signer.Signers
{
    public class PfxSigner : BaseSigner, ISigner
    {
        private readonly PfxSettings settings = new();  
        public PfxSigner(
            IConfiguration configuration) : base(configuration) {}

        public void Sign(UnsignedDocument document)
        {
            configuration?.GetSection(nameof(PfxSettings)).Bind(settings);

            var contentInfo = new ContentInfo(document.BytesToHash());
            var signedCms = new SignedCms(contentInfo, true);

            var certificate = new X509Certificate2(
                settings.Path ?? throw new ArgumentNullException(nameof(settings.Path)), 
                settings.Password ?? throw new ArgumentNullException(nameof(settings.Password)));
            var cmsSigner = new CmsSigner(certificate);

            signedCms.ComputeSignature(cmsSigner, false);
            document.Signature = signedCms.Encode(); 
        }
    }
} 