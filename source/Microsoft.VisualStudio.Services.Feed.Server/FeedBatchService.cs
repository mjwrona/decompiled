// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Feed.Server.FeedBatchService
// Assembly: Microsoft.VisualStudio.Services.Feed.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 55555083-3B79-4F4F-AA85-92D66019974E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Feed.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Feed.Common;
using Microsoft.VisualStudio.Services.Feed.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Feed.Server
{
  public class FeedBatchService : IFeedBatchService, IVssFrameworkService
  {
    private IFeedService feedService;

    void IVssFrameworkService.ServiceStart(IVssRequestContext requestContext) => requestContext.TraceBlock(10019000, 10019001, 10019002, "Feed", "Service", (Action) (() =>
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      this.feedService = requestContext.ServiceHost.Is(TeamFoundationHostType.ProjectCollection) ? requestContext.GetService<IFeedService>() : throw new UnexpectedHostTypeException(requestContext.ServiceHost.HostType);
    }), "ServiceStart");

    void IVssFrameworkService.ServiceEnd(IVssRequestContext requestContext) => requestContext.TraceBlock(10019003, 10019004, 10019005, "Feed", "Service", (Action) (() => ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext))), "ServiceEnd");

    public void SaveCachedPackages(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.Feed.WebApi.Feed feed,
      IEnumerable<string> normalizedPackageNames,
      IEnumerable<Guid> views)
    {
      FeedSecurityHelper.CheckModifyIndexPermissions(requestContext, (FeedCore) feed);
      if (this.feedService.GetFeedInternalState(requestContext, feed.Id).State != 3 || !feed.Capabilities.HasFlag((Enum) FeedCapabilities.UnderMaintenance))
        return;
      IEnumerable<FeedView> feedViews;
      using (FeedViewSqlResourceComponent component = requestContext.CreateComponent<FeedViewSqlResourceComponent>())
        feedViews = component.GetFeedViews(feed.GetIdentity());
      using (PackageSqlResourceComponent component = requestContext.CreateComponent<PackageSqlResourceComponent>())
        component.UpgradeCachedPackages(feed, normalizedPackageNames, feedViews != null ? feedViews.Where<FeedView>((Func<FeedView, bool>) (v => views.Contains<Guid>(v.Id))) : (IEnumerable<FeedView>) null);
      requestContext.GetService<IFeedCapabilitiesService>().EndFeedUpgrade(requestContext, feed);
    }

    public void RunBatch(IVssRequestContext requestContext, Microsoft.VisualStudio.Services.Feed.WebApi.Feed feed, FeedBatchData data)
    {
      if (data.Operation != FeedBatchOperation.SaveCachedPackages)
        throw new InvalidUserInputException(Resources.Error_UnknownBatchOperation());
      if (!(data.Data is SaveCachedPackagesData data1))
        throw new InvalidUserInputException(Resources.Error_BatchDataIsRequired());
      this.SaveCachedPackages(requestContext, feed, data1.NormalizedPackageNames, data1.ViewsForPromotion);
    }
  }
}
