using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using NUnit.Framework;
using System.Reflection;

namespace ConfigTest.Integration
{
    public class UnitTest1
    {
        private TestServerFixture _fixture;

        [SetUp]
        public void SetUp()
        {
            _fixture = new TestServerFixture();
        }

        [Test]
        public async Task TestMethod1()
        {
            HttpResponseMessage response = await _fixture.Client.GetAsync("/api/values/1");

            response.EnsureSuccessStatusCode();

            var responseStrong = await response.Content.ReadAsStringAsync();

            Assert.AreEqual("false", responseStrong);
        }
    }

    public class TestServerFixture : IDisposable
    {
        private readonly TestServer _testServer;
        public HttpClient Client { get; }

        public TestServerFixture()
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            var builder = new WebHostBuilder()
               //.UseContentRoot(@"C:\Users\clementoniovosa\Desktop\ConfigTest\ConfigTest")
               .UseContentRoot(GetContentRootPath())
               .UseEnvironment("Development")
               .UseConfiguration(config)
               .UseStartup<Startup>();  // Uses Start up class from your API Host project to configure the test server

            _testServer = new TestServer(builder);
            Client = _testServer.CreateClient();

        }

        private string GetContentRootPath()
        {
            var testProjectPath = Path.GetDirectoryName(Path.GetDirectoryName(Path.GetDirectoryName(Directory.GetCurrentDirectory())));
            var projectBasePath = Path.GetDirectoryName(testProjectPath);
            var fullPath = $@"{projectBasePath}\ConfigTest";
            return fullPath;
        }

        public void Dispose()
        {
            Client.Dispose();
        }
    }
}
