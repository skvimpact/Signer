using Xunit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using Signer.Services;
using Signer.Settings;
namespace Signer.Tests;

public class WorkerTaskTests
{
    IConfiguration config;
    public WorkerTaskTests()
    {
        config = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .Build();
    }    
    [Fact]
    public void WorkerTaskCanBeExecuted()
    {
        var logger = Mock.Of<ILogger<WorkerTask>>();
        var wt = new WorkerTask(
            logger, 
            new FakeSignerService(),
            new UnsignedDocuments(config)
        );
        wt.Execute();
    }            
}    