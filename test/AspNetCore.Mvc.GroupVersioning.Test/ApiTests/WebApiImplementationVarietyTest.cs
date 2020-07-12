    using System;
using System.Threading.Tasks;
using NUnit.Framework;
using Sample3;
using Sample5;
using SampleCommon.Models;
using Ycode.AspNetCore.Mvc.GroupVersioning.Test.Models;
using UrlToWFV1 = Ycode.AspNetCore.Mvc.GroupVersioning.Test.Models.UrlToWeatherForecastV1;
using UrlToWFV2 = Ycode.AspNetCore.Mvc.GroupVersioning.Test.Models.UrlToWeatherForecastV2;

namespace Ycode.AspNetCore.Mvc.GroupVersioning.Test.ApiTests
{
    public class WebApiImplementationVarietyTest<P> : TestBase<P> where P : new()
    {
        internal const string PathToV1_0 = "/api/v1.0";
        internal const string PathToV1_1 = "/api/v1.1";
        internal const string PathToV2_0 = "/api/v2.0";
        internal const string PathToV2020_05_12_1_0 = "/api/v2020-05-12.1.0";
        internal const string PathToV2020_05_12 = "/api/v2020-05-12";
        internal const string PathToV2020_07_05_1_1 = "/api/v2020-07-05.1.1";
        internal const string PathToV2020_07_05 = "/api/v2020-07-05";
        internal const string PathToV2020_12_02_2_0 = "/api/v2020-12-02.2.0";
        internal const string PathToV2020_12_02 = "/api/v2020-12-02";

        public WebApiImplementationVarietyTest(bool useGroupVersioning) : base(useGroupVersioning) { }

        public virtual async Task TestMultipleEndpointsMatchedError(string endpoint, string date)
        {
            var requestUrl = new UrlToWFV1(ApiBaseUrl, endpoint) { From = date }.ToUri();

            await VerifyWebException<InternalServerErrorResponseModel>(
                test: async () =>
                {
                    using (var response = await TestRequest<WeatherForecastV1_1[]>(requestUrl))
                    {
                    }
                },
                statusCode: System.Net.HttpStatusCode.InternalServerError,
                verify: err =>
                {
                    Assert.That(err.Title, Does.StartWith(
                        $"The request matched multiple endpoints. Matches:"));
                });
        }

        public virtual async Task TestWorkingMapToApiVersion(string endpoint, string date, string expectedSummary)
        {
            var requestUrl = new UrlToWFV1(ApiBaseUrl, endpoint) { From = date }.ToUri();

            await LogWebException(
                async () =>
                {
                    using (var response = await TestRequest<WeatherForecastV1_1[]>(requestUrl))
                    {
                        Assert.That(response.Data.Length, Is.EqualTo(5));
                        Assert.That(response.Data[0].Summary, Is.EqualTo(expectedSummary));
                    }
                });
        }
    }

    [TestFixture(false, TestOf = typeof(Sample3Program), TypeArgs = new[] { typeof(Sample3Program) })]
    [TestFixture(true, TestOf = typeof(Sample3Program), TypeArgs = new[] { typeof(Sample3Program) })]
    public class WebApiImplementationVarietyInApiVersioningTest<P>
        : WebApiImplementationVarietyTest<P>
        where P : new()
    {
        public WebApiImplementationVarietyInApiVersioningTest(bool useGroupVersioning)
            : base(useGroupVersioning) { }

        [TestCase(PathToV1_1 + "/Pattern1", "20200701")]
        [TestCase(PathToV1_1 + "/Pattern3", "20200701")]
        [TestCase(PathToV1_1 + "/Pattern4", "20200701")]
        [TestCase(PathToV1_1 + "/Pattern6", "20200701")]
        [TestCase(PathToV1_1 + "/Pattern11", "20200701")]
        [TestCase(PathToV1_1 + "/Pattern13", "20200701")]
        [TestCase(PathToV1_1 + "/Pattern14", "20200701")]
        [TestCase(PathToV1_1 + "/Pattern16", "20200701")]
        public override Task TestMultipleEndpointsMatchedError(string endpoint, string date)
            => base.TestMultipleEndpointsMatchedError(endpoint, date);

        [TestCase(PathToV1_1 + "/Pattern2", "20200701", "Pattern2:GetV1_1(string)")]
        [TestCase(PathToV1_1 + "/Pattern5", "20200701", "Pattern5:GetV1_1(string)")]
        [TestCase(PathToV1_1 + "/Pattern12", "20200701", "Pattern12:GetV1_1(string)")]
        [TestCase(PathToV1_1 + "/Pattern15", "20200701", "Pattern15:GetV1_1(string)")]
        public override Task TestWorkingMapToApiVersion(string endpoint, string date, string expectedSummary)
            => base.TestWorkingMapToApiVersion(endpoint, date, expectedSummary);
    }

    [TestFixture]
    public class WebApiImplementationVarietyInApiGroupVersioningTest
        : WebApiImplementationVarietyTest<Sample5Program>
    {
        public WebApiImplementationVarietyInApiGroupVersioningTest()
            : base(true) { }

        [TestCase(PathToV2020_07_05_1_1 + "/Pattern1", "20200701")]
        [TestCase(PathToV2020_07_05 + "/Pattern1", "20200701")]
        [TestCase(PathToV1_1 + "/Pattern1", "20200701")]
        [TestCase(PathToV2020_07_05_1_1 + "/Pattern3", "20200701")]
        [TestCase(PathToV2020_07_05 + "/Pattern3", "20200701")]
        [TestCase(PathToV1_1 + "/Pattern3", "20200701")]
        [TestCase(PathToV2020_07_05_1_1 + "/Pattern4", "20200701")]
        [TestCase(PathToV2020_07_05 + "/Pattern4", "20200701")]
        [TestCase(PathToV1_1 + "/Pattern4", "20200701")]
        [TestCase(PathToV2020_07_05_1_1 + "/Pattern6", "20200701")]
        [TestCase(PathToV2020_07_05 + "/Pattern6", "20200701")]
        [TestCase(PathToV1_1 + "/Pattern6", "20200701")]
        [TestCase(PathToV2020_07_05_1_1 + "/Pattern11", "20200701")]
        [TestCase(PathToV2020_07_05 + "/Pattern11", "20200701")]
        [TestCase(PathToV1_1 + "/Pattern11", "20200701")]
        [TestCase(PathToV2020_07_05_1_1 + "/Pattern13", "20200701")]
        [TestCase(PathToV2020_07_05 + "/Pattern13", "20200701")]
        [TestCase(PathToV1_1 + "/Pattern13", "20200701")]
        [TestCase(PathToV2020_07_05_1_1 + "/Pattern14", "20200701")]
        [TestCase(PathToV2020_07_05 + "/Pattern14", "20200701")]
        [TestCase(PathToV1_1 + "/Pattern14", "20200701")]
        [TestCase(PathToV2020_07_05_1_1 + "/Pattern16", "20200701")]
        [TestCase(PathToV2020_07_05 + "/Pattern16", "20200701")]
        [TestCase(PathToV1_1 + "/Pattern16", "20200701")]
        public override Task TestMultipleEndpointsMatchedError(string endpoint, string date)
            => base.TestMultipleEndpointsMatchedError(endpoint, date);

        [TestCase(PathToV2020_07_05_1_1 + "/Pattern2", "20200701", "Pattern2:GetV1_1(string)")]
        [TestCase(PathToV2020_07_05 + "/Pattern2", "20200701", "Pattern2:GetV1_1(string)")]
        [TestCase(PathToV1_1 + "/Pattern2", "20200701", "Pattern2:GetV1_1(string)")]
        [TestCase(PathToV2020_07_05_1_1 + "/Pattern5", "20200701", "Pattern5:GetV1_1(string)")]
        [TestCase(PathToV2020_07_05 + "/Pattern5", "20200701", "Pattern5:GetV1_1(string)")]
        [TestCase(PathToV1_1 + "/Pattern5", "20200701", "Pattern5:GetV1_1(string)")]
        [TestCase(PathToV2020_07_05_1_1 + "/Pattern12", "20200701", "Pattern12:GetV1_1(string)")]
        [TestCase(PathToV2020_07_05 + "/Pattern12", "20200701", "Pattern12:GetV1_1(string)")]
        [TestCase(PathToV1_1 + "/Pattern12", "20200701", "Pattern12:GetV1_1(string)")]
        [TestCase(PathToV2020_07_05_1_1 + "/Pattern15", "20200701", "Pattern15:GetV1_1(string)")]
        [TestCase(PathToV2020_07_05 + "/Pattern15", "20200701", "Pattern15:GetV1_1(string)")]
        [TestCase(PathToV1_1 + "/Pattern15", "20200701", "Pattern15:GetV1_1(string)")]
        public override Task TestWorkingMapToApiVersion(string endpoint, string date, string expectedSummary)
            => base.TestWorkingMapToApiVersion(endpoint, date, expectedSummary);
    }
}
