// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Identity.IdentityTenantCache
// Assembly: Microsoft.VisualStudio.Services.Identity, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1372DA81-8681-4BFE-8E91-D1AB4333F834
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Identity.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Redis;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Microsoft.VisualStudio.Services.Identity
{
  internal class IdentityTenantCache : IIdentityTenantCache
  {
    private readonly IRedisCacheService m_redisCacheService;
    internal static readonly Guid CacheNamespace = new Guid("B9D469FB-B332-47B4-80A8-5F4C65D70AF2");
    internal static readonly int cacheExpireInMintues = 10;
    internal static readonly TimeSpan CacheExpirationPolicy = TimeSpan.FromMinutes((double) IdentityTenantCache.cacheExpireInMintues);
    internal readonly ContainerSettings CacheContainerSettings = new ContainerSettings()
    {
      KeyExpiry = new TimeSpan?(IdentityTenantCache.CacheExpirationPolicy),
      CiAreaName = typeof (IdentityTenantCache).Name
    };
    private static readonly string s_CacheRegistryRoot = "/Service/IdentityTenantCache/";
    private static readonly string s_GetIdentityTenantCacheSetMinutesRegistryKey = IdentityTenantCache.s_CacheRegistryRoot + "GetIdentityTenantCacheSetMinutes";
    private const string TraceArea = "IdentityTenantCache";
    private const string TraceLayer = "IdentityTenantCache";

    public IdentityTenantCache(IVssRequestContext context)
      : this(context.To(TeamFoundationHostType.Deployment).GetService<IRedisCacheService>())
    {
    }

    internal IdentityTenantCache(IRedisCacheService redisCacheService) => this.m_redisCacheService = redisCacheService;

    public bool TryGetTenants(
      IVssRequestContext requestContext,
      Guid identityId,
      out List<TenantInfo> tenantsResponse)
    {
      requestContext.TraceEnter(2011704, nameof (IdentityTenantCache), nameof (IdentityTenantCache), nameof (TryGetTenants));
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForEmptyGuid(identityId, nameof (identityId));
      tenantsResponse = (List<TenantInfo>) null;
      bool tenants = false;
      IVssRequestContext requestContext1 = requestContext.To(TeamFoundationHostType.Deployment);
      if (this.m_redisCacheService.IsEnabled(requestContext1))
      {
        IMutableDictionaryCacheContainer<Guid, List<TenantInfo>> redisContainer = this.GetRedisContainer(requestContext1);
        try
        {
          tenantsResponse = redisContainer.Get<Guid, List<TenantInfo>>(requestContext1, identityId, (Func<Guid, List<TenantInfo>>) (key =>
          {
            throw new ApplicationException();
          }));
          tenants = true;
        }
        catch (ApplicationException ex)
        {
          tenantsResponse = (List<TenantInfo>) null;
        }
        string message = string.Format("Get cache for tenants return for identity id : {0} : {1}", (object) requestContext.GetUserId(), (object) tenants);
        requestContext.Trace(2011705, TraceLevel.Info, nameof (IdentityTenantCache), nameof (IdentityTenantCache), message);
      }
      requestContext.TraceLeave(2011706, nameof (IdentityTenantCache), nameof (IdentityTenantCache), nameof (TryGetTenants));
      return tenants;
    }

    public void SetTenants(
      IVssRequestContext requestContext,
      Guid identityId,
      List<TenantInfo> tenants)
    {
      requestContext.TraceEnter(2011701, nameof (IdentityTenantCache), nameof (IdentityTenantCache), nameof (SetTenants));
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForEmptyGuid(identityId, nameof (identityId));
      ArgumentUtility.CheckForNull<List<TenantInfo>>(tenants, nameof (tenants));
      IVssRequestContext requestContext1 = requestContext.To(TeamFoundationHostType.Deployment);
      if (this.m_redisCacheService.IsEnabled(requestContext1))
      {
        string message = string.Format("Set cache for tenants return for identity id : {0}", (object) requestContext.GetUserId());
        requestContext.Trace(2011702, TraceLevel.Info, nameof (IdentityTenantCache), nameof (IdentityTenantCache), message);
        this.GetRedisContainer(requestContext1).Set(requestContext1, (IDictionary<Guid, List<TenantInfo>>) new Dictionary<Guid, List<TenantInfo>>()
        {
          {
            identityId,
            tenants
          }
        });
      }
      requestContext.TraceLeave(2011703, nameof (IdentityTenantCache), nameof (IdentityTenantCache), nameof (SetTenants));
    }

    private IMutableDictionaryCacheContainer<Guid, List<TenantInfo>> GetRedisContainer(
      IVssRequestContext requestContext)
    {
      return this.m_redisCacheService.GetVolatileDictionaryContainer<Guid, List<TenantInfo>, IdentityTenantCache.RedisCacheSecurityToken>(requestContext, IdentityTenantCache.CacheNamespace, this.CacheContainerSettings);
    }

    public void InvalidateGetTenants(IVssRequestContext requestContext, Guid identityId) => throw new NotImplementedException();

    private int GetIdentityTenantCacheExpireRegistryKey(IVssRequestContext requestContext) => requestContext.GetService<IVssRegistryService>().GetValue<int>(requestContext, (RegistryQuery) IdentityTenantCache.s_GetIdentityTenantCacheSetMinutesRegistryKey, 10);

    internal sealed class RedisCacheSecurityToken
    {
    }

    private class TracePoints
    {
      public const int IdentityTenantCacheSetTenantsEnter = 2011701;
      public const int IdentityTenantCacheSetTenantsCache = 2011702;
      public const int IdentityTenantCacheSetTenantsExit = 2011703;
      public const int IdentityTenantCacheGetTenantsEnter = 2011704;
      public const int IdentityTenantCacheGetTenantsCache = 2011705;
      public const int IdentityTenantCacheGetTenantsExit = 2011706;
    }
  }
}
