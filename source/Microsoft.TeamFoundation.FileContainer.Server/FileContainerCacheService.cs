// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.FileContainerCacheService
// Assembly: Microsoft.TeamFoundation.FileContainer.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: D69E508F-D51F-4EF2-B979-7E96E61DA19E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.FileContainer.Server.dll

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class FileContainerCacheService : 
    VssCacheService,
    IFileContainerCacheService,
    IVssFrameworkService
  {
    private VssMemoryCacheList<long, SecurityEvaluationInformation> m_cache;

    protected override void ServiceStart(IVssRequestContext systemRequestContext)
    {
      base.ServiceStart(systemRequestContext);
      this.m_cache = new VssMemoryCacheList<long, SecurityEvaluationInformation>((IVssCachePerformanceProvider) this, 128);
    }

    protected override void ServiceEnd(IVssRequestContext systemRequestContext)
    {
      this.m_cache.Clear();
      base.ServiceEnd(systemRequestContext);
    }

    public bool TryGetValue(
      IVssRequestContext requestContext,
      long containerId,
      out SecurityEvaluationInformation info)
    {
      return this.m_cache.TryGetValue(containerId, out info);
    }

    public void Set(
      IVssRequestContext requestContext,
      long containerId,
      SecurityEvaluationInformation info)
    {
      this.m_cache.Add(containerId, info, true);
    }

    public void Remove(IVssRequestContext requestContext, long containerId) => this.m_cache.Remove(containerId);
  }
}
