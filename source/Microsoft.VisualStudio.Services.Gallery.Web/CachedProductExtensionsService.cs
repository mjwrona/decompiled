// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Web.CachedProductExtensionsService
// Assembly: Microsoft.VisualStudio.Services.Gallery.Web, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 17D36576-2EF3-4ABC-94BA-AF7891D15A3A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Web.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Gallery.Server;
using Microsoft.VisualStudio.Services.Gallery.Server.Extension.Cache;
using Microsoft.VisualStudio.Services.Gallery.Web.Helpers;
using Microsoft.VisualStudio.Services.Gallery.Web.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Gallery.Web
{
  public class CachedProductExtensionsService : 
    VssMemoryCacheService<string, object>,
    IProductExtensionsDataProvider
  {
    internal const string FEATURED_CATEGORY_NAME = "Featured";
    internal const string MOST_POPULAR_ITEMS_NAME = "Most Popular";
    internal const string TRENDING_WEEKLY = "TrendingWeekly";
    internal const string TRENDING_DAILY = "TrendingDaily";
    internal const string TRENDING_MONTHLY = "TrendingMonthly";
    private static readonly long s_refreshIntervalTicks = TimeSpan.FromMinutes(30.0).Ticks;
    private ConcurrentDictionary<string, long> s_lastRefreshTimeTicks = new ConcurrentDictionary<string, long>();
    private IMemoryCache<object> _vsInMemoryCache;
    private IMemoryCache<object> _vsformacInMemoryCache;
    private IMemoryCache<object> _vstsInMemoryCache;
    private IMemoryCache<object> _vscodeInMemoryCache;
    private IMemoryCache<object> _vssubsInMemoryCache;
    private readonly int cacheRefreshIntervalInSeconds = 1800;

    public virtual object GetProductExtensions(IVssRequestContext requestContext, string product)
    {
      if (string.IsNullOrEmpty(product))
        throw new ArgumentException("Argument product cannot be null");
      if (!requestContext.IsFeatureEnabled("Microsoft.VisualStudio.Services.Gallery.HomepageCacheReadAhead"))
      {
        object pe = (object) null;
        long ticks = this.DateTimeWrapper.Now.Ticks;
        long nextRefreshTicks = ticks + CachedProductExtensionsService.s_refreshIntervalTicks;
        long lastRefreshTimeTick = this.s_lastRefreshTimeTicks.ContainsKey(product) ? this.s_lastRefreshTimeTicks[product] : 0L;
        if (((!this.TryGetValue(requestContext, product, out pe) ? 1 : 0) | (ticks < lastRefreshTimeTick ? (false ? 1 : 0) : (this.UpdateRefreshTime(product, nextRefreshTicks, lastRefreshTimeTick) ? 1 : 0))) != 0)
        {
          pe = this.GetProductExtensionsInternal(requestContext, product);
          this.Set(requestContext, product, pe);
        }
        if (!product.Equals("subscriptions", StringComparison.InvariantCultureIgnoreCase))
          pe = (object) new ProductExtensions(pe as ProductExtensions);
        return pe;
      }
      switch (product.ToLower(CultureInfo.InvariantCulture))
      {
        case "vs":
          return (object) new ProductExtensions((ProductExtensions) this._vsInMemoryCache.GetCachedData(requestContext));
        case "vsformac":
          return (object) new ProductExtensions((ProductExtensions) this._vsformacInMemoryCache.GetCachedData(requestContext));
        case "vsts":
          return (object) new ProductExtensions((ProductExtensions) this._vstsInMemoryCache.GetCachedData(requestContext));
        case "vscode":
          return (object) new ProductExtensions((ProductExtensions) this._vscodeInMemoryCache.GetCachedData(requestContext));
        case "subscriptions":
          return this._vssubsInMemoryCache.GetCachedData(requestContext);
        default:
          return (object) null;
      }
    }

    private void TrimExtensionsForTrending(ExtensionPerCategory extensionPerCategory)
    {
      if (extensionPerCategory == null || extensionPerCategory.Extensions == null || extensionPerCategory.Extensions.Count <= 18)
        return;
      extensionPerCategory.Extensions.RemoveRange(18, extensionPerCategory.Extensions.Count - 18);
    }

    internal void DedupeFromFeaturedExtensions(
      IVssRequestContext requestContext,
      List<BaseExtensionItem> featuredCategoryExtensions,
      ExtensionPerCategory targetCategory)
    {
      if (featuredCategoryExtensions == null || featuredCategoryExtensions.Count == 0 || targetCategory.Extensions == null || targetCategory.Extensions.Count <= 6)
      {
        string message = string.Format((IFormatProvider) CultureInfo.CurrentCulture, "Target category {0} has {1} extensions", (object) targetCategory.CategoryName, (object) (targetCategory.Extensions == null ? 0 : targetCategory.Extensions.Count));
        requestContext.Trace(12062029, TraceLevel.Info, "Gallery", "HomepageCache", message);
      }
      else
      {
        requestContext.TraceEnter(12062029, "Gallery", "HomepageCache", nameof (DedupeFromFeaturedExtensions));
        HashSet<BaseExtensionItem> hashSet = featuredCategoryExtensions.ToHashSet<BaseExtensionItem>();
        int index1 = 0;
        List<BaseExtensionItem> duplicateExtensions = new List<BaseExtensionItem>();
        List<int> duplicateExtensionsIndices = new List<int>();
        for (int index2 = 18; index1 < index2 && index1 < targetCategory.Extensions.Count; ++index1)
        {
          BaseExtensionItemComparer comparer = new BaseExtensionItemComparer();
          if (hashSet.Contains<BaseExtensionItem>(targetCategory.Extensions[index1], (IEqualityComparer<BaseExtensionItem>) comparer))
          {
            duplicateExtensions.Add(targetCategory.Extensions[index1]);
            duplicateExtensionsIndices.Add(index1);
            ++index2;
          }
        }
        string message = string.Format((IFormatProvider) CultureInfo.CurrentCulture, "Number of duplicate extensions found = {0}", (object) duplicateExtensions.Count);
        requestContext.Trace(12062029, TraceLevel.Info, "Gallery", "HomepageCache", message);
        this.MoveDuplicatesToLastCarousel(requestContext, targetCategory, duplicateExtensions, duplicateExtensionsIndices);
        requestContext.TraceLeave(12062029, "Gallery", "HomepageCache", nameof (DedupeFromFeaturedExtensions));
      }
    }

    private void MoveDuplicatesToLastCarousel(
      IVssRequestContext requestContext,
      ExtensionPerCategory targetCategory,
      List<BaseExtensionItem> duplicateExtensions,
      List<int> duplicateExtensionsIndices)
    {
      if (duplicateExtensions == null || duplicateExtensions.Count == 0 || duplicateExtensionsIndices == null || duplicateExtensionsIndices.Count == 0)
        return;
      requestContext.TraceEnter(12062029, "Gallery", "HomepageCache", nameof (MoveDuplicatesToLastCarousel));
      int index1 = 0;
      int num = 0;
      for (int index2 = 0; num < 18 && index2 < targetCategory.Extensions.Count; targetCategory.Extensions[num++] = targetCategory.Extensions[index2++])
      {
        for (; index2 < 24 && index2 < targetCategory.Extensions.Count - 1 && index1 < duplicateExtensionsIndices.Count && duplicateExtensionsIndices[index1] == index2; ++index1)
          ++index2;
      }
      for (int index3 = 0; index3 < duplicateExtensions.Count && num < targetCategory.Extensions.Count; ++index3)
        targetCategory.Extensions[num++] = duplicateExtensions[index3];
      requestContext.TraceLeave(12062029, "Gallery", "HomepageCache", nameof (MoveDuplicatesToLastCarousel));
    }

    protected override void ServiceStart(IVssRequestContext systemRequestContext)
    {
      base.ServiceStart(systemRequestContext);
      if (this.DataProvider == null)
        this.DataProvider = (IProductExtensionsDataProvider) new ProductExtensionsProvider(systemRequestContext);
      this.Clear(systemRequestContext);
      ITeamFoundationSqlNotificationService service = systemRequestContext.GetService<ITeamFoundationSqlNotificationService>();
      service.RegisterNotification(systemRequestContext, "Default", GalleryNotificationEventIds.ExtensionUpdateDelete, new SqlNotificationCallback(this.NotificationCallbackHandler), false);
      service.RegisterNotification(systemRequestContext, "Default", GalleryNotificationEventIds.ProductExtensionServiceCacheForceRefresh, new SqlNotificationCallback(this.CacheRefreshNotificationCallbackHandler), false);
    }

    protected override void ServiceEnd(IVssRequestContext systemRequestContext)
    {
      base.ServiceEnd(systemRequestContext);
      ITeamFoundationSqlNotificationService service = systemRequestContext.GetService<ITeamFoundationSqlNotificationService>();
      service.UnregisterNotification(systemRequestContext, "Default", GalleryNotificationEventIds.ExtensionUpdateDelete, new SqlNotificationCallback(this.NotificationCallbackHandler), false);
      service.UnregisterNotification(systemRequestContext, "Default", GalleryNotificationEventIds.ProductExtensionServiceCacheForceRefresh, new SqlNotificationCallback(this.CacheRefreshNotificationCallbackHandler), false);
    }

    private void CacheRefreshNotificationCallbackHandler(
      IVssRequestContext requestContext,
      Guid eventClass,
      string eventData)
    {
      string str = eventData;
      if (!string.IsNullOrEmpty(str))
      {
        switch (str)
        {
          case "vs":
            this.InitializeVSCache();
            break;
          case "vsformac":
            this.InitializeVSForMacCache();
            break;
          case "vsts":
            this.InitializeVSTSCache();
            break;
          case "vscode":
            this.InitializeVSCodeCache();
            break;
          case "subscriptions":
            this.InitializeVSSubsCache();
            break;
          default:
            this.InitializeCache();
            break;
        }
      }
      else
        this.InitializeCache();
    }

    private void NotificationCallbackHandler(
      IVssRequestContext requestContext,
      Guid eventClass,
      string eventData)
    {
      if (!string.IsNullOrEmpty(eventData))
      {
        if (eventData.Contains(": vs "))
          this.Remove(requestContext, "vs");
        else if (eventData.Contains(": vsts "))
          this.Remove(requestContext, "vsts");
        else if (eventData.Contains(": vsformac "))
          this.Remove(requestContext, "vsformac");
        else if (eventData.Contains(": vscode "))
          this.Remove(requestContext, "vscode");
        else if (eventData.Contains(": subscriptions "))
          this.Remove(requestContext, "subscriptions");
        else
          this.Clear(requestContext);
      }
      else
        this.Clear(requestContext);
      CustomerIntelligenceData properties = new CustomerIntelligenceData();
      properties.Add("eventType", "cacheRefresh");
      properties.Add(nameof (eventData), eventData);
      requestContext.GetService<CustomerIntelligenceService>().Publish(requestContext, "Microsoft.VisualStudio.Services.Gallery", "HomepageCache", properties);
    }

    private bool UpdateRefreshTime(
      string product,
      long nextRefreshTicks,
      long previousRefreshTicks)
    {
      return this.s_lastRefreshTimeTicks.AddOrUpdate(product, nextRefreshTicks, (Func<string, long, long>) ((key, oldValue) => oldValue == previousRefreshTicks ? nextRefreshTicks : oldValue)) == nextRefreshTicks;
    }

    protected IProductExtensionsDataProvider DataProvider { get; set; }

    protected DateTimeWrapper DateTimeWrapper { get; set; } = new DateTimeWrapper();

    public CachedProductExtensionsService() => this.InitializeCache();

    public void InitializeCache()
    {
      this.InitializeVSCache();
      this.InitializeVSForMacCache();
      this.InitializeVSTSCache();
      this.InitializeVSCodeCache();
      this.InitializeVSSubsCache();
    }

    private void InitializeVSCache() => this._vsInMemoryCache = (IMemoryCache<object>) new Microsoft.VisualStudio.Services.Gallery.Server.Extension.Cache.MemoryCache<object>(this.cacheRefreshIntervalInSeconds, new Func<IVssRequestContext, object>(this.GetVsExtensionsData));

    private void InitializeVSForMacCache() => this._vsformacInMemoryCache = (IMemoryCache<object>) new Microsoft.VisualStudio.Services.Gallery.Server.Extension.Cache.MemoryCache<object>(this.cacheRefreshIntervalInSeconds, new Func<IVssRequestContext, object>(this.GetVsForMacExtensionsData));

    private void InitializeVSTSCache() => this._vstsInMemoryCache = (IMemoryCache<object>) new Microsoft.VisualStudio.Services.Gallery.Server.Extension.Cache.MemoryCache<object>(this.cacheRefreshIntervalInSeconds, new Func<IVssRequestContext, object>(this.GetVstsExtensionsData));

    private void InitializeVSCodeCache() => this._vscodeInMemoryCache = (IMemoryCache<object>) new Microsoft.VisualStudio.Services.Gallery.Server.Extension.Cache.MemoryCache<object>(this.cacheRefreshIntervalInSeconds, new Func<IVssRequestContext, object>(this.GetVscodeExtensionsData));

    private void InitializeVSSubsCache() => this._vssubsInMemoryCache = (IMemoryCache<object>) new Microsoft.VisualStudio.Services.Gallery.Server.Extension.Cache.MemoryCache<object>(this.cacheRefreshIntervalInSeconds, new Func<IVssRequestContext, object>(this.GetVssubsExtensionsData));

    public CachedProductExtensionsService(
      Microsoft.VisualStudio.Services.Gallery.Server.Extension.Cache.MemoryCache<object> vsCache,
      Microsoft.VisualStudio.Services.Gallery.Server.Extension.Cache.MemoryCache<object> vsfomacCache,
      Microsoft.VisualStudio.Services.Gallery.Server.Extension.Cache.MemoryCache<object> vstsCache,
      Microsoft.VisualStudio.Services.Gallery.Server.Extension.Cache.MemoryCache<object> vsCodeCache,
      Microsoft.VisualStudio.Services.Gallery.Server.Extension.Cache.MemoryCache<object> vsSubsCache)
    {
      this._vsInMemoryCache = (IMemoryCache<object>) vsCache;
      this._vsformacInMemoryCache = (IMemoryCache<object>) vsfomacCache;
      this._vstsInMemoryCache = (IMemoryCache<object>) vstsCache;
      this._vscodeInMemoryCache = (IMemoryCache<object>) vsCodeCache;
      this._vssubsInMemoryCache = (IMemoryCache<object>) vsSubsCache;
    }

    private object GetVsExtensionsData(IVssRequestContext requestContext) => this.GetProductExtensionsInternal(requestContext, "vs");

    private object GetVsForMacExtensionsData(IVssRequestContext requestContext) => this.GetProductExtensionsInternal(requestContext, "vsformac");

    private object GetVstsExtensionsData(IVssRequestContext requestContext) => this.GetProductExtensionsInternal(requestContext, "vsts");

    private object GetVscodeExtensionsData(IVssRequestContext requestContext) => this.GetProductExtensionsInternal(requestContext, "vscode");

    private object GetVssubsExtensionsData(IVssRequestContext requestContext) => this.GetProductExtensionsInternal(requestContext, "subscriptions");

    private object GetProductExtensionsInternal(IVssRequestContext requestContext, string product)
    {
      object productExtensions = this.DataProvider.GetProductExtensions(requestContext, product);
      ProductExtensions extensionsInternal = productExtensions as ProductExtensions;
      if (product.Equals("subscriptions", StringComparison.OrdinalIgnoreCase) || !requestContext.IsFeatureEnabled("Microsoft.VisualStudio.Services.Gallery.DedupeHomepageExtensions"))
        return productExtensions;
      ExtensionPerCategory extensionPerCategory1 = new ExtensionPerCategory();
      foreach (ExtensionPerCategory extensionPerCategory2 in extensionsInternal.ExtensionsPerCategory)
      {
        if (extensionPerCategory2.CategoryName == "Featured")
        {
          extensionPerCategory1 = extensionPerCategory2;
          break;
        }
      }
      string message = string.Format((IFormatProvider) CultureInfo.CurrentCulture, "Number of extensions for {0} category = {1}", (object) extensionPerCategory1.CategoryName, (object) (extensionPerCategory1.Extensions == null ? 0 : extensionPerCategory1.Extensions.Count));
      requestContext.Trace(12062029, TraceLevel.Info, "Gallery", "HomepageCache", message);
      foreach (ExtensionPerCategory extensionPerCategory3 in extensionsInternal.ExtensionsPerCategory)
      {
        if (extensionPerCategory3.CategoryName == "Most Popular" || extensionPerCategory3.CategoryName == "TrendingDaily" || extensionPerCategory3.CategoryName == "TrendingWeekly" || extensionPerCategory3.CategoryName == "TrendingMonthly")
        {
          this.DedupeFromFeaturedExtensions(requestContext, extensionPerCategory1.Extensions, extensionPerCategory3);
          if (!(extensionPerCategory3.CategoryName == "Most Popular"))
            this.TrimExtensionsForTrending(extensionPerCategory3);
        }
      }
      return (object) extensionsInternal;
    }
  }
}
