// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.MemoryCacheConfiguration`2
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Common;
using System;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public class MemoryCacheConfiguration<TKey, TValue>
  {
    private static readonly MemoryCacheConfiguration<TKey, TValue> s_defaultConfiguration = new MemoryCacheConfiguration<TKey, TValue>().WithMaxElements(int.MaxValue).WithMaxSize(long.MaxValue, (ISizeProvider<TKey, TValue>) new NoSizeProvider<TKey, TValue>()).WithExpiryInterval(VssCacheExpiryProvider.NoExpiry).WithInactivityInterval(VssCacheExpiryProvider.NoExpiry).WithExpiryDelay(VssCacheExpiryProvider.NoDelay).WithCleanupInterval(VssCacheService.NoCleanup);

    public int? MaxLength { get; private set; }

    public long? MaxSize { get; private set; }

    public ISizeProvider<TKey, TValue> SizeProvider { get; private set; }

    public TimeSpan? CleanupInterval { get; private set; }

    public Microsoft.TeamFoundation.Framework.Server.ExpiryProviderFactory<TKey, TValue> ExpiryProviderFactory { get; private set; }

    public TimeSpan? ExpiryInterval { get; private set; }

    public TimeSpan? InactivityInterval { get; private set; }

    public DateTime? ExpiryDelay { get; private set; }

    public static MemoryCacheConfiguration<TKey, TValue> Default => MemoryCacheConfiguration<TKey, TValue>.s_defaultConfiguration;

    public MemoryCacheConfiguration<TKey, TValue> Clone() => (MemoryCacheConfiguration<TKey, TValue>) this.MemberwiseClone();

    public MemoryCacheConfiguration<TKey, TValue> WithMaxElements(int maxLength)
    {
      this.MaxLength = new int?(maxLength);
      return this;
    }

    public MemoryCacheConfiguration<TKey, TValue> WithMaxSize(
      long maxSize,
      ISizeProvider<TKey, TValue> sizeProvider)
    {
      this.MaxSize = new long?(maxSize);
      this.SizeProvider = sizeProvider;
      return this;
    }

    public MemoryCacheConfiguration<TKey, TValue> WithCleanupInterval(TimeSpan cleanupInterval)
    {
      this.CleanupInterval = new TimeSpan?(cleanupInterval);
      return this;
    }

    public MemoryCacheConfiguration<TKey, TValue> WithExpiryProvider(
      Microsoft.TeamFoundation.Framework.Server.ExpiryProviderFactory<TKey, TValue> factory)
    {
      this.ExpiryProviderFactory = factory;
      return this;
    }

    public MemoryCacheConfiguration<TKey, TValue> WithExpiryInterval(TimeSpan expiry)
    {
      this.ExpiryInterval = new TimeSpan?(expiry);
      return this;
    }

    public MemoryCacheConfiguration<TKey, TValue> WithInactivityInterval(TimeSpan inactivity)
    {
      this.InactivityInterval = new TimeSpan?(inactivity);
      return this;
    }

    public MemoryCacheConfiguration<TKey, TValue> WithExpiryDelay(DateTime delay)
    {
      this.ExpiryDelay = new DateTime?(delay);
      return this;
    }

    public override string ToString() => string.Format("[MaxLength={0}, MaxSize={1}, ExpiryInterval={2}, InactivityInterval={3}, ExpiryDelay={4}, CleanupInterval={5}]", (object) this.MaxLength, (object) this.MaxSize, (object) this.ExpiryInterval, (object) this.InactivityInterval, (object) this.ExpiryDelay, (object) this.CleanupInterval);
  }
}
