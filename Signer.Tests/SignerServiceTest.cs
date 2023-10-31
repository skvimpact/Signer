using Xunit;
using Microsoft.Extensions.Configuration;
using Signer.Services;
using Signer.Settings;
namespace Signer.Tests;

public class SignerServiceTest
{
    IConfiguration config;
    public SignerServiceTest()
    {
        config = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .Build();
    }

    [Fact]
    public void SignerServiceCanSign()
    {
        var document = "test.txt";
        File.WriteAllText(document, $"CryptoPro {DateTime.UtcNow}"); 
         
        var fs = new FileSettings();
        config.GetSection(nameof(FileSettings)).Bind(fs);

        string docFile = Path.Combine(
                fs.DocumentFolder ?? throw new ArgumentNullException(nameof(fs.DocumentFolder)),
                document);
        docFile = document;

        string sigFile = Path.Combine(
                fs.SignFolder ?? throw new ArgumentNullException(nameof(fs.SignFolder)),
                $"{document}.{fs.SignExtension}" ?? throw new ArgumentNullException(nameof(fs.SignExtension)));
        
        //var sf = new StoreFinder(config);
        //var tf = new TokenFinder(config);
        //var ss = new SignerService(new ISignFinder[] {tf, sf});
        //ss.FindSignature();
        //ss.SignDocument(docFile, sigFile);        
    }
}