using System.Net;
using System.Threading.Tasks;
using NUnit.Framework;
using Ycode.AspNetCore.Mvc.GroupVersioning.Test.Models;
using Sample2;
using Sample4;
using SampleCommon.Models;
using UrlToWFV1 = Ycode.AspNetCore.Mvc.GroupVersioning.Test.Models.UrlToWeatherForecastV1;
using UrlToWFV2 = Ycode.AspNetCore.Mvc.GroupVersioning.Test.Models.UrlToWeatherForecastV2;
using UrlToNHV1 = Ycode.AspNetCore.Mvc.GroupVersioning.Test.Models.UrlToNewsHeadlinesV1;
using UrlToNHV2 = Ycode.AspNetCore.Mvc.GroupVersioning.Test.Models.UrlToNewsHeadlinesV2;
using System;

namespace Ycode.AspNetCore.Mvc.GroupVersioning.Test.ApiTests
{
    [TestFixture(TestOf = typeof(Sample2Program), TypeArgs = new[] { typeof(Sample2Program) })]
    [TestFixture(TestOf = typeof(Sample4Program), TypeArgs = new[] { typeof(Sample4Program) })]
    public class ImplicitOrExplicitGroupVersioningTest<P> : TestBase<P> where P : new()
    {
        const string PathToV2020_03_21 = "/api/v2020-03-21";
        const string PathToV2020_05_12 = "/api/v2020-05-12";
        const string PathToV2020_07_05 = "/api/v2020-07-05";
        const string PathToV2020_11_09 = "/api/v2020-11-09";
        const string PathToV2020_12_02 = "/api/v2020-12-02";

        const string PathToV2020_03_21_1_WF = PathToV2020_03_21 + ".1/WeatherForecast";
        const string PathToV2020_03_21_WF = PathToV2020_03_21 + "/WeatherForecast";
        const string PathToV2020_05_12_WF = PathToV2020_05_12 + "/WeatherForecast";
        const string PathToV2020_07_05_WF = PathToV2020_07_05 + "/WeatherForecast";
        const string PathToV2020_11_09_2_WF = PathToV2020_11_09 + ".2/WeatherForecast";
        const string PathToV2020_11_09_WF = PathToV2020_11_09 + "/WeatherForecast";
        const string PathToV2020_12_02_WF = PathToV2020_12_02 + "/WeatherForecast";

        const string PathToV2020_05_12_1_NH = PathToV2020_05_12 + ".1/News/Headlines";
        const string PathToV2020_05_12_NH = PathToV2020_05_12 + "/News/Headlines";
        const string PathToV2020_07_05_NH = PathToV2020_07_05 + "/News/Headlines";
        const string PathToV2020_11_09_NH = PathToV2020_11_09 + "/News/Headlines";
        const string PathToV2020_12_02_2_NH = PathToV2020_12_02 + ".2/News/Headlines";
        const string PathToV2020_12_02_NH = PathToV2020_12_02 + "/News/Headlines";

        [TestCase(PathToV2020_03_21_1_WF, "")]
        [TestCase(PathToV2020_03_21_1_WF, "/")]
        [TestCase(PathToV2020_03_21_WF, "")]
        [TestCase(PathToV2020_03_21_WF, "/")]
        [TestCase(PathToV2020_05_12_WF, "")]
        [TestCase(PathToV2020_05_12_WF, "/")]
        public async Task TestV1(string endpoint, string route)
        {
            var requestUrl = new UrlToWFV1(ApiBaseUrl, endpoint + route).ToUri();

            await LogWebException(
                async () =>
                {
                    var requestUrl = new UrlToWFV1(ApiBaseUrl, endpoint + route).ToUri();
                    using (var response = await TestRequest<WeatherForecastV1[]>(requestUrl))
                    {
                        Assert.That(response.Data.Length, Is.EqualTo(5));
                    }
                });
        }

        [TestCase(PathToV2020_03_21_1_WF, "20200303", 5, "3 Mar, 2020")]
        [TestCase(PathToV2020_03_21_WF, "20200303", 5, "3 Mar, 2020")]
        [TestCase(PathToV2020_05_12_WF, "20200303", 5, "3 Mar, 2020")]
        [TestCase(PathToV2020_07_05_WF, "20200303", 5, "3 Mar, 2020")]
        public async Task TestV1WithRouteParameter(string endpoint, string date,
            int expectedNumberOfResults, string expectedFirstDate)
        {
            var requestUrl = new UrlToWFV1(ApiBaseUrl, endpoint) { From = date }.ToUri();

            await LogWebException(async () =>
            {
                using (var response = await TestRequest<WeatherForecastV1[]>(requestUrl))
                {
                    Assert.That(response.Data.Length, Is.EqualTo(expectedNumberOfResults));
                    Assert.That(response.Data[0].Date, Is.EqualTo(expectedFirstDate));
                }
            });
        }

        [TestCase(PathToV2020_03_21_1_WF, "202003030")]
        [TestCase(PathToV2020_03_21_WF, "202003030")]
        [TestCase(PathToV2020_05_12_WF, "202003030")]
        public async Task TestV1WithInvalidParameter(string endpoint, string date)
        {
            var requestUrl = new UrlToWFV1(ApiBaseUrl, endpoint) { From = date }.ToUri();

            await VerifyWebException<ValidationErrorResponseModel>(
                test: async () =>
                {
                    using (var response = await TestRequest<WeatherForecastV1[]>(requestUrl))
                    {
                    }
                },
                statusCode: HttpStatusCode.InternalServerError,
                verify: err =>
                {
                    Assert.That(err.Title, Is.EqualTo(
                        $"'{date}' in URL should be in yyyyMMdd format. (Parameter 'date')"));
                });
        }

        [TestCase(PathToV2020_11_09_2_WF, "20200303", 15, "2020-03-03T00:00:00")]
        [TestCase(PathToV2020_11_09_WF, "20200303", 15, "2020-03-03T00:00:00")]
        [TestCase(PathToV2020_12_02_WF, "20200303", 15, "2020-03-03T00:00:00")]
        public async Task TestV2(string endpoint, string date,
            int expectedNumberOfResults, string expectedFirstDate)
        {
            var requestUrl = new UrlToWFV2(ApiBaseUrl, endpoint) { From = date }.ToUri();

            await LogWebException(async () =>
            {
                using (var response = await TestRequest<WeatherForecastV1[]>(requestUrl))
                {
                    Assert.That(response.Data.Length, Is.EqualTo(expectedNumberOfResults));
                    Assert.That(response.Data[0].Date, Is.EqualTo(expectedFirstDate));
                }
            });
        }

        [TestCase(PathToV2020_05_12_1_NH, 1, "News 005")]
        [TestCase(PathToV2020_05_12_NH, 1, "News 005")]
        [TestCase(PathToV2020_07_05_NH, 1, "News 005")]
        [TestCase(PathToV2020_11_09_NH, 1, "News 005")]
        public async Task TestAnotherV1(string endpoint,
            int expectedNumberOfResults, string expectedTitle)
        {
            var requestUrl = new UrlToNHV1(ApiBaseUrl, endpoint).ToUri();

            await LogWebException(async () =>
            {
                using (var response = await TestRequest<NewsV1[]>(requestUrl))
                {
                    Assert.That(response.Data.Length, Is.EqualTo(expectedNumberOfResults));
                    Assert.That(response.Data[0].Title, Is.EqualTo(expectedTitle));
                }
            });
        }

        [TestCase(PathToV2020_12_02_2_NH, 1, "News 004")]
        [TestCase(PathToV2020_12_02_NH, 1, "News 004")]
        public async Task TestAnotherV2(string endpoint,
            int expectedNumberOfResults, string expectedTitle)
        {
            var yesterday = DateTime.UtcNow - new TimeSpan(1, 0, 0, 0);
            var requestUrl = new UrlToNHV2(ApiBaseUrl, endpoint)
                { DateStartingFrom = yesterday.ToString("s") }.ToUri();

            await LogWebException(async () =>
            {
                using (var response = await TestRequest<NewsV1[]>(requestUrl))
                {
                    Assert.That(response.Data.Length, Is.EqualTo(expectedNumberOfResults));
                    Assert.That(response.Data[0].Title, Is.EqualTo(expectedTitle));
                }
            });
        }

        [TestCase(PathToV2020_05_12_1_NH, "2020-05-12.1")]
        [TestCase(PathToV2020_05_12_NH, "2020-05-12")]
        [TestCase(PathToV2020_07_05_NH, "2020-07-05")]
        [TestCase(PathToV2020_11_09_NH, "2020-11-09")]
        public async Task TestAnotherV2WithEarlierGroupVersion(string endpoint, string groupVersion)
        {
            var yesterday = DateTime.UtcNow - new TimeSpan(1, 0, 0, 0);
            var requestUrl = new UrlToNHV2(ApiBaseUrl, endpoint)
            { DateStartingFrom = yesterday.ToString("s") }.ToUri();

            await VerifyWebException<BadRequestResponseModel>(
                test: async () =>
                {
                    using (var response = await TestRequest<NewsV1[]>(requestUrl))
                    {
                    }
                },
                statusCode: HttpStatusCode.BadRequest,
                verify: err =>
                {
                    Assert.That(err.Error?.Message, Is.EqualTo(
                        $"The HTTP resource that matches the request URI '{requestUrl}' " +
                        $"does not support the API version '{groupVersion}'."));
                });
        }
    }
}
