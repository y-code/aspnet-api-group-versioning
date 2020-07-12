using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using Ycode.AspNetCore.Mvc.GroupVersioning.Test.Models;

namespace Ycode.AspNetCore.Mvc.GroupVersioning.Test
{
    public class TestBase<T> where T : new()
    {
        static int _portSequence = 62185;
        static object _portSequenceLock = new object();

        public string ApiBaseUrl { get; private set; }

        protected ILogger Logger;

        bool _useGroupVersioning;
        TestBed<T> _testBed;

        public TestBase(bool useGroupVersioning = true)
        {
            _useGroupVersioning = useGroupVersioning;

            lock (_portSequenceLock)
            {
                ApiBaseUrl = $"https://localhost:{_portSequence++}";
            }
        }

        [OneTimeSetUp]
        public async Task OneTimeSetUp()
        {
            Logger = SetUp.LoggerFactory.CreateLogger(this.GetType());

            _testBed = new TestBed<T>(
                RunEnvironment.Development,
                ApiBaseUrl,
                new Dictionary<string, string> { },
                new[] { "--useGroupVersioning", _useGroupVersioning.ToString() },
                SetUp.LoggerFactory);
            await _testBed.Start();
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            _testBed.Dispose();
        }

        protected async Task LogWebException(Func<Task> test)
        {
            try
            {
                await test();
            }
            catch (WebException e)
            {
                var error = $"Caught a web exception. {await GetErrorResponse(e)}";
                Logger.LogError($"Caught a web exception. {error}");
                throw;
            }
        }

        protected async Task<TestResponse<R>> TestRequest<R>(Uri requestUrl, Action<JsonSerializerOptions> setUpOptions = null)
        {
            Logger.LogInformation("Sending a request to \"{Url}\"", requestUrl);

            var client = new WebClient();
            var stream = await client.OpenReadTaskAsync(requestUrl);
            var options = new JsonSerializerOptions();
            setUpOptions?.Invoke(options);
            var data = await JsonSerializer.DeserializeAsync<R>(stream, options);
            return new TestResponse<R>(client, stream, data);
        }

        protected async Task VerifyWebException<R>(Func<Task> test, HttpStatusCode statusCode, Action<R> verify)
            where R : class
        {
            try
            {
                await test();

                Assert.Fail("This test should have ended up with an error response.");
            }
            catch (WebException e)
            {
                Logger.LogDebug("An WebException was caught: {Exception}", e.ToString());

                string errResponse = null;
                try
                {
                    var stream = e.Response.GetResponseStream();
                    var reader = new StreamReader(stream);
                    errResponse = await reader.ReadToEndAsync();
                    Logger.LogDebug("The error response says: {ErrorResponse}", errResponse);
                }
                catch (Exception e2)
                {
                    Logger.LogError("An exception was caught during reading an error response. {Exception}", e2);
                }

                Assert.That((e.Response as HttpWebResponse)?.StatusCode, Is.EqualTo(statusCode));
                if (verify == null)
                {
                    Assert.That(e.Response.ContentLength, Is.EqualTo(0));
                }
                else
                {
                    var err = string.IsNullOrEmpty(errResponse)
                        ? null
                        : JsonSerializer.Deserialize<R>(errResponse);
                    verify(err);
                }
            }
        }

        protected Task VerifyWebException(Func<Task> test, HttpStatusCode statusCode)
            => VerifyWebException<object>(test, statusCode, null);

        protected async Task<string> GetErrorResponse(WebException e)
        {
            if (e?.Response == null)
            {
                return null;
            }

            using (var reader = new StreamReader(e.Response.GetResponseStream()))
            {
                return await reader.ReadToEndAsync();
            }
        }
    }
}
