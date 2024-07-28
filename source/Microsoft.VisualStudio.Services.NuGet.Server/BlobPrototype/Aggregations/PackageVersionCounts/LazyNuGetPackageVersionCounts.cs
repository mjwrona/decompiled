// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.NuGet.Server.BlobPrototype.Aggregations.PackageVersionCounts.LazyNuGetPackageVersionCounts
// Assembly: Microsoft.VisualStudio.Services.NuGet.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B6DD8F0-A807-4AA3-9A6E-1E5CDBF27B34
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.NuGet.Server.dll

using Microsoft.VisualStudio.Services.NuGet.Server.PackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using System;
using System.Threading;

namespace Microsoft.VisualStudio.Services.NuGet.Server.BlobPrototype.Aggregations.PackageVersionCounts
{
  public class LazyNuGetPackageVersionCounts : 
    ILazyNuGetPackageVersionCounts,
    IPackageNameEntry<VssNuGetPackageName>
  {
    private readonly Lazy<INuGetPackageVersionCounts> lazyCounts;

    public LazyNuGetPackageVersionCounts(
      IPackageNameEntry<VssNuGetPackageName> package,
      Func<INuGetPackageVersionCounts> valueFactory)
    {
      this.Name = package.Name;
      this.LastUpdatedDateTime = package.LastUpdatedDateTime;
      this.lazyCounts = new Lazy<INuGetPackageVersionCounts>(valueFactory, LazyThreadSafetyMode.PublicationOnly);
    }

    public VssNuGetPackageName Name { get; }

    public DateTime LastUpdatedDateTime { get; }

    public INuGetPackageVersionCounts Get() => this.lazyCounts.Value;
  }
}
