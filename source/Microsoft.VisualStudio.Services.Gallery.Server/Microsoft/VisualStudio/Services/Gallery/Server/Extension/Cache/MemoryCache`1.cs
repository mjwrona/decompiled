// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.Extension.Cache.MemoryCache`1
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;

namespace Microsoft.VisualStudio.Services.Gallery.Server.Extension.Cache
{
  public class MemoryCache<T> : IMemoryCache<T> where T : class
  {
    private VssRefreshCache<T> m_cache;
    private T m_LocalInstance;

    public MemoryCache(int refreshIntervalInSeconds, Func<IVssRequestContext, T> refreshCallback) => this.m_cache = new VssRefreshCache<T>(TimeSpan.FromSeconds((double) refreshIntervalInSeconds), refreshCallback, true);

    public virtual T GetCachedData(IVssRequestContext requestContext, bool doNotRefresh = false)
    {
      if (!doNotRefresh || (object) this.m_LocalInstance == null)
        this.m_LocalInstance = this.m_cache.Get(requestContext);
      return this.m_LocalInstance;
    }
  }
}
