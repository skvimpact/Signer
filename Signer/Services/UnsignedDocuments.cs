using Signer.Settings;
using StatusGeneric;

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
        public IStatusGeneric<bool> UpdateList()
        {      
            var status = new StatusGenericHandler<bool>();
            Clear();
            configuration.GetSection(nameof(FileSettings)).Bind(settings);
            try
            {            
                AddDocuments(
                    settings.DocumentFolder,
                    settings.DocumentExtension,
                    settings.SignFolder,
                    settings.SignExtension);
            }
            catch(Exception ex)
            {
                status.AddError(ex.Message);
            }
            status.SetResult(!status.HasErrors);
            return status;              
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
                Select(_ => Document(_, documentFolder, signFolder, signExtension)));            
        }
        public UnsignedDocument Document(string name) 
        {
            configuration.GetSection(nameof(FileSettings)).Bind(settings);
            return Document(
                name,
                settings.DocumentFolder,
                settings.SignFolder,
                settings.SignExtension);            
        }  
        private UnsignedDocument Document(string name, string? documentFolder, string? signFolder, string? signExtension) 
        {
            if (documentFolder == null)
                throw new ArgumentNullException(nameof(documentFolder));
            if (signFolder == null)
                throw new ArgumentNullException(nameof(signFolder));
            if (signExtension == null)
                throw new ArgumentNullException(nameof(signExtension));            
            return new UnsignedDocument{
                    Path = Path.Combine(documentFolder, name),
                    SignPath = Path.Combine(signFolder, $"{name}.{signExtension}")
            };
        }             
    }
}