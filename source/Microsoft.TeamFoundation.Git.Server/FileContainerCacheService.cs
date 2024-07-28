// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.FileContainerCacheService
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using Microsoft.TeamFoundation.Framework.Server;

namespace Microsoft.TeamFoundation.Git.Server
{
  internal class FileContainerCacheService : VssCacheService
  {
    private VssMemoryCacheList<OdbId, long> m_cache;

    protected override void ServiceStart(IVssRequestContext systemRC)
    {
      base.ServiceStart(systemRC);
      this.m_cache = new VssMemoryCacheList<OdbId, long>((IVssCachePerformanceProvider) this, 256);
    }

    protected override void ServiceEnd(IVssRequestContext systemRC)
    {
      this.m_cache.Clear();
      base.ServiceEnd(systemRC);
    }

    public bool TryGetValue(IVssRequestContext systemRC, OdbId repoId, out long cacheId) => this.m_cache.TryGetValue(repoId, out cacheId);

    public void Set(IVssRequestContext systemRC, OdbId repoId, long cacheId) => this.m_cache[repoId] = cacheId;
  }
}
