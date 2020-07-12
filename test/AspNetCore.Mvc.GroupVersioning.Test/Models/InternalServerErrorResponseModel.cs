using System;
using System.Text.Json.Serialization;

namespace Ycode.AspNetCore.Mvc.GroupVersioning.Test.Models
{
    public class InternalServerErrorResponseModel
    {
        [JsonPropertyName("title")]
        public string Title { get; set; }
        [JsonPropertyName("status")]
        public int Status { get; set; }
        [JsonPropertyName("detail")]
        public string Detail { get; set; }
    }
}
