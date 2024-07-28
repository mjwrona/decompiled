// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Feed.Server.JobConstants
// Assembly: Microsoft.VisualStudio.Services.Feed.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 55555083-3B79-4F4F-AA85-92D66019974E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Feed.Server.dll

using System;

namespace Microsoft.VisualStudio.Services.Feed.Server
{
  internal static class JobConstants
  {
    public static class UserDownloadAggregationJob
    {
      public static readonly Guid DownloadAggregationJobId = Guid.Parse("EB3891D7-C14C-409E-986B-B67746EB49F4");
      public const string DownloadMetricsBatchSize = "/Configuration/Feed/DownloadMetricsAggregation/SqlBatchSize";
      public const int DefaultDownloadMetricsBatchSize = 1000;
      public const string MetricsAggregationJobLastExecutedTimestampPath = "/Configuration/Feed/DownloadMetricsAggregation/LastExecutedTimestamp";
    }

    public static class PackageRetentionJob
    {
      public const string DefaultPackageMetricsEnabledTimestamp = "01/01/2019";
      public const string DefaultPackageMetricsEnabledTimestampPath = "/Configuration/Feed/PackageRetention/PackageMetricsEnabledTimestamp";
    }
  }
}
