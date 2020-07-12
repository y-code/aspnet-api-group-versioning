using System;
namespace SampleCommon
{
    public static class Version
    {
        public const string V2020_03_21 = "2020-03-21";
        public const string V2020_05_12 = "2020-05-12";
        public const string V2020_07_05 = "2020-07-05";
        public const string V2020_11_09 = "2020-11-09";
        public const string V2020_12_02 = "2020-12-02";

        public static string[] All
            => new[]
            {
                V2020_03_21,
                V2020_05_12,
                V2020_07_05,
                V2020_11_09,
                V2020_12_02,
            };
    }
}
