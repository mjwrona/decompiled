// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Query.SecurityChecksCache.SecurityChecksCacheBase`1
// Assembly: Microsoft.VisualStudio.Services.Search.Query, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 71E00698-03D3-4C67-B313-A550333DA80C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Query.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry;
using Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Implementations;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Microsoft.VisualStudio.Services.Search.Query.SecurityChecksCache
{
  public abstract class SecurityChecksCacheBase<TValue> : VssCacheBase, ISecurityChecksCache<TValue>
  {
    private readonly TimeSpan m_securityCheckCacheExpiration;
    private readonly TimeSpan m_securityChecksCacheInactivityExpiration;
    private readonly int m_securityChecksCacheMaxSize;
    private readonly VssMemoryCacheList<Guid, TValue> m_userInfoMap;
    private TeamFoundationTask m_cleanupTask;
    private TimeSpan m_securityCheckCacheCleanupTaskInterval;

    protected SecurityChecksCacheBase(
      int SecurityChecksCacheMaxSize,
      TimeSpan securityCheckCacheExpiration,
      TimeSpan securityCheckCacheCleanupTaskInterval,
      TimeSpan securityChecksCacheInactivityExpiration)
    {
      this.m_securityChecksCacheMaxSize = SecurityChecksCacheMaxSize;
      this.m_securityCheckCacheExpiration = securityCheckCacheExpiration;
      this.m_securityCheckCacheCleanupTaskInterval = securityCheckCacheCleanupTaskInterval;
      this.m_securityChecksCacheInactivityExpiration = securityChecksCacheInactivityExpiration;
      this.m_userInfoMap = new VssMemoryCacheList<Guid, TValue>((IVssCachePerformanceProvider) this, CaptureLength.Create(this.m_securityChecksCacheMaxSize), new VssCacheExpiryProvider<Guid, TValue>(new Capture<TimeSpan>(this.m_securityCheckCacheExpiration), new Capture<TimeSpan>(this.m_securityChecksCacheInactivityExpiration)));
    }

    public virtual void Initialize(IVssRequestContext requestContext) => this.QueueCacheMaintenanceTask(requestContext);

    public virtual void TearDown(IVssRequestContext requestContext)
    {
      try
      {
        this.RemoveCacheMaintainenceTask(requestContext);
      }
      finally
      {
        this.ClearCache();
      }
    }

    public virtual void UpdateCacheWithUserInfo(
      IVssRequestContext userRequestContext,
      TValue userData)
    {
      this.m_userInfoMap.Add(userRequestContext.GetUserId(), userData, true);
    }

    public virtual bool TryGetUserData(IVssRequestContext userRequestContext, out TValue userData) => this.TryGetUserData(userRequestContext.GetUserId(), out userData);

    public virtual void ClearCache()
    {
      this.m_userInfoMap.Clear();
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.PublishKpi("SecurityChecksCacheCleared", "Query Pipeline", 1.0);
    }

    public virtual void QueueCacheMaintenanceTask(IVssRequestContext requestContext)
    {
      ITeamFoundationTaskService service = requestContext.To(TeamFoundationHostType.Deployment).GetService<ITeamFoundationTaskService>();
      this.m_cleanupTask = new TeamFoundationTask(new TeamFoundationTaskCallback(this.CleanupExpiredCache), (object) null, (int) this.m_securityCheckCacheCleanupTaskInterval.TotalMilliseconds);
      IVssRequestContext requestContext1 = requestContext;
      TeamFoundationTask cleanupTask = this.m_cleanupTask;
      service.AddTask(requestContext1, cleanupTask);
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceVerbose(1081353, "Query Pipeline", "SecurityChecks", "Queued task to cleanup expired cache");
    }

    public virtual void RemoveCacheMaintainenceTask(IVssRequestContext requestContext)
    {
      if (this.m_cleanupTask == null)
      {
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceError(1081357, "Query Pipeline", "SecurityChecks", "Cleanup task could not be removed as it was never updated.");
      }
      else
      {
        requestContext.To(TeamFoundationHostType.Deployment).GetService<ITeamFoundationTaskService>().RemoveTask(requestContext, this.m_cleanupTask);
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceVerbose(1081355, "Query Pipeline", "SecurityChecks", "Removing task that cleans up expired cache");
      }
    }

    private void CleanupExpiredCache(IVssRequestContext requestContext, object taskArgs)
    {
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.SetLogicalContext((IDiagnosticContext) new TfsDiagnosticContext(requestContext));
      try
      {
        int count = this.m_userInfoMap.Count;
        Stopwatch stopwatch = Stopwatch.StartNew();
        try
        {
          this.m_userInfoMap.Sweep();
        }
        catch (Exception ex) when (SecurityChecksCacheBase<TValue>.LogException(ex))
        {
        }
        finally
        {
          stopwatch.Stop();
        }
        FriendlyDictionary<string, object> properties = new FriendlyDictionary<string, object>()
        {
          ["SecurityChecksCacheUserDictSize_BeforeCleanup"] = (object) count,
          ["SecurityChecksCacheUserDictSize_AfterCleanup"] = (object) this.m_userInfoMap.Count,
          ["SecurityChecksCacheCleanupTime"] = (object) stopwatch.ElapsedMilliseconds
        };
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.PublishClientTrace("Query Pipeline", this.GetType().Name, (IDictionary<string, object>) properties);
      }
      finally
      {
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.ClearLogicalContext();
      }
    }

    private static bool LogException(Exception e)
    {
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceError(1081351, "Query Pipeline", "SecurityChecks", FormattableString.Invariant(FormattableStringFactory.Create("Cleanup expired cache task failed due to exception : {0}.", (object) e)));
      return false;
    }

    protected virtual bool TryGetUserData(Guid userId, out TValue userData)
    {
      try
      {
        return this.m_userInfoMap.TryGetValue(userId, out userData);
      }
      catch (Exception ex)
      {
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceException(1081356, "Query Pipeline", "SecurityChecks", ex);
        userData = default (TValue);
        return false;
      }
    }
  }
}
