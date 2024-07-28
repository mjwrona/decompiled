// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Commerce.CommerceRedisCacheProvider
// Assembly: Microsoft.VisualStudio.Services.Commerce, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1A903DA-646E-45AC-891B-7946BA20E824
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Commerce.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Redis;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Microsoft.VisualStudio.Services.Commerce
{
  internal class CommerceRedisCacheProvider : ICommerceDistributedCacheProvider
  {
    private const string Layer = "CommerceRedisCacheProvider";
    private const string Area = "Commerce";
    private const int DefaultCacheExpireInMinutes = 480;
    private Guid cacheNamespaceGuid;
    private string cacheFeatureFlag;
    private ContainerSettings cacheContainerSettings;

    public CommerceRedisCacheProvider(
      Guid cacheNamespace,
      string featureFlagName,
      TimeSpan? cacheExpiration)
    {
      this.cacheNamespaceGuid = cacheNamespace;
      this.cacheFeatureFlag = featureFlagName;
      this.cacheContainerSettings = new ContainerSettings()
      {
        CiAreaName = typeof (CommerceRedisCacheProvider).Name,
        KeyExpiry = new TimeSpan?(cacheExpiration ?? TimeSpan.FromMinutes(480.0))
      };
    }

    public virtual void Invalidate<T>(IVssRequestContext requestContext, string key)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckStringForNullOrEmpty(key, nameof (key));
      try
      {
        IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
        if (vssRequestContext.GetService<IRedisCacheService>().IsEnabled(vssRequestContext))
        {
          this.GetRedisContainer<T>(requestContext).Invalidate<string, T>(requestContext, key);
          requestContext.Trace(5106455, TraceLevel.Verbose, "Commerce", nameof (CommerceRedisCacheProvider), "Cache Invalidation was successful for key: " + key);
        }
        else
          requestContext.Trace(5106456, TraceLevel.Verbose, "Commerce", nameof (CommerceRedisCacheProvider), "RedisCacheService Disabled: Invalidation did not go through for key: " + key);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(5106456, "Commerce", nameof (CommerceRedisCacheProvider), ex);
      }
    }

    public virtual bool TrySet<T>(IVssRequestContext requestContext, string key, T valueToCache)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckStringForNullOrEmpty(key, nameof (key));
      try
      {
        if (this.IsCacheEnabled(requestContext))
        {
          IMutableDictionaryCacheContainer<string, T> redisContainer = this.GetRedisContainer<T>(requestContext);
          requestContext.Trace(5106461, TraceLevel.Verbose, "Commerce", nameof (CommerceRedisCacheProvider), string.Format("Cache Hit while querying for {0} with key: {1} and value: {2}.", (object) typeof (T), (object) key, (object) valueToCache));
          IVssRequestContext requestContext1 = requestContext;
          redisContainer.Set(requestContext1, (IDictionary<string, T>) new Dictionary<string, T>()
          {
            {
              key,
              valueToCache
            }
          });
          return true;
        }
      }
      catch (Exception ex)
      {
        requestContext.TraceException(5106459, "Commerce", nameof (CommerceRedisCacheProvider), ex);
      }
      return false;
    }

    public virtual bool TryGet<T>(IVssRequestContext requestContext, string key, out T cachedValue)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckStringForNullOrEmpty(key, nameof (key));
      try
      {
        if (this.IsCacheEnabled(requestContext))
        {
          if (this.GetRedisContainer<T>(requestContext).TryGet<string, T>(requestContext, key, out cachedValue))
          {
            CommerceKpi.CommerceCacheHit.IncrementByOne(requestContext);
            requestContext.Trace(5106451, TraceLevel.Verbose, "Commerce", nameof (CommerceRedisCacheProvider), string.Format("Cache Hit while querying for {0} with key: {1}.", (object) typeof (T), (object) key));
            return true;
          }
          CommerceKpi.CommerceCacheMiss.IncrementByOne(requestContext);
          requestContext.Trace(5106452, TraceLevel.Verbose, "Commerce", nameof (CommerceRedisCacheProvider), string.Format("Cache Hit while querying for {0} with key: {1}.", (object) typeof (T), (object) key));
        }
      }
      catch (Exception ex)
      {
        requestContext.TraceException(5106458, "Commerce", nameof (CommerceRedisCacheProvider), ex);
      }
      cachedValue = default (T);
      return false;
    }

    public bool IsCacheEnabled(IVssRequestContext requestContext)
    {
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
      int num = vssRequestContext.IsFeatureEnabled(this.cacheFeatureFlag) ? 1 : 0;
      bool flag = vssRequestContext.GetService<IRedisCacheService>().IsEnabled(vssRequestContext);
      if (num != 0 && !flag)
        requestContext.Trace(5106460, TraceLevel.Warning, "Commerce", nameof (CommerceRedisCacheProvider), string.Format("Commerce Cache feature {0} has been enabled for host {1}, but Redis is either disabled or unconfigured", (object) this.cacheFeatureFlag, (object) requestContext.ServiceHost.InstanceId));
      return (num & (flag ? 1 : 0)) != 0;
    }

    private IMutableDictionaryCacheContainer<string, T> GetRedisContainer<T>(
      IVssRequestContext requestContext)
    {
      return requestContext.GetService<IRedisCacheService>().GetVolatileDictionaryContainer<string, T, CommerceRedisCacheProvider.RedisCacheSecurityToken>(requestContext, this.cacheNamespaceGuid, this.cacheContainerSettings);
    }

    internal sealed class RedisCacheSecurityToken
    {
    }
  }
}
