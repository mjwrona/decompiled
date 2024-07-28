// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.NuGet.Server.Search3.NuGetSearchTelemetryRecorder
// Assembly: Microsoft.VisualStudio.Services.NuGet.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B6DD8F0-A807-4AA3-9A6E-1E5CDBF27B34
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.NuGet.Server.dll

using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Aggregations;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.Facades;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.NuGet.Server.Search3
{
  public class NuGetSearchTelemetryRecorder : 
    IAsyncHandler<NuGetSearchResultsInfo>,
    IAsyncHandler<NuGetSearchResultsInfo, NullResult>,
    IHaveInputType<NuGetSearchResultsInfo>,
    IHaveOutputType<NullResult>
  {
    private readonly ITracerService tracerService;
    private readonly IFeatureFlagService featureFlagService;
    private readonly IVersionCountsImplementationMetricsRecorder versionCountsRecorder;

    public NuGetSearchTelemetryRecorder(
      ITracerService tracerService,
      IFeatureFlagService featureFlagService,
      IVersionCountsImplementationMetricsRecorder versionCountsRecorder)
    {
      this.tracerService = tracerService;
      this.featureFlagService = featureFlagService;
      this.versionCountsRecorder = versionCountsRecorder;
    }

    public Task<NullResult> Handle(NuGetSearchResultsInfo request)
    {
      using (ITracerBlock tracerBlock = this.tracerService.Enter((object) this, nameof (Handle)))
      {
        NuGetSearchTelemetryCollector telemetry = request.Telemetry;
        if (telemetry.VersionCountsMetrics != null)
          this.versionCountsRecorder.Record(telemetry.VersionCountsMetrics);
        if (this.featureFlagService.IsEnabled("NuGet.Search3.TraceTelemetryAlways"))
          tracerBlock.TraceInfoAlways(Format());
        else
          tracerBlock.TraceConditionally(new Func<string>(Format));
        return NullResult.NullTask;

        string Format() => JsonConvert.SerializeObject((object) new
        {
          OverallTime = telemetry.OverallTime.Total,
          GetNameListTime = telemetry.GetNameList.Total,
          PackageNameEntries = telemetry.PackageNameEntries.Total,
          GetPackageMetadataTime = telemetry.GetPackageMetadata.Total,
          GetPackageMetadataCount = telemetry.GetPackageMetadata.Count,
          PackageMetadataEntries = telemetry.PackageMetadataEntries.Total,
          FilterVersionsStage1Count = telemetry.FilterVersionsStage1.Count,
          FilterVersionsStage1Time = telemetry.FilterVersionsStage1.Total,
          VersionsLeftAfterFilterStage1 = telemetry.VersionsAfterFilterStage1.Total,
          FilterVersionsStage2Count = telemetry.FilterVersionsStage2.Count,
          FilterVersionsStage2Time = telemetry.FilterVersionsStage2.Total,
          VersionsLeftAfterFilterStage2 = telemetry.VersionsAfterFilterStage2.Total,
          SkippedByCountPackageCount = telemetry.SkippedByCountVersions.Count,
          SkippedByCountVersionCount = telemetry.SkippedByCountVersions.Total,
          SkippedByZeroCountPackageCount = telemetry.SkippedByZeroCountPackages.Total
        });
      }
    }
  }
}
