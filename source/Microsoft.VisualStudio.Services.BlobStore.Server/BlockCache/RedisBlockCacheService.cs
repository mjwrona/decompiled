// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.Server.BlockCache.RedisBlockCacheService
// Assembly: Microsoft.VisualStudio.Services.BlobStore.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: D3AB5C9B-EB54-4477-A304-63BB297414A3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.BlobStore.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.BlobStore.Common;
using Microsoft.VisualStudio.Services.Redis;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.BlobStore.Server.BlockCache
{
  internal class RedisBlockCacheService : IBlockCacheService, IVssFrameworkService
  {
    private static readonly Guid blockCacheId = new Guid("95826510-c72f-4018-8212-7063c1305888");
    private static readonly string registryPath = "/Configuration/Caching/RedisBlockCacheService";
    private static readonly TimeSpan defaultUploadingExpiry = IBlockCacheServiceConstants.DefaultUploadingTtl;
    private static readonly TimeSpan defaultUploadedExpiry = IBlockCacheServiceConstants.DefaultUploadedTtl;
    private ILockName settingsLock;
    private TimeSpan UploadingExpiryInterval;
    private TimeSpan UploadedExpiryInterval;

    private IMutableDictionaryCacheContainer<Tuple<IDomainId, byte[]>, BlockUploadStatus> GetCacheContainer(
      IVssRequestContext requestContext,
      TimeSpan? ttl = null)
    {
      requestContext.CheckProjectCollectionRequestContext();
      return requestContext.GetService<IRedisCacheService>().GetVolatileDictionaryContainer<Tuple<IDomainId, byte[]>, BlockUploadStatus, RedisBlockCacheService.BlockHashSecurityToken>(requestContext, RedisBlockCacheService.blockCacheId, new ContainerSettings()
      {
        KeyExpiry = ttl,
        CiAreaName = "BlobStoreBlockCache",
        KeySerializer = (IKeySerializer) RedisBlockCacheService.KeySerializer.Instance
      });
    }

    public void ServiceStart(IVssRequestContext requestContext)
    {
      requestContext.CheckProjectCollectionRequestContext();
      this.settingsLock = this.CreateLockName(requestContext, "SettingsLock");
      requestContext.GetService<IVssRegistryService>().RegisterNotification(requestContext, new RegistrySettingsChangedCallback(this.OnConfigurationUpdated), true, (RegistryQuery) (RedisBlockCacheService.registryPath + "/**"));
      this.LoadSettings(requestContext);
    }

    private ILockName CreateLockName(IVssRequestContext requestContext, string name) => requestContext.ServiceHost.CreateUniqueLockName(string.Format("{0}/{1}", (object) this.GetType().FullName, (object) name));

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    private void LoadSettings(IVssRequestContext requestContext)
    {
      RegistryEntryCollection registryEntryCollection = requestContext.GetService<IVssRegistryService>().ReadEntriesFallThru(requestContext, (RegistryQuery) (RedisBlockCacheService.registryPath + "/**"));
      using (requestContext.Lock(this.settingsLock))
      {
        this.UploadingExpiryInterval = registryEntryCollection.GetValueFromPath<TimeSpan>(RedisBlockCacheService.registryPath + "/UploadingExpiryInterval", RedisBlockCacheService.defaultUploadingExpiry);
        this.UploadedExpiryInterval = registryEntryCollection.GetValueFromPath<TimeSpan>(RedisBlockCacheService.registryPath + "/UploadedExpiryInterval", RedisBlockCacheService.defaultUploadedExpiry);
      }
    }

    private void OnConfigurationUpdated(
      IVssRequestContext requestContext,
      RegistryEntryCollection ce)
    {
      this.LoadSettings(requestContext);
    }

    public void SetBlockStatus(
      IVssRequestContext requestContext,
      Tuple<IDomainId, byte[]> key,
      BlockUploadStatus status)
    {
      switch (status)
      {
        case BlockUploadStatus.Unknown:
          this.GetCacheContainer(requestContext).Invalidate<Tuple<IDomainId, byte[]>, BlockUploadStatus>(requestContext, key);
          break;
        case BlockUploadStatus.Uploading:
          this.GetCacheContainer(requestContext, new TimeSpan?(this.UploadingExpiryInterval)).Set(requestContext, (IDictionary<Tuple<IDomainId, byte[]>, BlockUploadStatus>) new Dictionary<Tuple<IDomainId, byte[]>, BlockUploadStatus>()
          {
            {
              key,
              status
            }
          });
          break;
        case BlockUploadStatus.Uploaded:
          this.GetCacheContainer(requestContext, new TimeSpan?(this.UploadedExpiryInterval)).Set(requestContext, (IDictionary<Tuple<IDomainId, byte[]>, BlockUploadStatus>) new Dictionary<Tuple<IDomainId, byte[]>, BlockUploadStatus>()
          {
            {
              key,
              status
            }
          });
          break;
      }
    }

    public BlockUploadStatus GetBlockStatus(
      IVssRequestContext requestContext,
      Tuple<IDomainId, byte[]> key)
    {
      BlockUploadStatus blockUploadStatus;
      return !this.GetCacheContainer(requestContext).TryGet<Tuple<IDomainId, byte[]>, BlockUploadStatus>(requestContext, key, out blockUploadStatus) ? BlockUploadStatus.Unknown : blockUploadStatus;
    }

    private class BlockHashSecurityToken
    {
    }

    private class KeySerializer : IKeySerializer
    {
      public static RedisBlockCacheService.KeySerializer Instance { get; } = new RedisBlockCacheService.KeySerializer();

      public T Deserialize<T>(string data)
      {
        if (!typeof (T).IsEquivalentTo(typeof (byte[])))
          throw new ArgumentException(string.Format("Expect a generic parameter of type {0}, but got parameter of type {1}", (object) typeof (byte[]), (object) typeof (T)), nameof (T));
        return (T) Convert.FromBase64String(data);
      }

      public string Serialize<T>(T value) => value is byte[] inArray ? Convert.ToBase64String(inArray) : throw new ArgumentException(string.Format("Expect a value of type {0}, but got value of type {1}", (object) typeof (byte[]), (object) typeof (T)), nameof (value));
    }
  }
}
