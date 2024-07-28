// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Feed.Common.FeedCacheService
// Assembly: Microsoft.VisualStudio.Services.Feed.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AAC6BCA4-7F6C-4DFE-8058-1CCDD886477F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Feed.Common.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Feed.Common.ClientServices;
using Microsoft.VisualStudio.Services.Feed.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Feed.Common
{
  public class FeedCacheService : 
    VssMemoryCacheService<RawFeedIdentity, FeedCore>,
    IFeedCacheService,
    IVssFrameworkService
  {
    public const double DefaultEvictionTimeInMinutes = 10.0;
    private const double DefaultCleanupTimeInMinutes = 30.0;
    private readonly ConcurrencyConsolidator<RawFeedIdentity, FeedCore> fetchFeedConsolidator = new ConcurrencyConsolidator<RawFeedIdentity, FeedCore>(false, 2);
    private readonly ConcurrencyConsolidator<Guid, FeedCore> fetchFeedByIdConsolidator = new ConcurrencyConsolidator<Guid, FeedCore>(false, 2);
    private IVssMemoryCacheGrouping<RawFeedIdentity, FeedCore, Guid> feedGuidGrouping;
    private IVssMemoryCacheGrouping<RawFeedIdentity, FeedCore, Guid> projectGuidGrouping;

    public FeedCacheService()
      : this(10.0, 30.0)
    {
    }

    public FeedCacheService(double evictionTimeInMinutes, double cleanupTimeInMinutes)
      : base(TimeSpan.FromMinutes(cleanupTimeInMinutes))
    {
      this.InactivityInterval.Value = (TimeSpan) Capture.Create<TimeSpan>(TimeSpan.FromMinutes(evictionTimeInMinutes));
    }

    public FeedCore AddFeed(
      IVssRequestContext requestContext,
      Guid projectId,
      string feedNameOrId,
      FeedCore feed)
    {
      RawFeedIdentity key = FeedCacheService.BuildKey(projectId, feedNameOrId);
      bool flag = this.TryAdd(requestContext, key, feed);
      IVssRequestContext requestContext1 = requestContext;
      string message;
      if (!flag)
        message = string.Format("Feed could not be added to cache: Key= {0}, Project= {1}, Id= {2}, Name= {3}, UpstreamEnabled= {4}, AllowUpstreamNameConflict= {5}", (object) key, (object) feed.Project?.Id, (object) feed.Id, (object) feed.Name, (object) feed.UpstreamEnabled, (object) feed.AllowUpstreamNameConflict);
      else
        message = string.Format("Feed added: Key= {0}, Project= {1}, Id= {2}, Name= {3}, UpstreamEnabled= {4}, AllowUpstreamNameConflict= {5}", (object) key, (object) feed.Project?.Id, (object) feed.Id, (object) feed.Name, (object) feed.UpstreamEnabled, (object) feed.AllowUpstreamNameConflict);
      requestContext1.Trace(10019059, TraceLevel.Info, "Feed", "CacheService", message);
      return feed;
    }

    public static RawFeedIdentity BuildKey(Guid projectId, string feedId) => new RawFeedIdentity(projectId == Guid.Empty ? new Guid?() : new Guid?(projectId), feedId);

    public FeedCore GetFeed(IVssRequestContext requestContext, Guid projectId, string feedNameOrId) => AsyncPump.Run<FeedCore>((Func<Task<FeedCore>>) (() => this.GetFeedAsync(requestContext, projectId, feedNameOrId)));

    public async Task<FeedCore> GetFeedByIdFromAnyScopeAsync(
      IVssRequestContext requestContext,
      Guid feedId,
      Func<FeedCore, bool> rejectCachedFeedIf)
    {
      FeedCacheService feedCacheService = this;
      FeedCore feed = (FeedCore) null;
      IEnumerable<RawFeedIdentity> keys;
      if (feedCacheService.feedGuidGrouping.TryGetKeys(feedId, out keys))
      {
        foreach (RawFeedIdentity key in keys)
        {
          FeedCore feedCore;
          if (feedCacheService.TryGetValue(requestContext, key, out feedCore) && feedCore.View == null)
          {
            if (rejectCachedFeedIf != null && rejectCachedFeedIf(feedCore))
            {
              requestContext.Trace(10019140, TraceLevel.Info, "Feed", "CacheService", "Rejecting cached feed because the caller can tell that it's out of date");
              feedCacheService.Remove(requestContext, key);
            }
            else
            {
              feed = feedCore;
              break;
            }
          }
        }
      }
      if (feed == null)
        feed = await feedCacheService.AddFeedToCacheFromServiceByIdFromAnyScopeAsync(requestContext, feedId);
      FeedSecurityHelper.CheckReadFeedPermissions(requestContext, feed);
      return feed;
    }

    public async Task<FeedCore> GetFeedAsync(
      IVssRequestContext requestContext,
      Guid projectId,
      string feedNameOrId)
    {
      FeedCore feed;
      if (!requestContext.IsFeatureEnabled("Packaging.Feed.CachingEnabled"))
        feed = await this.GetFeedFromFeedServiceAsync(requestContext, projectId, feedNameOrId);
      else if (!this.TryGetValueWrapper(requestContext, projectId, feedNameOrId, out feed))
        feed = await this.AddFeedToCacheFromServiceAsync(requestContext, projectId, feedNameOrId);
      FeedSecurityHelper.CheckReadFeedPermissions(requestContext, feed);
      return feed;
    }

    public void RemoveFeed(IVssRequestContext requestContext, Guid projectId, string feedNameOrId)
    {
      RawFeedIdentity key1 = FeedCacheService.BuildKey(projectId, feedNameOrId);
      Guid result;
      FeedCore feedCore;
      if (!Guid.TryParse(feedNameOrId, out result) && this.TryPeekValue(requestContext, key1, out feedCore))
        result = feedCore.Id;
      IEnumerable<RawFeedIdentity> keys;
      if (this.feedGuidGrouping.TryGetKeys(result, out keys))
      {
        foreach (RawFeedIdentity key2 in keys)
          this.Remove(requestContext, key2);
      }
      this.Remove(requestContext, key1);
    }

    public void RemoveFeedsIfProjectChanged(
      IVssRequestContext requestContext,
      ProjectInfo newProjectInfo)
    {
      ProjectReference projectReference = newProjectInfo.ToProjectReference();
      IEnumerable<RawFeedIdentity> keys;
      if (!this.projectGuidGrouping.TryGetKeys(projectReference.Id, out keys))
        return;
      foreach (RawFeedIdentity key in keys)
      {
        FeedCore feedCore;
        if (this.TryPeekValue(requestContext, key, out feedCore) && (feedCore.Project != projectReference || newProjectInfo.IsSoftDeleted))
          this.Remove(requestContext, key);
      }
    }

    protected override void ServiceStart(IVssRequestContext requestContext)
    {
      base.ServiceStart(requestContext);
      this.feedGuidGrouping = VssMemoryCacheGroupingFactory.Create<RawFeedIdentity, FeedCore, Guid>(requestContext, this.MemoryCache, (Func<RawFeedIdentity, FeedCore, IEnumerable<Guid>>) ((key, value) => (IEnumerable<Guid>) new Guid[1]
      {
        value.Id
      }));
      this.projectGuidGrouping = VssMemoryCacheGroupingFactory.Create<RawFeedIdentity, FeedCore, Guid>(requestContext, this.MemoryCache, (Func<RawFeedIdentity, FeedCore, IEnumerable<Guid>>) ((key, value) =>
      {
        Guid[] guidArray = new Guid[1];
        ProjectReference project = value.Project;
        guidArray[0] = (object) project != null ? project.Id : Guid.Empty;
        return (IEnumerable<Guid>) guidArray;
      }));
      ITeamFoundationSqlNotificationService service = requestContext.GetService<ITeamFoundationSqlNotificationService>();
      service.RegisterNotification(requestContext, "Default", FeedSqlNotificationEventClasses.CachedFeedNoLongerValid, new SqlNotificationHandler(this.OnCachedFeedNoLongerValid), false);
      service.RegisterNotification(requestContext, "Default", ProjectSqlNotificationEventClasses.ProjectUpdated, new SqlNotificationHandler(this.OnProjectChanged), false);
    }

    protected override void ServiceEnd(IVssRequestContext requestContext)
    {
      ITeamFoundationSqlNotificationService service = requestContext.GetService<ITeamFoundationSqlNotificationService>();
      service.UnregisterNotification(requestContext, "Default", FeedSqlNotificationEventClasses.CachedFeedNoLongerValid, new SqlNotificationHandler(this.OnCachedFeedNoLongerValid), false);
      service.UnregisterNotification(requestContext, "Default", ProjectSqlNotificationEventClasses.ProjectUpdated, new SqlNotificationHandler(this.OnProjectChanged), false);
      base.ServiceEnd(requestContext);
    }

    private void OnCachedFeedNoLongerValid(
      IVssRequestContext requestContext,
      NotificationEventArgs args)
    {
      FeedSqlNotificationMessage notificationMessage = JsonConvert.DeserializeObject<FeedSqlNotificationMessage>(args.Data);
      this.RemoveFeed(requestContext, notificationMessage.Feed.Project.ToProjectIdOrEmptyGuid(), notificationMessage.Feed.Id.ToString());
    }

    private void OnProjectChanged(IVssRequestContext requestContext, NotificationEventArgs args)
    {
      ProjectInfo newProjectInfo;
      if (!JsonUtilities.TryDeserialize<ProjectInfo>(args.Data, out newProjectInfo, true))
        requestContext.Trace(10019124, TraceLevel.Error, "Feed", "CacheService", "Invalid ProjectInfo: " + (args.Data ?? string.Empty));
      else
        this.RemoveFeedsIfProjectChanged(requestContext, newProjectInfo);
    }

    private bool TryGetValueWrapper(
      IVssRequestContext requestContext,
      Guid projectId,
      string feedId,
      out FeedCore feed)
    {
      RawFeedIdentity key = FeedCacheService.BuildKey(projectId, feedId);
      bool valueWrapper = this.TryGetValue(requestContext, key, out feed);
      IVssRequestContext requestContext1 = requestContext;
      string message;
      if (!valueWrapper)
        message = string.Format("Cache miss for feed: Key= {0}, Project= {1}, Id= {2}", (object) key, (object) projectId, (object) feedId);
      else
        message = string.Format("Cache hit: Key= {0}, Project= {1}, Id= {2}, Name= {3}, UpstreamEnabled= {4}, AllowUpstreamNameConflict= {5}", (object) key, (object) feed.Project?.Id, (object) feed.Id, (object) feed.Name, (object) feed.UpstreamEnabled, (object) feed.AllowUpstreamNameConflict);
      requestContext1.Trace(10019058, TraceLevel.Info, "Feed", "CacheService", message);
      return valueWrapper;
    }

    private async Task<FeedCore> AddFeedToCacheFromServiceAsync(
      IVssRequestContext requestContext,
      Guid projectId,
      string feedName)
    {
      FeedCore feedServiceAsync = await this.GetFeedFromFeedServiceAsync(requestContext, projectId, feedName);
      return this.AddFeed(requestContext, projectId, feedName, feedServiceAsync);
    }

    private async Task<FeedCore> AddFeedToCacheFromServiceByIdFromAnyScopeAsync(
      IVssRequestContext requestContext,
      Guid feedId)
    {
      FeedCacheService feedCacheService1 = this;
      FeedCore fromAnyScopeAsync = await feedCacheService1.GetFeedFromFeedServiceByIdFromAnyScopeAsync(requestContext, feedId);
      FeedCacheService feedCacheService2 = feedCacheService1;
      IVssRequestContext requestContext1 = requestContext;
      ProjectReference project = fromAnyScopeAsync.Project;
      Guid projectId = (object) project != null ? project.Id : Guid.Empty;
      string fullyQualifiedName = fromAnyScopeAsync.FullyQualifiedName;
      FeedCore feed = fromAnyScopeAsync;
      // ISSUE: explicit non-virtual call
      return __nonvirtual (feedCacheService2.AddFeed(requestContext1, projectId, fullyQualifiedName, feed));
    }

    private async Task<FeedCore> GetFeedFromFeedServiceAsync(
      IVssRequestContext requestContext,
      Guid projectId,
      string feedName)
    {
      requestContext.TraceEnter(10019145, "Feed", "CacheService", nameof (GetFeedFromFeedServiceAsync));
      FeedCore feedServiceAsync1;
      try
      {
        feedServiceAsync1 = await this.fetchFeedConsolidator.RunOnceAsync(FeedCacheService.BuildKey(projectId, feedName), (Func<Task<FeedCore>>) (async () =>
        {
          FeedCore feedServiceAsync2;
          try
          {
            Microsoft.VisualStudio.Services.Feed.WebApi.Feed feedAsync = await requestContext.GetService<IFeedClientService>().GetFeedAsync(requestContext, projectId.ToProjectReference(), feedName);
            if (projectId != feedAsync.Project.ToProjectIdOrEmptyGuid())
              throw FeedIdNotFoundException.Create(feedName);
            feedServiceAsync2 = (FeedCore) feedAsync;
          }
          catch (VssServiceResponseException ex) when (ex.HttpStatusCode == HttpStatusCode.Forbidden)
          {
            throw new UnauthorizedAccessException("GetFeed returned status code Forbidden", (Exception) ex);
          }
          return feedServiceAsync2;
        }));
      }
      finally
      {
        requestContext.TraceLeave(10019145, "Feed", "CacheService", nameof (GetFeedFromFeedServiceAsync));
      }
      return feedServiceAsync1;
    }

    private async Task<FeedCore> GetFeedFromFeedServiceByIdFromAnyScopeAsync(
      IVssRequestContext requestContext,
      Guid feedId)
    {
      requestContext.TraceEnter(10019145, "Feed", "CacheService", nameof (GetFeedFromFeedServiceByIdFromAnyScopeAsync));
      FeedCore fromAnyScopeAsync;
      try
      {
        fromAnyScopeAsync = await this.fetchFeedByIdConsolidator.RunOnceAsync(feedId, (Func<Task<FeedCore>>) (async () =>
        {
          FeedCore forAnyScopeAsync;
          try
          {
            forAnyScopeAsync = (FeedCore) await requestContext.GetService<IFeedInternalClientService>().GetFeedByIdForAnyScopeAsync(requestContext, feedId);
          }
          catch (VssServiceResponseException ex) when (ex.HttpStatusCode == HttpStatusCode.Forbidden)
          {
            throw new UnauthorizedAccessException("GetFeed returned status code Forbidden", (Exception) ex);
          }
          return forAnyScopeAsync;
        }));
      }
      finally
      {
        requestContext.TraceLeave(10019145, "Feed", "CacheService", nameof (GetFeedFromFeedServiceByIdFromAnyScopeAsync));
      }
      return fromAnyScopeAsync;
    }
  }
}
