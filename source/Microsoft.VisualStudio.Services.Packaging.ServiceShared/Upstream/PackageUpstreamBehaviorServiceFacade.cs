// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.Upstream.PackageUpstreamBehaviorServiceFacade
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Feed.WebApi;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.Upstream
{
  public class PackageUpstreamBehaviorServiceFacade : IPackageUpstreamBehaviorFacade
  {
    private readonly IVssRequestContext requestContext;
    private readonly IPackageUpstreamBehaviorService service;

    public PackageUpstreamBehaviorServiceFacade(IVssRequestContext requestContext)
    {
      this.requestContext = requestContext;
      this.service = requestContext.GetService<IPackageUpstreamBehaviorService>();
    }

    public UpstreamingBehavior GetBehavior(FeedCore feed, IPackageName packageName) => this.service.GetBehavior(this.requestContext, feed, packageName);

    public void SetBehavior(FeedCore feed, IPackageName packageName, UpstreamingBehavior behavior) => this.service.SetBehavior(this.requestContext, feed, packageName, behavior);
  }
}
