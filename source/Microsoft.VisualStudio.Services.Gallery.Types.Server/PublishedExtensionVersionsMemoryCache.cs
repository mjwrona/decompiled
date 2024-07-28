// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Types.Server.PublishedExtensionVersionsMemoryCache
// Assembly: Microsoft.VisualStudio.Services.Gallery.Types.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: FF687265-4AE2-4CD2-A134-409D61826008
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Types.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Gallery.WebApi;
using System;

namespace Microsoft.VisualStudio.Services.Gallery.Types.Server
{
  public class PublishedExtensionVersionsMemoryCache : 
    VssMemoryCacheService<string, PublishedExtension>
  {
    private static readonly TimeSpan s_maxCacheLife = TimeSpan.FromHours(16.0);
    private static readonly TimeSpan s_maxCacheInactivityAge = TimeSpan.FromHours(1.0);
    private static readonly TimeSpan s_cleanupInterval = new TimeSpan(0, 5, 0);

    public PublishedExtensionVersionsMemoryCache()
      : base(PublishedExtensionVersionsMemoryCache.s_cleanupInterval)
    {
      this.InactivityInterval.Value = PublishedExtensionVersionsMemoryCache.s_maxCacheInactivityAge;
      this.ExpiryInterval.Value = PublishedExtensionVersionsMemoryCache.s_maxCacheLife;
    }
  }
}
