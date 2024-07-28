// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetricsServiceFacade
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Feed.WebApi.Internal;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetrics;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared
{
  public class PackageMetricsServiceFacade : IPackageMetricsServiceFacade
  {
    private readonly IVssRequestContext requestContext;
    private readonly IPackageMetricsService service;

    public PackageMetricsServiceFacade(IVssRequestContext requestContext)
    {
      this.requestContext = requestContext;
      this.service = requestContext.GetService<IPackageMetricsService>();
    }

    public Task UpdatePackageMetricsAsync(
      Guid feedId,
      Guid? projectId,
      IProtocol protocol,
      IPackageIdentity packageIdentity,
      double downloadCount = 1.0)
    {
      return this.service.UpdatePackageMetricsAsync(this.requestContext, feedId, projectId, protocol, packageIdentity, downloadCount);
    }

    public IEnumerable<PackageMetricsData> Write() => this.service.Write(this.requestContext);
  }
}
