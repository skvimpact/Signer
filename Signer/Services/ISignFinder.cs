using System.Security.Cryptography.X509Certificates;

namespace Signer.Services
{
    public interface ISignFinder
    {
        public bool SignIsFound();
        public X509Certificate2? Sign();
    }
}    