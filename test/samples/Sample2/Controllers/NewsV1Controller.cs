using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using SampleCommon.Models;
using Ver = SampleCommon.Version;

namespace Sample2.Controllers
{
    [ApiController]
    [ApiVersion(V2020_05_12_1_0)]
    [ApiVersion(V2020_07_05_1_0)]
    [ApiVersion(V2020_11_09_1_0)]
    [Route("api/v{version:apiVersion}/News")]
    [OpenApiTag("API Example: News")]
    public class NewsV1Controller
    {
        internal const string V2020_05_12_1_0 = Ver.V2020_05_12 + ".1.0";
        internal const string V2020_07_05_1_0 = Ver.V2020_07_05 + ".1.0";
        internal const string V2020_11_09_1_0 = Ver.V2020_11_09 + ".1.0";

        static protected readonly IReadOnlyList<NewsV1> _data;

        static NewsV1Controller()
        {
            _data = new[]
            {
                new NewsV1
                {
                    Id = 1,
                    Title = "News 001",
                    Summary = "Summary 001",
                    Category = NewsCategoryV1.World,
                    PublishUtcDate = DateTime.UtcNow - new TimeSpan(2, 1, 0, 0),
                    Content = "Content 001",
                },
                new NewsV1
                {
                    Id = 2,
                    Title = "News 002",
                    Summary = "Summary 002",
                    Category = NewsCategoryV1.World,
                    PublishUtcDate = DateTime.UtcNow - new TimeSpan(2, 1, 0, 0),
                    Content = "Content 002",
                },
                new NewsV1
                {
                    Id = 3,
                    Title = "News 003",
                    Summary = "Summary 003",
                    Category = NewsCategoryV1.Lifestyle,
                    PublishUtcDate = DateTime.UtcNow - new TimeSpan(2, 1, 0, 0),
                    Content = "Content 003",
                },
                new NewsV1
                {
                    Id = 4,
                    Title = "News 004",
                    Summary = "Summary 004",
                    Category = NewsCategoryV1.World,
                    PublishUtcDate = DateTime.UtcNow - new TimeSpan(1, 1, 0, 0),
                    Content = "Content 004",
                },
                new NewsV1
                {
                    Id = 5,
                    Title = "News 005",
                    Summary = "Summary 005",
                    Category = NewsCategoryV1.World,
                    PublishUtcDate = DateTime.UtcNow - new TimeSpan(0, 1, 0, 0),
                    Content = "Content 005",
                },
            }.ToList().AsReadOnly();
        }

        public virtual NewsHeadlineV1[] GetHeadlines(DateTime? utcTime)
        {
            var from = utcTime - TimeSpan.FromDays(1);
            var to = utcTime;
            return _data.Where(n => from <= n.PublishUtcDate && n.PublishUtcDate <= to)
                .Select(n => n.ToHeadline()).ToArray();
        }

        [HttpGet("Headlines")]
        public NewsHeadlineV1[] GetHeadlines()
            => GetHeadlines(DateTime.UtcNow);
    }
}
