using System.Security.Cryptography.X509Certificates;

namespace Signer.Services
{
    public abstract class Finder
    {
        protected readonly IConfiguration configuration;        
        protected X509Certificate2? certificate;     
        public Finder(
            IConfiguration configuration)
        {
            this.configuration = configuration;
        }            
        public X509Certificate2? Sign()
        {
            return certificate;
        }        
    }
}    