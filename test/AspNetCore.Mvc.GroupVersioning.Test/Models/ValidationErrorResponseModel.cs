using System;
using System.Text.Json.Serialization;

namespace Ycode.AspNetCore.Mvc.GroupVersioning.Test.Models
{
    public class ValidationErrorResponseModel
    {
        [JsonPropertyName("title")]
        public string Title { get; set; }
    }
}
