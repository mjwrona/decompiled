// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.ExtensionDownloadCountMonitoringRedisCache
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.BusinessIntelligence;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Redis;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;

namespace Microsoft.VisualStudio.Services.Gallery.Server
{
  public class ExtensionDownloadCountMonitoringRedisCache
  {
    internal static readonly Guid CacheNamespace = new Guid("58b2e69e-54fe-46f9-9ccb-85785f2d5e9c");
    internal static readonly int cacheExpireInMintues = 30;
    private const string TraceLayer = "ExtensionDownloadCountMonitoringRedisCache";

    public virtual bool ShouldIncrement(
      IVssRequestContext requestContext,
      Guid extensionId,
      string version,
      string IPAddress,
      string statType,
      string targetPlatform)
    {
      requestContext.TraceEnter(12062065, "gallery", nameof (ExtensionDownloadCountMonitoringRedisCache), nameof (ShouldIncrement));
      ArgumentUtility.CheckStringForNullOrEmpty(IPAddress, nameof (IPAddress));
      ArgumentUtility.CheckForEmptyGuid(extensionId, nameof (extensionId));
      ArgumentUtility.CheckStringForNullOrEmpty(version, nameof (version));
      ArgumentUtility.CheckStringForNullOrEmpty(statType, nameof (statType));
      string message = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Verifying Redis cache for extension: {0}, version: {1}, statType: {2}", (object) extensionId, (object) version, (object) statType);
      requestContext.Trace(12062065, TraceLevel.Info, "gallery", nameof (ExtensionDownloadCountMonitoringRedisCache), message);
      bool shouldIncrement = true;
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
      DateTime minValue = DateTime.MinValue;
      string empty = string.Empty;
      string key;
      if (string.IsNullOrEmpty(targetPlatform))
        key = extensionId.ToString() + "-" + version + "-" + IPAddress + "-" + statType;
      else
        key = extensionId.ToString() + "-" + version + "-" + (targetPlatform + "-" + IPAddress) + "-" + statType;
      IRedisCacheService service = vssRequestContext.GetService<IRedisCacheService>();
      if (service.IsEnabled(vssRequestContext))
      {
        IMutableDictionaryCacheContainer<string, DateTime> redisContainer = this.GetRedisContainer(service, vssRequestContext);
        ExtensionDownloadCountMonitoringRedisCache.CacheStatus cacheStatus;
        try
        {
          DateTime extensionDownloadHistory = redisContainer.Get<string, DateTime>(vssRequestContext, key, (Func<string, DateTime>) (cacheKey =>
          {
            throw new ApplicationException();
          }));
          cacheStatus = this.GetCacheStatus(vssRequestContext, extensionDownloadHistory);
          if (cacheStatus == ExtensionDownloadCountMonitoringRedisCache.CacheStatus.IPRecent)
            shouldIncrement = false;
        }
        catch (ApplicationException ex)
        {
          cacheStatus = ExtensionDownloadCountMonitoringRedisCache.CacheStatus.NoKey;
        }
        this.PublishTelemetry(requestContext, extensionId, IPAddress, cacheStatus, shouldIncrement);
        if (shouldIncrement)
        {
          DateTime utcNow = DateTime.UtcNow;
          redisContainer.Set(vssRequestContext, (IDictionary<string, DateTime>) new Dictionary<string, DateTime>()
          {
            {
              key,
              utcNow
            }
          });
        }
      }
      requestContext.TraceLeave(12062066, "gallery", nameof (ExtensionDownloadCountMonitoringRedisCache), nameof (ShouldIncrement));
      return shouldIncrement;
    }

    private IMutableDictionaryCacheContainer<string, DateTime> GetRedisContainer(
      IRedisCacheService redisCacheService,
      IVssRequestContext deploymentContext)
    {
      ContainerSettings settings = new ContainerSettings()
      {
        KeyExpiry = new TimeSpan?(this.GetCacheExpiry(deploymentContext)),
        CiAreaName = nameof (ExtensionDownloadCountMonitoringRedisCache)
      };
      return redisCacheService.GetVolatileDictionaryContainer<string, DateTime, ExtensionDownloadCountMonitoringRedisCache.RedisCacheSecurityToken>(deploymentContext, ExtensionDownloadCountMonitoringRedisCache.CacheNamespace, settings);
    }

    private ExtensionDownloadCountMonitoringRedisCache.CacheStatus GetCacheStatus(
      IVssRequestContext requestContext,
      DateTime extensionDownloadHistory)
    {
      TimeSpan cacheExpiry = this.GetCacheExpiry(requestContext);
      if (extensionDownloadHistory == DateTime.MinValue)
        return ExtensionDownloadCountMonitoringRedisCache.CacheStatus.NoCache;
      return extensionDownloadHistory.Add(cacheExpiry) < DateTime.UtcNow ? ExtensionDownloadCountMonitoringRedisCache.CacheStatus.IPNotRecent : ExtensionDownloadCountMonitoringRedisCache.CacheStatus.IPRecent;
    }

    private TimeSpan GetCacheExpiry(IVssRequestContext deploymentContext) => TimeSpan.FromMinutes((double) deploymentContext.GetService<IVssRegistryService>().GetValue<int>(deploymentContext, (RegistryQuery) "/Configuration/Service/Gallery/ExtensionDownloadCountMonitoringCacheExpiry", ExtensionDownloadCountMonitoringRedisCache.cacheExpireInMintues));

    private void PublishTelemetry(
      IVssRequestContext requestContext,
      Guid extensionId,
      string IPAddress,
      ExtensionDownloadCountMonitoringRedisCache.CacheStatus cacheStatus,
      bool shouldIncrement)
    {
      ClientTraceData properties = new ClientTraceData();
      properties.Add("ExtensionId", (object) extensionId);
      properties.Add(nameof (IPAddress), (object) IPAddress);
      properties.Add(nameof (cacheStatus), (object) cacheStatus.ToString());
      properties.Add(nameof (shouldIncrement), (object) shouldIncrement);
      requestContext.GetService<ClientTraceService>().Publish(requestContext, "Microsoft.VisualStudio.Services.Gallery", nameof (ExtensionDownloadCountMonitoringRedisCache), properties);
    }

    internal sealed class RedisCacheSecurityToken
    {
    }

    private enum CacheStatus
    {
      None,
      NoKey,
      NoCache,
      IPNotRecent,
      IPRecent,
    }
  }
}
