// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.Extension.Cache.ProductExtensionsCacheBuilder
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.CircuitBreaker;
using Microsoft.VisualStudio.Services.Gallery.WebApi;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Microsoft.VisualStudio.Services.Gallery.Server.Extension.Cache
{
  internal abstract class ProductExtensionsCacheBuilder
  {
    protected const int DbCallPageSize = 2000;
    private const int CircuitBreakerRollingStatisticalWindowInMilliseconds = 60000;
    private const int ExtensionCacheMaxConcurrentRequestsPerMinute = 10;
    private const string CommandKeyForProductExtensionCachedDataBuilder = "{0}ExtensionCachedDataBuilder";
    private const string ProductExtensionCacheRefreshEventFallbackType = "{0}ExtensionCacheRefreshFallback";
    private CachedExtensionData _previousData;

    protected IPublishedExtensionService m_PublishedExtensionService { get; set; }

    private IProductStatisticService m_ProductStatisticService { get; set; }

    private int CachePopulationDbCallTimeOutInSeconds { get; set; } = 50;

    protected virtual int CacheTimeOutOverridenValueInSeconds { get; set; } = 1800;

    private DateTime CacheLastUpdatedTimeinUtc { get; set; } = DateTime.MinValue;

    public CachedExtensionData ExtensionCachedDataBuilder(IVssRequestContext requestContext)
    {
      Func<CachedExtensionData> run = (Func<CachedExtensionData>) (() =>
      {
        this.PublishCustomerIntelligenceEventForCacheRefreshTracing(requestContext, string.Format("{0}ExtensionCacheRefresh", (object) this.ProductType) + "-startRunDelegate");
        IVssRegistryService service = requestContext.GetService<IVssRegistryService>();
        this.CachePopulationDbCallTimeOutInSeconds = service.GetValue<int>(requestContext, (RegistryQuery) "/Configuration/Service/Gallery/ExtensionCachePopulator/DbCallTimeoutInSeconds", this.CachePopulationDbCallTimeOutInSeconds);
        CachedExtensionData cachedExtensionData = new CachedExtensionData();
        if (requestContext.IsFeatureEnabled("Microsoft.VisualStudio.Services.Gallery.EnableOverrideProductExtensionsCacheTimeOut") && this.ProductType == "vscode")
        {
          this.CacheTimeOutOverridenValueInSeconds = service.GetValue<int>(requestContext, (RegistryQuery) "/Configuration/Service/Gallery/ExtensionCachePopulator/CacheTimeoutOverridenValueInSeconds", this.CacheTimeOutOverridenValueInSeconds);
          if (this.CacheLastUpdatedTimeinUtc >= DateTime.UtcNow.AddSeconds((double) -this.CacheTimeOutOverridenValueInSeconds))
          {
            this.PublishCustomerIntelligenceEventForCacheRefreshTracing(requestContext, string.Format("{0}ExtensionCacheRefresh", (object) this.ProductType) + "-skipCacheRefresh");
            return this._previousData;
          }
        }
        Stopwatch stopwatch = Stopwatch.StartNew();
        int pageNumber = 1;
        List<PublishedExtension> extensionPage;
        // ISSUE: explicit non-virtual call
        do
        {
          extensionPage = this.GetExtensionPage(requestContext, pageNumber);
          cachedExtensionData.AddExtensions(extensionPage);
          ++pageNumber;
        }
        while (extensionPage != null && __nonvirtual (extensionPage.Count) == 2000);
        IProductStatisticService statisticService = this.m_ProductStatisticService ?? requestContext.GetService<IProductStatisticService>();
        cachedExtensionData.AverageRating = statisticService.GetProductStatistic(requestContext, this.ProductType, "averagerating");
        cachedExtensionData.MinVotesRequired = (int) statisticService.GetProductStatistic(requestContext, this.ProductType, "minvotesrequired");
        if (cachedExtensionData.MinVotesRequired <= 0)
          cachedExtensionData.MinVotesRequired = 10;
        stopwatch.Stop();
        try
        {
          this.PublishCustomerIntelligenceEventForCacheRefresh(requestContext, cachedExtensionData.GetExtensionCount(), stopwatch.ElapsedMilliseconds, string.Format("{0}ExtensionCacheRefresh", (object) this.ProductType));
        }
        catch
        {
        }
        this.CacheLastUpdatedTimeinUtc = DateTime.UtcNow;
        this._previousData = cachedExtensionData;
        return cachedExtensionData;
      });
      Func<CachedExtensionData> fallback = (Func<CachedExtensionData>) (() =>
      {
        CustomerIntelligenceData properties = new CustomerIntelligenceData();
        properties.Add("EventType", string.Format("{0}ExtensionCacheRefreshFallback", (object) this.ProductType));
        requestContext.GetService<CustomerIntelligenceService>().Publish(requestContext, "Microsoft.VisualStudio.Services.Gallery", "InMemoryQuery", properties);
        return this._previousData != null ? this._previousData : throw new Exception(string.Format("{0} InMemoryQuery cache not initialized.", (object) this.ProductType));
      });
      CommandSetter setter = CommandSetter.WithGroupKey((CommandGroupKey) "Marketplace.").AndCommandKey((CommandKey) string.Format("{0}ExtensionCachedDataBuilder", (object) this.ProductType)).AndCommandPropertiesDefaults(this.ExtensionCachedDataBuilderCircuitBreakerSettings);
      CommandService<CachedExtensionData> commandService = new CommandService<CachedExtensionData>(requestContext, setter, run, fallback);
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
        requestContext.TraceAlways(12062092, TraceLevel.Error, "gallery", nameof (ProductExtensionsCacheBuilder), ex.Message);
        throw;
      }
    }

    public void SetPublishedExtensionService(
      PublishedExtensionService publishedExtensionService)
    {
      this.m_PublishedExtensionService = (IPublishedExtensionService) publishedExtensionService;
    }

    internal CommandPropertiesSetter ExtensionCachedDataBuilderCircuitBreakerSettings => new CommandPropertiesSetter().WithExecutionTimeout(new TimeSpan(0, 0, this.CachePopulationDbCallTimeOutInSeconds)).WithExecutionMaxRequests(10).WithMetricsRollingStatisticalWindowInMilliseconds(60000);

    protected abstract List<PublishedExtension> GetExtensionPage(
      IVssRequestContext requestContext,
      int pageNumber);

    public abstract int CacheTimeoutInSeconds { get; set; }

    protected abstract string ProductType { get; set; }

    protected abstract int CiruitBreakerExceptionId { get; set; }

    protected abstract int CircuitBreakerThrottlingExceptionID { get; set; }

    protected List<FilterCriteria> GetInstallationTargetFilters()
    {
      List<FilterCriteria> installationTargetFilters = new List<FilterCriteria>();
      foreach (string str in GalleryUtil.GetInstallationTargetsForProduct(this.ProductType))
        installationTargetFilters.Add(new FilterCriteria()
        {
          FilterType = 8,
          Value = str
        });
      return installationTargetFilters;
    }

    private void PublishCustomerIntelligenceEventForCacheRefresh(
      IVssRequestContext requestContext,
      int extensionCount,
      long timeTaken,
      string action)
    {
      CustomerIntelligenceData intelligenceData = new CustomerIntelligenceData();
      intelligenceData.Add(CustomerIntelligenceProperty.Action, action);
      intelligenceData.Add("ExtensionCount", (double) extensionCount);
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
