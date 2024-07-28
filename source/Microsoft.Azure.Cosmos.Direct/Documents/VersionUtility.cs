// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.VersionUtility
// Assembly: Microsoft.Azure.Cosmos.Direct, Version=3.29.4.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: FFE3C00D-4333-4294-8947-B1C93A852E2F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Direct.dll

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;

namespace Microsoft.Azure.Documents
{
  internal static class VersionUtility
  {
    private const string versionDateTimeFormat = "yyyy-MM-dd";
    private const string previewVersionDateTimeFormat = "yyyy-MM-dd-preview";
    private static readonly IReadOnlyDictionary<string, DateTime> KnownDateTimes;

    static VersionUtility()
    {
      Dictionary<string, DateTime> dictionary = new Dictionary<string, DateTime>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      VersionUtility.KnownDateTimes = (IReadOnlyDictionary<string, DateTime>) new ReadOnlyDictionary<string, DateTime>((IDictionary<string, DateTime>) dictionary);
      foreach (string runtimeApiVersion in HttpConstants.Versions.SupportedRuntimeAPIVersions)
      {
        DateTime apiVersionDate;
        if (VersionUtility.TryParseApiVersion(runtimeApiVersion, out apiVersionDate))
          dictionary[runtimeApiVersion] = apiVersionDate;
      }
    }

    internal static bool IsLaterThan(string compareVersion, string baseVersion)
    {
      if (VersionUtility.IsPreviewApiVersion(baseVersion) && !VersionUtility.IsPreviewApiVersion(compareVersion))
        return false;
      DateTime apiVersionDate;
      if (!VersionUtility.TryParseApiVersion(baseVersion, out apiVersionDate))
        throw new BadRequestException(string.Format((IFormatProvider) CultureInfo.CurrentUICulture, RMResources.InvalidVersionFormat, (object) "base", (object) baseVersion));
      return VersionUtility.IsLaterThan(compareVersion, apiVersionDate);
    }

    internal static bool IsValidApiVersion(string apiVersion) => VersionUtility.TryParseApiVersion(apiVersion, out DateTime _);

    internal static bool IsPreviewApiVersion(string apiVersion) => apiVersion.ToLowerInvariant().Contains("preview");

    internal static bool IsLaterThan(string compareVersion, DateTime baseVersion)
    {
      DateTime apiVersionDate;
      if (!VersionUtility.TryParseApiVersion(compareVersion, out apiVersionDate))
        throw new BadRequestException(string.Format((IFormatProvider) CultureInfo.CurrentUICulture, RMResources.InvalidVersionFormat, (object) "compare", (object) compareVersion));
      return apiVersionDate.CompareTo(baseVersion) >= 0;
    }

    internal static bool IsLaterThanNotEqualTo(string compareVersion, DateTime baseVersion)
    {
      DateTime apiVersionDate;
      if (!VersionUtility.TryParseApiVersion(compareVersion, out apiVersionDate))
        throw new BadRequestException(string.Format((IFormatProvider) CultureInfo.CurrentUICulture, RMResources.InvalidVersionFormat, (object) "compare", (object) compareVersion));
      return apiVersionDate.CompareTo(baseVersion) > 0;
    }

    internal static DateTime ParseNonPreviewDateTimeExact(string apiVersion) => DateTime.ParseExact(apiVersion, "yyyy-MM-dd", (IFormatProvider) CultureInfo.InvariantCulture, DateTimeStyles.None);

    private static bool TryParseApiVersion(string apiVersion, out DateTime apiVersionDate) => VersionUtility.KnownDateTimes.TryGetValue(apiVersion, out apiVersionDate) || VersionUtility.TryParseApiVersionCore(apiVersion, out apiVersionDate);

    private static bool TryParseApiVersionCore(string apiVersion, out DateTime apiVersionDate)
    {
      string format = !apiVersion.ToLowerInvariant().Contains("preview") ? "yyyy-MM-dd" : "yyyy-MM-dd-preview";
      return DateTime.TryParseExact(apiVersion, format, (IFormatProvider) CultureInfo.InvariantCulture, DateTimeStyles.None, out apiVersionDate);
    }
  }
}
