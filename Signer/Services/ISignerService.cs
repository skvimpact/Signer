using StatusGeneric;

namespace Signer.Services
{
    public interface ISignerService
    {
        public IStatusGeneric<int> SignDocument(UnsignedDocument[] documents);
    }
}    