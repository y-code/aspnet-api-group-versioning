using System;
using Microsoft.AspNetCore.Mvc;

namespace Ycode.AspNetCore.Mvc.GroupVersioning
{
    static class ApiVersionExtensions
    {
        internal static bool EqualsIgnoringGroupVersion(this ApiVersion a, ApiVersion b)
            => Nullable.Equals(a?.MajorVersion, b?.MajorVersion)
                && (a?.MinorVersion ?? 0).Equals(b?.MinorVersion ?? 0)
                && string.Equals(a?.Status, b?.Status, StringComparison.OrdinalIgnoreCase);
    }
}
