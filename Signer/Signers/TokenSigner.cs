using System.Security.Cryptography;
using System.Security.Cryptography.Pkcs;
using System.Security.Cryptography.X509Certificates;
using LibCore.Security.Cryptography;
using Signer.Services;
using Signer.Settings;

namespace Signer.Signers
{
    public class TokenSigner : BaseSigner, ISigner
    {
        private readonly TokenSettings settings = new();  
        public TokenSigner(
            IConfiguration configuration) : base(configuration) {}

        public void Sign(UnsignedDocument document)
        {
            configuration?.GetSection(nameof(TokenSettings)).Bind(settings);

            using (var provider =
                new Gost3410_2012_256CryptoServiceProvider(
                    new CspParameters(  
                        settings.DwType ?? 80,
                        settings.ProviderName ?? "",                        
                        settings.ContainerName ?? throw new ArgumentNullException(nameof(settings.ContainerName)))))
            {
                var contentInfo = new ContentInfo(document.BytesToHash());
                var signedCms = new SignedCms(contentInfo, true);

                var certificate = new X509Certificate2(provider.ContainerCertificate);
                var cmsSigner = new CmsSigner(
                    SubjectIdentifierType.Unknown,
                    certificate,
                    provider);

                signedCms.ComputeSignature(cmsSigner, false);
                document.Signature = signedCms.Encode();
            } 
        }
    }
} 