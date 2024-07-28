// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Feed.Server.RetentionDefaultConstants
// Assembly: Microsoft.VisualStudio.Services.Feed.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 55555083-3B79-4F4F-AA85-92D66019974E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Feed.Server.dll

namespace Microsoft.VisualStudio.Services.Feed.Server
{
  internal static class RetentionDefaultConstants
  {
    public const int DefaultDaysToKeepRecentlyDownloadedPackagesLimit = 30;
    public const string DefaultDaysToKeepRecentlyDownloadedPackagesLimitPath = "/Configuration/Feed/PackageRetention/DefaultDaysToKeepRecentlyDownloadedPackagesLimit";
    public const int DefaultVersionCountLimit = 20;
    public const string DefaultVersionCountLimitPath = "/Configuration/Feed/PackageRetention/DefaultVersionCountLimit";
  }
}
