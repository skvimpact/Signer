using Signer.Settings;

namespace Signer.Services
{
    public class UnsignedDocuments : List<UnsignedDocument>
    {
        private readonly FileSettings settings = new();
        private readonly IConfiguration configuration;
        public UnsignedDocuments(IConfiguration configuration)
        {         
            this.configuration = configuration;
        }
        public void Reset()
        {      
            configuration.GetSection(nameof(FileSettings)).Bind(settings);      
            Clear();
            AddDocuments(
                settings.DocumentFolder,
                settings.DocumentExtension,
                settings.SignFolder,
                settings.SignExtension);
        }
        private void AddDocuments(string? documentFolder, string? documentExtension, string? signFolder, string? signExtension)
        {
            if (documentFolder == null)
                throw new ArgumentNullException(nameof(documentFolder));
            if (documentExtension == null)
                throw new ArgumentNullException(nameof(documentExtension));
            if (signFolder == null)
                throw new ArgumentNullException(nameof(signFolder));
            if (signExtension == null)
                throw new ArgumentNullException(nameof(signExtension));

            AddRange(Directory
                .GetFiles(documentFolder, $"*.{documentExtension}")
                    .Select(f => Path.GetFileName(f)).ToHashSet()
                .Except(Directory
                    .GetFiles(signFolder, $"*.{signExtension}")
                        .Select(f => Path.GetFileNameWithoutExtension(f)).ToHashSet()).
                Select(d => new UnsignedDocument {
                    Path = Path.Combine(documentFolder, d),
                    SignPath = Path.Combine(signFolder, $"{d}.{signExtension}")
                }));            
        }
    }
}