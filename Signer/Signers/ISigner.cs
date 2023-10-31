using Signer.Services;

namespace Signer.Signers
{
    public interface ISigner
    {
        public void Sign(UnsignedDocument document);
    }
} 