// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.PackagingMemoryCacheService`2
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype
{
  public abstract class PackagingMemoryCacheService<TKey, TVal> : VssMemoryCacheService<TKey, TVal>
  {
    private static readonly TimeSpan InactivityIntervalDefault = VssCacheExpiryProvider.NoExpiry;
    private readonly ITimeProvider timeProvider;

    protected PackagingMemoryCacheService()
      : this((ITimeProvider) new DefaultTimeProvider(), (IEqualityComparer<TKey>) EqualityComparer<TKey>.Default, MemoryCacheConfiguration<TKey, TVal>.Default)
    {
    }

    protected PackagingMemoryCacheService(
      MemoryCacheConfiguration<TKey, TVal> cacheConfiguration)
      : this((ITimeProvider) new DefaultTimeProvider(), (IEqualityComparer<TKey>) EqualityComparer<TKey>.Default, cacheConfiguration)
    {
    }

    protected PackagingMemoryCacheService(
      ITimeProvider timeProvider,
      IEqualityComparer<TKey> comparer,
      MemoryCacheConfiguration<TKey, TVal> cacheConfiguration)
      : base(comparer, cacheConfiguration)
    {
      this.timeProvider = timeProvider;
    }

    protected abstract Guid PackagingInvalidationNotificationGuid { get; }

    public virtual bool HasValue(IVssRequestContext requestContext, TKey key) => this.TryPeekValue(requestContext, key, out TVal _);

    public virtual void Invalidate(IVssRequestContext requestContext, TKey key)
    {
      this.Remove(requestContext, key);
      requestContext.GetService<ITeamFoundationSqlNotificationService>().SendNotification(requestContext, this.PackagingInvalidationNotificationGuid, key.ToString());
    }

    public virtual bool SetValue(IVssRequestContext requestContext, TKey key, TVal val) => this.MemoryCache.Add(key, val, false, new VssCacheExpiryProvider<TKey, TVal>(Capture.Create<TimeSpan>(TimeSpan.FromHours(24.0)), Capture.Create<TimeSpan>(PackagingMemoryCacheService<TKey, TVal>.InactivityIntervalDefault)));

    public virtual bool TryGet(IVssRequestContext requestContext, TKey key, out TVal val) => this.TryGetValue(requestContext, key, out val);

    protected override void ServiceStart(IVssRequestContext requestContext)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      base.ServiceStart(requestContext);
      requestContext.GetService<ITeamFoundationSqlNotificationService>().RegisterNotification(requestContext, "Default", this.PackagingInvalidationNotificationGuid, new SqlNotificationCallback(this.OnCachedUrlNoLongerValid), true);
    }

    protected override void ServiceEnd(IVssRequestContext requestContext)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      requestContext.GetService<ITeamFoundationSqlNotificationService>().UnregisterNotification(requestContext, "Default", this.PackagingInvalidationNotificationGuid, new SqlNotificationCallback(this.OnCachedUrlNoLongerValid), true);
      base.ServiceEnd(requestContext);
    }

    private void OnCachedUrlNoLongerValid(
      IVssRequestContext requestContext,
      Guid eventClass,
      string eventData)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(eventData, nameof (eventData));
      this.Remove(requestContext, this.StringToKey(eventData));
    }

    protected virtual TKey StringToKey(string keyString) => (TKey) Convert.ChangeType((object) keyString, typeof (TKey));
  }
}
