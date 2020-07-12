using System;
using System.Text.Json.Serialization;

namespace Ycode.AspNetCore.Mvc.GroupVersioning.Test.Models
{
    public class BadRequestResponseModel
    {
        public class ErrorModel
        {
            [JsonPropertyName("code")]
            public string Code { get; set; }
            [JsonPropertyName("message")]
            public string Message { get; set; }
        }

        [JsonPropertyName("error")]
        public ErrorModel Error { get; set; }
    }
}
