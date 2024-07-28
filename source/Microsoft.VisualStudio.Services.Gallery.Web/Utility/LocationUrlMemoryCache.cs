// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Web.Utility.LocationUrlMemoryCache
// Assembly: Microsoft.VisualStudio.Services.Gallery.Web, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 17D36576-2EF3-4ABC-94BA-AF7891D15A3A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Web.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;

namespace Microsoft.VisualStudio.Services.Gallery.Web.Utility
{
  public class LocationUrlMemoryCache : VssMemoryCacheService<string, string>
  {
    private static readonly TimeSpan s_maxCacheLife = TimeSpan.FromDays(1.0);
    private static readonly TimeSpan s_maxCacheInactivityAge = TimeSpan.FromDays(1.0);
    private static readonly TimeSpan s_cleanupInterval = new TimeSpan(24, 0, 0);

    public LocationUrlMemoryCache()
      : base(LocationUrlMemoryCache.s_cleanupInterval)
    {
      this.InactivityInterval.Value = LocationUrlMemoryCache.s_maxCacheInactivityAge;
      this.ExpiryInterval.Value = LocationUrlMemoryCache.s_maxCacheLife;
    }
  }
}
