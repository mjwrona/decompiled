// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.NotificationHubs.ApiVersionConstants
// Assembly: Microsoft.Azure.NotificationHubs, Version=2.16.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1F43328A-44A2-48DE-9CBC-06F3C4A41C2A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.NotificationHubs.dll

namespace Microsoft.Azure.NotificationHubs
{
  internal class ApiVersionConstants
  {
    public const string Name = "api-version";
    public const string VersionTwo = "2012-03";
    public const string VersionThree = "2012-08";
    public const string VersionFour = "2013-04";
    public const string VersionFive = "2013-07";
    public const string VersionSix = "2013-08";
    public const string VersionSeven = "2013-10";
    public const string VersionEight = "2014-01";
    public const string VersionNine = "2014-05";
    public const string VersionTen = "2014-08";
    public const string VersionEleven = "2014-09";
    public const string VersionTwelve = "2015-01";
    public const string VersionThirteen = "2015-04";
    public const string VersionFourteen = "2015-08";
    public const string VersionFifteen = "2016-03";
    public const string VersionSixteen = "2016-07";
    public const string MaxSupportedApiVersion = "2016-07";
    public const string OldRuntimeVersion = "2013-07";
    public const string PartitionedEntityMinimumRuntimeApiVersionText = "2013-10";
    public const string SubscriptionPartitioningMinimumRuntimeApiVersionText = "2014-01";
    public static readonly string SupportedVersions = string.Join(",", new string[15]
    {
      "2012-03",
      "2012-08",
      "2013-04",
      "2013-07",
      "2013-08",
      "2013-10",
      "2014-01",
      "2014-05",
      "2014-08",
      "2014-09",
      "2015-01",
      "2015-04",
      "2015-08",
      "2016-03",
      "2016-07"
    });
  }
}
