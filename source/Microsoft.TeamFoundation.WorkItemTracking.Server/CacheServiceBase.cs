// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.CacheServiceBase
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server
{
  public abstract class CacheServiceBase : BaseTeamFoundationWorkItemTrackingService
  {
    private ILockName m_cacheServiceBaseLockName;
    private CacheSnapshotBase m_snapshot;
    private bool m_forceRefresh;
    private string m_typeName;
    protected SemaphoreSlim m_simultaneousRefreshesSemaphore;
    protected int m_maxConcurrencyConfigSetting;
    private const int c_dbMaxConcurrencySettingDefault = 4;
    private const string c_dbNumberOfSimultaneousRefreshesAllowedRegistryPath = "/Service/WorkItemTracking/Settings/SimultaneousCacheRefreshesAllowedPerCacheType";
    private const int c_maximumWaitForCacheSemaphoreInMsDefault = 300000;
    private const string c_maximumWaitForCacheSemaphoreInMsRegistryPath = "/Service/WorkItemTracking/Settings/MaximumWaitForCacheSemaphoreInMs";

    protected abstract IEnumerable<MetadataTable> MetadataTables { get; }

    protected override void ServiceStart(IVssRequestContext systemRequestContext)
    {
      this.InitializeLockName(systemRequestContext);
      base.ServiceStart(systemRequestContext);
      this.m_typeName = this.GetType().Name.ToString();
      this.InitializeSemaphore(systemRequestContext);
    }

    protected void InitializeSemaphore(IVssRequestContext systemRequestContext)
    {
      IVssRegistryService service = systemRequestContext.GetService<IVssRegistryService>();
      RegistryQuery query = (RegistryQuery) "/Service/WorkItemTracking/Settings/SimultaneousCacheRefreshesAllowedPerCacheType/";
      this.m_maxConcurrencyConfigSetting = service.GetValue<int>(systemRequestContext, in query, true, 4);
      if (this.m_maxConcurrencyConfigSetting < 1)
        this.m_maxConcurrencyConfigSetting = 1;
      this.m_simultaneousRefreshesSemaphore = new SemaphoreSlim(this.m_maxConcurrencyConfigSetting, this.m_maxConcurrencyConfigSetting);
    }

    protected override void ServiceEnd(IVssRequestContext systemRequestContext)
    {
      if (this.m_simultaneousRefreshesSemaphore != null)
      {
        this.m_simultaneousRefreshesSemaphore.Dispose();
        this.m_simultaneousRefreshesSemaphore = (SemaphoreSlim) null;
      }
      base.ServiceEnd(systemRequestContext);
    }

    protected override void OnSqlNotification(
      IVssRequestContext requestContext,
      Guid eventClass,
      string eventData)
    {
      this.InvalidateCache(requestContext);
    }

    protected virtual T GetSnapshot<T>(IVssRequestContext context, bool checkFreshness = true) where T : CacheSnapshotBase
    {
      bool flag = false;
      CacheSnapshotBase snapshot;
      bool forceRefresh;
      using (context.AcquireReaderLock(this.m_cacheServiceBaseLockName))
      {
        snapshot = this.m_snapshot;
        forceRefresh = this.m_forceRefresh;
      }
      if (snapshot == null | forceRefresh || checkFreshness && !snapshot.IsFresh(context, this.MetadataTables))
      {
        try
        {
          int maximumWaitForCacheSemaphoreInMs = this.GetMaximumWaitForSemaphore(context);
          context.TraceBlock(913001, 913002, 913003, "Services", nameof (CacheServiceBase), string.Format("Waiting {0} ms for semaphore for {1}, semaphore current count is {2}.", (object) maximumWaitForCacheSemaphoreInMs, (object) this.m_typeName, (object) this.m_simultaneousRefreshesSemaphore.CurrentCount), (Action) (() =>
          {
            if (!this.m_simultaneousRefreshesSemaphore.Wait(maximumWaitForCacheSemaphoreInMs, context.CancellationToken))
              throw new WorkItemTrackingCacheRefreshTooLongException(maximumWaitForCacheSemaphoreInMs, this.m_typeName);
          }));
          flag = true;
          using (context.AcquireReaderLock(this.m_cacheServiceBaseLockName))
          {
            snapshot = this.m_snapshot;
            forceRefresh = this.m_forceRefresh;
          }
          if (!(snapshot == null | forceRefresh))
          {
            if (checkFreshness)
            {
              if (snapshot.IsFresh(context, this.MetadataTables))
                goto label_18;
            }
            else
              goto label_18;
          }
          snapshot = this.CreateSnapshot(context, snapshot);
          using (context.AcquireWriterLock(this.m_cacheServiceBaseLockName))
          {
            if (this.m_snapshot == null || this.m_forceRefresh || !this.m_snapshot.IsFresh(context, this.MetadataTables))
            {
              snapshot.MarkSnapshotForUse(context, this.m_snapshot);
              this.m_snapshot = snapshot;
              this.m_forceRefresh = false;
            }
            else
              snapshot = this.m_snapshot;
          }
        }
        finally
        {
          if (flag)
          {
            int num = this.m_simultaneousRefreshesSemaphore.Release();
            context.Trace(913004, TraceLevel.Info, "Services", nameof (CacheServiceBase), string.Format("Semaphore for {0} has {1} slots currently in use out of {2} available to use.", (object) this.m_typeName, (object) num, (object) this.m_maxConcurrencyConfigSetting));
          }
        }
      }
label_18:
      return (T) snapshot;
    }

    internal virtual void InvalidateCache(IVssRequestContext requestContext)
    {
      using (requestContext.AcquireWriterLock(this.m_cacheServiceBaseLockName))
        this.m_forceRefresh = true;
    }

    protected abstract CacheSnapshotBase CreateSnapshot(
      IVssRequestContext requestContext,
      CacheSnapshotBase existingSnapshot);

    protected virtual void InitializeLockName(IVssRequestContext systemRequestContext)
    {
      if (this.m_cacheServiceBaseLockName != null)
        return;
      this.m_cacheServiceBaseLockName = systemRequestContext.ServiceHost.CreateUniqueLockName(this.GetType().Name);
    }

    private int GetMaximumWaitForSemaphore(IVssRequestContext requestContext)
    {
      IVssRegistryService service = requestContext.GetService<IVssRegistryService>();
      RegistryQuery registryQuery = (RegistryQuery) "/Service/WorkItemTracking/Settings/MaximumWaitForCacheSemaphoreInMs/";
      IVssRequestContext requestContext1 = requestContext;
      ref RegistryQuery local = ref registryQuery;
      int waitForSemaphore = service.GetValue<int>(requestContext1, in local, true, 300000);
      if (waitForSemaphore < 0)
        waitForSemaphore = 0;
      return waitForSemaphore;
    }
  }
}
