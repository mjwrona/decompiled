// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Commerce.CommerceCache
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
  internal class CommerceCache : ICommerceCache, IVssFrameworkService
  {
    private const string Layer = "CommerceCache";
    private const string Area = "Commerce";
    internal static readonly int defaultCacheExpireInMinutes = 60;
    private TimeSpan cacheExpirationPolicy = TimeSpan.FromMinutes((double) CommerceCache.defaultCacheExpireInMinutes);
    private Guid cacheNamespaceGuid = new Guid("5b2bc4b6-dde2-470f-961b-3f92d473a7f4");
    private string cacheFeatureFlag = "VisualStudio.Services.Commerce.EnableGetCloudServiceCache";
    private ContainerSettings cacheContainerSettings = new ContainerSettings()
    {
      CiAreaName = typeof (CommerceCache).Name
    };

    public void ServiceStart(IVssRequestContext requestContext) => requestContext.Trace(5106453, TraceLevel.Info, "Commerce", nameof (CommerceCache), "CommerceCache service starting");

    public void ServiceEnd(IVssRequestContext requestContext) => requestContext.Trace(5106454, TraceLevel.Info, "Commerce", nameof (CommerceCache), "CommerceCache service ending");

    public void SetupCache(Guid cacheNamespace, string featureFlagName, TimeSpan? cacheExpiration)
    {
      this.cacheNamespaceGuid = cacheNamespace;
      this.cacheFeatureFlag = featureFlagName;
      this.cacheExpirationPolicy = cacheExpiration ?? this.cacheExpirationPolicy;
    }

    public virtual void Invalidate<T>(IVssRequestContext requestContext, string key)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckStringForNullOrEmpty(key, nameof (key));
      try
      {
        if (this.IsCacheEnabled(requestContext))
        {
          this.GetRedisContainer<T>(requestContext).Invalidate<string, T>(requestContext, key);
          requestContext.Trace(5106455, TraceLevel.Verbose, "Commerce", nameof (CommerceCache), "Cache Invalidation was successful for key: " + key);
        }
        else
          requestContext.Trace(5106456, TraceLevel.Verbose, "Commerce", nameof (CommerceCache), "CommerceCache or RedisCacheService Disabled: Invalidation did not go through for key: " + key);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(5106456, "Commerce", nameof (CommerceCache), ex);
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
          requestContext.Trace(5106461, TraceLevel.Verbose, "Commerce", nameof (CommerceCache), string.Format("TrySet. Cache Hit while querying for {0} with key: {1} and value: {2}.", (object) typeof (T), (object) key, (object) valueToCache));
          IVssRequestContext requestContext1 = requestContext;
          return redisContainer.Set(requestContext1, (IDictionary<string, T>) new Dictionary<string, T>()
          {
            {
              key,
              valueToCache
            }
          });
        }
      }
      catch (Exception ex)
      {
        requestContext.TraceException(5106459, "Commerce", nameof (CommerceCache), ex);
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
            requestContext.Trace(5106451, TraceLevel.Verbose, "Commerce", nameof (CommerceCache), string.Format("Cache Hit while querying for {0} with key: {1}.", (object) typeof (T), (object) key));
            return true;
          }
          CommerceKpi.CommerceCacheMiss.IncrementByOne(requestContext);
          requestContext.Trace(5106452, TraceLevel.Verbose, "Commerce", nameof (CommerceCache), string.Format("Cache Miss while querying for {0} with key: {1}.", (object) typeof (T), (object) key));
        }
      }
      catch (Exception ex)
      {
        requestContext.TraceException(5106458, "Commerce", nameof (CommerceCache), ex);
      }
      cachedValue = default (T);
      return false;
    }

    internal virtual IMutableDictionaryCacheContainer<string, T> GetRedisContainer<T>(
      IVssRequestContext requestContext)
    {
      this.cacheContainerSettings.KeyExpiry = new TimeSpan?(this.cacheExpirationPolicy);
      return requestContext.GetService<IRedisCacheService>().GetVolatileDictionaryContainer<string, T, CommerceCache.RedisCacheSecurityToken>(requestContext, this.cacheNamespaceGuid, this.cacheContainerSettings);
    }

    internal virtual bool IsCacheEnabled(IVssRequestContext requestContext)
    {
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
      int num = vssRequestContext.IsFeatureEnabled(this.cacheFeatureFlag) ? 1 : 0;
      bool flag = vssRequestContext.GetService<IRedisCacheService>().IsEnabled(vssRequestContext);
      if (num != 0 && !flag)
        requestContext.Trace(5106460, TraceLevel.Warning, "Commerce", nameof (CommerceCache), string.Format("Commerce Cache feature {0} has been enabled for host {1}, but Redis is either disabled or unconfigured", (object) this.cacheFeatureFlag, (object) requestContext.ServiceHost.InstanceId));
      return (num & (flag ? 1 : 0)) != 0;
    }

    internal sealed class RedisCacheSecurityToken
    {
    }
  }
}
