// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.Server.DedupValidation.DedupRedisCache
// Assembly: Microsoft.VisualStudio.Services.BlobStore.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: D3AB5C9B-EB54-4477-A304-63BB297414A3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.BlobStore.Server.dll

using BuildXL.Cache.ContentStore.Hashing;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.BlobStore.AzureStorage;
using Microsoft.VisualStudio.Services.BlobStore.Server.Common;
using Microsoft.VisualStudio.Services.Content.Server.Common;
using Microsoft.VisualStudio.Services.Redis;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.BlobStore.Server.DedupValidation
{
  public class DedupRedisCache : IDedupProcessingCache, IRemoteCache<string, string>
  {
    private IRedisCacheService redisCacheService;
    private const string ContainerNamespaceGuid = "0621506e-5b14-48d4-b33f-ec63b044c494";
    private const string RedisContainerIdentifier = "DedupGCRedisCache";
    private CacheBuffer dedupGCRedisBuffer;

    private IMutableDictionaryCacheContainer<string, string> redisInfoCache { get; set; }

    public DedupRedisCache(IVssRequestContext requestContext, int dedupGCRedisMaxBufferCount)
    {
      this.redisCacheService = requestContext.GetService<IRedisCacheService>();
      TimeSpan? nullable = new TimeSpan?(TimeSpan.FromDays(7.0));
      this.redisInfoCache = this.redisCacheService.GetVolatileDictionaryContainer<string, string, DedupRedisCache.DedupRedisToken>(requestContext, new Guid("0621506e-5b14-48d4-b33f-ec63b044c494"), new ContainerSettings()
      {
        KeyExpiry = nullable,
        CiAreaName = "DedupGCRedisCache",
        NoThrowMode = new bool?(true)
      });
      this.dedupGCRedisBuffer = new CacheBuffer((IRemoteCache<string, string>) this, dedupGCRedisMaxBufferCount);
    }

    public void AddValidatedParentChild(DedupIdentifier parent, DedupIdentifier child)
    {
    }

    public bool IsParentChildValidated(DedupIdentifier parent, DedupIdentifier child) => false;

    public IDedupInfo GetDedupInfo(VssRequestPump.Processor processor, DedupIdentifier dedupId)
    {
      string s;
      DateTime result;
      return (IDedupInfo) processor.ExecuteWorkAsync<AzureDedupInfo>((Func<IVssRequestContext, AzureDedupInfo>) (requestContext => this.redisInfoCache.TryGet<string, string>(requestContext, dedupId.ValueString, out s) && DateTime.TryParse(s, out result) ? new AzureDedupInfo(dedupId, result) : (AzureDedupInfo) null)).GetAwaiter().GetResult();
    }

    public void SetDedupInfo(
      VssRequestPump.Processor processor,
      DedupIdentifier dedupId,
      IDedupInfo info)
    {
      if (dedupId == (DedupIdentifier) null)
        return;
      DateTime? keepUntil;
      int num;
      if (info == null)
      {
        num = 1;
      }
      else
      {
        keepUntil = info.KeepUntil;
        num = !keepUntil.HasValue ? 1 : 0;
      }
      if (num != 0)
        return;
      keepUntil = info.KeepUntil;
      if (!keepUntil.HasValue)
        return;
      keepUntil = info.KeepUntil;
      DateTime dateTime = keepUntil.Value;
      this.dedupGCRedisBuffer.Add(processor, dedupId.ValueString, dateTime.ToString());
    }

    public void Add(VssRequestPump.Processor processor, IDictionary<string, string> kvps) => processor.ExecuteWorkAsync((Action<IVssRequestContext>) (requestContext => this.redisInfoCache.Set(requestContext, kvps)));

    public void Reset()
    {
    }

    internal sealed class DedupRedisToken
    {
    }
  }
}
