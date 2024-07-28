// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.Aggregations.VersionListWithSize.PackagingTracesVersionCountsWithSizeImplMetricsRecorder
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.Aggregations.VersionListWithSize
{
  public class PackagingTracesVersionCountsWithSizeImplMetricsRecorder : 
    IVersionCountsImplementationMetricsRecorder
  {
    private readonly IPackagingTraces packagingTraces;

    public PackagingTracesVersionCountsWithSizeImplMetricsRecorder(IPackagingTraces packagingTraces) => this.packagingTraces = packagingTraces;

    public void Record(IVersionCountsImplementationMetrics metrics, string prefix = "Last")
    {
      this.packagingTraces.AddProperty(prefix + "NVSPkgsUnpacked", (object) metrics.PackagesUnpacked);
      this.packagingTraces.AddProperty(prefix + "NVSPkgsPacked", (object) metrics.PackagesPacked);
      this.packagingTraces.AddProperty(prefix + "NVSPkgsNeedUnpack", (object) metrics.NumPackagesNeedingUnpack);
      this.packagingTraces.AddProperty(prefix + "NVSPkgsNeedRepack", (object) metrics.NumPackagesNeedingRepack);
      this.packagingTraces.AddProperty(prefix + "NVSPkgsNeedSave", (object) metrics.NumPackagesNeedingSave);
      this.packagingTraces.AddProperty(prefix + "NVSPkgsCount", (object) metrics.NumPackages);
      this.packagingTraces.AddProperty(prefix + "NVSPkgsVerCount", (object) metrics.NumTotalVersions);
    }
  }
}
