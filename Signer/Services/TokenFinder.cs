using System.Security.Cryptography.X509Certificates;
using Signer.Settings;

namespace Signer.Services
{
    public class TokenFinder : Finder, ISignFinder
    {
        private readonly TokenSettings settings = new();        
        public TokenFinder(
            IConfiguration configuration) : base(configuration) {}

        public bool SignIsFound()
        {
            certificate = null;            
            configuration.GetSection(nameof(TokenSettings)).Bind(settings);

            if ((settings.Path == null) ||
                (settings.Password == null))
                return false;       

            certificate = new X509Certificate2(settings.Path, settings.Password);
            return certificate != null;
        }
    }
}    