// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetrics.PackageMetricsSettings
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.TeamFoundation.Framework.Server;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetrics
{
  public class PackageMetricsSettings
  {
    public PackageMetricsSettings()
    {
    }

    public PackageMetricsSettings(IVssRequestContext requestContext)
    {
      if (!this.IsPackageMetricsEnabled(requestContext))
        return;
      this.LoadSettings(requestContext);
    }

    private void LoadSettings(IVssRequestContext requestContext)
    {
      IVssRegistryService service = requestContext.GetService<IVssRegistryService>();
      this.MaxDownloadStatsDictionarySize = service.GetValue<int>(requestContext, (RegistryQuery) PackageMetricsConstants.MaxDownloadStatsDictionarySizePath, true, PackageMetricsConstants.DefaultMaxDownloadStatsDictionarySize);
      this.SaveDownloadStatsDictionarySize = service.GetValue<int>(requestContext, (RegistryQuery) PackageMetricsConstants.SaveDownloadStatsDictionarySizePath, true, PackageMetricsConstants.DefaultSaveDownloadStatsDictionarySize);
      this.FlushMetricsTaskScheduleInterval = service.GetValue<int>(requestContext, (RegistryQuery) PackageMetricsConstants.FlushMetricsTaskScheduleIntervalPath, true, PackageMetricsConstants.DefaultFlushMetricsTaskScheduleInterval);
      this.WriteToFeedBatchSize = service.GetValue<int>(requestContext, (RegistryQuery) PackageMetricsConstants.WriteToFeedMetricsBatchSizePath, true, PackageMetricsConstants.DefaultWriteToFeedMetricsBatchSize);
      this.MaxConcurrentBatchWrites = service.GetValue<int>(requestContext, (RegistryQuery) PackageMetricsConstants.MaxConcurrentBatchWritesPath, true, PackageMetricsConstants.DefaultMaxConcurrentBatchWrites);
    }

    public bool IsPackageMetricsEnabled(IVssRequestContext requestContext) => requestContext.IsFeatureEnabled("Packaging.PackageMetrics");

    public bool ShouldWriteToFeed(IVssRequestContext requestContext) => requestContext.IsFeatureEnabled("Packaging.PackageMetricsWriteToFeed");

    public int MaxDownloadStatsDictionarySize { get; internal set; }

    public int SaveDownloadStatsDictionarySize { get; internal set; }

    public int FlushMetricsTaskScheduleInterval { get; internal set; }

    public int WriteToFeedBatchSize { get; internal set; }

    public int MaxConcurrentBatchWrites { get; internal set; }
  }
}
