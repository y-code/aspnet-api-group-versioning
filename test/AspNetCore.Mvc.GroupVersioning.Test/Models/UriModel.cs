using System;
using Ycode.UriConvert;

namespace Ycode.AspNetCore.Mvc.GroupVersioning.Test.Models
{
    public class UriModel
    {
        [UriBase]
        public string Base { get; set; }
        [UriIgnore]
        public string PathBase { get; set; }

        public UriModel(string @base, string path)
        {
            Base = @base;
            PathBase = path;
        }

        public Uri ToUri()
            => UriConvert.UriConvert.Convert(this);
    }

    public class UrlToWeatherForecastV1 : UriModel
    {
        [UriPath]
        public string Path
        {
            get
            {
                var path = PathBase;
                if (From != null)
                {
                    path += "/" + From;
                }
                return path;
            }
        }

        [UriIgnore]
        public string From { get; set; }

        public UrlToWeatherForecastV1(string @base, string path) : base(@base, path) { }
    }

    public class UrlToWeatherForecastV2 : UriModel
    {
        [UriPath]
        public string Path
        {
            get
            {
                var path = PathBase;
                if (Area != null)
                {
                    path += "/" + Area;
                }
                return path;
            }
        }

        [UriIgnore]
        public string Area { get; set; }
        [UriQueryParameter("from")]
        public string From { get; set; }
        [UriQueryParameter("days")]
        public int? Days { get; set; }

        public UrlToWeatherForecastV2(string @base, string path) : base(@base, path) { }
    }

    public class UrlToNewsHeadlinesV1 : UriModel
    {
        [UriPath]
        public string Path => PathBase;

        public UrlToNewsHeadlinesV1(string @base, string path) : base(@base, path) { }
    }

    public class UrlToNewsHeadlinesV2 : UriModel
    {
        [UriPath]
        public string Path
        {
            get
            {
                var path = PathBase;
                if (DateStartingFrom != null)
                {
                    path += "/" + DateStartingFrom;
                }
                return path;
            }
        }

        [UriIgnore]
        public string DateStartingFrom { get; set; }

        public UrlToNewsHeadlinesV2(string @base, string path) : base(@base, path) { }
    }
}
