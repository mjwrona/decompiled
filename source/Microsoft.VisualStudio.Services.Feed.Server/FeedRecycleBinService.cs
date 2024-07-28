// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Feed.Server.FeedRecycleBinService
// Assembly: Microsoft.VisualStudio.Services.Feed.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 55555083-3B79-4F4F-AA85-92D66019974E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Feed.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Feed.Common;
using Microsoft.VisualStudio.Services.Feed.Server.MessageBus;
using Microsoft.VisualStudio.Services.Feed.Server.Telemetry;
using Microsoft.VisualStudio.Services.Feed.Server.Utils;
using Microsoft.VisualStudio.Services.Feed.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Feed.Server
{
  public class FeedRecycleBinService : IFeedRecycleBinService, IVssFrameworkService
  {
    public void ServiceStart(IVssRequestContext systemRequestContext) => systemRequestContext.CheckProjectCollectionRequestContext();

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    public IEnumerable<Microsoft.VisualStudio.Services.Feed.WebApi.Feed> GetFeedsFromRecycleBin(
      IVssRequestContext requestContext,
      ProjectReference project)
    {
      return (IEnumerable<Microsoft.VisualStudio.Services.Feed.WebApi.Feed>) requestContext.TraceBlock<List<Microsoft.VisualStudio.Services.Feed.WebApi.Feed>>(10019706, 10019707, 10019708, "FeedRecycleBin", "Service", (Func<List<Microsoft.VisualStudio.Services.Feed.WebApi.Feed>>) (() =>
      {
        FeatureAvailabilityHelper.ThrowIfUnsupportedProjectScope(requestContext, project);
        IEnumerable<Microsoft.VisualStudio.Services.Feed.WebApi.Feed> array;
        using (FeedRecycleBinSqlResourceComponent component = requestContext.CreateComponent<FeedRecycleBinSqlResourceComponent>())
          array = (IEnumerable<Microsoft.VisualStudio.Services.Feed.WebApi.Feed>) component.GetFeedsInRecycleBin(project?.Id).ToArray<Microsoft.VisualStudio.Services.Feed.WebApi.Feed>();
        List<Microsoft.VisualStudio.Services.Feed.WebApi.Feed> first = new List<Microsoft.VisualStudio.Services.Feed.WebApi.Feed>();
        foreach (Microsoft.VisualStudio.Services.Feed.WebApi.Feed feed in array)
        {
          if (FeedSecurityHelper.HasFeedPermissions(requestContext, (FeedCore) feed, 32, true))
            first.Add(feed);
        }
        if (project != (ProjectReference) null)
        {
          first.ForEach((Action<Microsoft.VisualStudio.Services.Feed.WebApi.Feed>) (x => ProjectHelper.HydrateProjectReference(x, project)));
        }
        else
        {
          IEnumerable<Microsoft.VisualStudio.Services.Feed.WebApi.Feed> filteredOutFeeds;
          ProjectHelper.HydrateProjectReferences(requestContext, (IEnumerable<Microsoft.VisualStudio.Services.Feed.WebApi.Feed>) first, out filteredOutFeeds);
          first = first.Except<Microsoft.VisualStudio.Services.Feed.WebApi.Feed>(filteredOutFeeds).ToList<Microsoft.VisualStudio.Services.Feed.WebApi.Feed>();
        }
        first.Sort((Comparison<Microsoft.VisualStudio.Services.Feed.WebApi.Feed>) ((list1, list2) => string.Compare(list1.Name, list2.Name, StringComparison.Ordinal)));
        return first;
      }), nameof (GetFeedsFromRecycleBin));
    }

    public void PermanentDeleteFeed(
      IVssRequestContext requestContext,
      ProjectReference project,
      string feedId)
    {
      requestContext.TraceBlock(10019700, 10019701, 10019702, "FeedRecycleBin", "Service", (Action) (() =>
      {
        FeatureAvailabilityHelper.ThrowIfUnsupportedProjectScope(requestContext, project);
        feedId.ThrowIfNullOrWhitespace((Func<Exception>) (() => (Exception) InvalidUserInputException.Create(nameof (feedId))));
        Microsoft.VisualStudio.Services.Feed.WebApi.Feed feed = this.GetFeedFromRecycleBinInternal(requestContext, project, feedId).ThrowIfReadOnly(FeedSecurityHelper.CanBypassUnderMaintenance(requestContext));
        FeedSecurityHelper.CheckDeleteFeedPermissions(requestContext, (FeedCore) feed);
        using (FeedRecycleBinSqlResourceComponent component = requestContext.CreateComponent<FeedRecycleBinSqlResourceComponent>())
          component.PermanentDeleteFeed(feed.GetIdentity());
        FeedChangedPublisher.PermanentDeletedFeed(requestContext, feed);
        FeedCiPublisher.PublishPermanentDeleteFeedEvent(requestContext, feed.Id);
        FeedAuditHelper.AuditHardDeleteFeed(requestContext, feed);
      }), nameof (PermanentDeleteFeed));
    }

    public void RestoreDeletedFeed(
      IVssRequestContext requestContext,
      ProjectReference project,
      string feedId)
    {
      requestContext.TraceBlock(10019703, 10019704, 10019705, "FeedRecycleBin", "Service", (Action) (() =>
      {
        FeatureAvailabilityHelper.ThrowIfUnsupportedProjectScope(requestContext, project);
        feedId.ThrowIfNullOrWhitespace((Func<Exception>) (() => (Exception) InvalidUserInputException.Create(nameof (feedId))));
        Microsoft.VisualStudio.Services.Feed.WebApi.Feed feed = this.GetFeedFromRecycleBinInternal(requestContext, project, feedId).ThrowIfReadOnly(FeedSecurityHelper.CanBypassUnderMaintenance(requestContext));
        FeedSecurityHelper.CheckAdministerFeedPermissions(requestContext, (FeedCore) feed);
        using (FeedRecycleBinSqlResourceComponent component = requestContext.CreateComponent<FeedRecycleBinSqlResourceComponent>())
          component.RestoreDeletedFeed(feed.GetIdentity());
        FeedChangedPublisher.RestoredDeletedFeed(requestContext, feed);
        FeedCiPublisher.PublishRestoreDeletedFeedEvent(requestContext, feed.Id);
      }), nameof (RestoreDeletedFeed));
    }

    private Microsoft.VisualStudio.Services.Feed.WebApi.Feed GetFeedFromRecycleBinInternal(
      IVssRequestContext requestContext,
      ProjectReference project,
      string feedIdOrName)
    {
      feedIdOrName = FeedViewsHelper.ParseFeedNameParameter(feedIdOrName, out string _);
      Microsoft.VisualStudio.Services.Feed.WebApi.Feed targetFeed = (Microsoft.VisualStudio.Services.Feed.WebApi.Feed) null;
      try
      {
        using (FeedRecycleBinSqlResourceComponent component = requestContext.CreateComponent<FeedRecycleBinSqlResourceComponent>())
        {
          Guid result;
          targetFeed = Guid.TryParse(feedIdOrName, out result) ? component.GetFeedInRecycleBinById(new FeedIdentity(project?.Id, result)) : component.GetFeedInRecycleBinByName(project?.Id, feedIdOrName);
        }
      }
      catch (DataspaceNotFoundException ex)
      {
        throw new FeedIdNotFoundException(Resources.Error_FeedIdNotFoundInRecycleBinMessage((object) feedIdOrName));
      }
      if ((targetFeed != null ? (!targetFeed.DeletedDate.HasValue ? 1 : 0) : 1) != 0)
        throw new FeedIdNotFoundException(Resources.Error_FeedIdNotFoundInRecycleBinMessage((object) feedIdOrName));
      ProjectHelper.HydrateProjectReference(targetFeed, project);
      return targetFeed;
    }
  }
}
