// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.Extension.Cache.BackConsolidatedExtensionsCacheBuilder
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.CircuitBreaker;
using Microsoft.VisualStudio.Services.Gallery.Server.Components;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Microsoft.VisualStudio.Services.Gallery.Server.Extension.Cache
{
  internal abstract class BackConsolidatedExtensionsCacheBuilder
  {
    private const int CircuitBreakerRollingStatisticalWindowInMilliseconds = 60000;
    private const int ExtensionCacheMaxConcurrentRequestsPerMinute = 10;
    private const string CommandKeyForBackConsExtensionCachedDataBuilder = "BackConsExtensionCachedDataBuilder";
    private const string BackConsolidatedExtensionCacheRefreshEventFallbackType = "BackConsolidatedExtensionCacheRefreshFallback";
    private CachedBackConsolidatedExtensionData _previousData;

    private int CachePopulationDbCallTimeOutInSeconds { get; set; } = 50;

    protected virtual int BackConsolidatedCacheTimeOutValueInSeconds { get; set; } = 43200;

    private DateTime CacheLastUpdatedTimeUtc { get; set; } = DateTime.MinValue;

    public CachedBackConsolidatedExtensionData BackConsolidatedExtensionCachedDataBuilder(
      IVssRequestContext requestContext)
    {
      Func<CachedBackConsolidatedExtensionData> run = (Func<CachedBackConsolidatedExtensionData>) (() =>
      {
        this.PublishCustomerIntelligenceEventForCacheRefreshTracing(requestContext, "BackConsolidatedExtensionCacheRefresh-startRunDelegate");
        this.BackConsolidatedCacheTimeOutValueInSeconds = requestContext.GetService<IVssRegistryService>().GetValue<int>(requestContext, (RegistryQuery) "/Configuration/Service/Gallery/BackConsolidation/CacheTimeoutValueInSeconds", this.BackConsolidatedCacheTimeOutValueInSeconds);
        if (this.CacheLastUpdatedTimeUtc >= DateTime.UtcNow.AddSeconds((double) -this.BackConsolidatedCacheTimeOutValueInSeconds))
        {
          this.PublishCustomerIntelligenceEventForCacheRefreshTracing(requestContext, string.Format("BackConsolidatedExtensionCacheRefresh") + "-skipCacheRefresh");
          return this._previousData;
        }
        CachedBackConsolidatedExtensionData consolidatedExtensionData = new CachedBackConsolidatedExtensionData();
        Stopwatch stopwatch = Stopwatch.StartNew();
        using (PublishedExtensionComponent component = requestContext.CreateComponent<PublishedExtensionComponent>())
        {
          foreach (KeyValuePair<string, BackConsolidationMappingEntry> keyValuePair in (IEnumerable<KeyValuePair<string, BackConsolidationMappingEntry>>) component.GetBackConsolidationMapping(requestContext))
            consolidatedExtensionData.VsixIdToExtensionMap.TryAdd<string, BackConsolidationMappingEntry>(keyValuePair.Key, keyValuePair.Value);
          stopwatch.Stop();
          try
          {
            this.PublishCustomerIntelligenceEventForCacheRefresh(requestContext, consolidatedExtensionData.VsixIdToExtensionMap.Count, stopwatch.ElapsedMilliseconds, "BackConsolidatedExtensionCacheRefresh");
          }
          catch
          {
          }
          this.CacheLastUpdatedTimeUtc = DateTime.UtcNow;
          this._previousData = consolidatedExtensionData;
          return consolidatedExtensionData;
        }
      });
      Func<CachedBackConsolidatedExtensionData> fallback = (Func<CachedBackConsolidatedExtensionData>) (() =>
      {
        CustomerIntelligenceData properties = new CustomerIntelligenceData();
        properties.Add("EventType", "BackConsolidatedExtensionCacheRefreshFallback");
        requestContext.GetService<CustomerIntelligenceService>().Publish(requestContext, "Microsoft.VisualStudio.Services.Gallery", "InMemoryQuery", properties);
        return this._previousData != null ? this._previousData : throw new Exception("BackConsolidation InMemoryQuery cache not initialized.");
      });
      CommandSetter setter = CommandSetter.WithGroupKey((CommandGroupKey) "Marketplace.").AndCommandKey((CommandKey) "BackConsExtensionCachedDataBuilder").AndCommandPropertiesDefaults(this.ExtensionCachedDataBuilderCircuitBreakerSettings);
      CommandService<CachedBackConsolidatedExtensionData> commandService = new CommandService<CachedBackConsolidatedExtensionData>(requestContext, setter, run, fallback);
      try
      {
        return commandService.Execute();
      }
      catch (CircuitBreakerException ex)
      {
        int eventId = this.CircuitBreakerThrottlingExceptionID;
        if (ex is CircuitBreakerShortCircuitException)
          eventId = this.CiruitBreakerExceptionId;
        TeamFoundationEventLog.Default.Log(requestContext, ex.Message, eventId, EventLogEntryType.Error);
        throw;
      }
      catch (Exception ex)
      {
        requestContext.TraceAlways(12062113, TraceLevel.Error, "gallery", "ProductExtensionsCacheBuilder", ex.Message);
        throw;
      }
    }

    internal CommandPropertiesSetter ExtensionCachedDataBuilderCircuitBreakerSettings => new CommandPropertiesSetter().WithExecutionTimeout(new TimeSpan(0, 0, this.CachePopulationDbCallTimeOutInSeconds)).WithExecutionMaxRequests(10).WithMetricsRollingStatisticalWindowInMilliseconds(60000);

    public abstract int CacheTimeoutInSeconds { get; set; }

    protected abstract int CiruitBreakerExceptionId { get; set; }

    protected abstract int CircuitBreakerThrottlingExceptionID { get; set; }

    private void PublishCustomerIntelligenceEventForCacheRefresh(
      IVssRequestContext requestContext,
      int entryCount,
      long timeTaken,
      string action)
    {
      CustomerIntelligenceData intelligenceData = new CustomerIntelligenceData();
      intelligenceData.Add(CustomerIntelligenceProperty.Action, action);
      intelligenceData.Add("EntryCount", (double) entryCount);
      intelligenceData.Add("TimeTaken", (double) timeTaken);
      intelligenceData.AddGalleryUserIdentifier(requestContext);
      requestContext.GetService<CustomerIntelligenceService>().Publish(requestContext, "Microsoft.VisualStudio.Services.Gallery", "Extension", intelligenceData);
    }

    private void PublishCustomerIntelligenceEventForCacheRefreshTracing(
      IVssRequestContext requestContext,
      string action)
    {
      try
      {
        CustomerIntelligenceData intelligenceData = new CustomerIntelligenceData();
        intelligenceData.Add(CustomerIntelligenceProperty.Action, action);
        intelligenceData.AddGalleryUserIdentifier(requestContext);
        requestContext.GetService<CustomerIntelligenceService>().Publish(requestContext, "Microsoft.VisualStudio.Services.Gallery", "Extension", intelligenceData);
      }
      catch
      {
      }
    }
  }
}
