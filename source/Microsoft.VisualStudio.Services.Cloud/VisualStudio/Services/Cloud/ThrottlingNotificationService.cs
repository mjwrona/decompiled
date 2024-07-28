// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.ThrottlingNotificationService
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;

namespace Microsoft.VisualStudio.Services.Cloud
{
  internal class ThrottlingNotificationService : IThrottlingNotificationService, IVssFrameworkService
  {
    private ConcurrentDictionary<UserIdAndNamespacePair, ThrottleInfo> m_recentThrottleEvents = new ConcurrentDictionary<UserIdAndNamespacePair, ThrottleInfo>();
    private static readonly TimeSpan CacheCleanupTime = TimeSpan.FromMinutes(60.0);
    private const string c_RegistrySettingsRoot = "/Service/ThrottlingNotification/Settings/";
    private static readonly RegistryQuery s_DefaultEmailCooldownRegistryQuery = new RegistryQuery("/Service/ThrottlingNotification/Settings/DefaultEmailCooldown");
    private static readonly RegistryQuery s_EmailHistoryPurgeThresholdRegistryQuery = new RegistryQuery("/Service/ThrottlingNotification/Settings/EmailHistoryPurgeThreshold");
    private const string c_area = "ThrottlingNotificationService";
    private const string c_layer = "Service";

    public void ServiceStart(IVssRequestContext requestContext)
    {
    }

    public void ServiceEnd(IVssRequestContext requestContext)
    {
    }

    public int CleanOldEmailHistory(IVssRequestContext requestContext)
    {
      TimeSpan purgeThreshold = requestContext.GetService<IVssRegistryService>().GetValue<TimeSpan>(requestContext, in ThrottlingNotificationService.s_EmailHistoryPurgeThresholdRegistryQuery, true, TimeSpan.FromDays(7.0));
      using (ThrottlingNotificationEmailHistoryComponent component = requestContext.CreateComponent<ThrottlingNotificationEmailHistoryComponent>())
        return component.CleanOldEmailHistory(purgeThreshold);
    }

    public void QueueThrottlingEventForPotentialEmailNotification(
      IVssRequestContext requestContext,
      ThrottleInfo throttleInfo)
    {
      Guid userId = requestContext.GetUserId();
      if (userId != Guid.Empty && !ServicePrincipals.IsServicePrincipal(requestContext, requestContext.UserContext) && !requestContext.IsRootContextAnonymous() && !requestContext.IsPipelineIdentity())
      {
        if (this.m_recentThrottleEvents.TryAdd(new UserIdAndNamespacePair(userId, throttleInfo.ThrottleType), throttleInfo))
        {
          requestContext.To(TeamFoundationHostType.Deployment).GetService<ITeamFoundationTaskService>().AddTask(requestContext, new TeamFoundationTask(new TeamFoundationTaskCallback(this.ProcessThrottlingEventsForPotentialEmailNotification), (object) null, DateTime.UtcNow.Add(ThrottlingNotificationService.EmailDelay), 0));
          requestContext.Trace(522304001, TraceLevel.Info, nameof (ThrottlingNotificationService), "Service", "Added throttle event for request [{0}] to m_RecentThrottleEvents for user {1}, throttle type {2}", (object) requestContext.ActivityId, (object) userId, (object) throttleInfo.ThrottleType);
        }
        else
          requestContext.Trace(522304002, TraceLevel.Info, nameof (ThrottlingNotificationService), "Service", "Throttle event for request [{0}] was already in m_RecentThrottleEvents for user {1}, throttle type {2}", (object) requestContext.ActivityId, (object) userId, (object) throttleInfo.ThrottleType);
      }
      else
        requestContext.Trace(522304000, TraceLevel.Info, nameof (ThrottlingNotificationService), "Service", "Identity of request [{0}] is Guid.Empty or service principal/build/anonymous for user {1} - not sending email.", (object) requestContext.ActivityId, (object) userId);
    }

    internal void ProcessThrottlingEventsForPotentialEmailNotification(
      IVssRequestContext requestContext,
      object taskArgs)
    {
      ConcurrentDictionary<UserIdAndNamespacePair, ThrottleInfo> recentThrottleEvents = this.m_recentThrottleEvents;
      this.m_recentThrottleEvents = new ConcurrentDictionary<UserIdAndNamespacePair, ThrottleInfo>();
      if (recentThrottleEvents.Count <= 0)
        return;
      List<UserIdAndNamespacePair> listOfUsersToPollDatabase = new List<UserIdAndNamespacePair>();
      ThrottlingNotificationService.RecentEmailHistoryCacheService service = requestContext.GetService<ThrottlingNotificationService.RecentEmailHistoryCacheService>();
      foreach (KeyValuePair<UserIdAndNamespacePair, ThrottleInfo> keyValuePair in recentThrottleEvents)
      {
        if (!service.TryGetValue(requestContext, keyValuePair.Key, out byte _))
          listOfUsersToPollDatabase.Add(keyValuePair.Key);
      }
      if (listOfUsersToPollDatabase.Count <= 0)
        return;
      TimeSpan defaultCooldown = requestContext.GetService<IVssRegistryService>().GetValue<TimeSpan>(requestContext, in ThrottlingNotificationService.s_DefaultEmailCooldownRegistryQuery, true, TimeSpan.FromHours(24.0));
      List<UserEmailStatus> userEmailStatuses = this.CheckAndUpdateRecentEmailHistory(requestContext, listOfUsersToPollDatabase, defaultCooldown);
      this.UpdateCacheAndQueueEmails(requestContext, service, recentThrottleEvents, defaultCooldown, userEmailStatuses);
    }

    internal void UpdateCacheAndQueueEmails(
      IVssRequestContext requestContext,
      ThrottlingNotificationService.RecentEmailHistoryCacheService cacheService,
      ConcurrentDictionary<UserIdAndNamespacePair, ThrottleInfo> recentThrottleEventsCopy,
      TimeSpan defaultCooldown,
      List<UserEmailStatus> userEmailStatuses)
    {
      for (int index = 0; index < userEmailStatuses.Count; ++index)
      {
        UserEmailStatus userEmailStatuse = userEmailStatuses[index];
        if (userEmailStatuse.ShouldEmail)
          this.QueueThrottlingNotificationEmail(requestContext, userEmailStatuse.UserAndNamespace, recentThrottleEventsCopy[userEmailStatuse.UserAndNamespace]);
        cacheService.SetWithIndividualElementExpiration(requestContext, userEmailStatuse.UserAndNamespace, (byte) 0, userEmailStatuse.LastEmailTime.Add(defaultCooldown).Subtract(DateTime.UtcNow));
      }
    }

    internal virtual List<UserEmailStatus> CheckAndUpdateRecentEmailHistory(
      IVssRequestContext requestContext,
      List<UserIdAndNamespacePair> listOfUsersToPollDatabase,
      TimeSpan defaultCooldown)
    {
      using (ThrottlingNotificationEmailHistoryComponent component = requestContext.CreateComponent<ThrottlingNotificationEmailHistoryComponent>())
        return component.CheckAndUpdateRecentEmailHistory(listOfUsersToPollDatabase, defaultCooldown);
    }

    internal virtual void QueueThrottlingNotificationEmail(
      IVssRequestContext requestContext,
      UserIdAndNamespacePair userAndNamespace,
      ThrottleInfo cachedSummary)
    {
      string ruleName = cachedSummary.Rule.RuleName;
      requestContext.TraceAlways(522304004, TraceLevel.Info, nameof (ThrottlingNotificationService), "Service", string.Format("Sending throttling notification email to UserId={0}; rule={1}; time={2}; namespace={3}; key={4}; throttleType={5}; window={6}", (object) userAndNamespace.UserId, (object) ruleName, (object) cachedSummary.TimeOfThrottleEvent, (object) cachedSummary.Namespace, (object) cachedSummary.Key, (object) cachedSummary.ThrottleType, (object) cachedSummary.WindowSeconds));
      requestContext.GetService<ITeamFoundationEventService>().SyncPublishNotification(requestContext, (object) cachedSummary);
    }

    internal static TimeSpan EmailDelay { get; set; } = TimeSpan.FromSeconds(15.0);

    internal class RecentEmailHistoryCacheService : 
      VssMemoryCacheService<UserIdAndNamespacePair, byte>
    {
      public RecentEmailHistoryCacheService()
        : base(ThrottlingNotificationService.CacheCleanupTime)
      {
      }

      public virtual void SetWithIndividualElementExpiration(
        IVssRequestContext requestContext,
        UserIdAndNamespacePair key,
        byte value,
        TimeSpan expirationTime)
      {
        this.MemoryCache.Add(key, value, true, new VssCacheExpiryProvider<UserIdAndNamespacePair, byte>(Capture.Create<TimeSpan>(expirationTime), Capture.Create<TimeSpan>(VssCacheExpiryProvider.NoExpiry)));
      }
    }
  }
}
