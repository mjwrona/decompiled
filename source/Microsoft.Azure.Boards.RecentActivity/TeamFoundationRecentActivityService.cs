// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Boards.RecentActivity.TeamFoundationRecentActivityService
// Assembly: Microsoft.Azure.Boards.RecentActivity, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 684DCFA4-4764-4794-94A6-960AF811434C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Boards.RecentActivity.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.Azure.Boards.RecentActivity
{
  public class TeamFoundationRecentActivityService : 
    ITeamFoundationRecentActivityService,
    IVssFrameworkService
  {
    private IDisposableReadOnlyList<IRecentActivityArtifactProvider> m_artifactProviders;
    private const string s_area = "RecentActivity";
    private const string s_layer = "TeamFoundationRecentActivityService";
    private const string c_throttleTimeInMilliSecondsRegistryPath = "/Service/RecentActivity/Settings/ThrottleTimeInMilliSeconds";
    private const int c_defaultThrottleTimeInMilliSeconds = 500;
    private const string c_cleanupUserLastProcessedTime = "/Service/RecentActivity/Settings/CleanupUserLastProcessedTime/{0}";

    public void ServiceEnd(IVssRequestContext requestContext)
    {
      if (this.m_artifactProviders == null)
        return;
      this.m_artifactProviders.Dispose();
      this.m_artifactProviders = (IDisposableReadOnlyList<IRecentActivityArtifactProvider>) null;
    }

    public void ServiceStart(IVssRequestContext requestContext) => this.m_artifactProviders = requestContext.GetExtensions<IRecentActivityArtifactProvider>();

    public void UpdateActivities(
      IVssRequestContext requestContext,
      IReadOnlyCollection<Microsoft.Azure.Boards.RecentActivity.RecentActivity> activities)
    {
      if (requestContext.IsFeatureEnabled("TeamFoundationRecentActivityService.SkipUpdateActivities"))
        return;
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<IReadOnlyCollection<Microsoft.Azure.Boards.RecentActivity.RecentActivity>>(activities, nameof (activities));
      Microsoft.Azure.Boards.RecentActivity.RecentActivity firstActivity = (Microsoft.Azure.Boards.RecentActivity.RecentActivity) null;
      long queueWaitTimeInMS = 0;
      int userActivityCount = 0;
      int projectActivityCount = 0;
      requestContext.TraceBlock(15162000, 15162001, "RecentActivity", nameof (TeamFoundationRecentActivityService), nameof (UpdateActivities), (Action) (() =>
      {
        requestContext.CheckServiceHostType(TeamFoundationHostType.ProjectCollection);
        if (!activities.Any<Microsoft.Azure.Boards.RecentActivity.RecentActivity>())
          return;
        if (activities.Any<Microsoft.Azure.Boards.RecentActivity.RecentActivity>((Func<Microsoft.Azure.Boards.RecentActivity.RecentActivity, bool>) (act => !this.IsValid(act))))
          throw new InvalidRecentActivityException();
        if (activities.GroupBy(a => new
        {
          ArtifactKind = a.ArtifactKind,
          IdentityId = a.IdentityId
        }).Count<IGrouping<\u003C\u003Ef__AnonymousType0<Guid, Guid>, Microsoft.Azure.Boards.RecentActivity.RecentActivity>>() > 1)
          throw new MultipleArtifactKindNotAllowedException();
        IRecentActivityArtifactProvider artifactProvider = this.GetArtifactProvider(activities.First<Microsoft.Azure.Boards.RecentActivity.RecentActivity>().ArtifactKind);
        activities = artifactProvider.Filter(requestContext, activities);
        if (!activities.Any<Microsoft.Azure.Boards.RecentActivity.RecentActivity>() || this.ThrottleActivity(requestContext, activities))
          return;
        bool flag = true;
        Stopwatch stopwatch = new Stopwatch();
        CustomerIntelligenceData properties = new CustomerIntelligenceData();
        try
        {
          firstActivity = activities.First<Microsoft.Azure.Boards.RecentActivity.RecentActivity>();
          queueWaitTimeInMS = (long) (DateTime.UtcNow - firstActivity.ActivityDate).TotalMilliseconds;
          stopwatch.Start();
          RecentActivityRetentionPolicy retentionPolicy = artifactProvider.GetRetentionPolicy(requestContext);
          IReadOnlyCollection<Microsoft.Azure.Boards.RecentActivity.RecentActivity> list1 = (IReadOnlyCollection<Microsoft.Azure.Boards.RecentActivity.RecentActivity>) activities.Where<Microsoft.Azure.Boards.RecentActivity.RecentActivity>((Func<Microsoft.Azure.Boards.RecentActivity.RecentActivity, bool>) (a => (a.Scope & RecentActivityScope.User) == RecentActivityScope.User)).Take<Microsoft.Azure.Boards.RecentActivity.RecentActivity>(retentionPolicy.RetentionCountPerUser).ToList<Microsoft.Azure.Boards.RecentActivity.RecentActivity>();
          IReadOnlyCollection<Microsoft.Azure.Boards.RecentActivity.RecentActivity> list2 = (IReadOnlyCollection<Microsoft.Azure.Boards.RecentActivity.RecentActivity>) activities.Where<Microsoft.Azure.Boards.RecentActivity.RecentActivity>((Func<Microsoft.Azure.Boards.RecentActivity.RecentActivity, bool>) (a => (a.Scope & RecentActivityScope.Project) == RecentActivityScope.Project)).Take<Microsoft.Azure.Boards.RecentActivity.RecentActivity>(retentionPolicy.RetentionCountPerProject).ToList<Microsoft.Azure.Boards.RecentActivity.RecentActivity>();
          userActivityCount = list1.Count;
          projectActivityCount = list2.Count;
          using (TeamFoundationRecentActivityComponent component = requestContext.CreateComponent<TeamFoundationRecentActivityComponent>())
            component.UpdateRecentActivities(requestContext, list1, list2, retentionPolicy);
        }
        catch (Exception ex)
        {
          flag = false;
          properties.Add("ExceptionMessage", ex.Message);
          throw;
        }
        finally
        {
          stopwatch.Stop();
          if (firstActivity != null)
          {
            properties.Add("ArtifactKind", (object) firstActivity.ArtifactKind);
            properties.Add("UpdateSucceded", flag);
            properties.Add("FirstArtifactId", firstActivity.ArtifactId);
            properties.Add("IdentityId", (object) firstActivity.IdentityId);
            properties.Add("QueueWaitTimeInMS", (double) queueWaitTimeInMS);
            properties.Add("ElapsedTimeInMS", (double) stopwatch.ElapsedMilliseconds);
            properties.Add("ActivityCount", (double) activities.Count);
            properties.Add("UserActivityCount", (double) userActivityCount);
            properties.Add("ProjectActivityCount", (double) projectActivityCount);
            if (stopwatch.ElapsedMilliseconds >= 100L)
              properties.Add("Timing", requestContext.GetTraceTimingAsString());
            requestContext.GetService<CustomerIntelligenceService>().Publish(requestContext, "RecentActivity", nameof (UpdateActivities), properties);
          }
        }
      }));
    }

    public IReadOnlyDictionary<string, Microsoft.Azure.Boards.RecentActivity.RecentActivity> GetUserActivities(
      IVssRequestContext requestContext,
      Guid identityId,
      Guid artifactKind)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForEmptyGuid(identityId, nameof (identityId));
      ArgumentUtility.CheckForEmptyGuid(artifactKind, nameof (artifactKind));
      RecentActivityRetentionPolicy retentionPolicy = this.GetArtifactProvider(artifactKind).GetRetentionPolicy(requestContext);
      using (requestContext.TraceBlock(15162005, 15162006, "RecentActivity", nameof (TeamFoundationRecentActivityService), nameof (GetUserActivities)))
      {
        IReadOnlyCollection<Microsoft.Azure.Boards.RecentActivity.RecentActivity> userActivities;
        using (TeamFoundationRecentActivityComponent component = requestContext.CreateComponent<TeamFoundationRecentActivityComponent>())
          userActivities = component.GetUserActivities(requestContext, identityId, artifactKind, retentionPolicy.RetentionCountPerUser);
        return (IReadOnlyDictionary<string, Microsoft.Azure.Boards.RecentActivity.RecentActivity>) userActivities.ToDedupedDictionary<Microsoft.Azure.Boards.RecentActivity.RecentActivity, string, Microsoft.Azure.Boards.RecentActivity.RecentActivity>((Func<Microsoft.Azure.Boards.RecentActivity.RecentActivity, string>) (act => act.ArtifactId), (Func<Microsoft.Azure.Boards.RecentActivity.RecentActivity, Microsoft.Azure.Boards.RecentActivity.RecentActivity>) (act => act), (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      }
    }

    public IReadOnlyDictionary<string, Microsoft.Azure.Boards.RecentActivity.RecentActivity> GetProjectActivities(
      IVssRequestContext requestContext,
      Guid projectId,
      Guid artifactKind,
      int limit)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForEmptyGuid(projectId, nameof (projectId));
      ArgumentUtility.CheckForEmptyGuid(artifactKind, nameof (artifactKind));
      ArgumentUtility.CheckForNonPositiveInt(limit, nameof (limit));
      using (requestContext.TraceBlock(15162007, 15162008, "RecentActivity", nameof (TeamFoundationRecentActivityService), nameof (GetProjectActivities)))
      {
        limit = this.GetMinRetentionCount(requestContext, artifactKind, limit);
        IReadOnlyCollection<Microsoft.Azure.Boards.RecentActivity.RecentActivity> projectActivities;
        using (TeamFoundationRecentActivityComponent component = requestContext.CreateComponent<TeamFoundationRecentActivityComponent>())
          projectActivities = component.GetProjectActivities(requestContext, projectId, artifactKind, limit);
        return (IReadOnlyDictionary<string, Microsoft.Azure.Boards.RecentActivity.RecentActivity>) projectActivities.ToDedupedDictionary<Microsoft.Azure.Boards.RecentActivity.RecentActivity, string, Microsoft.Azure.Boards.RecentActivity.RecentActivity>((Func<Microsoft.Azure.Boards.RecentActivity.RecentActivity, string>) (act => act.ArtifactId), (Func<Microsoft.Azure.Boards.RecentActivity.RecentActivity, Microsoft.Azure.Boards.RecentActivity.RecentActivity>) (act => act), (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      }
    }

    public IReadOnlyList<Microsoft.Azure.Boards.RecentActivity.RecentActivity> GetProjectUserActivities(
      IVssRequestContext requestContext,
      Guid projectId,
      Guid identityId,
      IEnumerable<Guid> artifactKindIds)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForEmptyGuid(identityId, nameof (identityId));
      int maxActivitiesPerUser = requestContext.GetService<IVssRegistryService>().GetValue<int>(requestContext, (RegistryQuery) "/Service/WorkRecentActivity/Settings/BoardsRetentionCount", 5);
      using (requestContext.TraceBlock(15162005, 15162006, "RecentActivity", nameof (TeamFoundationRecentActivityService), "GetUserActivities"))
      {
        IReadOnlyCollection<Microsoft.Azure.Boards.RecentActivity.RecentActivity> recentActivities;
        using (TeamFoundationProjectUserRecentActivityComponent component = requestContext.CreateComponent<TeamFoundationProjectUserRecentActivityComponent>())
          recentActivities = component.GetProjectUserRecentActivities(requestContext, projectId, identityId, maxActivitiesPerUser, artifactKindIds);
        return (IReadOnlyList<Microsoft.Azure.Boards.RecentActivity.RecentActivity>) recentActivities.ToList<Microsoft.Azure.Boards.RecentActivity.RecentActivity>();
      }
    }

    public int CleanupProjectActivities(IVssRequestContext requestContext, Guid artifactKind)
    {
      ArgumentUtility.CheckForEmptyGuid(artifactKind, nameof (artifactKind));
      using (requestContext.TraceBlock(15162009, 15162010, "RecentActivity", nameof (TeamFoundationRecentActivityService), nameof (CleanupProjectActivities)))
      {
        RecentActivityRetentionPolicy retentionPolicy = this.GetArtifactProvider(artifactKind).GetRetentionPolicy(requestContext);
        using (TeamFoundationRecentActivityComponent component = requestContext.CreateComponent<TeamFoundationRecentActivityComponent>())
          return component.CleanupProjectActivities(requestContext, artifactKind, retentionPolicy.RetentionCountPerProject);
      }
    }

    public void CleanupRecentUserActivities(IVssRequestContext requestContext)
    {
      IVssRegistryService service = requestContext.GetService<IVssRegistryService>();
      int maxActivitiesPerUser = service.GetValue<int>(requestContext, (RegistryQuery) "/Service/WorkRecentActivity/Settings/BoardsRetentionCount", 5) * 2;
      int maxDaysPerActivity = service.GetValue<int>(requestContext, (RegistryQuery) "/Service/WorkRecentActivity/Settings/MaxNumberOfDaysPerActivity", 30);
      foreach (IRecentActivityArtifactProvider artifactProvider in (IEnumerable<IRecentActivityArtifactProvider>) this.m_artifactProviders)
      {
        using (requestContext.TraceBlock(15162012, 15162013, "RecentActivity", nameof (TeamFoundationRecentActivityService), nameof (CleanupRecentUserActivities)))
        {
          using (TeamFoundationProjectUserRecentActivityComponent component = requestContext.CreateComponent<TeamFoundationProjectUserRecentActivityComponent>())
            component.CleanupRecentProjectUserActivity(requestContext, maxActivitiesPerUser, maxDaysPerActivity);
        }
      }
    }

    public void CleanupRecentProjectUserActivities(IVssRequestContext requestContext)
    {
      IVssRegistryService service = requestContext.GetService<IVssRegistryService>();
      foreach (IRecentActivityArtifactProvider artifactProvider in (IEnumerable<IRecentActivityArtifactProvider>) this.m_artifactProviders)
      {
        using (requestContext.TraceBlock(15162017, 15162018, "RecentActivity", nameof (TeamFoundationRecentActivityService), nameof (CleanupRecentProjectUserActivities)))
        {
          RecentActivityRetentionPolicy retentionPolicy = artifactProvider.GetRetentionPolicy(requestContext);
          string str = string.Format("/Service/RecentActivity/Settings/CleanupUserLastProcessedTime/{0}", (object) artifactProvider.ArtifactKind);
          long lastprocessedTime = service.GetValue<long>(requestContext, new RegistryQuery(str, false), false, 0L);
          long num = 0;
          using (TeamFoundationRecentActivityComponent component = requestContext.CreateComponent<TeamFoundationRecentActivityComponent>())
            num = component.CleanupRecentUserActivity(requestContext, artifactProvider.ArtifactKind, retentionPolicy.RetentionCountPerUser, lastprocessedTime);
          service.SetValue<long>(requestContext, str, num);
        }
      }
    }

    public void UpdateProjectUserActivities(
      IVssRequestContext requestContext,
      Guid projectId,
      Guid identityId,
      DateTime activityDate,
      Guid artifactKind,
      string artifactId,
      IDictionary<string, string> activityDetails)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForEmptyGuid(projectId, nameof (projectId));
      ArgumentUtility.CheckForEmptyGuid(identityId, nameof (identityId));
      ArgumentUtility.CheckForNull<string>(artifactId, nameof (artifactId));
      ArgumentUtility.CheckForNull<IDictionary<string, string>>(activityDetails, nameof (activityDetails));
      long queueWaitTimeInMS = 0;
      requestContext.TraceBlock(15162015, 15162016, "RecentActivity", nameof (TeamFoundationRecentActivityService), nameof (UpdateProjectUserActivities), (Action) (() =>
      {
        requestContext.CheckServiceHostType(TeamFoundationHostType.ProjectCollection);
        Stopwatch stopwatch = new Stopwatch();
        CustomerIntelligenceData properties = new CustomerIntelligenceData();
        try
        {
          queueWaitTimeInMS = (long) (DateTime.UtcNow - activityDate).TotalMilliseconds;
          queueWaitTimeInMS = queueWaitTimeInMS < 0L ? 0L : queueWaitTimeInMS;
          stopwatch.Start();
          using (TeamFoundationProjectUserRecentActivityComponent component = requestContext.CreateComponent<TeamFoundationProjectUserRecentActivityComponent>())
            component.UpdateProjectUserRecentActivities(requestContext, projectId, identityId, activityDate, artifactKind, artifactId, activityDetails);
        }
        catch (Exception ex)
        {
          properties.Add("ExceptionMessage", ex.Message);
          throw;
        }
        finally
        {
          stopwatch.Stop();
          properties.Add("ArtifactKind", (object) artifactKind);
          properties.Add("ArtifactId", artifactId);
          properties.Add("IdentityId", (object) identityId);
          properties.Add("QueueWaitTimeInMS", (double) queueWaitTimeInMS);
          requestContext.GetService<CustomerIntelligenceService>().Publish(requestContext, "RecentActivity", "UpdateActivities", properties);
        }
      }));
    }

    private int GetMinRetentionCount(
      IVssRequestContext requestContext,
      Guid artifactKind,
      int limit)
    {
      RecentActivityRetentionPolicy retentionPolicy = this.GetArtifactProvider(artifactKind).GetRetentionPolicy(requestContext);
      limit = Math.Min(limit, retentionPolicy.RetentionCountPerProject);
      return limit;
    }

    private bool ThrottleActivity(
      IVssRequestContext requestContext,
      IReadOnlyCollection<Microsoft.Azure.Boards.RecentActivity.RecentActivity> activities)
    {
      if (!activities.Any<Microsoft.Azure.Boards.RecentActivity.RecentActivity>())
        return false;
      TeamFoundationRecentActivityService.RecentActivityUserCacheService activityUserCache = this.GetRecentActivityUserCache(requestContext);
      Guid identityId = activities.First<Microsoft.Azure.Boards.RecentActivity.RecentActivity>().IdentityId;
      DateTime activityDate = activities.First<Microsoft.Azure.Boards.RecentActivity.RecentActivity>().ActivityDate;
      DateTime lastActivity;
      if (activityUserCache.TryGetUserActivityDate(requestContext, identityId, out lastActivity))
      {
        int num = requestContext.GetService<IVssRegistryService>().GetValue<int>(requestContext, (RegistryQuery) "/Service/RecentActivity/Settings/ThrottleTimeInMilliSeconds", 500);
        if ((activityDate - lastActivity).Milliseconds < num)
        {
          requestContext.Trace(15162002, TraceLevel.Info, "RecentActivity", nameof (TeamFoundationRecentActivityService), string.Format("Throttled activity userId: {0}, activityDate: {1}, lastKnownActivityDate: {2}, throttleTimeInMilliSeconds: {3}", (object) identityId, (object) activityDate, (object) lastActivity, (object) num));
          return true;
        }
      }
      activityUserCache.SetUserActivity(requestContext, identityId, activityDate);
      return false;
    }

    internal virtual TeamFoundationRecentActivityService.RecentActivityUserCacheService GetRecentActivityUserCache(
      IVssRequestContext requestContext)
    {
      return requestContext.GetService<TeamFoundationRecentActivityService.RecentActivityUserCacheService>();
    }

    internal virtual IRecentActivityArtifactProvider GetArtifactProvider(Guid artifactKind)
    {
      IRecentActivityArtifactProvider artifactProvider = this.m_artifactProviders.Where<IRecentActivityArtifactProvider>((Func<IRecentActivityArtifactProvider, bool>) (p => p.ArtifactKind == artifactKind)).FirstOrDefault<IRecentActivityArtifactProvider>();
      if (artifactProvider != null)
        return artifactProvider;
      string allProviders = string.Join<Guid>(" ,", this.m_artifactProviders.Select<IRecentActivityArtifactProvider, Guid>((Func<IRecentActivityArtifactProvider, Guid>) (p => p.ArtifactKind)));
      throw new RecentActivityProviderNotFoundException(artifactKind, allProviders);
    }

    private bool IsValid(Microsoft.Azure.Boards.RecentActivity.RecentActivity activity) => activity.ArtifactKind != Guid.Empty && !string.IsNullOrWhiteSpace(activity.ArtifactId) && activity.IdentityId != Guid.Empty;

    internal class RecentActivityUserCacheService : VssMemoryCacheService<Guid, DateTime>
    {
      private static readonly TimeSpan s_inactivityInterval = TimeSpan.FromMilliseconds(500.0);
      private static readonly TimeSpan s_cleanupInterval = TimeSpan.FromMinutes(10.0);

      public RecentActivityUserCacheService()
        : base((IEqualityComparer<Guid>) EqualityComparer<Guid>.Default, TeamFoundationRecentActivityService.RecentActivityUserCacheService.s_cleanupInterval)
      {
        this.InactivityInterval.Value = TeamFoundationRecentActivityService.RecentActivityUserCacheService.s_inactivityInterval;
      }

      public bool TryGetUserActivityDate(
        IVssRequestContext requestContext,
        Guid userId,
        out DateTime lastActivity)
      {
        return this.TryGetValue(requestContext, userId, out lastActivity);
      }

      public void SetUserActivity(
        IVssRequestContext requestContext,
        Guid userId,
        DateTime lastActivity)
      {
        this.Set(requestContext, userId, lastActivity);
      }
    }
  }
}
