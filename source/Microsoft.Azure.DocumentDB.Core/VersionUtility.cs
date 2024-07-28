// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.VersionUtility
// Assembly: Microsoft.Azure.DocumentDB.Core, Version=2.10.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 20BB0EB7-1465-494C-8F4E-F898448B85D9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DocumentDB.Core.dll

using System;
using System.Globalization;

namespace Microsoft.Azure.Documents
{
  internal static class VersionUtility
  {
    private const string versionDateTimeFormat = "yyyy-MM-dd";
    private const string previewVersionDateTimeFormat = "yyyy-MM-dd-preview";

    internal static bool IsLaterThan(string compareVersion, string baseVersion)
    {
      DateTime apiVersionDate1;
      if (!VersionUtility.TryParseApiVersion(baseVersion, out apiVersionDate1))
        throw new BadRequestException(string.Format((IFormatProvider) CultureInfo.CurrentUICulture, RMResources.InvalidVersionFormat, (object) "base", (object) baseVersion));
      DateTime apiVersionDate2;
      if (!VersionUtility.TryParseApiVersion(compareVersion, out apiVersionDate2))
        throw new BadRequestException(string.Format((IFormatProvider) CultureInfo.CurrentUICulture, RMResources.InvalidVersionFormat, (object) "compare", (object) compareVersion));
      return apiVersionDate2.CompareTo(apiVersionDate1) >= 0;
    }

    internal static bool TryParseApiVersion(string apiVersion, out DateTime apiVersionDate)
    {
      string format = !apiVersion.ToLowerInvariant().Contains("preview") ? "yyyy-MM-dd" : "yyyy-MM-dd-preview";
      return DateTime.TryParseExact(apiVersion, format, (IFormatProvider) CultureInfo.InvariantCulture, DateTimeStyles.None, out apiVersionDate);
    }
  }
}
