// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.DuplicatePublisherCheckCache
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.BusinessIntelligence;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Redis;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;

namespace Microsoft.VisualStudio.Services.Gallery.Server
{
  public class DuplicatePublisherCheckCache
  {
    internal static readonly Guid CacheNamespace = new Guid("89b5d5f7-82e8-4a80-bc49-68730a38a4f1");
    internal static readonly TimeSpan DefaultDisplayNameKeyExpiry = TimeSpan.FromDays(30.0);

    public virtual bool AddRemovePublishersName(
      IVssRequestContext requestContext,
      IReadOnlyList<string> publisherDisplayNames,
      bool isRemove,
      bool isSha256Encoded = false)
    {
      requestContext.TraceEnter(12062105, "gallery", nameof (DuplicatePublisherCheckCache), nameof (AddRemovePublishersName));
      ArgumentUtility.CheckEnumerableForNullOrEmpty((IEnumerable) publisherDisplayNames, nameof (publisherDisplayNames));
      string message1 = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "adding {0} publishers in Redis cache", (object) publisherDisplayNames.Count);
      requestContext.Trace(12062105, TraceLevel.Info, "gallery", nameof (DuplicatePublisherCheckCache), message1);
      bool flag = true;
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
      IRedisCacheService service = vssRequestContext.GetService<IRedisCacheService>();
      if (service.IsEnabled(vssRequestContext))
      {
        IMutableDictionaryCacheContainer<string, int> redisContainer = this.GetRedisContainer(service, vssRequestContext);
        try
        {
          Dictionary<string, int> dictionary = new Dictionary<string, int>();
          foreach (string publisherDisplayName in (IEnumerable<string>) publisherDisplayNames)
          {
            if (!string.IsNullOrWhiteSpace(publisherDisplayName))
            {
              string key = isSha256Encoded ? publisherDisplayName : GalleryServerUtil.CalculateSHA256Hash(publisherDisplayName.ToUpper());
              if (isRemove)
              {
                try
                {
                  int num = redisContainer.Get<string, int>(vssRequestContext, key, (Func<string, int>) (cacheKey =>
                  {
                    throw new ApplicationException();
                  }));
                  if (num <= 1)
                  {
                    redisContainer.Invalidate<string, int>(requestContext, key);
                    string message2 = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Invalidate {0} display name in Redis cache", (object) publisherDisplayName);
                    requestContext.Trace(12062105, TraceLevel.Info, "gallery", nameof (DuplicatePublisherCheckCache), message2);
                  }
                  else
                  {
                    redisContainer.Set(requestContext, (IDictionary<string, int>) new Dictionary<string, int>()
                    {
                      {
                        key,
                        num - 1
                      }
                    });
                    string message3 = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "decreasing count to {0} for display name {1} in Redis cache", (object) (num - 1), (object) publisherDisplayName);
                    requestContext.Trace(12062105, TraceLevel.Info, "gallery", nameof (DuplicatePublisherCheckCache), message3);
                  }
                }
                catch (ApplicationException ex)
                {
                  requestContext.Trace(12062105, TraceLevel.Info, "gallery", nameof (DuplicatePublisherCheckCache), "Not able to find key to invalidate: " + ex?.ToString());
                }
              }
              else if (dictionary.ContainsKey(key))
                dictionary[key]++;
              else
                dictionary.TryAdd<string, int>(key, 1);
            }
          }
          if (!isRemove)
            redisContainer.Set(requestContext, (IDictionary<string, int>) dictionary);
        }
        catch (Exception ex)
        {
          requestContext.TraceException(12062105, "gallery", nameof (DuplicatePublisherCheckCache), ex);
          flag = false;
        }
        this.PublishTelemetry(requestContext, publisherDisplayNames.Count);
      }
      requestContext.TraceLeave(12062105, "gallery", nameof (DuplicatePublisherCheckCache), nameof (AddRemovePublishersName));
      return flag;
    }

    public virtual bool IsDuplicatePublisherDisplayName(
      IVssRequestContext requestContext,
      string publisherDisplayName)
    {
      requestContext.TraceEnter(12062105, "gallery", nameof (DuplicatePublisherCheckCache), nameof (IsDuplicatePublisherDisplayName));
      ArgumentUtility.CheckStringForNullOrWhiteSpace(publisherDisplayName, nameof (publisherDisplayName));
      string message = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "checking for publisher {0} in Redis cache", (object) publisherDisplayName);
      requestContext.Trace(12062105, TraceLevel.Info, "gallery", nameof (DuplicatePublisherCheckCache), message);
      bool flag = false;
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
      IRedisCacheService service = vssRequestContext.GetService<IRedisCacheService>();
      if (service.IsEnabled(vssRequestContext))
      {
        IMutableDictionaryCacheContainer<string, int> redisContainer = this.GetRedisContainer(service, vssRequestContext);
        try
        {
          string shA256Hash = GalleryServerUtil.CalculateSHA256Hash(publisherDisplayName.ToUpper());
          return redisContainer.Get<string, int>(vssRequestContext, shA256Hash, (Func<string, int>) (cacheKey =>
          {
            throw new ApplicationException();
          })) > 0;
        }
        catch (ApplicationException ex)
        {
          flag = false;
        }
        catch (Exception ex)
        {
          requestContext.TraceException(12062105, "gallery", nameof (DuplicatePublisherCheckCache), ex);
        }
      }
      requestContext.TraceLeave(12062105, "gallery", nameof (DuplicatePublisherCheckCache), nameof (IsDuplicatePublisherDisplayName));
      return flag;
    }

    private IMutableDictionaryCacheContainer<string, int> GetRedisContainer(
      IRedisCacheService redisCacheService,
      IVssRequestContext deploymentContext)
    {
      ContainerSettings settings = new ContainerSettings()
      {
        CiAreaName = nameof (DuplicatePublisherCheckCache),
        KeyExpiry = new TimeSpan?(DuplicatePublisherCheckCache.DefaultDisplayNameKeyExpiry)
      };
      return redisCacheService.GetVolatileDictionaryContainer<string, int, DuplicatePublisherCheckCache.RedisCacheSecurityToken>(deploymentContext, DuplicatePublisherCheckCache.CacheNamespace, settings);
    }

    private void PublishTelemetry(IVssRequestContext requestContext, int publisherCount)
    {
      ClientTraceData properties = new ClientTraceData();
      properties.Add(nameof (publisherCount), (object) publisherCount);
      requestContext.GetService<ClientTraceService>().Publish(requestContext, "Microsoft.VisualStudio.Services.Gallery", nameof (DuplicatePublisherCheckCache), properties);
    }

    internal sealed class RedisCacheSecurityToken
    {
    }

    private enum CacheStatus
    {
      None,
      NoKey,
      NoCache,
    }
  }
}
