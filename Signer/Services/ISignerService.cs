using StatusGeneric;

namespace Signer.Services
{
    public interface ISignerService
    {
        public IStatusGeneric SignDocument(string? docFile, string? sigFile);
        public bool SignIsFound();
        public IStatusGeneric FindSignature();
    }
}    