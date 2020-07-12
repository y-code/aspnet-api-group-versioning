using System;
using System.Text.Json.Serialization;
using NJsonSchema.Annotations;

namespace SampleCommon.Models
{
    public enum NewsCategoryV1
    {
        World,
        Lifestyle,
        Sports,
        Finance,
    }

    [JsonSchema("NewsHeadline")]
    public class NewsHeadlineV1
    {
        [JsonPropertyName("id")]
        public long Id { get; set; }
        [JsonPropertyName("title")]
        public string Title { get; set; }
        [JsonPropertyName("summary")]
        public virtual string Summary { get; set; }
        [JsonPropertyName("category"), JsonConverter(typeof(JsonStringEnumConverter))]
        public NewsCategoryV1 Category { get; set; }
        [JsonPropertyName("publishDate")]
        public DateTime PublishUtcDate { get; set; }
    }

    [JsonSchema("News")]
    public class NewsV1 : NewsHeadlineV1
    {
        [JsonIgnore]
        public override string Summary { get; set; }
        [JsonPropertyName("content")]
        public string Content { get; set; }

        public NewsHeadlineV1 ToHeadline()
            => new NewsHeadlineV1
            {
                Id = Id,
                Title = Title,
                Summary = Summary,
                Category = Category,
                PublishUtcDate = PublishUtcDate,
            };
    }
}
