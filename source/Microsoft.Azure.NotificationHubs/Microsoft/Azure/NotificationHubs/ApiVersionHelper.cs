// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.NotificationHubs.ApiVersionHelper
// Assembly: Microsoft.Azure.NotificationHubs, Version=2.16.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1F43328A-44A2-48DE-9CBC-06F3C4A41C2A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.NotificationHubs.dll

using System;
using System.Globalization;
using System.Web;

namespace Microsoft.Azure.NotificationHubs
{
  internal static class ApiVersionHelper
  {
    public static int OldRuntimeApiVersion = (int) ApiVersionHelper.GetVersion("2013-07");
    public static int CurrentRuntimeApiVersion = (int) ApiVersionHelper.GetVersion("2016-07");
    public static int PartitionedEntityMinimumRuntimeApiVersion = (int) ApiVersionHelper.GetVersion("2013-10");
    public static int VersionSix = (int) ApiVersionHelper.GetVersion("2013-08");
    public static int VersionSeven = (int) ApiVersionHelper.GetVersion("2013-10");
    public static int VersionEight = (int) ApiVersionHelper.GetVersion("2014-01");
    public static int VersionNine = (int) ApiVersionHelper.GetVersion("2014-05");
    public static int VersionTen = (int) ApiVersionHelper.GetVersion("2014-08");
    public static int VersionEleven = (int) ApiVersionHelper.GetVersion("2014-09");
    public static int VersionTwelve = (int) ApiVersionHelper.GetVersion("2015-01");
    public static int VersionThirteen = (int) ApiVersionHelper.GetVersion("2015-04");
    public static int VersionFourteen = (int) ApiVersionHelper.GetVersion("2015-08");
    public static int VersionFifteen = (int) ApiVersionHelper.GetVersion("2016-03");
    public static int VersionSixteen = (int) ApiVersionHelper.GetVersion("2016-07");
    public static int SubscriptionPartitioningMinimumRuntimeApiVersion = (int) ApiVersionHelper.GetVersion("2014-01");
    public static readonly ApiVersion CurrentApiVersion = ApiVersionHelper.GetVersion("2016-07");

    public static ApiVersion GetClientApiVersion(Uri requestUri) => ApiVersionHelper.GetClientApiVersion(requestUri.Query);

    public static ApiVersion GetClientApiVersion(string queryString)
    {
      string version = (string) null;
      if (!string.IsNullOrEmpty(queryString))
        version = HttpUtility.ParseQueryString(queryString)["api-version"];
      return ApiVersionHelper.GetVersion(version);
    }

    public static string GetApiVersionString(Uri requestUri) => !string.IsNullOrEmpty(requestUri.Query) ? HttpUtility.ParseQueryString(requestUri.Query)["api-version"] : (string) null;

    public static string GetApiVersionString(ApiVersion version)
    {
      switch (version)
      {
        case ApiVersion.One:
          return string.Empty;
        case ApiVersion.Two:
          return "2012-03";
        case ApiVersion.Three:
          return "2012-08";
        case ApiVersion.Four:
          return "2013-04";
        case ApiVersion.Five:
          return "2013-07";
        case ApiVersion.Six:
          return "2013-08";
        case ApiVersion.Seven:
          return "2013-10";
        case ApiVersion.Eight:
          return "2014-01";
        case ApiVersion.Nine:
          return "2014-05";
        case ApiVersion.Ten:
          return "2014-08";
        case ApiVersion.Eleven:
          return "2014-09";
        case ApiVersion.Twelve:
          return "2015-01";
        case ApiVersion.Thirteen:
          return "2015-04";
        case ApiVersion.Fourteen:
          return "2015-08";
        case ApiVersion.Fifteen:
          return "2016-03";
        case ApiVersion.Sixteen:
          return "2016-07";
        default:
          throw new ArgumentException("api-version");
      }
    }

    public static string GetApiVersionQueryString(ApiVersion version)
    {
      string apiVersionString = ApiVersionHelper.GetApiVersionString(version);
      if (string.IsNullOrEmpty(apiVersionString))
        return string.Empty;
      return string.Join("=", new string[2]
      {
        "api-version",
        apiVersionString
      });
    }

    public static ApiVersion GetVersion(string version)
    {
      if (string.IsNullOrEmpty(version))
        return ApiVersion.One;
      if (version.Equals("2012-03", StringComparison.OrdinalIgnoreCase))
        return ApiVersion.Two;
      if (version.Equals("2012-08", StringComparison.OrdinalIgnoreCase))
        return ApiVersion.Three;
      if (version.Equals("2013-04", StringComparison.OrdinalIgnoreCase))
        return ApiVersion.Four;
      if (version.Equals("2013-07", StringComparison.OrdinalIgnoreCase))
        return ApiVersion.Five;
      if (version.Equals("2013-08", StringComparison.OrdinalIgnoreCase))
        return ApiVersion.Six;
      if (version.Equals("2013-10", StringComparison.OrdinalIgnoreCase))
        return ApiVersion.Seven;
      if (version.Equals("2014-01", StringComparison.OrdinalIgnoreCase))
        return ApiVersion.Eight;
      if (version.Equals("2014-05", StringComparison.OrdinalIgnoreCase))
        return ApiVersion.Nine;
      if (version.Equals("2014-08", StringComparison.OrdinalIgnoreCase))
        return ApiVersion.Ten;
      if (version.Equals("2014-09", StringComparison.OrdinalIgnoreCase))
        return ApiVersion.Eleven;
      if (version.Equals("2015-01", StringComparison.OrdinalIgnoreCase))
        return ApiVersion.Twelve;
      if (version.Equals("2015-04", StringComparison.OrdinalIgnoreCase))
        return ApiVersion.Thirteen;
      if (version.Equals("2015-08", StringComparison.OrdinalIgnoreCase))
        return ApiVersion.Fourteen;
      if (version.Equals("2016-03", StringComparison.OrdinalIgnoreCase))
        return ApiVersion.Fifteen;
      if (version.Equals("2016-07", StringComparison.OrdinalIgnoreCase))
        return ApiVersion.Sixteen;
      throw new ArgumentException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, SRClient.UnknownApiVersion((object) version, (object) ApiVersionConstants.SupportedVersions)));
    }
  }
}
