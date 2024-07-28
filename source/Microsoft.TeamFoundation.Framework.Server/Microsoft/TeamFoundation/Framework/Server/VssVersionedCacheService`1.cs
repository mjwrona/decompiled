// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.VssVersionedCacheService`1
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public abstract class VssVersionedCacheService<TCacheData> : VssCacheService
  {
    private ILockName m_lock;
    private TCacheData m_cacheData;
    private volatile bool m_initialized;
    private volatile int m_cacheVersion = -1;

    internal override void InternalInitialize(IVssRequestContext systemRequestContext)
    {
      base.InternalInitialize(systemRequestContext);
      this.m_lock = this.CreateLockName(systemRequestContext, "lock");
      this.m_cacheData = this.InitializeCache(systemRequestContext);
      this.m_initialized = true;
    }

    internal override void InternalFinalize(IVssRequestContext systemRequestContext)
    {
      this.m_initialized = false;
      base.InternalFinalize(systemRequestContext);
    }

    public void Reset(IVssRequestContext requestContext)
    {
      this.m_initialized = false;
      this.NotifyCacheReset();
    }

    public T Read<T>(IVssRequestContext requestContext, Func<TCacheData, T> readCache)
    {
      TCacheData cacheData = this.EnsureInitialized(requestContext);
      using (requestContext.AcquireReaderLock(this.m_lock))
        return readCache(cacheData);
    }

    public bool TryRead(IVssRequestContext requestContext, Func<TCacheData, bool> readCache)
    {
      TCacheData cacheData = this.EnsureInitialized(requestContext);
      using (requestContext.AcquireReaderLock(this.m_lock))
        return readCache(cacheData);
    }

    public void Synchronize(
      IVssRequestContext requestContext,
      Action writeOperation,
      Action<TCacheData> writeCache)
    {
      int cacheVersion = this.m_cacheVersion;
      writeOperation();
      using (requestContext.AcquireWriterLock(this.m_lock))
        this.TryUpdateCache(requestContext, cacheVersion, (Action) (() => writeCache(this.m_cacheData)));
    }

    public T Synchronize<T>(
      IVssRequestContext requestContext,
      Func<T> writeOperation,
      Action<TCacheData, T> writeCache)
    {
      int cacheVersion = this.m_cacheVersion;
      T data = writeOperation();
      using (requestContext.AcquireWriterLock(this.m_lock))
        this.TryUpdateCache(requestContext, cacheVersion, (Action) (() => writeCache(this.m_cacheData, data)));
      return data;
    }

    public bool TrySynchronize(
      IVssRequestContext requestContext,
      Func<bool> writeOperation,
      Action<TCacheData> writeCache)
    {
      int cacheVersion = this.m_cacheVersion;
      bool flag = writeOperation();
      if (flag)
      {
        using (requestContext.AcquireWriterLock(this.m_lock))
          this.TryUpdateCache(requestContext, cacheVersion, (Action) (() => writeCache(this.m_cacheData)));
      }
      return flag;
    }

    public void Invalidate<T>(
      IVssRequestContext requestContext,
      Func<TCacheData, T> invalidateCache)
    {
      T obj;
      this.Synchronize(requestContext, (Action) (() => { }), (Action<TCacheData>) (cacheData => obj = invalidateCache(cacheData)));
    }

    protected abstract TCacheData InitializeCache(IVssRequestContext requestContext);

    private TCacheData EnsureInitialized(IVssRequestContext requestContext)
    {
      if (this.m_initialized)
        return this.m_cacheData;
      int cacheVersion = this.m_cacheVersion;
      TCacheData cacheData = this.InitializeCache(requestContext);
      using (requestContext.AcquireWriterLock(this.m_lock))
      {
        if (this.m_initialized)
          return this.m_cacheData;
        this.m_cacheData = cacheData;
        if (cacheVersion == this.m_cacheVersion)
          this.m_initialized = true;
      }
      return this.m_cacheData;
    }

    private void TryUpdateCache(
      IVssRequestContext requestContext,
      int cacheVersion,
      Action updateCache)
    {
      if (cacheVersion == this.m_cacheVersion)
        updateCache();
      else
        this.Reset(requestContext);
      ++this.m_cacheVersion;
    }

    internal TCacheData CacheData => this.m_cacheData;

    internal bool Initialized => this.m_initialized;
  }
}
