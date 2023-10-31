using System.Security.Cryptography.Pkcs;
using Force.Crc32;

namespace  Signer.Services
{
    public static partial class UnsignedDocumentExtensions    
    {
        public static string Crc32(this UnsignedDocument document)
        {
            return $"{Crc32Algorithm.Compute(File.ReadAllBytes(document.Path ?? throw new ArgumentNullException(nameof(document.Path))))}";
        }
        public static byte[] BytesToHash(this UnsignedDocument document)
        {
            return File.ReadAllBytes(document.Path ?? throw new ArgumentNullException(nameof(document.Path)));
        }                
        public static UnsignedDocument SaveSignatureToFile(this UnsignedDocument document)
        {            
            File.WriteAllBytes(
                document.SignPath ?? throw new ArgumentNullException(nameof(document.SignPath)), 
                document.Signature ?? throw new ArgumentNullException(nameof(document.Signature))
            );
            return document;
        }   
        public static UnsignedDocument VerifySignature (this UnsignedDocument document)
        {
            var contentInfoVerify = new ContentInfo(document.BytesToHash());
            var signedCmsVerify = new SignedCms(contentInfoVerify, true);
            signedCmsVerify.Decode(document.Signature ?? throw new ArgumentNullException(nameof(document.Signature)));
            signedCmsVerify.CheckSignature(true);
            return document;            
        }         
    }
}    