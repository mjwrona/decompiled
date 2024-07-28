// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.Server.BlockCache.LocalBlockCacheService
// Assembly: Microsoft.VisualStudio.Services.BlobStore.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: D3AB5C9B-EB54-4477-A304-63BB297414A3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.BlobStore.Server.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.BlobStore.Common;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.BlobStore.Server.BlockCache
{
  public class LocalBlockCacheService : VssCacheService, IBlockCacheService, IVssFrameworkService
  {
    private static readonly Capture<TimeSpan> inactivityInterval = Capture.Create<TimeSpan>(VssCacheExpiryProvider.NoExpiry);
    private static readonly TimeSpan cleanupInterval = VssCacheService.NoCleanup;
    private static readonly TimeSpan defaultUploadingExpiry = IBlockCacheServiceConstants.DefaultUploadingTtl;
    private static readonly TimeSpan defaultUploadedExpiry = IBlockCacheServiceConstants.DefaultUploadedTtl;
    private static readonly int defaultMaxCacheLength = IBlockCacheServiceConstants.DefaultMaxCacheLength;
    private static readonly string registryPath = "/Configuration/Caching/LocalBlockCacheService";
    private Capture<TimeSpan> UploadingExpiryInterval;
    private Capture<TimeSpan> UploadedExpiryInterval;
    private CaptureLength MaxCacheLength;
    private readonly VssCacheExpiryProvider<Tuple<IDomainId, byte[]>, BlockUploadStatus> UploadingExpiryProvider;
    private readonly VssCacheExpiryProvider<Tuple<IDomainId, byte[]>, BlockUploadStatus> UploadedExpiryProvider;
    private readonly VssMemoryCacheList<Tuple<IDomainId, byte[]>, BlockUploadStatus> cache;
    private ILockName settingsLock;

    public LocalBlockCacheService()
    {
      this.UploadingExpiryInterval = Capture.Create<TimeSpan>(LocalBlockCacheService.defaultUploadingExpiry);
      this.UploadedExpiryInterval = Capture.Create<TimeSpan>(LocalBlockCacheService.defaultUploadedExpiry);
      this.MaxCacheLength = CaptureLength.Create(LocalBlockCacheService.defaultMaxCacheLength);
      this.UploadingExpiryProvider = new VssCacheExpiryProvider<Tuple<IDomainId, byte[]>, BlockUploadStatus>(this.UploadingExpiryInterval, LocalBlockCacheService.inactivityInterval);
      this.UploadedExpiryProvider = new VssCacheExpiryProvider<Tuple<IDomainId, byte[]>, BlockUploadStatus>(this.UploadedExpiryInterval, LocalBlockCacheService.inactivityInterval);
      this.cache = new VssMemoryCacheList<Tuple<IDomainId, byte[]>, BlockUploadStatus>((IVssCachePerformanceProvider) this, (IEqualityComparer<Tuple<IDomainId, byte[]>>) new LocalBlockCacheService.LocalBlockCacheKeyComparer(), this.MaxCacheLength);
    }

    protected override void ServiceStart(IVssRequestContext requestContext)
    {
      this.settingsLock = this.CreateLockName(requestContext, "SettingsLock");
      requestContext.GetService<IVssRegistryService>().RegisterNotification(requestContext, new RegistrySettingsChangedCallback(this.OnConfigurationUpdated), true, (RegistryQuery) (LocalBlockCacheService.registryPath + "/**"));
      this.LoadSettings(requestContext);
      this.RegisterCacheServicing<Tuple<IDomainId, byte[]>, BlockUploadStatus>(requestContext, (IMemoryCacheList<Tuple<IDomainId, byte[]>, BlockUploadStatus>) this.cache, LocalBlockCacheService.cleanupInterval);
      base.ServiceStart(requestContext);
    }

    protected override void ServiceEnd(IVssRequestContext requestContext)
    {
      this.cache.Clear();
      requestContext.GetService<IVssRegistryService>().UnregisterNotification(requestContext, new RegistrySettingsChangedCallback(this.OnConfigurationUpdated));
      base.ServiceEnd(requestContext);
    }

    private void LoadSettings(IVssRequestContext requestContext)
    {
      RegistryEntryCollection registryEntryCollection = requestContext.GetService<IVssRegistryService>().ReadEntriesFallThru(requestContext, (RegistryQuery) (LocalBlockCacheService.registryPath + "/**"));
      using (requestContext.Lock(this.settingsLock))
      {
        this.MaxCacheLength.Value = registryEntryCollection.GetValueFromPath<int>(LocalBlockCacheService.registryPath + "/MaxLength", LocalBlockCacheService.defaultMaxCacheLength);
        this.UploadingExpiryInterval.Value = registryEntryCollection.GetValueFromPath<TimeSpan>(LocalBlockCacheService.registryPath + "/UploadingExpiryInterval", LocalBlockCacheService.defaultUploadingExpiry);
        this.UploadedExpiryInterval.Value = registryEntryCollection.GetValueFromPath<TimeSpan>(LocalBlockCacheService.registryPath + "/UploadedExpiryInterval", LocalBlockCacheService.defaultUploadedExpiry);
      }
    }

    private void OnConfigurationUpdated(
      IVssRequestContext requestContext,
      RegistryEntryCollection ce)
    {
      this.LoadSettings(requestContext);
    }

    public BlockUploadStatus GetBlockStatus(
      IVssRequestContext requestContext,
      Tuple<IDomainId, byte[]> key)
    {
      BlockUploadStatus blockUploadStatus;
      return !this.cache.TryGetValue(key, out blockUploadStatus) ? BlockUploadStatus.Unknown : blockUploadStatus;
    }

    public void SetBlockStatus(
      IVssRequestContext requestContext,
      Tuple<IDomainId, byte[]> key,
      BlockUploadStatus status)
    {
      switch (status)
      {
        case BlockUploadStatus.Unknown:
          this.cache.Remove(key);
          break;
        case BlockUploadStatus.Uploading:
          this.cache.Add(key, status, true, this.UploadingExpiryProvider);
          break;
        case BlockUploadStatus.Uploaded:
          this.cache.Add(key, status, true, this.UploadedExpiryProvider);
          break;
      }
    }

    private sealed class LocalBlockCacheKeyComparer : IEqualityComparer<Tuple<IDomainId, byte[]>>
    {
      public bool Equals(Tuple<IDomainId, byte[]> x, Tuple<IDomainId, byte[]> y)
      {
        if (x == y)
          return true;
        int? length1 = x.Item2?.Length;
        int? length2 = y.Item2?.Length;
        if (!(length1.GetValueOrDefault() == length2.GetValueOrDefault() & length1.HasValue == length2.HasValue) || !x.Item1.Equals(y.Item1))
          return false;
        for (int index = 0; index < x.Item2.Length; ++index)
        {
          if ((int) x.Item2[index] != (int) y.Item2[index])
            return false;
        }
        return true;
      }

      public int GetHashCode(Tuple<IDomainId, byte[]> obj)
      {
        int length = obj.Item2.Length;
        return 21 + 27 * BitConverter.ToInt32(obj.Item2, 0) + 27 * BitConverter.ToInt32(obj.Item2, length - 4) + 27 * BitConverter.ToInt32(obj.Item2, length / 2 - 2);
      }
    }
  }
}
