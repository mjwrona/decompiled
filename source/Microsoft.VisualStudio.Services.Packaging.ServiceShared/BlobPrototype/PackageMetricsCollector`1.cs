// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.PackageMetricsCollector`1
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetrics;
using System;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype
{
  public class PackageMetricsCollector<TPackageIdentity> : IPackageMetricsCollector<TPackageIdentity>
    where TPackageIdentity : IPackageIdentity
  {
    private readonly IVssRequestContext requestContext;

    public PackageMetricsCollector(IVssRequestContext requestContext) => this.requestContext = requestContext;

    public Task UpdatePackageMetricsAsync(
      Guid feedId,
      Guid? projectId,
      TPackageIdentity packageIdentity,
      double downloadCount = 1.0)
    {
      return this.requestContext.GetService<IPackageMetricsService>().UpdatePackageMetricsAsync(this.requestContext, feedId, projectId, packageIdentity.Name.Protocol, (IPackageIdentity) packageIdentity, downloadCount);
    }
  }
}
