namespace  Signer.Services
{
    public class UnsignedDocument
    {
        public string? Path { get; set; }
        public string? SignPath { get; set; }
        public byte[]? Signature { get; set; }
        public bool SourceExists => File.Exists(Path);
        public bool SignExists => File.Exists(SignPath);
    }
}