
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

namespace Signer.Services
{
    public class SignerService
    {
        //private CryptoSettings _cryptoSettings;
        private FileSettings _fileSettings;
        private readonly X509Certificate2 _certificate;
        public SignerService(
            CryptoSettings cryptoSettings, 
            FileSettings fileSettings)
        {
            //_cryptoSettings = cryptoSettings;
            _fileSettings = fileSettings;

            LibCore.Initializer.Initialize();
            X509Store store = new X509Store(StoreLocation.CurrentUser);
            store.Open(OpenFlags.OpenExistingOnly | OpenFlags.ReadOnly);
            _certificate = store.Certificates.Find(X509FindType.FindBySubjectName, cryptoSettings.SubjectName, true)[0];            
            // throw new ArgumentException(ExceptionMessage(nameof(fileSettings.DocumentFolder)))
        }
        public void SignDocument(string document)
        {
            var docFile = Path.Combine(
                _fileSettings.DocumentFolder ?? string.Empty, 
                $"{document}.{_fileSettings.DocumentExtension}");
            var sigFile = Path.Combine(
                _fileSettings.SignFolder ?? string.Empty,
                $"{document}.{_fileSettings.SignExtension}");
            //File.Copy(docFile, sigFile);

            ContentInfo contentInfo = new ContentInfo(File.ReadAllBytes(docFile));
            SignedCms signedCms = new SignedCms(contentInfo, true);
            CmsSigner cmsSigner = new CmsSigner(_certificate);
            signedCms.ComputeSignature(cmsSigner, false);
            //byte[] pk = signedCms.Encode();
            File.WriteAllBytes(sigFile, signedCms.Encode());            
        }
        public static string ExceptionMessage(string settingName) =>
            $"Setting \"{settingName}\" can't be unspecified";        
    }
}