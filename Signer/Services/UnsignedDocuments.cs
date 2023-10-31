using Signer.Settings;

namespace Signer.Services
{
    public class UnsignedDocuments : List<string>
    {
        private readonly FileSettings _fileSettings;
        public UnsignedDocuments(FileSettings fileSettings)
        {
            _fileSettings = fileSettings;
            Reset();
        }
        public void Reset()
        {
            
            Clear();
            AddRange(Directory
                .GetFiles(
                    _fileSettings.DocumentFolder 
                        ?? throw new ArgumentException(ExceptionMessage(nameof(_fileSettings.DocumentFolder))),
                    @$"*.{_fileSettings.DocumentExtension 
                        ?? throw new ArgumentException(ExceptionMessage(nameof(_fileSettings.DocumentExtension)))}")
                .Select(f => Path.GetFileNameWithoutExtension(f)).ToHashSet()
                .Except(Directory
                    .GetFiles(
                        _fileSettings.SignFolder 
                            ?? throw new ArgumentException(ExceptionMessage(nameof(_fileSettings.SignFolder))),
                        @$"*.{_fileSettings.SignExtension 
                            ?? throw new ArgumentException(ExceptionMessage(nameof(_fileSettings.SignExtension)))}")
                    .Select(f => Path.GetFileNameWithoutExtension(f)).ToHashSet()));
        }
        public static string ExceptionMessage(string settingName) =>
            $"Setting \"{settingName}\" can't be unspecified";
    }
}