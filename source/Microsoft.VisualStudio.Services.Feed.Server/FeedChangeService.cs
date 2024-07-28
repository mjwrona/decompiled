// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Feed.Server.FeedChangeService
// Assembly: Microsoft.VisualStudio.Services.Feed.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 55555083-3B79-4F4F-AA85-92D66019974E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Feed.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Feed.Common;
using Microsoft.VisualStudio.Services.Feed.Server.Utils;
using Microsoft.VisualStudio.Services.Feed.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Feed.Server
{
  public class FeedChangeService : IFeedChangeService, IVssFrameworkService
  {
    private const int MAX_BATCH_SIZE = 500000;

    void IVssFrameworkService.ServiceStart(IVssRequestContext requestContext) => requestContext.TraceBlock(10019400, 10019401, 10019402, "FeedChange", "Service", (Action) (() => ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext))), "ServiceStart");

    void IVssFrameworkService.ServiceEnd(IVssRequestContext requestContext) => requestContext.TraceBlock(10019403, 10019404, 10019405, "FeedChange", "Service", (Action) (() => ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext))), "ServiceEnd");

    public IEnumerable<FeedChange> GetFeedChanges(
      IVssRequestContext requestContext,
      ProjectReference project,
      bool includeDeleted = false,
      long continuationToken = 0,
      int batchSize = 1000)
    {
      return requestContext.TraceBlock<IEnumerable<FeedChange>>(10019406, 10019407, 10019408, "FeedChange", "Service", (Func<IEnumerable<FeedChange>>) (() =>
      {
        ArgumentUtility.CheckBoundsInclusive(batchSize, 1, 500000, nameof (batchSize));
        FeatureAvailabilityHelper.ThrowIfUnsupportedProjectScope(requestContext, project);
        if (requestContext.ExecutionEnvironment.IsOnPremisesDeployment)
          FeedSecurityHelper.CheckModifyIndexPermissions(requestContext);
        IVssSecurityNamespace feedSecurityNamespace = FeedSecurityHelper.GetFeedSecurity(requestContext);
        feedSecurityNamespace.PollForRequestLocalInvalidation(requestContext);
        IEnumerable<FeedChange> feedChanges;
        using (FeedChangeSqlResourceComponent component = requestContext.CreateComponent<FeedChangeSqlResourceComponent>())
        {
          try
          {
            feedChanges = component.GetFeedChanges(project?.Id, includeDeleted, continuationToken, batchSize).Where<FeedChange>((Func<FeedChange, bool>) (feedChange => feedSecurityNamespace.HasPermission(requestContext, FeedSecurityHelper.CalculateSecurityToken((FeedCore) feedChange.Feed), 32)));
          }
          catch (DataspaceNotFoundException ex)
          {
            feedChanges = (IEnumerable<FeedChange>) new List<FeedChange>();
          }
          catch (VssServiceException ex) when (FeedException.IsHostShutdownResponse(ex))
          {
            throw new HostShutdownException(ex.Message);
          }
        }
        if ((object) project == null)
        {
          IEnumerable<FeedChange> filteredOutFeeds;
          ProjectHelper.HydrateProjectReferences(requestContext, feedChanges, out filteredOutFeeds);
          feedChanges = feedChanges.Except<FeedChange>(filteredOutFeeds);
        }
        else
          feedChanges.ForEach<FeedChange>((Action<FeedChange>) (x => ProjectHelper.HydrateProjectReference(x.Feed, project)));
        return feedChanges;
      }), nameof (GetFeedChanges));
    }

    public FeedChange GetFeedChange(
      IVssRequestContext requestContext,
      ProjectReference project,
      string feedId)
    {
      return requestContext.TraceBlock<FeedChange>(100192409, 10019410, 10019411, "FeedChange", "Service", (Func<FeedChange>) (() =>
      {
        feedId.ThrowIfNullOrWhitespace((Func<Exception>) (() => (Exception) InvalidUserInputException.Create(nameof (feedId))));
        if (requestContext.ExecutionEnvironment.IsOnPremisesDeployment)
          FeedSecurityHelper.CheckModifyIndexPermissions(requestContext);
        Guid result;
        FeedIdentity feedIdentity;
        if (Guid.TryParse(feedId, out result))
        {
          feedIdentity = new FeedIdentity(project?.Id, result);
          FeedSecurityHelper.CheckReadFeedPermissions(requestContext, ProjectFeedsConversionHelper.ExplicitlyCreateFeedCoreWithoutCallingGetFeed(feedIdentity));
        }
        else
          feedIdentity = requestContext.GetService<IFeedService>().GetFeed(requestContext, feedId, project, true).GetIdentity();
        FeedChange feedChange;
        using (FeedChangeSqlResourceComponent component = requestContext.CreateComponent<FeedChangeSqlResourceComponent>())
          feedChange = component.GetFeedChange(feedIdentity);
        if (feedChange != null)
          ProjectHelper.HydrateProjectReference(feedChange.Feed, project);
        return feedChange;
      }), nameof (GetFeedChange));
    }

    public IEnumerable<PackageChange> GetPackageChanges(
      IVssRequestContext requestContext,
      FeedCore feed,
      long continuationToken = 0,
      int batchSize = 1000)
    {
      return requestContext.TraceBlock<IEnumerable<PackageChange>>(10019412, 10019413, 10019414, "FeedChange", "Service", (Func<IEnumerable<PackageChange>>) (() =>
      {
        feed.ThrowIfNull<FeedCore>((Func<Exception>) (() => (Exception) InvalidUserInputException.Create(nameof (feed))));
        if (requestContext.ExecutionEnvironment.IsOnPremisesDeployment)
          FeedSecurityHelper.CheckModifyIndexPermissions(requestContext);
        using (FeedChangeSqlResourceComponent component = requestContext.CreateComponent<FeedChangeSqlResourceComponent>())
          return component.GetPackageChanges(feed.GetIdentity(), continuationToken, batchSize);
      }), nameof (GetPackageChanges));
    }
  }
}
