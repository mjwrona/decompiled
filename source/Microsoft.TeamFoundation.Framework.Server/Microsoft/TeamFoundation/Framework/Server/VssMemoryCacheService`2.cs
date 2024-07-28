// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.VssMemoryCacheService`2
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public abstract class VssMemoryCacheService<TKey, TValue> : VssCacheService
  {
    private readonly ISizeProvider<TKey, TValue> m_sizeProvider;
    private readonly VssMemoryCacheList<TKey, TValue> m_cache;
    private readonly TimeSpan m_cleanupInterval;
    private ILockName m_settingsLock;
    private MemoryCacheConfiguration<TKey, TValue> m_initialConfiguration;

    protected Capture<TimeSpan> ExpiryInterval { get; private set; }

    protected Capture<TimeSpan> InactivityInterval { get; private set; }

    protected Capture<DateTime> ExpiryDelay { get; private set; }

    protected CaptureLength MaxCacheLength { get; private set; }

    protected CaptureSize MaxCacheSize { get; private set; }

    protected string RegistryPath { get; private set; }

    protected VssMemoryCacheService()
      : this(VssCacheService.NoCleanup)
    {
    }

    protected VssMemoryCacheService(TimeSpan cleanupInterval)
      : this((IEqualityComparer<TKey>) EqualityComparer<TKey>.Default, cleanupInterval)
    {
    }

    protected VssMemoryCacheService(IEqualityComparer<TKey> comparer)
      : this(comparer, VssCacheService.NoCleanup)
    {
    }

    protected VssMemoryCacheService(IEqualityComparer<TKey> comparer, TimeSpan cleanupInterval)
      : this(comparer, new MemoryCacheConfiguration<TKey, TValue>().WithCleanupInterval(cleanupInterval))
    {
    }

    protected VssMemoryCacheService(
      IEqualityComparer<TKey> comparer,
      MemoryCacheConfiguration<TKey, TValue> configuration)
      : this(comparer, configuration, (ITimeProvider) null)
    {
    }

    internal VssMemoryCacheService(
      IEqualityComparer<TKey> comparer,
      MemoryCacheConfiguration<TKey, TValue> configuration,
      ITimeProvider timeProvider)
    {
      this.m_cleanupInterval = configuration.CleanupInterval ?? MemoryCacheConfiguration<TKey, TValue>.Default.CleanupInterval.Value;
      TimeSpan? nullable = configuration.ExpiryInterval;
      this.ExpiryInterval = Capture.Create<TimeSpan>(nullable ?? MemoryCacheConfiguration<TKey, TValue>.Default.ExpiryInterval.Value);
      nullable = configuration.InactivityInterval;
      this.InactivityInterval = Capture.Create<TimeSpan>(nullable ?? MemoryCacheConfiguration<TKey, TValue>.Default.InactivityInterval.Value);
      this.ExpiryDelay = Capture.Create<DateTime>(configuration.ExpiryDelay ?? MemoryCacheConfiguration<TKey, TValue>.Default.ExpiryDelay.Value);
      this.MaxCacheLength = CaptureLength.Create(configuration.MaxLength ?? MemoryCacheConfiguration<TKey, TValue>.Default.MaxLength.Value);
      this.MaxCacheSize = CaptureSize.Create(configuration.MaxSize ?? MemoryCacheConfiguration<TKey, TValue>.Default.MaxSize.Value);
      VssCacheExpiryProvider<TKey, TValue> expiryProvider = configuration.ExpiryProviderFactory == null ? new VssCacheExpiryProvider<TKey, TValue>(this.ExpiryInterval, this.InactivityInterval, this.ExpiryDelay, timeProvider) : configuration.ExpiryProviderFactory(this.ExpiryInterval, this.InactivityInterval, this.ExpiryDelay, timeProvider);
      this.m_sizeProvider = configuration.SizeProvider ?? MemoryCacheConfiguration<TKey, TValue>.Default.SizeProvider;
      this.m_cache = new VssMemoryCacheList<TKey, TValue>((IVssCachePerformanceProvider) this, comparer, this.MaxCacheLength, this.MaxCacheSize, this.m_sizeProvider, expiryProvider, timeProvider);
    }

    internal override void InternalInitialize(IVssRequestContext requestContext)
    {
      base.InternalInitialize(requestContext);
      this.m_initialConfiguration = this.CurrentConfiguration;
      requestContext.Trace(10007006, TraceLevel.Info, "MemoryCache", this.Name, "Initial cache configuration: {0}", (object) this.m_initialConfiguration);
      this.m_settingsLock = this.CreateLockName(requestContext, "SettingsLock");
      this.RegistryPath = "/Configuration/Caching/MemoryCache/" + this.Name;
      requestContext.GetService<IVssRegistryService>().RegisterNotification(requestContext, new RegistrySettingsChangedCallback(this.OnConfigurationUpdated), true, this.RegistryPath + "/**");
      this.InternalLoadSettings(requestContext);
      this.RegisterCacheServicing<TKey, TValue>(requestContext, (IMemoryCacheList<TKey, TValue>) this.m_cache, this.m_cleanupInterval);
    }

    internal override void InternalFinalize(IVssRequestContext requestContext)
    {
      this.m_cache.Clear();
      requestContext.GetService<IVssRegistryService>().UnregisterNotification(requestContext, new RegistrySettingsChangedCallback(this.OnConfigurationUpdated));
      base.InternalFinalize(requestContext);
    }

    public virtual bool TryGetValue(IVssRequestContext requestContext, TKey key, out TValue value) => this.m_cache.TryGetValueUpdateTtl<TKey, TValue>(key, out value);

    public virtual bool TryGetValue(
      IVssRequestContext requestContext,
      TKey key,
      out TValue value,
      out DateTime modifiedOn,
      out DateTime accessedOn)
    {
      return this.m_cache.TryGetValueUpdateTtl<TKey, TValue>(key, out value, out modifiedOn, out accessedOn);
    }

    public virtual bool TryPeekValue(IVssRequestContext requestContext, TKey key, out TValue value) => this.m_cache.TryGetValueKeepTtl<TKey, TValue>(key, out value);

    public virtual bool TryPeekValue(
      IVssRequestContext requestContext,
      TKey key,
      out TValue value,
      out DateTime modifiedOn,
      out DateTime accessedOn)
    {
      return this.m_cache.TryGetValueKeepTtl<TKey, TValue>(key, out value, out modifiedOn, out accessedOn);
    }

    public virtual bool TryAdd(IVssRequestContext requestContext, TKey key, TValue value) => this.m_cache.Add(key, value, false);

    public virtual bool TryAdd(
      IVssRequestContext requestContext,
      TKey key,
      TValue value,
      DateTime timestamp)
    {
      return this.m_cache.Add(key, value, timestamp, false);
    }

    public virtual void Set(IVssRequestContext requestContext, TKey key, TValue value) => this.m_cache.Add(key, value, true);

    public virtual void Set(
      IVssRequestContext requestContext,
      TKey key,
      TValue value,
      DateTime timestamp)
    {
      this.m_cache.Add(key, value, timestamp, true);
    }

    public virtual bool Remove(IVssRequestContext requestContext, TKey key) => this.m_cache.Remove(key);

    public virtual void Clear(IVssRequestContext requestContext) => this.m_cache.Clear();

    public VssMemoryCacheList<TKey, TValue> MemoryCache => this.m_cache;

    protected void InternalLoadSettings(IVssRequestContext requestContext)
    {
      RegistryEntryCollection ce = requestContext.GetService<IVssRegistryService>().ReadEntriesFallThru(requestContext, (RegistryQuery) (this.RegistryPath + "/**"));
      using (requestContext.Lock(this.m_settingsLock))
        this.LoadSettings(requestContext, ce);
    }

    protected virtual void LoadSettings(
      IVssRequestContext requestContext,
      RegistryEntryCollection ce)
    {
      this.MaxCacheLength.Value = ce.GetValueFromPath<int>(this.RegistryPath + "/MaxLength", this.m_initialConfiguration.MaxLength.Value);
      this.MaxCacheSize.Value = ce.GetValueFromPath<long>(this.RegistryPath + "/MaxSize", this.m_initialConfiguration.MaxSize.Value);
      this.ExpiryInterval.Value = ce.GetValueFromPath<TimeSpan>(this.RegistryPath + "/ExpiryInterval", this.m_initialConfiguration.ExpiryInterval.Value);
      this.InactivityInterval.Value = ce.GetValueFromPath<TimeSpan>(this.RegistryPath + "/InactivityInterval", this.m_initialConfiguration.InactivityInterval.Value);
      this.ExpiryDelay.Value = ce.GetValueFromPath<DateTime>(this.RegistryPath + "/ExpiryDelay", this.m_initialConfiguration.ExpiryDelay.Value).ToUniversalTime();
      requestContext.Trace(10007007, TraceLevel.Info, "MemoryCache", this.Name, "Effective cache configuration: {0}", (object) this.CurrentConfiguration);
    }

    private void OnConfigurationUpdated(
      IVssRequestContext requestContext,
      RegistryEntryCollection ce)
    {
      this.InternalLoadSettings(requestContext);
    }

    private MemoryCacheConfiguration<TKey, TValue> CurrentConfiguration => new MemoryCacheConfiguration<TKey, TValue>().WithMaxElements(this.MaxCacheLength.Value).WithMaxSize(this.MaxCacheSize.Value, this.m_sizeProvider).WithExpiryInterval(this.ExpiryInterval.Value).WithInactivityInterval(this.InactivityInterval.Value).WithExpiryDelay(this.ExpiryDelay.Value).WithCleanupInterval(this.m_cleanupInterval);
  }
}
