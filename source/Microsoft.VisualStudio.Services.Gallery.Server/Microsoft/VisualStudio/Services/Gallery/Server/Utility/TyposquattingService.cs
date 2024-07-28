// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.Utility.TyposquattingService
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Gallery.Server.Components;
using Microsoft.VisualStudio.Services.Gallery.Server.Extension.Cache;
using Microsoft.VisualStudio.Services.Gallery.Server.Facade.Validation.Typosquatting;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Gallery.Server.Utility
{
  public class TyposquattingService : ITyposquattingService, IVssFrameworkService
  {
    private TyposquattingService.TyposquattingServiceSettings m_settings;
    private static readonly RegistryQuery s_registryQuery = (RegistryQuery) "/Configuration/Service/Gallery/TyposquattingConfigurations/**";
    private const int c_CacheRefreshIntervalInSeconds = 86400;
    private readonly IMemoryCache<TypoSquattingData> m_inMemoryCacheForTypoSquattingData;

    public TyposquattingService() => this.m_inMemoryCacheForTypoSquattingData = (IMemoryCache<TypoSquattingData>) new MemoryCache<TypoSquattingData>(86400, new Func<IVssRequestContext, TypoSquattingData>(this.LoadTyposquattingData));

    public void ServiceStart(IVssRequestContext requestContext)
    {
      requestContext.GetService<IVssRegistryService>().RegisterNotification(requestContext, new RegistrySettingsChangedCallback(this.OnRegistryChanged), false, in TyposquattingService.s_registryQuery);
      Interlocked.CompareExchange<TyposquattingService.TyposquattingServiceSettings>(ref this.m_settings, new TyposquattingService.TyposquattingServiceSettings(requestContext), (TyposquattingService.TyposquattingServiceSettings) null);
    }

    public void ServiceEnd(IVssRequestContext requestContext) => requestContext.GetService<IVssRegistryService>().UnregisterNotification(requestContext, new RegistrySettingsChangedCallback(this.OnRegistryChanged));

    private void OnRegistryChanged(
      IVssRequestContext requestContext,
      RegistryEntryCollection changedEntries)
    {
      Volatile.Write<TyposquattingService.TyposquattingServiceSettings>(ref this.m_settings, new TyposquattingService.TyposquattingServiceSettings(requestContext));
    }

    public bool DoesSimilarExtensionDisplayNameExist(
      IVssRequestContext requestContext,
      string extensionDisplayName,
      string publisherName,
      out HashSet<string> collidedExtensionDisplayNames)
    {
      TyposquattingService.TyposquattingServiceSettings settings = this.m_settings;
      collidedExtensionDisplayNames = new HashSet<string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      HashSet<string> skipList = settings.SkipList;
      return (skipList == null || skipList.Count == 0 || !skipList.Contains(publisherName)) && this.DoesSimilarDisplayNameExist(requestContext, TyposquattingCheckType.ExtensionDisplayName, extensionDisplayName, (this.m_inMemoryCacheForTypoSquattingData.GetCachedData(requestContext) ?? this.LoadTyposquattingData(requestContext)).ExtensionDisplayNames, settings.ExtensionDisplayNameThresholds, ref collidedExtensionDisplayNames);
    }

    public bool DoesSimilarPublisherDisplayNameExist(
      IVssRequestContext requestContext,
      string publisherDisplayName,
      string publisherName,
      out HashSet<string> collidedPublisherDisplayNames)
    {
      TyposquattingService.TyposquattingServiceSettings settings = this.m_settings;
      collidedPublisherDisplayNames = new HashSet<string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      HashSet<string> skipList = settings.SkipList;
      return (skipList == null || skipList.Count == 0 || !skipList.Contains(publisherName)) && this.DoesSimilarDisplayNameExist(requestContext, TyposquattingCheckType.PublisherDisplayName, publisherDisplayName, (this.m_inMemoryCacheForTypoSquattingData.GetCachedData(requestContext) ?? this.LoadTyposquattingData(requestContext)).PublisherDisplayNames, settings.PublisherDisplayNameThresholds, ref collidedPublisherDisplayNames);
    }

    internal bool DoesSimilarDisplayNameExist(
      IVssRequestContext requestContext,
      TyposquattingCheckType checkType,
      string displayName,
      IReadOnlyList<string> protectedDisplayNames,
      IList<TyposquattingThreshold> thresholds,
      ref HashSet<string> collidedDisplayNames)
    {
      bool doesExist = false;
      try
      {
        Stopwatch stopwatch = Stopwatch.StartNew();
        int threshold = TyposquattingThreshold.GetThreshold(displayName, thresholds);
        string normalizedDisplayName = TyposquattingStringNormalization.NormalizeString(displayName);
        ConcurrentBag<string> collisions = new ConcurrentBag<string>();
        Parallel.ForEach<string>((IEnumerable<string>) protectedDisplayNames, (Action<string, ParallelLoopState>) ((protectedDisplayName, loopState) =>
        {
          if (!TyposquattingDistanceCalculation.IsDistanceLessThanOrEqualToThreshold(normalizedDisplayName, TyposquattingStringNormalization.NormalizeString(protectedDisplayName), threshold))
            return;
          collisions.Add(protectedDisplayName);
        }));
        stopwatch.Stop();
        if (collisions.Count != 0)
        {
          collidedDisplayNames = collisions.ToHashSet<string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
          doesExist = true;
        }
        TyposquattingService.LogTelemetry(requestContext, checkType, displayName, collidedDisplayNames, doesExist, stopwatch.Elapsed);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(12062112, "Microsoft.VisualStudio.Services.Gallery", nameof (DoesSimilarDisplayNameExist), ex);
      }
      return doesExist;
    }

    private TypoSquattingData LoadTyposquattingData(IVssRequestContext requestContext)
    {
      TypoSquattingData typoSquattingData = new TypoSquattingData();
      try
      {
        using (PublishedExtensionComponent component = requestContext.CreateComponent<PublishedExtensionComponent>())
          typoSquattingData = component.GetTyposquattingData(requestContext);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(12062112, "Microsoft.VisualStudio.Services.Gallery", nameof (LoadTyposquattingData), ex);
      }
      return typoSquattingData;
    }

    public static void LogTelemetry(
      IVssRequestContext requestContext,
      TyposquattingCheckType checkType,
      string displayName,
      HashSet<string> collidedDisplayNames,
      bool doesExist,
      TimeSpan timeTaken)
    {
      CustomerIntelligenceService service = requestContext.GetService<CustomerIntelligenceService>();
      CustomerIntelligenceData intelligenceData = new CustomerIntelligenceData();
      intelligenceData.Add("CheckType", Enum.GetName(typeof (TyposquattingCheckType), (object) checkType));
      intelligenceData.Add("DisplayName", displayName);
      intelligenceData.Add("DoesSimilarDisplayNameExist", doesExist);
      intelligenceData.Add("CollidedDisplayNames", string.Join(",", (IEnumerable<string>) collidedDisplayNames));
      intelligenceData.Add("TimeTaken", timeTaken.TotalMilliseconds);
      IVssRequestContext requestContext1 = requestContext;
      CustomerIntelligenceData properties = intelligenceData;
      service.Publish(requestContext1, "Microsoft.VisualStudio.Services.Gallery", nameof (TyposquattingService), properties);
    }

    private class TyposquattingServiceSettings
    {
      public readonly IList<TyposquattingThreshold> ExtensionDisplayNameThresholds;
      public readonly IList<TyposquattingThreshold> PublisherDisplayNameThresholds;
      public readonly HashSet<string> SkipList;

      public TyposquattingServiceSettings(IVssRequestContext requestContext)
      {
        RegistryEntryCollection registryEntryCollection = requestContext.GetService<IVssRegistryService>().ReadEntries(requestContext, TyposquattingService.s_registryQuery);
        this.ExtensionDisplayNameThresholds = TyposquattingThreshold.LoadThresholds(requestContext, registryEntryCollection, "ExtensionDisplayNameMatch/Threshold", "0/50:1;50/100:2;100/129:3");
        this.PublisherDisplayNameThresholds = TyposquattingThreshold.LoadThresholds(requestContext, registryEntryCollection, "PublisherDisplayNameMatch/Threshold", "0/50:1;50/100:2;100/129:3");
        this.SkipList = TyposquattingSkipList.LoadSkipList(registryEntryCollection, "Skiplist");
      }
    }
  }
}
