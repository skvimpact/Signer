using System.Security.Cryptography.X509Certificates;
using Signer.Settings;

namespace Signer.Services
{
    public class StoreFinder : Finder, ISignFinder
    {
        private readonly StoreSettings settings = new();        
        public StoreFinder(
            IConfiguration configuration) : base(configuration) {}
        public bool SignIsFound()
        {
            certificate = null;
            configuration.GetSection(nameof(StoreSettings)).Bind(settings);

            if ((settings.SerialNumber == null) && (settings.SubjectName == null))
                return false;

            var store = new X509Store(StoreLocation.CurrentUser);
            X509Certificate2Collection? found = null;

            store.Open(OpenFlags.OpenExistingOnly | OpenFlags.ReadOnly);

            if (settings.SerialNumber != null)
                found = store.Certificates.Find(
                    X509FindType.FindBySerialNumber, 
                    settings.SerialNumber,
                    true); 

            if ((found?.Count ?? 0) == 0)
            {
                if (settings.SubjectName != null)
                    found = store.Certificates.Find(
                        X509FindType.FindBySubjectName, 
                        settings.SubjectName,
                        true); 
            }
            
            store.Close();

            if (found?.Count > 1)
                throw new ArgumentException("There is more than one secret key");
            if (found?.Count == 1)
                certificate = found[0];

            return certificate != null;
        }
    }
}