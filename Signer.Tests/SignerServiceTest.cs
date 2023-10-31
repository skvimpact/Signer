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
        var cs = new CryptoSettings();
        config.GetSection(nameof(CryptoSettings)).Bind(cs);        
        var fs = new FileSettings();
        config.GetSection(nameof(FileSettings)).Bind(fs);
        var ss = new SignerService(cs, fs);
        var documents = new UnsignedDocuments(fs);
        foreach(var document in documents)
        {
            ss.SignDocument(document);
        }
    }
}