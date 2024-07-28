// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetricsConstants
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll


#nullable enable
namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared
{
  public static class PackageMetricsConstants
  {
    internal static readonly string PackageMetricsSettingsRoot = "/Configuration/Packaging/Metrics/";
    public static readonly string MaxDownloadStatsDictionarySizePath = PackageMetricsConstants.PackageMetricsSettingsRoot + "MaxDownloadStatsDictionarySize";
    public static readonly string SaveDownloadStatsDictionarySizePath = PackageMetricsConstants.PackageMetricsSettingsRoot + "SaveDownloadStatsDictionarySize";
    public static readonly string FlushMetricsTaskScheduleIntervalPath = PackageMetricsConstants.PackageMetricsSettingsRoot + "FlushMetricsTaskScheduleInterval";
    public static readonly string WriteToFeedMetricsBatchSizePath = PackageMetricsConstants.PackageMetricsSettingsRoot + "MaxWriteToFeedBatchSize";
    public static readonly string MaxConcurrentBatchWritesPath = PackageMetricsConstants.PackageMetricsSettingsRoot + "MaxConcurrentBatchWrites";
    public static readonly int DefaultMaxDownloadStatsDictionarySize = 100000;
    public static readonly int DefaultSaveDownloadStatsDictionarySize = 50000;
    public static readonly int DefaultFlushMetricsTaskScheduleInterval = 600000;
    public static readonly int DefaultWriteToFeedMetricsBatchSize = 1000;
    public static readonly int DefaultMaxConcurrentBatchWrites = 3;
    public static readonly int DefaultFlushMetricsTaskTimeout = 300000;
  }
}
