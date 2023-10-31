using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Signer.Settings;

namespace Signer.Services
{
    public class SignerService
    {
        private CryptoSettings _cryptoSettings;
        private FileSettings _fileSettings;
        public SignerService(
            CryptoSettings cryptoSettings, 
            FileSettings fileSettings)
        {
            _cryptoSettings = cryptoSettings;
            _fileSettings = fileSettings;
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
            File.Copy(docFile, sigFile);
        }
        public static string ExceptionMessage(string settingName) =>
            $"Setting \"{settingName}\" can't be unspecified";        
    }
}