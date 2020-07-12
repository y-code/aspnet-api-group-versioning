using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using NUnit.Framework;
using Ycode.AspNetCore.Mvc.GroupVersioning.Test.Models;
using Sample1;
using Sample2;
using SampleCommon.Models;
using UrlToWFV1 = Ycode.AspNetCore.Mvc.GroupVersioning.Test.Models.UrlToWeatherForecastV1;
using UrlToWFV2 = Ycode.AspNetCore.Mvc.GroupVersioning.Test.Models.UrlToWeatherForecastV2;
using Microsoft.Extensions.Logging;
using Ycode.AspNetCore.Mvc.GroupVersioning.Test.Utilities;

namespace Ycode.AspNetCore.Mvc.GroupVersioning.Test.ApiTests
{
    [TestFixture(false, TestOf = typeof(Sample1Program), TypeArgs = new[] { typeof(Sample1Program) })]
    [TestFixture(true, TestOf = typeof(Sample1Program), TypeArgs = new[] { typeof(Sample1Program) })]
    [TestFixture(true, TestOf = typeof(Sample2Program), TypeArgs = new[] { typeof(Sample2Program) })]
    public class CommonTest<T> : TestBase<T> where T: new()
    {
        public const string PathToV1 = "/api/v1.0";
        public const string PathToV1WeatherForecast = PathToV1 + "/WeatherForecast";
        public const string PathToV2 = "/api/v2.0";
        public const string PathToV2WeatherForecast = PathToV2 + "/WeatherForecast";
        public const string PathToV2WeatherForecastArea = PathToV2 + "/WeatherForecast/Area";

        public void LogRequestUrl(Uri url)
            => Logger.Log(LogLevel.Information, "Requesting to {RequestUrl}", url.ToString());

        public CommonTest(bool useGroupVersioning) : base(useGroupVersioning)
        {
        }

        [TestCase("")]
        [TestCase("/")]
        public async Task TestWFV1(string extraPath)
        {
            await LogWebException(async () =>
            {
                var requestUrl = new UrlToWFV1(ApiBaseUrl, PathToV1WeatherForecast + extraPath).ToUri();
                LogRequestUrl(requestUrl);

                using (var response = await TestRequest<WeatherForecastV1[]>(requestUrl))
                {
                    Assert.That(response.Data.Length, Is.EqualTo(5));
                }
            });
        }

        [TestCase("20200303", new[] { "3 Mar, 2020", "4 Mar, 2020", "5 Mar, 2020", "6 Mar, 2020", "7 Mar, 2020" })]
        public async Task TestWFWithDateV1(string date, string[] dates)
        {
            var requestUrl = new UrlToWFV1(ApiBaseUrl, PathToV1WeatherForecast)
                { From = date }.ToUri();
            LogRequestUrl(requestUrl);

            await LogWebException(async () =>
            {
                using (var response = await TestRequest<WeatherForecastV1[]>(requestUrl))
                {
                    Assert.That(response.Data.Length, Is.EqualTo(5));
                    Assert.That(response.Data[0].Date, Is.EqualTo(dates[0]));
                    Assert.That(response.Data[1].Date, Is.EqualTo(dates[1]));
                    Assert.That(response.Data[2].Date, Is.EqualTo(dates[2]));
                    Assert.That(response.Data[3].Date, Is.EqualTo(dates[3]));
                    Assert.That(response.Data[4].Date, Is.EqualTo(dates[4]));
                }
            });
        }

        [TestCase("202003030")]
        [TestCase("2020033")]
        [TestCase("test")]
        public async Task TestWFWithInvalidDateV1(string date)
        {
            var requestUrl = new UrlToWFV1(ApiBaseUrl, PathToV1WeatherForecast)
                { From = date }.ToUri();
            LogRequestUrl(requestUrl);

            await VerifyWebException<ValidationErrorResponseModel>(
                test: async () =>
                {
                    using (var response = await TestRequest<WeatherForecastV1[]>(requestUrl)) { }
                },
                statusCode: HttpStatusCode.InternalServerError,
                verify: err => Assert.That(err.Title,
                    Is.EqualTo($"'{date}' in URL should be in yyyyMMdd format. (Parameter 'date')")));
        }

        [TestCase("")]
        [TestCase("/")]
        public async Task TestV2(string extraPath)
        {
            var requestUrl = new UrlToWFV2(ApiBaseUrl, PathToV2 + extraPath).ToUri();
            LogRequestUrl(requestUrl);

            await VerifyWebException(
                test: async () =>
                {
                    using (var response = await TestRequest<WeatherForecastV2[]>(requestUrl,
                        options => options.Converters.Add(new DateTimeJsonConverter()))) { }
                },
                statusCode: HttpStatusCode.NotFound);
        }

        [TestCase("")]
        [TestCase("/")]
        public async Task TestWFV2(string extraPath)
        {
            var requestUrl = new UrlToWFV2(ApiBaseUrl, PathToV2WeatherForecast + extraPath).ToUri();
            LogRequestUrl(requestUrl);

            await LogWebException(async () =>
            {
                using (var response = await TestRequest<WeatherForecastV2[]>(requestUrl,
                    options => options.Converters.Add(new DateTimeJsonConverter())))
                {
                    Assert.That(response.Data.Length, Is.EqualTo(5 * 3));
                }
            });
        }

        [TestCase("20200303", "2020-03-03")]
        [TestCase("", null)]
        public async Task TestWFWithValidDateV2(string date, DateTime? expetedFirstDate)
        {
            var requestUrl = new UrlToWFV2(ApiBaseUrl, PathToV2WeatherForecast)
                { From = date }.ToUri();
            LogRequestUrl(requestUrl);

            await LogWebException(async () =>
            {
                var now = DateTime.UtcNow;
                using (var response = await TestRequest<WeatherForecastV2[]>(requestUrl,
                        options => options.Converters.Add(new DateTimeJsonConverter())))
                {
                    Assert.That(response.Data.Length, Is.EqualTo(5 * 3));
                    Assert.That(response.Data.Min(d => d.Date),
                        Is.EqualTo(expetedFirstDate ?? new DateTime(now.Year, now.Month, now.Day)));
                }
            });
        }

        [TestCase("202003030")]
        [TestCase("2020033")]
        [TestCase("New York")]
        public async Task TestWFWithInvalidDateV2(string date)
        {
            var requestUrl = new UrlToWFV2(ApiBaseUrl, PathToV2WeatherForecast)
                { From = date }.ToUri();
            LogRequestUrl(requestUrl);

            await VerifyWebException<ValidationErrorResponseModel>(
                test: async () =>
                {
                    using (var response = await TestRequest<WeatherForecastV2[]>(requestUrl,
                        options => options.Converters.Add(new DateTimeJsonConverter()))) { }
                },
                statusCode: HttpStatusCode.InternalServerError,
                verify: err => Assert.That(err.Title,
                    Is.EqualTo($"'{date}' should be in yyyyMMdd format. (Parameter 'from')")));
        }

        [TestCase("New York", "20200305", 4, "2020-03-05")]
        [TestCase("New York", "20200305", null, "2020-03-05")]
        [TestCase("New York", "", 3, null)]
        [TestCase("New York", "", null, null)]
        public async Task TestWFWithValidAreaAndValidDateV2(string area, string date, int? days, DateTime? firstDate)
        {
            var requestUrl = new UrlToWFV2(ApiBaseUrl, PathToV2WeatherForecastArea)
                { Area = area, From = date, Days = days }.ToUri();
            LogRequestUrl(requestUrl);

            await LogWebException(async () =>
            {
                var now = DateTime.UtcNow;
                using (var response = await TestRequest<WeatherForecastV2[]>(requestUrl,
                        options => options.Converters.Add(new DateTimeJsonConverter())))
                {
                    Assert.That(response.Data.Length, Is.EqualTo(days ?? 5));
                    Assert.That(response.Data.Min(d => d.Date),
                        Is.EqualTo(firstDate ?? new DateTime(now.Year, now.Month, now.Day)));
                }
            });
        }

        [TestCase("Auckland", "20200303")]
        public async Task TestWFWithInvalidAreaAndValidDateV2(string area, string date)
        {
            var requestUrl = new UrlToWFV2(ApiBaseUrl, PathToV2WeatherForecastArea)
                { Area = area, From = date }.ToUri();
            LogRequestUrl(requestUrl);

            await VerifyWebException<ValidationErrorResponseModel>(
                test: async () =>
                {
                    using (var response = await TestRequest<WeatherForecastV2[]>(requestUrl,
                        options => options.Converters.Add(new DateTimeJsonConverter()))) { }
                },
                statusCode: HttpStatusCode.InternalServerError,
                verify: err => Assert.That(err.Title,
                    Is.EqualTo($"'{area}' is not supported area. (Parameter 'area')")));
        }

        [TestCase("New York", "202003030")]
        [TestCase("New York", "2020033")]
        [TestCase("New York", "test")]
        public async Task TestWFWithValidAreaAndInvalidDateV2(string area, string date)
        {
            var requestUrl = new UrlToWFV2(ApiBaseUrl, PathToV2WeatherForecastArea)
                { Area = area, From = date }.ToUri();
            LogRequestUrl(requestUrl);

            await VerifyWebException<ValidationErrorResponseModel>(
                test: async () =>
                {
                    using (var response = await TestRequest<WeatherForecastV2[]>(requestUrl,
                        options => options.Converters.Add(new DateTimeJsonConverter()))) { }
                },
                statusCode: HttpStatusCode.InternalServerError,
                verify: err => Assert.That(err.Title,
                    Is.EqualTo($"'{date}' should be in yyyyMMdd format. (Parameter 'from')")));
        }

        [TestCase("v1.0", "1.0", "ApiVersionUnspecified", "An API version is required, but was not specified.")]
        [TestCase("v1.0", "initial", "ApiVersionUnspecified", "An API version is required, but was not specified.")]
        [TestCase("v1.0", "victory", "InvalidApiVersion", "The HTTP resource that matches the request URI '{requestUrl}' does not support the API version 'ictory'.")]
        [TestCase("v1.0", "v999.0", "UnsupportedApiVersion", "The HTTP resource that matches the request URI '{requestUrl}' does not support the API version '999.0'.")]
        public async Task TestInvalidVersion(string match, string replacement, string expectedCode, string expectedMessage)
        {
            var path = PathToV1WeatherForecast.Replace(match, replacement);
            var requestUrl = new UrlToWFV1(ApiBaseUrl, path).ToUri();
            LogRequestUrl(requestUrl);

            expectedMessage = expectedMessage.Replace("{requestUrl}", requestUrl.ToString());

            await VerifyWebException<BadRequestResponseModel>(
                test: async () =>
                {
                    using (var response = await TestRequest<WeatherForecastV2[]>(requestUrl,
                        options => options.Converters.Add(new DateTimeJsonConverter()))) { }
                },
                statusCode: HttpStatusCode.BadRequest,
                verify: err =>
                {
                    Assert.That(err.Error?.Code, Is.EqualTo(expectedCode));
                    Assert.That(err.Error?.Message, Is.EqualTo(expectedMessage));
                });
        }
    }
}
