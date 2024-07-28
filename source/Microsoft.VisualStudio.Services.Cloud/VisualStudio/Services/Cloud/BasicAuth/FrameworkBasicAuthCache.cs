// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.BasicAuth.FrameworkBasicAuthCache
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Redis;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Cloud.BasicAuth
{
  internal class FrameworkBasicAuthCache : IFrameworkBasicAuthCache, IVssFrameworkService
  {
    private IRedisCacheService m_redisCacheService;
    private const string TraceArea = "FrameworkBasicAuthCache";
    private const string TraceLayer = "Cache";
    internal static readonly Guid CacheNamespace = new Guid("F4C6C7E6-D4C2-4EE4-A941-70C2CEE4EC83");
    internal static readonly TimeSpan CacheExpirationPolicy = TimeSpan.FromMinutes(10.0);
    internal static readonly ContainerSettings CacheContainerSettings = new ContainerSettings()
    {
      KeyExpiry = new TimeSpan?(FrameworkBasicAuthCache.CacheExpirationPolicy),
      CiAreaName = typeof (FrameworkBasicAuthCache).Name,
      NoThrowMode = new bool?(true)
    };

    public FrameworkBasicAuthCache()
    {
    }

    internal FrameworkBasicAuthCache(IRedisCacheService redisCacheService) => this.m_redisCacheService = redisCacheService;

    public void ServiceStart(IVssRequestContext requestContext) => this.m_redisCacheService = requestContext.GetService<IRedisCacheService>();

    public void ServiceEnd(IVssRequestContext requestContext)
    {
    }

    public bool TryGetHash(
      IVssRequestContext requestContext,
      Guid identityId,
      out byte[] cachedHash)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForEmptyGuid(identityId, nameof (identityId));
      cachedHash = (byte[]) null;
      if (this.m_redisCacheService.IsEnabled(requestContext))
      {
        IMutableDictionaryCacheContainer<Guid, byte[]> redisContainer = this.GetRedisContainer(requestContext);
        try
        {
          redisContainer.TryGet<Guid, byte[]>(requestContext, identityId, out cachedHash);
        }
        catch (Exception ex)
        {
          requestContext.TraceException(1007055, nameof (FrameworkBasicAuthCache), "Cache", ex);
        }
      }
      return cachedHash != null && cachedHash.Length != 0;
    }

    public bool TrySetHash(IVssRequestContext requestContext, Guid identityId, byte[] hash)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForEmptyGuid(identityId, nameof (identityId));
      ArgumentUtility.CheckForNull<byte[]>(hash, nameof (hash));
      ArgumentUtility.CheckForOutOfRange(hash.Length, nameof (hash), 1);
      bool flag = false;
      if (this.m_redisCacheService.IsEnabled(requestContext))
      {
        IMutableDictionaryCacheContainer<Guid, byte[]> redisContainer = this.GetRedisContainer(requestContext);
        try
        {
          flag = redisContainer.Set(requestContext, (IDictionary<Guid, byte[]>) new Dictionary<Guid, byte[]>()
          {
            {
              identityId,
              hash
            }
          });
        }
        catch (Exception ex)
        {
          requestContext.TraceException(1007053, nameof (FrameworkBasicAuthCache), "Cache", ex);
        }
      }
      return flag;
    }

    public bool TryInvalidateHash(IVssRequestContext requestContext, Guid identityId)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForEmptyGuid(identityId, nameof (identityId));
      bool flag = false;
      if (this.m_redisCacheService.IsEnabled(requestContext))
      {
        this.GetRedisContainer(requestContext).Invalidate<Guid, byte[]>(requestContext, identityId);
        flag = true;
      }
      return flag;
    }

    private IMutableDictionaryCacheContainer<Guid, byte[]> GetRedisContainer(
      IVssRequestContext requestContext)
    {
      return this.m_redisCacheService.GetVolatileDictionaryContainer<Guid, byte[], FrameworkBasicAuthCache.RedisCacheSecurityToken>(requestContext, FrameworkBasicAuthCache.CacheNamespace, FrameworkBasicAuthCache.CacheContainerSettings);
    }

    internal sealed class RedisCacheSecurityToken
    {
    }
  }
}
