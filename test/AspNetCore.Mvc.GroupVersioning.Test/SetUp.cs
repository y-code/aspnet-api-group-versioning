using Microsoft.Extensions.Logging;
using NUnit.Framework;
using Serilog;
using Serilog.Extensions.Logging;

namespace Ycode.AspNetCore.Mvc.GroupVersioning.Test
{
    [SetUpFixture]
    public class SetUp
    {
        public static ILoggerFactory LoggerFactory { get; private set; }

        [OneTimeSetUp]
        public void SetUpBeforeAll()
        {
            LoggerFactory = new SerilogLoggerFactory(new LoggerConfiguration()
                    .MinimumLevel.Debug()
                    .WriteTo.Console()
                    .CreateLogger());

        }

        [OneTimeTearDown]
        public void TearDownAfterAll()
        {
            LoggerFactory.Dispose();
        }
    }
}
