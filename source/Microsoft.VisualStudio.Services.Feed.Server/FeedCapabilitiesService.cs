// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Feed.Server.FeedCapabilitiesService
// Assembly: Microsoft.VisualStudio.Services.Feed.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 55555083-3B79-4F4F-AA85-92D66019974E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Feed.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Feed.Common;
using Microsoft.VisualStudio.Services.Feed.WebApi;
using System;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Feed.Server
{
  public class FeedCapabilitiesService : IFeedCapabilitiesService, IVssFrameworkService
  {
    private IFeedService feedService;

    void IVssFrameworkService.ServiceStart(IVssRequestContext requestContext) => requestContext.TraceBlock(10019000, 10019001, 10019002, "Feed", "Service", (Action) (() =>
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      this.feedService = requestContext.ServiceHost.Is(TeamFoundationHostType.ProjectCollection) ? requestContext.GetService<IFeedService>() : throw new UnexpectedHostTypeException(requestContext.ServiceHost.HostType);
    }), "ServiceStart");

    void IVssFrameworkService.ServiceEnd(IVssRequestContext requestContext) => requestContext.TraceBlock(10019003, 10019004, 10019005, "Feed", "Service", (Action) (() => ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext))), "ServiceEnd");

    public FeedCapabilities GetCapabilities(IVssRequestContext requestContext, Microsoft.VisualStudio.Services.Feed.WebApi.Feed feed)
    {
      FeedSecurityHelper.CheckAdministerFeedPermissions(requestContext, (FeedCore) feed);
      return feed.Capabilities;
    }

    public Task<bool> UpdateCapabilityAsync(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.Feed.WebApi.Feed feed,
      FeedCapabilities capabilities)
    {
      return capabilities == feed.Capabilities ? Task.FromResult<bool>(true) : Task.FromResult<bool>(false);
    }

    public void EndFeedUpgrade(IVssRequestContext requestContext, Microsoft.VisualStudio.Services.Feed.WebApi.Feed feed)
    {
      using (FeedSqlResourceComponent component = requestContext.CreateComponent<FeedSqlResourceComponent>())
        component.EndFeedUpgrade(feed);
      requestContext.GetService<IFeedServiceFeedCache>().Invalidate(requestContext, feed.Id);
    }
  }
}
