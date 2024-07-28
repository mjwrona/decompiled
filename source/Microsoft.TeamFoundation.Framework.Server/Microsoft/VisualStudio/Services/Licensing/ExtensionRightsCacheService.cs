// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Licensing.ExtensionRightsCacheService
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Commerce;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Licensing
{
  internal class ExtensionRightsCacheService : IExtensionRightsCacheService, IVssFrameworkService
  {
    private static readonly string s_EnableL1CacheFeatureFlag = "VisualStudio.Services.ExtensionRights.L1Cache";
    private const string s_area = "ExtensionRightsCacheService";
    private const string s_layer = "ExtensionRightsCacheService";
    private INotificationRegistration m_extensionEntitlementRegistration;
    private INotificationRegistration m_offerSubscriptionTrialRegistration;

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(systemRequestContext, nameof (systemRequestContext));
      systemRequestContext.CheckProjectCollectionRequestContext();
      ITeamFoundationSqlNotificationService service = systemRequestContext.GetService<ITeamFoundationSqlNotificationService>();
      this.m_extensionEntitlementRegistration = service.CreateRegistration(systemRequestContext, "Default", SqlNotificationEventClasses.ExtensionEntitlementChanged, new SqlNotificationHandler(this.OnExtensionEntitlementChanged), false, true);
      this.m_offerSubscriptionTrialRegistration = service.CreateRegistration(systemRequestContext, "Default", SqlNotificationEventClasses.OfferSubscriptionTrialStatusChanged, new SqlNotificationHandler(this.OnExtensionTrialStatusChange), false, true);
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(systemRequestContext, nameof (systemRequestContext));
      this.m_extensionEntitlementRegistration.Unregister(systemRequestContext);
      this.m_offerSubscriptionTrialRegistration.Unregister(systemRequestContext);
    }

    public ExtensionRightsResult GetOrSetExtensionRights(
      IVssRequestContext requestContext,
      ExtensionRightsCacheKey key,
      Func<IVssRequestContext, IEnumerable<string>, ExtensionRightsResult> evaluateExtensionRights,
      IEnumerable<string> extensionIds)
    {
      requestContext.TraceEnter(1039000, nameof (ExtensionRightsCacheService), nameof (ExtensionRightsCacheService), nameof (GetOrSetExtensionRights));
      requestContext.CheckProjectCollectionRequestContext();
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<ExtensionRightsCacheKey>(key, nameof (key));
      ArgumentUtility.CheckForNull<Func<IVssRequestContext, IEnumerable<string>, ExtensionRightsResult>>(evaluateExtensionRights, nameof (evaluateExtensionRights));
      bool flag = false;
      try
      {
        ExtensionRightsResult extensionRights1 = (ExtensionRightsResult) null;
        IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Application);
        IExtensionRightsMemoryCacheService service = vssRequestContext.GetService<IExtensionRightsMemoryCacheService>();
        if (ExtensionRightsCacheService.IsL1CacheEnabled(requestContext))
        {
          if (ExtensionRightsCacheService.TryGetValueFromCache(vssRequestContext, (IExtensionRightsCache) service, key, out extensionRights1))
            return extensionRights1;
          flag = true;
        }
        else
          requestContext.Trace(1039001, TraceLevel.Info, nameof (ExtensionRightsCacheService), nameof (ExtensionRightsCacheService), "L1 Cache disabled. Evaluating Extension Rights at source");
        if (!requestContext.IsSystemContext)
        {
          requestContext.Trace(1039003, TraceLevel.Info, nameof (ExtensionRightsCacheService), nameof (ExtensionRightsCacheService), "L1 Cache miss for key: " + key.ToString() + ". Evaluating Extension Rights at source");
          ExtensionRightsResult extensionRights2 = evaluateExtensionRights(requestContext, extensionIds);
          ExtensionRightsCacheService.SetCacheValue(vssRequestContext, key, service, extensionRights2);
          return extensionRights2;
        }
        requestContext.Trace(1039005, TraceLevel.Info, nameof (ExtensionRightsCacheService), nameof (ExtensionRightsCacheService), "Not saving the extension rights in the cache as the context was system context for key " + key.ToString());
        throw new NotSupportedException("Extension Rights cannot be called with elevated context");
      }
      catch (NotSupportedException ex)
      {
        throw;
      }
      catch (Exception ex)
      {
        requestContext.TraceException(1039008, nameof (ExtensionRightsCacheService), nameof (ExtensionRightsCacheService), ex);
        if (!flag)
          return evaluateExtensionRights(requestContext, extensionIds);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(1039009, nameof (ExtensionRightsCacheService), nameof (ExtensionRightsCacheService), nameof (GetOrSetExtensionRights));
      }
    }

    private ConcurrentDictionary<string, bool> FallbackExtensionRights(
      IVssRequestContext requestContext,
      IEnumerable<string> extensionIds,
      bool defaultRight = true,
      TraceLevel traceLevel = TraceLevel.Info)
    {
      requestContext.Trace(1039006, traceLevel, nameof (ExtensionRightsCacheService), nameof (ExtensionRightsCacheService), string.Format("Fallback of ExtensionRights for AccountId: {0}, UserId: {1}, ExtensionIds: {2}, rights {3}", (object) requestContext.ServiceHost.InstanceId, (object) requestContext.GetUserId(), (object) string.Join(",", extensionIds), (object) defaultRight.ToString()));
      ConcurrentDictionary<string, bool> concurrentDictionary = new ConcurrentDictionary<string, bool>();
      foreach (string extensionId in extensionIds)
        concurrentDictionary.TryAdd(extensionId, defaultRight);
      return concurrentDictionary;
    }

    private void OnExtensionEntitlementChanged(
      IVssRequestContext requestContext,
      NotificationEventArgs args)
    {
      try
      {
        Guid[] userIds = TeamFoundationSerializationUtility.Deserialize<EntitlementChangeMessage>(args.Data).UserIds;
        if (userIds == null || !((IEnumerable<Guid>) userIds).Any<Guid>())
          return;
        requestContext.Trace(1039030, TraceLevel.Info, nameof (ExtensionRightsCacheService), nameof (ExtensionRightsCacheService), string.Format("Removing cache for user: {0}", (object) string.Join<Guid>(",", (IEnumerable<Guid>) userIds)));
        this.InvalidateL1Cache(requestContext, (IList<Guid>) userIds);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(1039039, nameof (ExtensionRightsCacheService), nameof (ExtensionRightsCacheService), ex);
      }
    }

    private void OnExtensionTrialStatusChange(
      IVssRequestContext requestContext,
      NotificationEventArgs args)
    {
      try
      {
        OfferSubscriptionTrialStatusChangeMessage trialStatusChangeMessage = TeamFoundationSerializationUtility.Deserialize<OfferSubscriptionTrialStatusChangeMessage>(args.Data);
        this.InvalidateAllUsersInHost(requestContext, trialStatusChangeMessage);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(1039041, nameof (ExtensionRightsCacheService), nameof (ExtensionRightsCacheService), ex);
      }
    }

    private void InvalidateAllUsersInHost(
      IVssRequestContext requestContext,
      OfferSubscriptionTrialStatusChangeMessage trialStatusChangeMessage)
    {
      requestContext.Trace(1039033, TraceLevel.Info, nameof (ExtensionRightsCacheService), nameof (ExtensionRightsCacheService), string.Format("Invalidate L1 cache called for account {0}, Extension {1}.", (object) trialStatusChangeMessage.TargetHostId, (object) trialStatusChangeMessage.GalleryId));
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Application);
      vssRequestContext.GetService<IExtensionRightsMemoryCacheService>().InvalidateAllUsersInHost(vssRequestContext, trialStatusChangeMessage.TargetHostId, trialStatusChangeMessage.GalleryId);
    }

    public void InvalidateL1Cache(IVssRequestContext requestContext, Guid userId)
    {
      requestContext.Trace(1039030, TraceLevel.Info, nameof (ExtensionRightsCacheService), nameof (ExtensionRightsCacheService), "Invalidate L1 cache called for account {0}, user {1}.", (object) requestContext.ServiceHost.InstanceId, (object) userId);
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Application);
      IExtensionRightsMemoryCacheService service = vssRequestContext.GetService<IExtensionRightsMemoryCacheService>();
      service.Remove(vssRequestContext, new ExtensionRightsCacheKey(requestContext.ServiceHost.InstanceId, userId, false));
      service.Remove(vssRequestContext, new ExtensionRightsCacheKey(requestContext.ServiceHost.InstanceId, userId, true));
    }

    public void InvalidateL1Cache(IVssRequestContext requestContext, IList<Guid> userIds)
    {
      requestContext.Trace(1039040, TraceLevel.Info, nameof (ExtensionRightsCacheService), nameof (ExtensionRightsCacheService), "Invalidate L1 cache called for account {0} multiple users.", (object) requestContext.ServiceHost.InstanceId);
      foreach (Guid userId in (IEnumerable<Guid>) userIds)
        this.InvalidateL1Cache(requestContext, userId);
    }

    public void InvalidateCacheAll(
      IVssRequestContext requestContext,
      IList<Guid> userIds,
      bool sendMessageBusMessage = true)
    {
      requestContext.TraceEnter(1039070, nameof (ExtensionRightsCacheService), nameof (ExtensionRightsCacheService), nameof (InvalidateCacheAll));
      try
      {
        string str = string.Join<Guid>(",", (IEnumerable<Guid>) userIds);
        EntitlementChangeMessage entitlementChangeMessage = new EntitlementChangeMessage();
        entitlementChangeMessage.EntitlementChangeType = EntitlementChangeType.ExtensionRights;
        entitlementChangeMessage.AccountId = requestContext.ServiceHost.InstanceId;
        entitlementChangeMessage.UserIds = userIds.ToArray<Guid>();
        requestContext.Trace(1039071, TraceLevel.Verbose, nameof (ExtensionRightsCacheService), nameof (ExtensionRightsCacheService), "Sending sql notification for account {0}, user ids {1}", (object) requestContext.ServiceHost.InstanceId, (object) str);
        requestContext.GetService<ITeamFoundationSqlNotificationService>().SendNotification(requestContext, SqlNotificationEventClasses.ExtensionEntitlementChanged, TeamFoundationSerializationUtility.SerializeToString<EntitlementChangeMessage>(entitlementChangeMessage));
        requestContext.Trace(1039074, TraceLevel.Verbose, nameof (ExtensionRightsCacheService), nameof (ExtensionRightsCacheService), "Sending sql notification at deployment level for user ids {0}", (object) str);
        IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
        vssRequestContext.GetService<ITeamFoundationSqlNotificationService>().SendNotification(vssRequestContext, SqlNotificationEventClasses.ExtensionEntitlementChangedForPlatformCache, TeamFoundationSerializationUtility.SerializeToString<EntitlementChangeMessage>(entitlementChangeMessage));
        if (!sendMessageBusMessage)
          return;
        requestContext.Trace(1039073, TraceLevel.Verbose, nameof (ExtensionRightsCacheService), nameof (ExtensionRightsCacheService), "Sending message bus message for extension rights changed for account {0}, user ids {1}", (object) requestContext.ServiceHost.InstanceId, (object) str);
        EntitlementChangeNotifier.Publish(requestContext, entitlementChangeMessage, EntitlementChangePublisherType.ServiceBus);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(1039078, nameof (ExtensionRightsCacheService), nameof (ExtensionRightsCacheService), ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(1039079, nameof (ExtensionRightsCacheService), nameof (ExtensionRightsCacheService), nameof (InvalidateCacheAll));
      }
    }

    private static bool TryGetValueFromCache(
      IVssRequestContext requestContext,
      IExtensionRightsCache cache,
      ExtensionRightsCacheKey key,
      out ExtensionRightsResult extensionRights)
    {
      try
      {
        requestContext.Trace(1039010, TraceLevel.Info, nameof (ExtensionRightsCacheService), nameof (ExtensionRightsCacheService), string.Format("Looking up cache entries for key: {0} in CacheType: {1}", (object) key.ToString(), (object) cache.GetType().Name));
        string name = cache.GetType().Name;
        extensionRights = (ExtensionRightsResult) null;
        if (cache.TryGetValue(requestContext, key, out extensionRights))
        {
          requestContext.Trace(1039012, TraceLevel.Info, nameof (ExtensionRightsCacheService), nameof (ExtensionRightsCacheService), string.Format("Cache Hit for Key: {0} in CacheType: {1}, Value = [{2}]", (object) key.ToString(), (object) name, (object) string.Join(",", (object) extensionRights)));
          return true;
        }
        requestContext.Trace(1039013, TraceLevel.Info, nameof (ExtensionRightsCacheService), nameof (ExtensionRightsCacheService), string.Format("Cache Miss for Key: {0} in CacheType: {1}", (object) key.ToString(), (object) name));
        return false;
      }
      catch (Exception ex)
      {
        requestContext.TraceException(1039018, nameof (ExtensionRightsCacheService), nameof (ExtensionRightsCacheService), ex);
        extensionRights = (ExtensionRightsResult) null;
        return false;
      }
    }

    private static void SetCacheValue(
      IVssRequestContext requestContext,
      ExtensionRightsCacheKey key,
      IExtensionRightsMemoryCacheService extensionRightsMemoryCache,
      ExtensionRightsResult extensionRights)
    {
      try
      {
        if (extensionRightsMemoryCache == null)
          return;
        requestContext.Trace(1039020, TraceLevel.Info, nameof (ExtensionRightsCacheService), nameof (ExtensionRightsCacheService), string.Format("Updating L1 cache value for Key: {0}, Value: [{1}]", (object) key.ToString(), (object) extensionRights.ToString()));
        extensionRightsMemoryCache.Set(requestContext, key, extensionRights);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(1039028, nameof (ExtensionRightsCacheService), nameof (ExtensionRightsCacheService), ex);
      }
    }

    private static IVssRequestContext GetRequiredContextForFFRead(IVssRequestContext requestContext) => !requestContext.ServiceHost.Is(TeamFoundationHostType.ProjectCollection) ? requestContext.To(TeamFoundationHostType.Deployment) : requestContext;

    private static bool IsL1CacheEnabled(IVssRequestContext requestContext) => ExtensionRightsCacheService.GetRequiredContextForFFRead(requestContext).IsFeatureEnabled(ExtensionRightsCacheService.s_EnableL1CacheFeatureFlag);
  }
}
