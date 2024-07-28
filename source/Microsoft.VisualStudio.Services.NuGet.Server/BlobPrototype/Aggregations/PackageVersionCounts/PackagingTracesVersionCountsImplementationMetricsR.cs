// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.NuGet.Server.BlobPrototype.Aggregations.PackageVersionCounts.PackagingTracesVersionCountsImplementationMetricsRecorder
// Assembly: Microsoft.VisualStudio.Services.NuGet.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B6DD8F0-A807-4AA3-9A6E-1E5CDBF27B34
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.NuGet.Server.dll

using Microsoft.VisualStudio.Services.Packaging.ServiceShared;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Aggregations;

namespace Microsoft.VisualStudio.Services.NuGet.Server.BlobPrototype.Aggregations.PackageVersionCounts
{
  internal class PackagingTracesVersionCountsImplementationMetricsRecorder : 
    IVersionCountsImplementationMetricsRecorder
  {
    private readonly IPackagingTraces packagingTraces;

    public PackagingTracesVersionCountsImplementationMetricsRecorder(
      IPackagingTraces packagingTraces)
    {
      this.packagingTraces = packagingTraces;
    }

    public void Record(IVersionCountsImplementationMetrics metrics, string prefix = "Last")
    {
      this.packagingTraces.AddProperty(prefix + "NVBPkgsUnpacked", (object) metrics.PackagesUnpacked);
      this.packagingTraces.AddProperty(prefix + "NVBPkgsPacked", (object) metrics.PackagesPacked);
      this.packagingTraces.AddProperty(prefix + "NVBPkgsNeedUnpack", (object) metrics.NumPackagesNeedingUnpack);
      this.packagingTraces.AddProperty(prefix + "NVBPkgsNeedRepack", (object) metrics.NumPackagesNeedingRepack);
      this.packagingTraces.AddProperty(prefix + "NVBPkgsNeedSave", (object) metrics.NumPackagesNeedingSave);
      this.packagingTraces.AddProperty(prefix + "NVBPkgsCount", (object) metrics.NumPackages);
      this.packagingTraces.AddProperty(prefix + "NVBPkgsVerCount", (object) metrics.NumTotalVersions);
    }
  }
}
