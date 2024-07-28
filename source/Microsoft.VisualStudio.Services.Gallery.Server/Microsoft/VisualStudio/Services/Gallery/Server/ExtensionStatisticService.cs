// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.ExtensionStatisticService
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Gallery.Server.Components;
using Microsoft.VisualStudio.Services.Gallery.WebApi;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Gallery.Server
{
  public class ExtensionStatisticService : IExtensionStatisticService, IVssFrameworkService
  {
    private BatchExecutionHandler<ExtensionStatisticUpdate> _batchExecutionHandler;
    private ExtensionDownloadCountMonitoringRedisCache _extensionDownloadCountMonitoringCache;

    public ExtensionStatisticService() => this._extensionDownloadCountMonitoringCache = new ExtensionDownloadCountMonitoringRedisCache();

    internal ExtensionStatisticService(
      ExtensionDownloadCountMonitoringRedisCache extensionDownloadCountMonitoringCache)
    {
      this._extensionDownloadCountMonitoringCache = extensionDownloadCountMonitoringCache;
    }

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
      IVssRegistryService service = systemRequestContext.GetService<IVssRegistryService>();
      RegistryEntryCollection registryEntryCollection = service.ReadEntries(systemRequestContext, (RegistryQuery) "/Configuration/Service/Gallery/BatchSize/ExtensionStat");
      service.RegisterNotification(systemRequestContext, new RegistrySettingsChangedCallback(this.ConfigurationChangeCallback), false, "/Configuration/Service/Gallery/BatchSize/ExtensionStat");
      int valueFromPath = registryEntryCollection.GetValueFromPath<int>("/Configuration/Service/Gallery/BatchSize/ExtensionStat", 100);
      this.InitBatchHandler(systemRequestContext, valueFromPath);
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
      this._batchExecutionHandler.Flush(systemRequestContext);
      this._batchExecutionHandler = (BatchExecutionHandler<ExtensionStatisticUpdate>) null;
      systemRequestContext.GetService<IVssRegistryService>().UnregisterNotification(systemRequestContext, new RegistrySettingsChangedCallback(this.ConfigurationChangeCallback));
    }

    internal virtual bool BatchProcessor(
      IVssRequestContext requestContext,
      List<ExtensionStatisticUpdate> statistics)
    {
      List<ExtensionStatisticUpdate> statistics1 = new List<ExtensionStatisticUpdate>((IEnumerable<ExtensionStatisticUpdate>) statistics);
      for (int index1 = 0; index1 < statistics1.Count; ++index1)
      {
        int index2 = index1 + 1;
        while (index2 < statistics1.Count)
        {
          if (statistics1[index1].ExtensionName.Equals(statistics1[index2].ExtensionName, StringComparison.OrdinalIgnoreCase) && statistics1[index1].PublisherName.Equals(statistics1[index2].PublisherName, StringComparison.OrdinalIgnoreCase) && statistics1[index1].Statistic.StatisticName.Equals(statistics1[index2].Statistic.StatisticName, StringComparison.OrdinalIgnoreCase))
          {
            switch (statistics1[index2].Operation)
            {
              case ExtensionStatisticOperation.Increment:
                statistics1[index1].Statistic.Value += statistics1[index2].Statistic.Value;
                break;
              case ExtensionStatisticOperation.Decrement:
                statistics1[index1].Statistic.Value -= statistics1[index2].Statistic.Value;
                break;
            }
            statistics1.RemoveAt(index2);
          }
          else
            ++index2;
        }
      }
      using (PublishedExtensionStatisticComponent component = requestContext.CreateComponent<PublishedExtensionStatisticComponent>())
        component.UpdateStatistics((IEnumerable<ExtensionStatisticUpdate>) statistics1);
      return true;
    }

    public bool ShouldUpdateStatCount(
      IVssRequestContext requestContext,
      Guid extensionId,
      string version,
      string statType,
      string targetPlatform = null)
    {
      if (this._extensionDownloadCountMonitoringCache != null)
      {
        if (requestContext.IsFeatureEnabled("Microsoft.VisualStudio.Services.Gallery.PreventDownloadAbuse"))
        {
          try
          {
            string str1 = requestContext.RemoteIPAddress();
            string str2;
            if (str1 == null)
              str2 = (string) null;
            else
              str2 = ((IEnumerable<string>) str1.Split(',')).FirstOrDefault<string>();
            string IPAddress = str2;
            return this._extensionDownloadCountMonitoringCache.ShouldIncrement(requestContext, extensionId, version, IPAddress, statType, targetPlatform);
          }
          catch (Exception ex)
          {
            requestContext.TraceException(12062067, "Gallery", "ExtensionDownloadCountMonitoringRedisCache", ex);
          }
        }
      }
      return true;
    }

    public void IncrementStatCount(
      IVssRequestContext requestContext,
      PublishedExtension publishedExtension,
      string statisticType)
    {
      if (publishedExtension == null)
        return;
      this.UpdateStatistics(requestContext, (IEnumerable<ExtensionStatisticUpdate>) new List<ExtensionStatisticUpdate>()
      {
        new ExtensionStatisticUpdate()
        {
          Statistic = new ExtensionStatistic()
          {
            StatisticName = statisticType,
            Value = 1.0
          },
          Operation = ExtensionStatisticOperation.Increment,
          PublisherName = publishedExtension.Publisher.PublisherName,
          ExtensionName = publishedExtension.ExtensionName
        }
      });
    }

    public void UpdateStatistics(
      IVssRequestContext requestContext,
      IEnumerable<ExtensionStatisticUpdate> statistics)
    {
      if (statistics == null)
        return;
      List<ExtensionStatisticUpdate> extensionStatisticUpdateList;
      if (requestContext.IsFeatureEnabled("Microsoft.VisualStudio.Services.Gallery.EnableBatchingForExtensionStatisticUpdate"))
      {
        extensionStatisticUpdateList = new List<ExtensionStatisticUpdate>();
        foreach (ExtensionStatisticUpdate statistic in statistics)
        {
          switch (statistic.Operation)
          {
            case ExtensionStatisticOperation.Set:
            case ExtensionStatisticOperation.Delete:
              extensionStatisticUpdateList.Add(statistic);
              continue;
            case ExtensionStatisticOperation.Increment:
            case ExtensionStatisticOperation.Decrement:
              this._batchExecutionHandler.Add(requestContext, statistic);
              continue;
            default:
              continue;
          }
        }
      }
      else
        extensionStatisticUpdateList = new List<ExtensionStatisticUpdate>(statistics);
      if (extensionStatisticUpdateList == null || extensionStatisticUpdateList.Count <= 0)
        return;
      using (PublishedExtensionStatisticComponent component = requestContext.CreateComponent<PublishedExtensionStatisticComponent>())
        component.UpdateStatistics(statistics);
    }

    public void UpdateWeightedRating(
      IVssRequestContext requestContext,
      string product,
      IEnumerable<string> installationTargets)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(product, nameof (product));
      ArgumentUtility.CheckEnumerableForEmpty((IEnumerable) installationTargets, nameof (installationTargets));
      using (WeightedRatingComponent component = requestContext.CreateComponent<WeightedRatingComponent>())
        component.UpdateWeightedRating(product, installationTargets);
    }

    private void ConfigurationChangeCallback(
      IVssRequestContext requestContext,
      RegistryEntryCollection changedEntries)
    {
      int valueFromPath = changedEntries.GetValueFromPath<int>("/Configuration/Service/Gallery/BatchSize/ExtensionStat", 100);
      this.InitBatchHandler(requestContext, valueFromPath);
    }

    private void InitBatchHandler(IVssRequestContext requestContext, int batchSize)
    {
      if (this._batchExecutionHandler != null)
      {
        this._batchExecutionHandler.Flush(requestContext);
        this._batchExecutionHandler = (BatchExecutionHandler<ExtensionStatisticUpdate>) null;
      }
      this._batchExecutionHandler = new BatchExecutionHandler<ExtensionStatisticUpdate>(batchSize, new Func<IVssRequestContext, List<ExtensionStatisticUpdate>, bool>(this.BatchProcessor));
    }
  }
}
