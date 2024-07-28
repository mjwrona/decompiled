// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Policy.Server.PolicyConfigurationVersionedCacheService
// Assembly: Microsoft.TeamFoundation.Policy.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C7B03386-B27B-4823-BB4F-89F7D7E42DDD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Policy.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Microsoft.TeamFoundation.Policy.Server
{
  public class PolicyConfigurationVersionedCacheService : 
    IPolicyConfigurationVersionedCacheService,
    IVssFrameworkService
  {
    private IPolicyConfigurationBasicCacheService m_policyCache;
    private ILockName m_policyCacheLock;
    private int m_cacheVersion;
    private static readonly string s_area = "VersionControl";
    private static readonly string s_layer = "PolicyConfigurationCacheService";

    public PolicyConfigurationVersionedCacheService() => this.m_cacheVersion = 0;

    public void ServiceStart(IVssRequestContext requestContext)
    {
      this.m_policyCacheLock = requestContext.ServiceHost.CreateUniqueLockName("Policy.Server.PolicyConfigurationCache");
      this.m_policyCache = requestContext.GetService<IPolicyConfigurationBasicCacheService>();
      requestContext.GetService<ITeamFoundationSqlNotificationService>().RegisterNotification(requestContext, "Default", PolicyConfigurationNotifications.PolicyConfigurationChanged, new SqlNotificationCallback(this.OnPolicyConfigurationChanged), true);
    }

    public void ServiceEnd(IVssRequestContext requestContext) => requestContext.GetService<ITeamFoundationSqlNotificationService>().UnregisterNotification(requestContext, "Default", PolicyConfigurationNotifications.PolicyConfigurationChanged, new SqlNotificationCallback(this.OnPolicyConfigurationChanged), true);

    public void Clear(IVssRequestContext requestContext)
    {
      using (requestContext.Lock(this.m_policyCacheLock))
      {
        ++this.m_cacheVersion;
        this.m_policyCache.Clear(requestContext);
      }
    }

    public IList<PolicyConfigurationRecord> Get(
      IVssRequestContext requestContext,
      int key,
      Func<IList<PolicyConfigurationRecord>> query)
    {
      int cacheVersion = this.m_cacheVersion;
      IList<PolicyConfigurationRecord> configurationRecordList1;
      if (this.m_policyCache.TryGetValue(requestContext, key, out configurationRecordList1))
        return configurationRecordList1;
      requestContext.Trace(1390128, TraceLevel.Verbose, PolicyConfigurationVersionedCacheService.s_area, PolicyConfigurationVersionedCacheService.s_layer, "Policy Cache Miss: " + key.ToString());
      IList<PolicyConfigurationRecord> configurationRecordList2 = query();
      using (requestContext.Lock(this.m_policyCacheLock))
      {
        if (cacheVersion == this.m_cacheVersion)
        {
          requestContext.Trace(1390129, TraceLevel.Verbose, PolicyConfigurationVersionedCacheService.s_area, PolicyConfigurationVersionedCacheService.s_layer, "Policy Cache Set: " + key.ToString());
          this.m_policyCache.Set(requestContext, key, configurationRecordList2);
        }
      }
      return configurationRecordList2;
    }

    public bool Remove(IVssRequestContext requestContext, int key)
    {
      using (requestContext.Lock(this.m_policyCacheLock))
      {
        ++this.m_cacheVersion;
        return this.m_policyCache.Remove(requestContext, key);
      }
    }

    public VssMemoryCacheList<int, IList<PolicyConfigurationRecord>> MemoryCache => this.m_policyCache.MemoryCache;

    public void OnPolicyConfigurationChanged(
      IVssRequestContext requestContext,
      Guid eventClass,
      string eventData)
    {
      requestContext.Trace(1390125, TraceLevel.Verbose, PolicyConfigurationVersionedCacheService.s_area, PolicyConfigurationVersionedCacheService.s_layer, "OnPolicyConfigurationChanged: " + (eventData ?? string.Empty));
      int result;
      if (int.TryParse(eventData, out result))
      {
        this.Remove(requestContext, result);
      }
      else
      {
        requestContext.Trace(1390126, TraceLevel.Error, PolicyConfigurationVersionedCacheService.s_area, PolicyConfigurationVersionedCacheService.s_layer, "Invalid event payload: " + (eventData ?? string.Empty));
        this.Clear(requestContext);
      }
    }
  }
}
