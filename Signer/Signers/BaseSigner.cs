namespace Signer.Signers
{
    public abstract class BaseSigner
    {
        protected readonly IConfiguration configuration;        
        public BaseSigner(IConfiguration configuration) =>
            this.configuration = configuration;
    }
} 