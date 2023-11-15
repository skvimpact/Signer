using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using LibCore.Security.Cryptography;
using Signer.Settings;

namespace Signer.Services
{
    public class TokenUsbFinder : Finder, ISignFinder
    {
        private readonly TokenSettings settings = new();        
        public TokenUsbFinder(
            IConfiguration configuration) : base(configuration) {}

        public bool SignIsFound()
        {
            certificate = null;            
            configuration.GetSection(nameof(TokenSettings)).Bind(settings);


            if ((settings.DwType == null) ||
                (settings.ProviderName == null) ||
                (settings.ContainerName == null))
                return false;       

            using (var provider =
                new Gost3410_2012_256CryptoServiceProvider(
                    new CspParameters(
                        settings.DwType ?? 80,
                        settings.ProviderName,
                        settings.ContainerName)))     
                        // 80,
                        // "",                                           
                        // "\\\\.\\HDIMAGE\\G2012256")))
            {
                byte[]? cert = provider.ContainerCertificate;
                certificate = new X509Certificate2(cert);
            }            
            return certificate != null;
        }
    }
} 