using System;
using System.IO;
using System.Net;

namespace Ycode.AspNetCore.Mvc.GroupVersioning.Test.Models
{
    public class TestResponse<T> : IDisposable
    {
        public WebClient Client { get; }
        public Stream Stream { get; }
        public T Data { get; }

        public TestResponse(WebClient client, Stream stream, T data)
        {
            Client = client;
            Stream = stream;
            Data = data;
        }

        public void Dispose()
        {
            Stream.Dispose();
            Client.Dispose();
        }
    }
}
