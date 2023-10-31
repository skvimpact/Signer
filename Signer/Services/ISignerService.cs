using StatusGeneric;

namespace Signer.Services
{
    public interface ISignerService
    {
        public void SignDocument(string? docFile, string? sigFile);
        public IStatusGeneric FindSignature();
    }
}    