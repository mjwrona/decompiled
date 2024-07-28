// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Feed.Server.ClientServices.PlatformCachedPackagesInternalClientService
// Assembly: Microsoft.VisualStudio.Services.Feed.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 55555083-3B79-4F4F-AA85-92D66019974E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Feed.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Feed.Common;
using Microsoft.VisualStudio.Services.Feed.Common.ClientServices;
using Microsoft.VisualStudio.Services.Feed.WebApi;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Feed.Server.ClientServices
{
  public class PlatformCachedPackagesInternalClientService : 
    ICachedPackagesInternalClientService,
    IVssFrameworkService
  {
    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
    }

    public Task ClearCachedPackagesAsync(
      IVssRequestContext requestContext,
      FeedCore feedCore,
      string protocolType,
      string upstreamId)
    {
      Microsoft.VisualStudio.Services.Feed.WebApi.Feed feed = requestContext.GetService<IFeedService>().GetFeed(requestContext, feedCore.Id.ToString(), feedCore.Project).ThrowIfReadOnly(FeedSecurityHelper.CanBypassUnderMaintenance(requestContext));
      requestContext.GetService<IFeedIndexService>().ClearCachedPackages(requestContext, feed, protocolType);
      return Task.CompletedTask;
    }

    public IEnumerable<string> GetCachedPackages(IVssRequestContext requestContext, Microsoft.VisualStudio.Services.Feed.WebApi.Feed feed)
    {
      if (!feed.UpstreamEnabled)
        return Enumerable.Empty<string>();
      using (PackageSqlResourceComponent component = requestContext.CreateComponent<PackageSqlResourceComponent>())
        return (IEnumerable<string>) component.GetCachedPackages(feed, "npm");
    }
  }
}
