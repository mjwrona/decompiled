// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.SoapService.VStudio.CustomExpiryCache
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using System;

namespace Microsoft.VisualStudio.Services.Gallery.Server.SoapService.VStudio
{
  internal class CustomExpiryCache : VssMemoryCacheService<string, object>
  {
    private static readonly TimeSpan defaultCacheExpiryInterval = TimeSpan.FromMinutes(10.0);
    private static readonly TimeSpan cacheCleanupInterval = TimeSpan.FromMinutes(10.0);

    public CustomExpiryCache()
      : base(CustomExpiryCache.cacheCleanupInterval)
    {
      this.ExpiryInterval.Value = CustomExpiryCache.defaultCacheExpiryInterval;
      this.OneWeekExpiryProvider = new VssCacheExpiryProvider<string, object>(Capture.Create<TimeSpan>(TimeSpan.FromDays(7.0)), Capture.Create<TimeSpan>(VssCacheExpiryProvider.NoExpiry));
      this.OneDayExpiryProvider = new VssCacheExpiryProvider<string, object>(Capture.Create<TimeSpan>(TimeSpan.FromDays(1.0)), Capture.Create<TimeSpan>(VssCacheExpiryProvider.NoExpiry));
      this.TwelveHoursExpiryProvider = new VssCacheExpiryProvider<string, object>(Capture.Create<TimeSpan>(TimeSpan.FromHours(12.0)), Capture.Create<TimeSpan>(VssCacheExpiryProvider.NoExpiry));
      this.TenMinutesExpiryProvider = new VssCacheExpiryProvider<string, object>(Capture.Create<TimeSpan>(TimeSpan.FromMinutes(10.0)), Capture.Create<TimeSpan>(VssCacheExpiryProvider.NoExpiry));
      this.OneHourExpiryProvider = new VssCacheExpiryProvider<string, object>(Capture.Create<TimeSpan>(TimeSpan.FromMinutes(60.0)), Capture.Create<TimeSpan>(VssCacheExpiryProvider.NoExpiry));
    }

    public VssCacheExpiryProvider<string, object> OneWeekExpiryProvider { get; private set; }

    public VssCacheExpiryProvider<string, object> OneDayExpiryProvider { get; private set; }

    public VssCacheExpiryProvider<string, object> TwelveHoursExpiryProvider { get; private set; }

    public VssCacheExpiryProvider<string, object> TenMinutesExpiryProvider { get; private set; }

    public VssCacheExpiryProvider<string, object> OneHourExpiryProvider { get; private set; }

    public virtual void Set(
      string key,
      object value,
      VssCacheExpiryProvider<string, object> expiryProvider)
    {
      this.MemoryCache.Add(key, value, true, expiryProvider);
    }

    public virtual bool TryGetValue<T>(IVssRequestContext requestContext, string key, out T value) where T : class
    {
      object obj1;
      if (this.TryGetValue(requestContext, key, out obj1))
      {
        value = obj1 == null || !(obj1 is T obj2) ? default (T) : obj2;
        return true;
      }
      value = default (T);
      return false;
    }
  }
}
