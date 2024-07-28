// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Licensing.LicenseClaimRemoteCache
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Redis;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Licensing
{
  internal class LicenseClaimRemoteCache : IVssFrameworkService
  {
    private Guid m_namespaceId;
    private Guid m_serviceHostId;
    private ContainerSettings m_containerSettings;
    private static readonly string s_RegistryRoot = "/Configuration/Licensing/L2Cache/";
    private static readonly string s_RedisNamespaceIdRegistryKey = LicenseClaimRemoteCache.s_RegistryRoot + "RedisNamespace";
    private static readonly string s_RedisCacheKeyExpiryRegistryKey = LicenseClaimRemoteCache.s_RegistryRoot + "L2CacheTimespanTTL";
    private static readonly string s_RegistryUpdateFilter = LicenseClaimRemoteCache.s_RegistryRoot + "...";
    private static readonly string s_DisableRemoteCacheFeatureFlag = "VisualStudio.Services.Licensing.ClaimsCache.Remote.Disable";
    private static readonly Guid s_DefaultRedisNamespaceId = new Guid("4F8B7DC8-CBD8-471A-B11E-45E008368120");
    private static readonly TimeSpan s_DefaultRedisCacheKeyExpiry = TimeSpan.FromHours(9.0);
    private static readonly PerformanceTracer Tracer = new PerformanceTracer(LicenseClaimPerfCounters.StandardSet, "Licensing", "RemoteCache");
    private const string TraceArea = "Licensing";
    private const string TraceLayer = "RemoteCache";

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
      this.m_serviceHostId = systemRequestContext.ServiceHost.InstanceId;
      systemRequestContext.GetService<IVssRegistryService>().RegisterNotification(systemRequestContext, new RegistrySettingsChangedCallback(this.OnRegistryChanged), LicenseClaimRemoteCache.s_RegistryUpdateFilter);
      this.ReloadConfiguration(systemRequestContext);
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext) => systemRequestContext.GetService<IVssRegistryService>().UnregisterNotification(systemRequestContext, new RegistrySettingsChangedCallback(this.OnRegistryChanged));

    private void OnRegistryChanged(
      IVssRequestContext context,
      RegistryEntryCollection changedEntries)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(context, nameof (context));
      ArgumentUtility.CheckForNull<RegistryEntryCollection>(changedEntries, nameof (changedEntries));
      if (!changedEntries.Any<RegistryEntry>())
        return;
      this.ReloadConfiguration(context);
    }

    private void ReloadConfiguration(IVssRequestContext requestContext)
    {
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
      IVssRegistryService service = vssRequestContext.GetService<IVssRegistryService>();
      TimeSpan timeSpan = service.GetValue<TimeSpan>(vssRequestContext, (RegistryQuery) LicenseClaimRemoteCache.s_RedisCacheKeyExpiryRegistryKey, LicenseClaimRemoteCache.s_DefaultRedisCacheKeyExpiry);
      this.m_containerSettings = new ContainerSettings()
      {
        KeyExpiry = new TimeSpan?(timeSpan),
        CiAreaName = typeof (LicenseClaimCacheService).Name
      };
      this.m_namespaceId = service.GetValue<Guid>(vssRequestContext, (RegistryQuery) LicenseClaimRemoteCache.s_RedisNamespaceIdRegistryKey, LicenseClaimRemoteCache.s_DefaultRedisNamespaceId);
    }

    public bool Remove(IVssRequestContext requestContext, string key)
    {
      this.ValidateRequestContext(requestContext);
      ArgumentUtility.CheckStringForNullOrEmpty(key, nameof (key));
      if (!LicenseClaimRemoteCache.IsRemoteCacheEnabled(requestContext))
        return false;
      using (LicenseClaimRemoteCache.Tracer.TraceTimedAction(requestContext, LicenseClaimRemoteCache.TracePoints.Remove.Slow, 200, nameof (Remove)))
        return LicenseClaimRemoteCache.Tracer.TraceAction<bool>(requestContext, (ActionTracePoints) LicenseClaimRemoteCache.TracePoints.Remove, (Func<bool>) (() =>
        {
          try
          {
            using (LicenseClaimRemoteCache.Tracer.TraceTimedAction(requestContext, 1045706, 100, LicenseClaimRemoteCache.TraceConstants.RemoteCacheRemoveOperationName))
              this.GetCacheContainer(requestContext).Invalidate<string, AccountRightsClaimsContainer>(requestContext, key);
            LicenseClaimRemoteCache.Tracer.Trace(requestContext, 1045704, TraceLevel.Verbose, (Func<string>) (() => string.Format("Removed {0} from L2 cache for host {1}", (object) key, (object) requestContext.ServiceHost.InstanceId)), nameof (Remove));
            return true;
          }
          catch (Exception ex)
          {
            LicenseClaimRemoteCache.Tracer.TraceException(requestContext, 1045706, ex, nameof (Remove));
          }
          return false;
        }), nameof (Remove));
    }

    public void Set(IVssRequestContext requestContext, string key, ILicenseClaim value)
    {
      this.ValidateRequestContext(requestContext);
      ArgumentUtility.CheckStringForNullOrEmpty(key, nameof (key));
      if (!LicenseClaimRemoteCache.IsRemoteCacheEnabled(requestContext))
        return;
      using (LicenseClaimRemoteCache.Tracer.TraceTimedAction(requestContext, LicenseClaimRemoteCache.TracePoints.Set.Slow, 200, nameof (Set)))
        LicenseClaimRemoteCache.Tracer.TraceAction(requestContext, (ActionTracePoints) LicenseClaimRemoteCache.TracePoints.Set, (Action) (() =>
        {
          try
          {
            IMutableDictionaryCacheContainer<string, AccountRightsClaimsContainer> cacheContainer = this.GetCacheContainer(requestContext);
            ILicenseClaim rhs;
            if (LicenseClaimRemoteCache.TryGetValueInternal(requestContext, key, cacheContainer, out rhs))
            {
              if (LicenseClaimUtility.AreClaimsEqual(value, rhs))
              {
                LicenseClaimRemoteCache.Tracer.Trace(requestContext, 1045713, TraceLevel.Verbose, (Func<string>) (() => "Ignoring set operation on remote cache. The value for key " + key + " is up to date."), nameof (Set));
                return;
              }
              LicenseClaimRemoteCache.Tracer.Trace(requestContext, 1045714, TraceLevel.Verbose, (Func<string>) (() => "Mis-match in the updated claim and the remote cache value for key " + key), nameof (Set));
            }
            AccountRightsClaimsContainer claimsContainer = new AccountRightsClaimsContainer();
            claimsContainer.SetClaim(value);
            using (LicenseClaimRemoteCache.Tracer.TraceTimedAction(requestContext, 1045745, 100, LicenseClaimRemoteCache.TraceConstants.RemoteCacheSetOperationName))
              cacheContainer.Set(requestContext, (IDictionary<string, AccountRightsClaimsContainer>) new Dictionary<string, AccountRightsClaimsContainer>()
              {
                {
                  key,
                  claimsContainer
                }
              });
            LicenseClaimRemoteCache.Tracer.Trace(requestContext, 1045715, TraceLevel.Verbose, (Func<string>) (() => "Successfully performed remote cache set on key '" + key + "' and value '" + claimsContainer.Serialize<AccountRightsClaimsContainer>(true) + "'"), nameof (Set));
          }
          catch (Exception ex)
          {
            LicenseClaimRemoteCache.Tracer.TraceException(requestContext, 1045716, ex, nameof (Set));
          }
        }), nameof (Set));
    }

    public bool TryGetValue(IVssRequestContext requestContext, string key, out ILicenseClaim value)
    {
      this.ValidateRequestContext(requestContext);
      ArgumentUtility.CheckStringForNullOrEmpty(key, nameof (key));
      if (!LicenseClaimRemoteCache.IsRemoteCacheEnabled(requestContext))
      {
        value = (ILicenseClaim) null;
        return false;
      }
      using (LicenseClaimRemoteCache.Tracer.TraceTimedAction(requestContext, LicenseClaimRemoteCache.TracePoints.TryGet.Slow, 200, nameof (TryGetValue)))
      {
        try
        {
          requestContext.TraceEnter(LicenseClaimRemoteCache.TracePoints.TryGet.Enter, "Licensing", "RemoteCache", nameof (TryGetValue));
          try
          {
            ILicenseClaim licenseClaim;
            if (LicenseClaimRemoteCache.TryGetValueInternal(requestContext, key, this.GetCacheContainer(requestContext), out licenseClaim))
            {
              value = licenseClaim;
              LicenseClaimRemoteCache.Tracer.TraceCacheHit(requestContext, 1045723, key, LicenseClaimRemoteCache.TraceConstants.RemoteCacheLookupOperationName);
              return true;
            }
            LicenseClaimRemoteCache.Tracer.TraceCacheMiss(requestContext, 1045724, key, LicenseClaimRemoteCache.TraceConstants.RemoteCacheLookupOperationName);
          }
          catch (Exception ex)
          {
            requestContext.TraceException(1045726, "Licensing", "RemoteCache", ex);
          }
          value = (ILicenseClaim) null;
          return false;
        }
        finally
        {
          requestContext.TraceLeave(LicenseClaimRemoteCache.TracePoints.TryGet.Exit, "Licensing", "RemoteCache", nameof (TryGetValue));
        }
      }
    }

    private void ValidateRequestContext(IVssRequestContext requestContext)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      if (!this.m_serviceHostId.Equals(requestContext.ServiceHost.InstanceId))
        throw new InvalidRequestContextHostException(FrameworkResources.CacheServiceRequestContextHostMessage((object) this.m_serviceHostId, (object) requestContext.ServiceHost.InstanceId));
    }

    private static bool IsRemoteCacheEnabled(IVssRequestContext requestContext)
    {
      int num = requestContext.IsFeatureEnabled(LicenseClaimRemoteCache.s_DisableRemoteCacheFeatureFlag) ? 1 : 0;
      bool flag = requestContext.GetService<IRedisCacheService>().IsEnabled(requestContext);
      if (num == 0 && !flag)
        requestContext.TraceConditionally(1045743, TraceLevel.Warning, "Licensing", "RemoteCache", (Func<string>) (() => string.Format("The remote caching feature is enabled for host {0}, but Redis is either disabled or unconfigured", (object) requestContext.ServiceHost.InstanceId)));
      return num == 0 & flag;
    }

    private IMutableDictionaryCacheContainer<string, AccountRightsClaimsContainer> GetCacheContainer(
      IVssRequestContext requestContext)
    {
      return requestContext.GetService<IRedisCacheService>().GetVolatileDictionaryContainer<string, AccountRightsClaimsContainer, LicenseClaimRemoteCache.LicenseClaimCacheSecurityToken>(requestContext, this.m_namespaceId, this.m_containerSettings);
    }

    private static bool TryGetValueInternal(
      IVssRequestContext requestContext,
      string key,
      IMutableDictionaryCacheContainer<string, AccountRightsClaimsContainer> cacheContainer,
      out ILicenseClaim value)
    {
      try
      {
        requestContext.TraceEnter(LicenseClaimRemoteCache.TracePoints.TryGetInternal.Enter, "Licensing", "RemoteCache", nameof (TryGetValueInternal));
        AccountRightsClaimsContainer claimsContainer;
        if (cacheContainer == null || !cacheContainer.TryGet<string, AccountRightsClaimsContainer>(requestContext, key, out claimsContainer))
        {
          value = (ILicenseClaim) null;
          return false;
        }
        requestContext.TraceConditionally(1045733, TraceLevel.Verbose, "Licensing", "RemoteCache", (Func<string>) (() =>
        {
          string[] strArray = new string[5]
          {
            "Key '",
            key,
            "' returned value from remote store: '",
            null,
            null
          };
          AccountRightsClaimsContainer rightsClaimsContainer = claimsContainer;
          strArray[3] = rightsClaimsContainer != null ? rightsClaimsContainer.Serialize<AccountRightsClaimsContainer>(true) : (string) null;
          strArray[4] = "'";
          return string.Concat(strArray);
        }));
        if (claimsContainer == null || claimsContainer.GetClaims().Count<ILicenseClaim>() != 1)
        {
          string str = key;
          AccountRightsClaimsContainer rightsClaimsContainer = claimsContainer;
          // ISSUE: variable of a boxed type
          __Boxed<int> local = (ValueType) (rightsClaimsContainer != null ? rightsClaimsContainer.GetClaims().Count<ILicenseClaim>() : 0);
          throw new LicenseNotAvailableException(string.Format("Claims container for key {0} contained {1} claims", (object) str, (object) local));
        }
        value = claimsContainer.GetClaims().First<ILicenseClaim>();
        return true;
      }
      catch (Exception ex)
      {
        requestContext.TraceException(LicenseClaimRemoteCache.TracePoints.TryGetInternal.Exception, "Licensing", "RemoteCache", ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(LicenseClaimRemoteCache.TracePoints.TryGetInternal.Exit, "Licensing", "RemoteCache", nameof (TryGetValueInternal));
      }
    }

    private static class TraceConstants
    {
      internal static readonly string RemoteCacheLookupOperationName = "RemoteGet";
      internal static readonly string RemoteCacheSetOperationName = "RemoteSet";
      internal static readonly string RemoteCacheRemoveOperationName = "RemoteRemove";
    }

    private static class TracePoints
    {
      internal static readonly TimedActionTracePoints Remove = new TimedActionTracePoints(1045701, 1045707, 1045708, 1045709);
      internal static readonly TimedActionTracePoints Set = new TimedActionTracePoints(1045711, 1045717, 1045718, 1045719);
      internal static readonly TimedActionTracePoints TryGet = new TimedActionTracePoints(1045721, 1045727, 1045728, 1045729);
      internal static readonly TimedActionTracePoints TryGetInternal = new TimedActionTracePoints(1045731, 1045737, 1045738, 1045739);
    }

    private class LicenseClaimCacheSecurityToken
    {
    }
  }
}
