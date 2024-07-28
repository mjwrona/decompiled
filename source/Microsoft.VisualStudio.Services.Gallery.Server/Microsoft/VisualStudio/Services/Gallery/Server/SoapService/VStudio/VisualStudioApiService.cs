// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.SoapService.VStudio.VisualStudioApiService
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using Galleries.Domain.Model;
using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.CircuitBreaker;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Gallery.Server.Extension.Cache;
using Microsoft.VisualStudio.Services.Gallery.Server.SoapService.Cache;
using Microsoft.VisualStudio.Services.Gallery.Server.SoapService.Galleries;
using Microsoft.VisualStudio.Services.Gallery.Server.SoapService.Models;
using Microsoft.VisualStudio.Services.Gallery.WebApi;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Web;
using VsGallery.WebServices;

namespace Microsoft.VisualStudio.Services.Gallery.Server.SoapService.VStudio
{
  internal class VisualStudioApiService : IVisualStudioApiService, IVssFrameworkService
  {
    private const string ReportAbuse = "#report-abuse";
    private const string TargetPlatformQueryParamName = "targetPlatform";
    private const string CDNRedirectQueryParamName = "redirect";
    private const string InstallQueryParamName = "install";
    private const string UpdateQueryParamName = "update";
    private const int FixedTemplatesCategoryId = 1;
    private const int SearchCachingSkipSize = 0;
    private const int SearchCachingTakeSize = 25;
    private const int ExtensionCacheRefreshIntervalInSeconds = 600;
    private const string GetRootCategoriesMethod = "GetRootCategories";
    private const string GetCategoryTreeMethod = "GetCategoryTree";
    private const string SearchReleasesMethod = "SearchReleases";
    private const string SearchForVsIdeMethod = "SearchForVsIdeMethod";
    private const string VisualStudioApiServiceCacheLayer = "VisualStudioApiServiceCache";
    private const string VsExtensionCacheRefreshEventType = "VSExtensionCacheRefresh";
    private const string VsExtensionCacheRefreshEventFallbackType = "VSExtensionCacheRefreshFallback";
    private const string CommandKeyForVsExtensionCachedDataBuilder = "VsExtensionCachedDataBuilder";
    private const string CommandKeyForGetCategories = "GetCategories";
    private const string CommandKeyForSearchReleases = "SearchReleases";
    private const int CircuitBreakerRollingStatisticalWindowInMilliseconds = 60000;
    private const int ExtensionCacheDbCallTimeoutInSeconds = 10;
    private const int ExtensionCacheMaxConcurrentRequestsPerMinute = 10;
    private const int SearchReleasesDbCallTimeoutInSeconds = 5;
    private const int SearchReleasesMaxConcurrentRequestsPerMinute = 100;
    private const int GetCategoriesDbCallTimeoutInSeconds = 5;
    private const int GetCategoriesMaxConcurrentRequestsPerMinute = 100;
    private const int SearchReleasesCircuitBreakerExceptionID = 160005;
    private const int GetCategoriesCircuitBreakerExceptionID = 160006;
    private const int ExtensionCacheCircuitBreakerExceptionID = 160007;
    private const int SearchReleasesCircuitBreakerThrottlingExceptionID = 160008;
    private const int GetCategoriesCircuitBreakerThrottlingExceptionID = 160009;
    private const int ExtensionCacheCircuitBreakerThrottlingExceptionID = 160010;
    private volatile CachedPublishedExtensionData _previousData;
    private volatile bool _isFirstVsCacheUpdate;
    [StaticSafe("Grandfathered")]
    private static readonly ISet<string> AllowedMetadataValuesForVsIde = (ISet<string>) new HashSet<string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase)
    {
      "CategoryID",
      "VsixId",
      "VsixVersion",
      "VsixReferences",
      "SubType",
      "TemplateGroupID",
      "Type",
      "SupportsMasterPage",
      "SupportsCodeSeparation",
      "NetFxVersion",
      "ProjectTypeFriendly",
      "DefaultName",
      "ProjectType",
      "ContentTypes",
      "PackedExtensionsVsixIDs",
      "ReleaseNotes",
      "VsixCompatibilityCode"
    };
    [StaticSafe("Grandfathered")]
    private static readonly ISet<string> MetadataValuesToIncludeInMinimalSet = (ISet<string>) new HashSet<string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase)
    {
      "VsixId",
      "VsixVersion",
      "DownloadUpdateUrl"
    };
    [StaticSafe("Grandfathered")]
    private static readonly IDictionary<string, string> AssetTypeToMetadataKeyMap = (IDictionary<string, string>) new Dictionary<string, string>()
    {
      {
        "Microsoft.VisualStudio.Ide.Payload",
        "DownloadUrl"
      },
      {
        "Microsoft.VisualStudio.Services.Image.Preview",
        "PreviewImage"
      },
      {
        "Microsoft.VisualStudio.Services.Icons.Default",
        "Icon"
      }
    };
    private string _vsGalleryBaseUrl;
    private readonly IExtensionSearchApiQueryBuilder _searchApiQuerybuilder;
    protected ExtensionCategory TemplatesCategory;
    private readonly IExtensionQueryHelper _extensionQueryHelper;
    private readonly IMemoryCache<CachedPublishedExtensionData> _inMemoryCache;
    private readonly string DefaultCategoryCultureName = "en-us";
    private readonly string TemplatesCategoryName = "templates";

    public VisualStudioApiService()
    {
      this._inMemoryCache = (IMemoryCache<CachedPublishedExtensionData>) new MemoryCache<CachedPublishedExtensionData>(600, new Func<IVssRequestContext, CachedPublishedExtensionData>(this.VsExtensionCachedDataBuilder));
      this._extensionQueryHelper = (IExtensionQueryHelper) new ExtensionQueryHelper("VisualStudioApiServiceCache", 12061087);
      this._searchApiQuerybuilder = (IExtensionSearchApiQueryBuilder) new ExtensionsSearchApiQueryBuilder((IFilterTranslator) new FilterTranslator());
      this._previousData = new CachedPublishedExtensionData();
    }

    internal VisualStudioApiService(
      IExtensionSearchApiQueryBuilder searchApiQueryBuilder,
      string vsGalleryMockUrl,
      IExtensionQueryHelper extensionQueryHelper,
      IMemoryCache<CachedPublishedExtensionData> inMemoryCache)
    {
      this._searchApiQuerybuilder = searchApiQueryBuilder;
      this._vsGalleryBaseUrl = vsGalleryMockUrl;
      this._extensionQueryHelper = extensionQueryHelper;
      this._inMemoryCache = inMemoryCache;
    }

    internal VisualStudioApiService(
      IExtensionSearchApiQueryBuilder searchApiQueryBuilder,
      string vsGalleryMockUrl,
      IExtensionQueryHelper extensionQueryHelper)
    {
      this._inMemoryCache = (IMemoryCache<CachedPublishedExtensionData>) new MemoryCache<CachedPublishedExtensionData>(600, new Func<IVssRequestContext, CachedPublishedExtensionData>(this.VsExtensionCachedDataBuilder));
      this._searchApiQuerybuilder = searchApiQueryBuilder;
      this._vsGalleryBaseUrl = vsGalleryMockUrl;
      this._extensionQueryHelper = extensionQueryHelper;
    }

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
      IPublishedExtensionService service = systemRequestContext.GetService<IPublishedExtensionService>();
      IVssRequestContext requestContext = systemRequestContext;
      foreach (ExtensionCategory category in service.QueryAvailableCategories(requestContext, (IEnumerable<string>) new List<string>()
      {
        this.DefaultCategoryCultureName
      }, product: "vs").Categories)
      {
        if (string.Equals(category.CategoryName, this.TemplatesCategoryName))
        {
          this.TemplatesCategory = category;
          break;
        }
      }
      this._vsGalleryBaseUrl = systemRequestContext.GetService<IVssRegistryService>().GetValue<string>(systemRequestContext, (RegistryQuery) "/Configuration/Service/Gallery/VSGallery/HostedUrl", "");
      this._isFirstVsCacheUpdate = !systemRequestContext.IsFeatureEnabled("Microsoft.VisualStudio.Services.Gallery.CreateVSCacheOnStart");
      try
      {
        this._inMemoryCache.GetCachedData(systemRequestContext);
      }
      catch (Exception ex)
      {
        systemRequestContext.TraceException(12061087, "Gallery", "VisualStudioApiServiceCache", ex);
      }
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    public virtual CachedPublishedExtensionData VsExtensionCachedDataBuilder(
      IVssRequestContext requestContext)
    {
      if (this._isFirstVsCacheUpdate)
      {
        this._isFirstVsCacheUpdate = false;
        return this._previousData;
      }
      Func<CachedPublishedExtensionData> run = (Func<CachedPublishedExtensionData>) (() =>
      {
        CachedPublishedExtensionData publishedExtensionData = new CachedPublishedExtensionData();
        string searchTerm = "target:\"Microsoft.VisualStudio.Ide\"";
        IPublishedExtensionService service = requestContext.GetService<IPublishedExtensionService>();
        ExtensionQueryFlags queryOptions = ExtensionQueryFlags.IncludeFiles | ExtensionQueryFlags.ExcludeNonValidated | ExtensionQueryFlags.IncludeInstallationTargets | ExtensionQueryFlags.IncludeAssetUri | ExtensionQueryFlags.IncludeStatistics | ExtensionQueryFlags.IncludeLatestVersionOnly | ExtensionQueryFlags.IncludeMetadata;
        Stopwatch stopwatch = new Stopwatch();
        stopwatch.Start();
        try
        {
          publishedExtensionData.AllExtensions = this._extensionQueryHelper.GetAllExtensions(requestContext, service, (string[]) null, queryOptions, 4000, searchTerm);
        }
        catch (InvalidExtensionQueryException ex)
        {
          ex.Data.Add((object) "{421AC3F1-A306-4C9B-B3F6-5812F9121FC8}", (object) true);
          throw;
        }
        catch (ArgumentException ex)
        {
          ex.Data.Add((object) "{421AC3F1-A306-4C9B-B3F6-5812F9121FC8}", (object) true);
          throw;
        }
        foreach (PublishedExtension allExtension in (IEnumerable<PublishedExtension>) publishedExtensionData.AllExtensions)
        {
          publishedExtensionData.ExtensionIdToExtensionMap.TryAdd<string, PublishedExtension>(allExtension.ExtensionId.ToString(), allExtension);
          foreach (ExtensionMetadata extensionMetadata in allExtension.Metadata)
          {
            if (string.Equals(extensionMetadata.Key, "VsixId", StringComparison.OrdinalIgnoreCase))
              publishedExtensionData.VsixIdToExtensionMap.TryAdd<string, PublishedExtension>(extensionMetadata.Value.ToString(), allExtension);
          }
        }
        stopwatch.Stop();
        CustomerIntelligenceData properties = new CustomerIntelligenceData();
        properties.Add("EventType", "VSExtensionCacheRefresh");
        properties.Add("AllExtensionsCount", (double) publishedExtensionData.AllExtensions.Count);
        properties.Add("VsixIdToExtensionMapCount", (double) publishedExtensionData.VsixIdToExtensionMap.Count);
        properties.Add("ExtensionIdToExtensionMapCount", (double) publishedExtensionData.ExtensionIdToExtensionMap.Count);
        properties.Add("TimeTakenMs", (double) stopwatch.ElapsedMilliseconds);
        requestContext.GetService<CustomerIntelligenceService>().Publish(requestContext, "Microsoft.VisualStudio.Services.Gallery", "VsApiService", properties);
        this._previousData = publishedExtensionData;
        return publishedExtensionData;
      });
      Func<CachedPublishedExtensionData> fallback = (Func<CachedPublishedExtensionData>) (() =>
      {
        CustomerIntelligenceData properties = new CustomerIntelligenceData();
        properties.Add("EventType", "VSExtensionCacheRefreshFallback");
        requestContext.GetService<CustomerIntelligenceService>().Publish(requestContext, "Microsoft.VisualStudio.Services.Gallery", "VsApiService", properties);
        return this._previousData;
      });
      CommandSetter setter = CommandSetter.WithGroupKey((CommandGroupKey) "Marketplace.").AndCommandKey((CommandKey) nameof (VsExtensionCachedDataBuilder)).AndCommandPropertiesDefaults(this.VsExtensionCachedDataBuilderCircuitBreakerSettings);
      CommandService<CachedPublishedExtensionData> commandService = new CommandService<CachedPublishedExtensionData>(requestContext, setter, run, fallback);
      try
      {
        return commandService.Execute();
      }
      catch (CircuitBreakerException ex)
      {
        int eventId = 160010;
        if (ex is CircuitBreakerShortCircuitException)
          eventId = 160007;
        TeamFoundationEventLog.Default.Log(requestContext, ex.Message, eventId, EventLogEntryType.Error);
        throw;
      }
    }

    internal CommandPropertiesSetter VsExtensionCachedDataBuilderCircuitBreakerSettings => new CommandPropertiesSetter().WithExecutionTimeout(new TimeSpan(0, 0, 10)).WithExecutionMaxRequests(10).WithMetricsRollingStatisticalWindowInMilliseconds(60000);

    public ICollection<IdeCategory> GetRootCategories(
      IVssRequestContext requestContext,
      VisualStudioIdeVersion vsVersion,
      IDictionary<string, string> requestParams)
    {
      if (requestParams == null || !VSServiceContextKeys.RequestContextIsValid(requestParams))
        throw new ArgumentException("Request context is not valid");
      this.CheckIfIdeApisAllowed(requestContext);
      int culture = int.Parse(requestParams["LCID"], (IFormatProvider) CultureInfo.InvariantCulture);
      CultureInfo cultureInfo = new CultureInfo(culture);
      RootCategoriesCache service = requestContext.GetService<RootCategoriesCache>();
      string forRootCategories = this.GetKeyForRootCategories(nameof (GetRootCategories), culture.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      List<IdeCategory> rootCategories1;
      if (service.TryGetValue<List<IdeCategory>>(requestContext, forRootCategories, out rootCategories1))
        return (ICollection<IdeCategory>) rootCategories1;
      VsIdeExtensionQuery queryForCategoryTree = this.GetQueryForCategoryTree(requestParams.ContainsKey("VSVersion") ? requestParams["VSVersion"] : this.GetVisualStudioIdeVersionStringFromEnum(vsVersion), (string) null, (string) null, (string[]) null, (string[]) null, new int?(), new int?(), cultureInfo);
      IList<ExtensionCategory> categories = this.GetCategories(requestContext, queryForCategoryTree, cultureInfo, new int?());
      List<IdeCategory> rootCategories2 = new List<IdeCategory>();
      foreach (ExtensionCategory extensionCategory in (IEnumerable<ExtensionCategory>) categories)
      {
        IdeCategory ideCategory = this.ConvertToIdeCategory(extensionCategory, cultureInfo.Name);
        rootCategories2.Add(ideCategory);
      }
      rootCategories2.Sort((IComparer<IdeCategory>) new IdeCategoryComparor());
      service.Set(forRootCategories, (object) rootCategories2, service.OneWeekExpiryProvider);
      return (ICollection<IdeCategory>) rootCategories2;
    }

    public ICollection<IdeCategory> GetRootCategories(
      IVssRequestContext requestContext,
      VisualStudioIdeVersion vsVersion,
      string cultureName)
    {
      Dictionary<string, string> requestParams = new Dictionary<string, string>()
      {
        {
          "SearchSource",
          ""
        },
        {
          "LCID",
          new CultureInfo(cultureName).LCID.ToString((IFormatProvider) CultureInfo.InvariantCulture)
        },
        {
          "TemplateType",
          ""
        },
        {
          "Skus",
          ""
        },
        {
          "SubSkus",
          ""
        },
        {
          "OSVersion",
          ""
        }
      };
      return this.GetRootCategories(requestContext, vsVersion, (IDictionary<string, string>) requestParams);
    }

    public IdeCategory GetCategoryTree(
      IVssRequestContext requestContext,
      VisualStudioIdeVersion vsVersion,
      Guid categoryGuid,
      int level,
      IDictionary<string, string> requestParams)
    {
      if (requestParams == null || !VSServiceContextKeys.RequestContextIsValid(requestParams))
        throw new ArgumentException("Request context is not valid");
      this.CheckIfIdeApisAllowed(requestContext);
      int culture = int.Parse(requestParams["LCID"], (IFormatProvider) CultureInfo.InvariantCulture);
      CultureInfo cultureInfo = new CultureInfo(culture);
      string requestParam1 = requestParams["TemplateType"];
      string[] skus = requestParams["Skus"].Split(',');
      string[] subSkus = (string[]) null;
      if (requestParams.ContainsKey("SubSkus"))
        subSkus = requestParams["SubSkus"].Split(',');
      string requestParam2 = requestParams["SearchSource"];
      string str = requestParams.ContainsKey("VSVersion") ? requestParams["VSVersion"] : this.GetVisualStudioIdeVersionStringFromEnum(vsVersion);
      string requestParam3 = requestParams.ContainsKey("ProductArchitecture") ? requestParams["ProductArchitecture"] : (string) null;
      int[] ids = VsIdeCategoryIdParser.ParseIds(categoryGuid);
      int categoryId = ids[0];
      int programmingLanguageId = ids[1];
      int num1 = ids[2];
      bool hasMore = false;
      bool flag1 = false;
      bool flag2 = false;
      bool useProgrammingLanguageId = false;
      int num2 = this.SanitizeIncomingCategoryId(categoryId);
      CategoryTreeCache service = requestContext.GetService<CategoryTreeCache>();
      string keyForCategoryTree = this.GetKeyForCategoryTree(nameof (GetCategoryTree), culture.ToString((IFormatProvider) CultureInfo.InvariantCulture), str, categoryGuid.ToString(), requestParam1, requestParam2, skus, subSkus, requestParam3);
      IdeCategory categoryTree1;
      if (service.TryGetValue<IdeCategory>(requestContext, keyForCategoryTree, out categoryTree1))
        return categoryTree1;
      IList<ExtensionCategory> extensionCategoryList;
      if (num2 == this.TemplatesCategory.CategoryId && programmingLanguageId == 0)
      {
        VsIdeExtensionQuery queryForCategoryTree = this.GetQueryForCategoryTree(str, requestParam2, requestParam1, skus, subSkus, new int?(num2), new int?(programmingLanguageId), cultureInfo, requestParam3);
        extensionCategoryList = this.GetProgrammingLanguages(requestContext, queryForCategoryTree, cultureInfo);
        hasMore = true;
      }
      else if (num2 == this.TemplatesCategory.CategoryId && programmingLanguageId != 0)
      {
        VsIdeExtensionQuery queryForCategoryTree = this.GetQueryForCategoryTree(str, requestParam2, requestParam1, skus, subSkus, new int?(num2), new int?(programmingLanguageId), cultureInfo, requestParam3);
        extensionCategoryList = this.GetCategories(requestContext, queryForCategoryTree, cultureInfo, new int?(num2));
        flag1 = true;
        useProgrammingLanguageId = true;
      }
      else
      {
        VsIdeExtensionQuery queryForCategoryTree = this.GetQueryForCategoryTree(str, requestParam2, requestParam1, skus, subSkus, new int?(num2), new int?(programmingLanguageId), cultureInfo, requestParam3);
        extensionCategoryList = this.GetCategories(requestContext, queryForCategoryTree, cultureInfo, new int?(num2));
        flag2 = true;
      }
      IdeCategory categoryTree2 = (IdeCategory) null;
      if (!extensionCategoryList.IsNullOrEmpty<ExtensionCategory>())
      {
        categoryTree2 = this.ConvertToIdeCategory(!flag1 ? extensionCategoryList.First<ExtensionCategory>().Parent : this.GetCategoryForProgrammingLanguage(programmingLanguageId), cultureInfo.Name);
        List<IdeCategory> ideCategoryList = new List<IdeCategory>();
        foreach (ExtensionCategory extensionCategory in (IEnumerable<ExtensionCategory>) extensionCategoryList)
        {
          if (flag2)
            extensionCategory.Parent = (ExtensionCategory) null;
          ideCategoryList.Add(this.ConvertToIdeCategory(extensionCategory, cultureInfo.Name, hasMore, programmingLanguageId, useProgrammingLanguageId));
        }
        ideCategoryList.Sort((IComparer<IdeCategory>) new IdeCategoryComparor());
        categoryTree2.Children = (IList<IdeCategory>) ideCategoryList;
      }
      bool flag3 = requestContext.IsFeatureEnabled("Microsoft.VisualStudio.Services.Gallery.EnableShorterCategoriesCacheExpiryIntervalForDev17");
      if (VisualStudioIdeVersion.Dev17.Equals((object) vsVersion) & flag3)
        service.Set(keyForCategoryTree, (object) categoryTree2, service.TenMinutesExpiryProvider);
      else
        service.Set(keyForCategoryTree, (object) categoryTree2, service.OneWeekExpiryProvider);
      return categoryTree2;
    }

    public ExtensionQueryResult SearchForVsIde(
      IVssRequestContext requestContext,
      ExtensionQuery extensionQuery)
    {
      this.CheckIfIdeApisAllowed(requestContext);
      if (extensionQuery == null)
        return (ExtensionQueryResult) null;
      if (extensionQuery.Filters.IsNullOrEmpty<QueryFilter>() || extensionQuery.Filters.Count != 1)
        throw new ArgumentException("Exactly one filter should be present in extensionQuery").Expected("Gallery");
      QueryFilter filter = extensionQuery.Filters[0];
      ArgumentUtility.CheckForNull<List<FilterCriteria>>(filter.Criteria, "Queryfilter.Criteria", "Gallery");
      this.AddDefaultProductArchitectureFilterCriteriaIfNotPresent(requestContext, filter);
      bool sendMinimalData = extensionQuery.Flags.HasFlag((Enum) ExtensionQueryFlags.IncludeMinimalPayloadForVsIde);
      IDictionary<string, string> dictionary = (IDictionary<string, string>) new Dictionary<string, string>();
      bool flag1 = false;
      bool hasSearchText = false;
      List<MetadataFilterItem> collection = new List<MetadataFilterItem>();
      Version version = (Version) null;
      ISet<string> targetSkus = (ISet<string>) new HashSet<string>();
      string productArchitecture = (string) null;
      foreach (FilterCriteria criterion in filter.Criteria)
      {
        if (criterion.FilterType == 17)
        {
          try
          {
            MetadataFilterItem fromFilterCriteria = this.CreateMetadataFilterItemFromFilterCriteria(criterion);
            if (fromFilterCriteria.Key.Equals("VsixId", StringComparison.OrdinalIgnoreCase))
            {
              flag1 = true;
              string[] strArray = fromFilterCriteria.Value.Split(new string[1]
              {
                ":"
              }, StringSplitOptions.None);
              if (strArray.Length != 0)
              {
                string key = strArray[0];
                string str = (string) null;
                if (strArray.Length > 1)
                  str = strArray[1];
                dictionary.TryAdd<string, string>(key, str);
              }
            }
            else
              collection.Add(fromFilterCriteria);
          }
          catch (ArgumentException ex)
          {
            throw new ArgumentException(extensionQuery.Serialize<ExtensionQuery>(), (Exception) ex);
          }
        }
        else if (criterion.FilterType == 10)
          hasSearchText = true;
        else if (criterion.FilterType == 15)
        {
          try
          {
            version = GalleryServerUtil.ConvertStringToVersion(criterion.Value, false);
          }
          catch (InvalidVersionException ex)
          {
            throw new ArgumentException("Version provided in query is invalid.");
          }
        }
        else if (criterion.FilterType == 8)
          targetSkus.Add(criterion.Value);
        else if (criterion.FilterType == 22)
          productArchitecture = criterion.Value;
      }
      filter.Criteria.RemoveAll((Predicate<FilterCriteria>) (x => x.FilterType == 17));
      try
      {
        if (flag1)
        {
          IDictionary<string, PublishedExtension> vsixIdAndVersion = this.GetExtensionsByVsixIdAndVersion(requestContext, dictionary, targetSkus, version, productArchitecture);
          return this.GetExtensionQueryResultForUpdate(requestContext, vsixIdAndVersion.Values.ToList<PublishedExtension>(), sendMinimalData, targetSkus, version, productArchitecture);
        }
        VsIdeExtensionQuery vsIdeExtensionQuery = this._searchApiQuerybuilder.BuildQueryFromExtensionQuery(extensionQuery);
        vsIdeExtensionQuery.MetadataFilters.AddRange((IEnumerable<MetadataFilterItem>) collection);
        this.FixIncomingCategoryId(vsIdeExtensionQuery);
        bool flag2 = this.IsApplicableForCache(vsIdeExtensionQuery, hasSearchText);
        string forSearchForVsIde = this.GetKeyForSearchForVsIde("SearchForVsIdeMethod", vsIdeExtensionQuery);
        SearchReleasesCache service = requestContext.GetService<SearchReleasesCache>();
        ExtensionQueryResult extensionQueryResult1;
        if (flag2 && service.TryGetValue<ExtensionQueryResult>(requestContext, forSearchForVsIde, out extensionQueryResult1))
          return extensionQueryResult1;
        ExtensionQueryResult extensionQueryResult2 = this.SearchViaCircuitBreaker(requestContext, vsIdeExtensionQuery);
        ExtensionQueryResult extensionQueryResult3 = this.GetExtensionQueryResult(requestContext, extensionQueryResult2.Results[0], targetSkus, version, productArchitecture);
        if (flag2)
          service.Set(forSearchForVsIde, (object) extensionQueryResult3, service.TenMinutesExpiryProvider);
        return extensionQueryResult3;
      }
      finally
      {
        if (requestContext.IsFeatureEnabled("Microsoft.VisualStudio.Services.Gallery.EnableVsIdeRestCallTelemetry"))
        {
          CustomerIntelligenceData properties = new CustomerIntelligenceData();
          properties.Add("EventType", "VSIdeExtensionQuery");
          properties.Add("IsUpdateCheck", flag1);
          properties.Add("IsSearch", hasSearchText);
          properties.Add("TargetVersion", (object) version);
          if (!string.IsNullOrWhiteSpace(productArchitecture))
            properties.Add("ProductArchitecture", productArchitecture);
          requestContext.GetService<CustomerIntelligenceService>().Publish(requestContext, "Microsoft.VisualStudio.Services.Gallery", "VsApiService", properties);
        }
      }
    }

    private void AddDefaultProductArchitectureFilterCriteriaIfNotPresent(
      IVssRequestContext requestContext,
      QueryFilter queryFilter)
    {
      if (!requestContext.IsFeatureEnabled("Microsoft.VisualStudio.Services.Gallery.EnableProductArchitectureSupportForVS") || queryFilter.Criteria.Any<FilterCriteria>((Func<FilterCriteria, bool>) (criteria => criteria.FilterType == 22)))
        return;
      queryFilter.Criteria.Add(new FilterCriteria()
      {
        FilterType = 22,
        Value = "x86"
      });
    }

    private bool IsApplicableForCache(VsIdeExtensionQuery vsIdeExtensionQuery, bool hasSearchText)
    {
      QueryFilter filter = vsIdeExtensionQuery.ExtensionQuery.Filters[0];
      return filter.PageNumber == 1 && filter.PageSize == 25 && !hasSearchText;
    }

    private ExtensionCategory GetCategoryForProgrammingLanguage(int programmingLanguageId)
    {
      ExtensionCategory programmingLanguage = new ExtensionCategory();
      programmingLanguage.Parent = this.TemplatesCategory;
      programmingLanguage.CategoryId = programmingLanguageId;
      string templateLanguage = ProgrammingLanguage.ProgrammingLanguageIdToVsixTemplateLanguageMap[programmingLanguageId];
      string str = ProgrammingLanguage.VsixTemplateLanguageToLanguageName[templateLanguage];
      programmingLanguage.LanguageTitles = new List<CategoryLanguageTitle>()
      {
        new CategoryLanguageTitle()
        {
          Lang = "",
          Lcid = 1033,
          Title = str
        }
      };
      return programmingLanguage;
    }

    public IdeCategory GetCategoryTree(
      IVssRequestContext requestContext,
      VisualStudioIdeVersion vsVersion,
      Guid categoryId,
      int level,
      string projectType,
      string templateType,
      string[] skus,
      string[] subSkus,
      int[] templateGroupIds,
      int[] vsVersions,
      string cultureName)
    {
      Dictionary<string, string> requestParams = new Dictionary<string, string>()
      {
        {
          "SearchSource",
          ""
        },
        {
          "LCID",
          new CultureInfo(cultureName).LCID.ToString((IFormatProvider) CultureInfo.InvariantCulture)
        },
        {
          "TemplateType",
          templateType
        },
        {
          "Skus",
          string.Join(",", skus ?? Array.Empty<string>())
        },
        {
          "SubSkus",
          string.Join(",", subSkus ?? Array.Empty<string>())
        },
        {
          "OSVersion",
          ""
        }
      };
      return this.GetCategoryTree(requestContext, vsVersion, categoryId, 0, (IDictionary<string, string>) requestParams);
    }

    public string[] GetCurrentVersionsForVsixList(
      IVssRequestContext requestContext,
      VisualStudioIdeVersion vsVersion,
      string[] vsixIds,
      IDictionary<string, string> requestParams)
    {
      if (requestParams == null || !VSServiceContextKeys.RequestContextIsValid(requestParams))
        throw new ArgumentException("Request context is not valid");
      string[] strArray = (string[]) null;
      ISet<string> stringSet = (ISet<string>) new HashSet<string>();
      Version version = (Version) null;
      if (requestParams.ContainsKey("Skus") && !requestParams["Skus"].IsNullOrEmpty<char>())
      {
        strArray = requestParams["Skus"].Split(',');
        for (int index = 0; index < strArray.Length; ++index)
        {
          if (!strArray[index].IsNullOrEmpty<char>())
            stringSet.Add("Microsoft.VisualStudio." + strArray[index]);
        }
      }
      if (requestParams.ContainsKey("SubSkus") && !requestParams["Skus"].IsNullOrEmpty<char>())
      {
        string[] values = requestParams["SubSkus"].Split(',');
        for (int index = 0; index < strArray.Length; ++index)
        {
          if (!values[index].IsNullOrEmpty<char>())
            stringSet.Add("Microsoft.VisualStudio." + values[index]);
        }
        stringSet.AddRange<string, ISet<string>>((IEnumerable<string>) values);
      }
      if (requestParams.ContainsKey("VSVersion") && !requestParams["VSVersion"].IsNullOrEmpty<char>())
        version = GalleryServerUtil.ConvertStringToVersion(requestParams["VSVersion"], false);
      this.CheckIfIdeApisAllowed(requestContext);
      if (vsixIds == null || ((IEnumerable<string>) vsixIds).IsNullOrEmpty<string>())
        return Array.Empty<string>();
      List<string> stringList = new List<string>();
      IDictionary<string, string> vsixIdsAndVersions = (IDictionary<string, string>) new Dictionary<string, string>();
      foreach (string vsixId in vsixIds)
        vsixIdsAndVersions.Add(vsixId, (string) null);
      IDictionary<string, PublishedExtension> vsixIdAndVersion = this.GetExtensionsByVsixIdAndVersion(requestContext, vsixIdsAndVersions, stringSet, version);
      foreach (string vsixId in vsixIds)
      {
        if (vsixIdAndVersion.ContainsKey(vsixId))
        {
          PublishedExtension extension = vsixIdAndVersion[vsixId];
          string str;
          if (extension != null && extension.Versions != null && extension.Versions.Count > 1)
          {
            int versionRecordIndex = GalleryServerUtil.GetApplicableLatestVersionRecordIndex(requestContext, extension, stringSet, version, "x86");
            str = extension.Versions[versionRecordIndex].Version;
          }
          else
            str = this.GetVsixVersionValue(extension);
          stringList.Add(str);
        }
        else
          stringList.Add((string) null);
      }
      requestContext.UpdateTimeToFirstPage();
      return stringList.ToArray();
    }

    public ReleaseQueryResult SearchReleases(
      IVssRequestContext requestContext,
      VisualStudioIdeVersion vsVersion,
      string searchText,
      string whereClause,
      string orderByClause,
      int? skip,
      int? take,
      IDictionary<string, string> requestParams)
    {
      if (requestParams == null || !VSServiceContextKeys.RequestContextIsValid(requestParams) || whereClause.IsNullOrEmpty<char>())
        throw new ArgumentException("Request context is not valid");
      this.CheckIfIdeApisAllowed(requestContext);
      VsIdeExtensionQuery vsIdeExtensionQuery = this._searchApiQuerybuilder.BuildQuery(searchText, whereClause, orderByClause, requestParams, skip, take);
      List<FilterCriteria> criteria = vsIdeExtensionQuery.ExtensionQuery.Filters[0].Criteria;
      ISet<string> targetSkUsFromFilter = this.GetTargetSKUsFromFilter(criteria);
      string valueOfFilterType = this.GetValueOfFilterType(criteria, 15);
      Version version = string.IsNullOrWhiteSpace(valueOfFilterType) ? (Version) null : Version.Parse(valueOfFilterType);
      if (!vsIdeExtensionQuery.MetadataFilters.IsNullOrEmpty<MetadataFilterItem>())
      {
        string valuesForFilterType1 = this.GetFilterValuesForFilterType((IList<MetadataFilterItem>) vsIdeExtensionQuery.MetadataFilters, "VsixId");
        string valuesForFilterType2 = this.GetFilterValuesForFilterType((IList<MetadataFilterItem>) vsIdeExtensionQuery.MetadataFilters, "VsixVersion");
        if (!valuesForFilterType1.IsNullOrEmpty<char>())
        {
          IDictionary<string, string> vsixIdsAndVersions = (IDictionary<string, string>) new Dictionary<string, string>();
          vsixIdsAndVersions.Add(valuesForFilterType1, valuesForFilterType2);
          IDictionary<string, PublishedExtension> vsixIdAndVersion = this.GetExtensionsByVsixIdAndVersion(requestContext, vsixIdsAndVersions, targetSkUsFromFilter, version);
          return this.GetReleaseQueryResultForUpdate(requestContext, vsixIdAndVersion.Values.ToList<PublishedExtension>(), targetSkUsFromFilter, version, "x86");
        }
      }
      this.FixIncomingCategoryId(vsIdeExtensionQuery);
      string requestParam1 = requestParams["LCID"];
      string requestParam2 = requestParams["SearchSource"];
      string forSearchReleases = this.GetKeyForSearchReleases(nameof (SearchReleases), requestParam1, whereClause, orderByClause, requestParam2);
      bool flag = searchText.IsNullOrEmpty<char>() && VisualStudioApiService.IsFirstPage(skip, take);
      SearchReleasesCache service = requestContext.GetService<SearchReleasesCache>();
      ReleaseQueryResult releaseQueryResult1;
      if (flag && service.TryGetValue<ReleaseQueryResult>(requestContext, forSearchReleases, out releaseQueryResult1))
        return releaseQueryResult1;
      ExtensionQueryResult extensionQueryResult = this.SearchViaCircuitBreaker(requestContext, vsIdeExtensionQuery);
      ReleaseQueryResult releaseQueryResult2 = this.GetReleaseQueryResult(requestContext, extensionQueryResult.Results[0], targetSkUsFromFilter, version);
      if (flag)
        service.Set(forSearchReleases, (object) releaseQueryResult2, service.TenMinutesExpiryProvider);
      return releaseQueryResult2;
    }

    private ISet<string> GetTargetSKUsFromFilter(List<FilterCriteria> filters)
    {
      ISet<string> targetSkUsFromFilter = (ISet<string>) new HashSet<string>();
      foreach (FilterCriteria filter in filters)
      {
        if (ExtensionQueryFilterType.InstallationTarget.Equals((object) filter.FilterType))
          targetSkUsFromFilter.Add(filter.Value);
      }
      return targetSkUsFromFilter;
    }

    private string GetValueOfFilterType(List<FilterCriteria> filters, int filterType)
    {
      foreach (FilterCriteria filter in filters)
      {
        if (filterType.Equals(filter.FilterType))
          return filter.Value;
      }
      return (string) null;
    }

    private void FixIncomingCategoryId(VsIdeExtensionQuery vsIdeExtensionQuery)
    {
      foreach (FilterCriteria criterion in vsIdeExtensionQuery.ExtensionQuery.Filters[0].Criteria)
      {
        if (criterion.FilterType == 5)
        {
          int num = this.SanitizeIncomingCategoryId(int.Parse(criterion.Value, (IFormatProvider) CultureInfo.InvariantCulture));
          criterion.Value = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        }
      }
    }

    private ExtensionQueryResult SearchViaCircuitBreaker(
      IVssRequestContext requestContext,
      VsIdeExtensionQuery vsIdeExtensionQuery)
    {
      Func<ExtensionQueryResult> run = (Func<ExtensionQueryResult>) (() =>
      {
        IPublishedExtensionService service = requestContext.GetService<IPublishedExtensionService>();
        try
        {
          return service.SearchExtensions(requestContext, vsIdeExtensionQuery.ExtensionQuery, (string) null, SearchOverrideFlags.DoNotTranslateCategoryFilter, (IEnumerable<MetadataFilterItem>) vsIdeExtensionQuery.MetadataFilters);
        }
        catch (InvalidExtensionQueryException ex)
        {
          ex.Data.Add((object) "{421AC3F1-A306-4C9B-B3F6-5812F9121FC8}", (object) true);
          throw;
        }
        catch (ArgumentException ex)
        {
          ex.Data.Add((object) "{421AC3F1-A306-4C9B-B3F6-5812F9121FC8}", (object) true);
          throw;
        }
      });
      CommandSetter setter = CommandSetter.WithGroupKey((CommandGroupKey) "Marketplace.").AndCommandKey((CommandKey) "SearchReleases").AndCommandPropertiesDefaults(this.SearchReleasesCircuitBreakerSettings);
      CommandService<ExtensionQueryResult> commandService = new CommandService<ExtensionQueryResult>(requestContext, setter, run);
      try
      {
        return commandService.Execute();
      }
      catch (CircuitBreakerException ex)
      {
        int eventId = 160008;
        if (ex is CircuitBreakerShortCircuitException)
          eventId = 160005;
        TeamFoundationEventLog.Default.Log(requestContext, ex.Message, eventId, EventLogEntryType.Error);
        throw;
      }
    }

    internal CommandPropertiesSetter SearchReleasesCircuitBreakerSettings => new CommandPropertiesSetter().WithExecutionTimeout(new TimeSpan(0, 0, 5)).WithExecutionMaxRequests(100).WithMetricsRollingStatisticalWindowInMilliseconds(60000);

    public ReleaseQueryResult SearchReleases(
      IVssRequestContext requestContext,
      VisualStudioIdeVersion vsVersion,
      string searchText,
      string whereClause,
      string orderByClause,
      int? locale,
      int? skip,
      int? take)
    {
      Dictionary<string, string> requestParams = new Dictionary<string, string>()
      {
        {
          "SearchSource",
          ""
        },
        {
          "LCID",
          (locale ?? 1033).ToString((IFormatProvider) CultureInfo.InvariantCulture)
        },
        {
          "OSVersion",
          ""
        }
      };
      return this.SearchReleases(requestContext, vsVersion, searchText, whereClause, orderByClause, skip, take, (IDictionary<string, string>) requestParams);
    }

    private ExtensionQueryResult GetExtensionQueryResultForUpdate(
      IVssRequestContext requestContext,
      List<PublishedExtension> publishedExtensions,
      bool sendMinimalData,
      ISet<string> targetSkus,
      Version targetVsIdeVersion,
      string productArchitecture)
    {
      if (publishedExtensions.IsNullOrEmpty<PublishedExtension>())
        publishedExtensions = new List<PublishedExtension>();
      ExtensionFilterResult extensionFilterResult = new ExtensionFilterResult()
      {
        Extensions = publishedExtensions
      };
      if (!sendMinimalData)
        extensionFilterResult.ResultMetadata = new List<ExtensionFilterResultMetadata>()
        {
          new ExtensionFilterResultMetadata()
          {
            MetadataType = QueryMetadataConstants.ResultCount,
            MetadataItems = new List<MetadataItem>()
            {
              new MetadataItem()
              {
                Name = QueryMetadataConstants.TotalCount,
                Count = publishedExtensions.Count
              }
            }
          }
        };
      return this.GetExtensionQueryResult(requestContext, extensionFilterResult, targetSkus, targetVsIdeVersion, productArchitecture, true, sendMinimalData);
    }

    private ExtensionQueryResult GetExtensionQueryResult(
      IVssRequestContext requestContext,
      ExtensionFilterResult extensionFilterResult,
      ISet<string> targetSkus,
      Version targetVsIdeVersion,
      string productArchitecture,
      bool isResultForUpdateCall = false,
      bool sendMinimalData = false)
    {
      ExtensionQueryResult extensionQueryResult = new ExtensionQueryResult();
      extensionQueryResult.Results = new List<ExtensionFilterResult>();
      if (extensionFilterResult != null)
      {
        List<PublishedExtension> publishedExtensionList = new List<PublishedExtension>();
        if (!extensionFilterResult.Extensions.IsNullOrEmpty<PublishedExtension>())
        {
          foreach (PublishedExtension extension in extensionFilterResult.Extensions)
          {
            int versionRecordIndex = GalleryServerUtil.GetApplicableLatestVersionRecordIndex(requestContext, extension, targetSkus, targetVsIdeVersion, productArchitecture);
            PublishedExtension publishedExtension = this.GetExtensionForVsIdeFromPublishedExtension(requestContext, extension, isResultForUpdateCall, sendMinimalData, versionRecordIndex);
            publishedExtensionList.Add(publishedExtension);
          }
        }
        extensionFilterResult.Extensions = publishedExtensionList;
        extensionQueryResult.Results.Add(extensionFilterResult);
      }
      else
        extensionQueryResult.Results.Add(new ExtensionFilterResult()
        {
          Extensions = new List<PublishedExtension>()
        });
      return extensionQueryResult;
    }

    private PublishedExtension GetExtensionForVsIdeFromPublishedExtension(
      IVssRequestContext requestContext,
      PublishedExtension publishedExtension,
      bool isResultForUpdateCall,
      bool sendMinimalData,
      int applicableVersionIndex)
    {
      if (publishedExtension == null)
        return (PublishedExtension) null;
      PublishedExtension toExtension = new PublishedExtension();
      if (!sendMinimalData)
      {
        toExtension.ExtensionId = publishedExtension.ExtensionId;
        toExtension.ExtensionName = publishedExtension.ExtensionName;
        toExtension.LastUpdated = publishedExtension.LastUpdated;
        toExtension.PublishedDate = publishedExtension.PublishedDate;
      }
      toExtension.DisplayName = publishedExtension.DisplayName;
      toExtension.ShortDescription = publishedExtension.ShortDescription;
      toExtension.Flags = publishedExtension.Flags;
      toExtension.Versions = new List<ExtensionVersion>();
      ExtensionVersion extensionVersion = new ExtensionVersion()
      {
        Properties = new List<KeyValuePair<string, string>>()
      };
      if (!sendMinimalData)
      {
        extensionVersion.Version = publishedExtension.Versions[applicableVersionIndex].Version;
        extensionVersion.LastUpdated = publishedExtension.Versions[applicableVersionIndex].LastUpdated;
      }
      toExtension.Versions.Add(extensionVersion);
      if (!sendMinimalData)
        this.AddStatisticsToExtension(requestContext, publishedExtension, toExtension);
      this.AddMetadataToVsIdePublishedExtension(requestContext, publishedExtension, toExtension, applicableVersionIndex, isResultForUpdateCall, sendMinimalData);
      return toExtension;
    }

    private void AddStatisticsToExtension(
      IVssRequestContext requestContext,
      PublishedExtension fromExtension,
      PublishedExtension toExtension)
    {
      if (fromExtension == null || fromExtension.Statistics.IsNullOrEmpty<ExtensionStatistic>())
        return;
      toExtension.Statistics = new List<ExtensionStatistic>();
      int num = 0;
      foreach (ExtensionStatistic statistic in fromExtension.Statistics)
      {
        if (string.Equals(statistic.StatisticName, "averagerating") || string.Equals(statistic.StatisticName, "ratingcount"))
          toExtension.Statistics.Add(new ExtensionStatistic()
          {
            StatisticName = statistic.StatisticName,
            Value = statistic.Value
          });
        else if (string.Equals(statistic.StatisticName, "install") || string.Equals(statistic.StatisticName, "migratedInstallCount"))
          num += (int) statistic.Value;
      }
      if (num == 0)
        return;
      toExtension.Statistics.Add(new ExtensionStatistic()
      {
        StatisticName = "downloadCount",
        Value = (double) num
      });
    }

    private void AddMetadataToVsIdePublishedExtension(
      IVssRequestContext requestContext,
      PublishedExtension fromExtension,
      PublishedExtension toExtension,
      int applicableVersionRecordIndex,
      bool isResultForUpdateCall = false,
      bool sendMinimalData = false)
    {
      foreach (KeyValuePair<string, string> keyValuePair in (IEnumerable<KeyValuePair<string, string>>) this.GetMetadataForVsIde(requestContext, fromExtension, false, applicableVersionRecordIndex, isResultForUpdateCall))
      {
        if (!sendMinimalData || VisualStudioApiService.MetadataValuesToIncludeInMinimalSet.Contains(keyValuePair.Key))
          toExtension.Versions[0].Properties.Add(keyValuePair);
      }
    }

    private ReleaseQueryResult GetReleaseQueryResultForUpdate(
      IVssRequestContext requestContext,
      List<PublishedExtension> publishedExtensions,
      ISet<string> targetSKUs,
      Version targetVsIdeVersion,
      string productArchitecture)
    {
      if (publishedExtensions.IsNullOrEmpty<PublishedExtension>())
        publishedExtensions = new List<PublishedExtension>();
      ExtensionFilterResult extensionFilterResult = new ExtensionFilterResult()
      {
        Extensions = publishedExtensions,
        ResultMetadata = new List<ExtensionFilterResultMetadata>()
        {
          new ExtensionFilterResultMetadata()
          {
            MetadataType = QueryMetadataConstants.ResultCount,
            MetadataItems = new List<MetadataItem>()
            {
              new MetadataItem()
              {
                Name = QueryMetadataConstants.TotalCount,
                Count = publishedExtensions.Count
              }
            }
          }
        }
      };
      return this.GetReleaseQueryResult(requestContext, extensionFilterResult, targetSKUs, targetVsIdeVersion, productArchitecture, true);
    }

    private ReleaseQueryResult GetReleaseQueryResult(
      IVssRequestContext requestContext,
      ExtensionFilterResult extensionFilterResult,
      ISet<string> targetSKUs,
      Version targetVsIdeVersion,
      string productArchitecture = "x86",
      bool isResultForUpdateCall = false)
    {
      ReleaseQueryResult releaseQueryResult = new ReleaseQueryResult();
      if (extensionFilterResult != null)
      {
        if (!extensionFilterResult.Extensions.IsNullOrEmpty<PublishedExtension>())
        {
          IList<Release> releaseList = (IList<Release>) new List<Release>();
          foreach (PublishedExtension extension in extensionFilterResult.Extensions)
          {
            int versionRecordIndex = GalleryServerUtil.GetApplicableLatestVersionRecordIndex(requestContext, extension, targetSKUs, targetVsIdeVersion, productArchitecture);
            Release publishedExtension = this.GetReleaseFromPublishedExtension(requestContext, extension, versionRecordIndex, isResultForUpdateCall);
            releaseList.Add(publishedExtension);
          }
          releaseQueryResult.Releases = (IEnumerable<Release>) releaseList;
        }
        if (extensionFilterResult.ResultMetadata != null)
        {
          foreach (ExtensionFilterResultMetadata filterResultMetadata in extensionFilterResult.ResultMetadata)
          {
            if (string.Equals(filterResultMetadata.MetadataType, QueryMetadataConstants.ResultCount))
            {
              using (List<MetadataItem>.Enumerator enumerator = filterResultMetadata.MetadataItems.GetEnumerator())
              {
                while (enumerator.MoveNext())
                {
                  MetadataItem current = enumerator.Current;
                  if (string.Equals(current.Name, QueryMetadataConstants.TotalCount))
                  {
                    releaseQueryResult.TotalCount = current.Count;
                    break;
                  }
                }
                break;
              }
            }
          }
        }
      }
      if (isResultForUpdateCall)
        requestContext.UpdateTimeToFirstPage();
      return releaseQueryResult;
    }

    private Release GetReleaseFromPublishedExtension(
      IVssRequestContext requestContext,
      PublishedExtension publishedExtension,
      int applicableVersionRecordIndex,
      bool isResultForUpdateCall = false)
    {
      if (publishedExtension == null)
        return (Release) null;
      Release release = new Release();
      this.AddStatisticsToRelease(publishedExtension, ref release);
      release.Project = new Project()
      {
        Title = publishedExtension.DisplayName,
        ModifiedDate = publishedExtension.LastUpdated,
        Description = publishedExtension.ShortDescription,
        Id = 0,
        Metadata = (IDictionary<string, string>) new Dictionary<string, string>()
      };
      this.AddMetadataToRelease(requestContext, publishedExtension, ref release, applicableVersionRecordIndex, isResultForUpdateCall);
      return release;
    }

    private string GetDeploymentTechnologyForExtension(PublishedExtension extension)
    {
      if (extension != null && extension.Metadata != null && extension.Metadata.Count > 0)
      {
        foreach (ExtensionMetadata extensionMetadata in extension.Metadata)
        {
          if (extensionMetadata.Key.Equals("DeploymentTechnology"))
            return extensionMetadata.Value;
        }
      }
      return (string) null;
    }

    private void AddStatisticsToRelease(PublishedExtension extension, ref Release release)
    {
      if (extension == null || extension.Statistics.IsNullOrEmpty<ExtensionStatistic>())
        return;
      int num = 0;
      foreach (ExtensionStatistic statistic in extension.Statistics)
      {
        if (string.Equals(statistic.StatisticName, "averagerating"))
          release.Rating = statistic.Value;
        else if (string.Equals(statistic.StatisticName, "ratingcount"))
          release.RatingsCount = (int) statistic.Value;
        else if (string.Equals(statistic.StatisticName, "install") || string.Equals(statistic.StatisticName, "updateCount") || string.Equals(statistic.StatisticName, "migratedInstallCount"))
          num += (int) statistic.Value;
      }
      if (num == 0)
        return;
      release.Files = (IList<ReleaseFile>) new List<ReleaseFile>()
      {
        new ReleaseFile() { DownloadCount = num }
      };
    }

    private void AddMetadataToRelease(
      IVssRequestContext requestContext,
      PublishedExtension extension,
      ref Release release,
      int applicableVersionRecordIndex,
      bool isResultForUpdateCall = false)
    {
      foreach (KeyValuePair<string, string> keyValuePair in (IEnumerable<KeyValuePair<string, string>>) this.GetMetadataForVsIde(requestContext, extension, true, applicableVersionRecordIndex, isResultForUpdateCall))
        release.Project.Metadata.Add(keyValuePair.Key, keyValuePair.Value);
    }

    private IDictionary<string, string> GetMetadataForVsIde(
      IVssRequestContext requestContext,
      PublishedExtension extension,
      bool isSOAPCall,
      int applicableVersionRecordIndex,
      bool isResultForUpdateCall = false)
    {
      IDictionary<string, string> dictionary = (IDictionary<string, string>) new Dictionary<string, string>();
      bool flag = false;
      IPublisherAssetService service = requestContext.GetService<IPublisherAssetService>();
      string str1 = "";
      string installOrUpdateQueryParam = isResultForUpdateCall ? "update" : "install";
      bool redirectVsAssetsToCdn = true;
      if (!extension.Metadata.IsNullOrEmpty<ExtensionMetadata>())
      {
        str1 = this.GetDeploymentTechnologyForExtension(extension);
        if (str1.Equals("Referral Link") || str1.Equals("EXE") || str1.Equals("MSI"))
          flag = true;
        foreach (ExtensionMetadata extensionMetadata in extension.Metadata)
        {
          if (VisualStudioApiService.AllowedMetadataValuesForVsIde.Contains(extensionMetadata.Key))
          {
            if ("VsixVersion".Equals(extensionMetadata.Key))
              dictionary.Add(extensionMetadata.Key, extension.Versions[applicableVersionRecordIndex].Version);
            else
              dictionary.Add(extensionMetadata.Key, extensionMetadata.Value);
          }
          if (flag && string.Equals(extensionMetadata.Key, "ReferralUrl"))
          {
            string payloadAssetUri = this.AddQueryParamsToPayloadAssetUri(service.GetAssetUri(requestContext, extension.Versions[0].FallbackAssetUri, "Microsoft.VisualStudio.Ide.Payload"), redirectVsAssetsToCdn, installOrUpdateQueryParam, (string) null);
            dictionary.Add("ReferralUrl", payloadAssetUri);
          }
        }
      }
      if (!dictionary.ContainsKey("Author"))
        dictionary.Add("Author", extension.Publisher.DisplayName);
      if (!isSOAPCall)
      {
        dictionary.Add("PublisherDomain", extension.Publisher.Domain);
        dictionary.Add("IsPublisherDomainVerified", extension.Publisher.IsDomainVerified.ToString());
      }
      ExtensionVersion version = extension.Versions[applicableVersionRecordIndex];
      foreach (ExtensionFile file1 in version.Files)
      {
        string key1 = (string) null;
        if (VisualStudioApiService.AssetTypeToMetadataKeyMap.TryGetValue(file1.AssetType, out key1))
        {
          if (file1.AssetType.Equals("Microsoft.VisualStudio.Ide.Payload"))
          {
            ExtensionFile extensionFile = file1;
            if (str1.Equals("EXE") || str1.Equals("MSI"))
            {
              foreach (ExtensionFile file2 in version.Files)
              {
                if (file2.FileId == file1.FileId && !file2.AssetType.Equals(file1.AssetType))
                  extensionFile = file2;
              }
            }
            Uri assetUri = service.GetAssetUri(requestContext, version.FallbackAssetUri, extensionFile.AssetType);
            string key2 = flag ? "ReferralUrl" : key1;
            dictionary.TryAdd<string, string>(key2, this.AddQueryParamsToPayloadAssetUri(assetUri, redirectVsAssetsToCdn, installOrUpdateQueryParam, version.TargetPlatform));
            if (!flag)
              dictionary.Add("DownloadUpdateUrl", this.AddQueryParamsToPayloadAssetUri(assetUri, redirectVsAssetsToCdn, "update", version.TargetPlatform));
          }
          else if (redirectVsAssetsToCdn)
            dictionary.Add(key1, file1.Source);
        }
      }
      string galleryDetailsPageUrl = GalleryServerUtil.GetGalleryDetailsPageUrl(requestContext, extension.Publisher.PublisherName, extension.ExtensionName);
      string str2 = GalleryServerUtil.GetGalleryDetailsPageUrl(requestContext, extension.Publisher.PublisherName, extension.ExtensionName) + "#report-abuse";
      dictionary.Add("MoreInfoUrl", galleryDetailsPageUrl);
      dictionary.Add("ReportAbuseUrl", str2);
      dictionary.Add("ReviewUrl", new UriBuilder(galleryDetailsPageUrl)
      {
        Fragment = ("#review-details".ElementAt<char>(0).Equals('#') ? "#review-details".Substring(1) : "#review-details")
      }.ToString());
      return dictionary;
    }

    private IDictionary<string, PublishedExtension> GetExtensionsByVsixIdAndVersion(
      IVssRequestContext requestContext,
      IDictionary<string, string> vsixIdsAndVersions,
      ISet<string> targetSkus,
      Version targetVersion,
      string productArchitecture = "x86")
    {
      Dictionary<string, PublishedExtension> dictionary = new Dictionary<string, PublishedExtension>();
      CachedPublishedExtensionData cachedData = this._inMemoryCache.GetCachedData(requestContext);
      foreach (KeyValuePair<string, string> vsixIdsAndVersion in (IEnumerable<KeyValuePair<string, string>>) vsixIdsAndVersions)
      {
        KeyValuePair<string, string> vsixIdAndVersion = vsixIdsAndVersion;
        PublishedExtension extension;
        cachedData.VsixIdToExtensionMap.TryGetValue(vsixIdAndVersion.Key, out extension);
        if (extension != null)
        {
          if (!vsixIdAndVersion.Value.IsNullOrEmpty<char>())
          {
            bool flag = false;
            if (GalleryServerUtil.IsVsixConsolidationEnabledForVsExtension(extension.Metadata))
            {
              flag = extension.Versions.Any<ExtensionVersion>((Func<ExtensionVersion, bool>) (extensionVersion => vsixIdAndVersion.Value.Equals(extensionVersion.Version)));
            }
            else
            {
              foreach (ExtensionMetadata extensionMetadata in extension.Metadata)
              {
                if (extensionMetadata.Key.Equals("VsixVersion") && extensionMetadata.Value.Equals(vsixIdAndVersion.Value))
                  flag = true;
              }
            }
            if (flag && this.IsExtensionApplicableForGivenTargetsAndVersion(extension, targetSkus, targetVersion, productArchitecture))
              dictionary.TryAdd<string, PublishedExtension>(vsixIdAndVersion.Key, extension);
          }
          else if (this.IsExtensionApplicableForGivenTargetsAndVersion(extension, targetSkus, targetVersion, productArchitecture))
            dictionary.TryAdd<string, PublishedExtension>(vsixIdAndVersion.Key, extension);
        }
      }
      return (IDictionary<string, PublishedExtension>) dictionary;
    }

    private bool IsExtensionApplicableForGivenTargetsAndVersion(
      PublishedExtension extension,
      ISet<string> targets,
      Version version,
      string productArchitecture)
    {
      bool flag = false;
      if (version == (Version) null)
        return true;
      if (extension != null && !extension.InstallationTargets.IsNullOrEmpty<InstallationTarget>() && version != (Version) null)
      {
        foreach (InstallationTarget installationTarget in extension.InstallationTargets)
        {
          if (!targets.IsNullOrEmpty<string>())
          {
            foreach (string target in (IEnumerable<string>) targets)
            {
              if (installationTarget.IsApplicableForVersion(version, target) && (string.IsNullOrWhiteSpace(installationTarget.ProductArchitecture) || string.Equals(installationTarget.ProductArchitecture, productArchitecture, StringComparison.OrdinalIgnoreCase)))
              {
                flag = true;
                break;
              }
            }
          }
          else if (installationTarget.IsApplicableForVersion(version) && (string.IsNullOrWhiteSpace(installationTarget.ProductArchitecture) || string.Equals(installationTarget.ProductArchitecture, productArchitecture, StringComparison.OrdinalIgnoreCase)))
          {
            flag = true;
            break;
          }
        }
      }
      return flag;
    }

    private MetadataFilterItem CreateMetadataFilterItemFromFilterCriteria(
      FilterCriteria filterCriteria)
    {
      string str1 = "!=";
      string str2 = "=";
      MetadataFilterItem.ComparisonOperator op = MetadataFilterItem.ComparisonOperator.Equal;
      if (filterCriteria.Value == null)
        throw new ArgumentException("FilterType = " + filterCriteria.FilterType.ToString() + " FilterValue = " + filterCriteria.Value);
      if (filterCriteria.Value.Contains(str1))
      {
        str2 = str1;
        op = MetadataFilterItem.ComparisonOperator.NotEqual;
      }
      string[] strArray = filterCriteria.Value.Split(new string[1]
      {
        str2
      }, StringSplitOptions.RemoveEmptyEntries);
      string key = "";
      string str3 = "";
      if (strArray.Length != 0)
        key = strArray[0].Trim();
      if (strArray.Length > 1)
        str3 = strArray[1].Trim();
      return new MetadataFilterItem(key, str3, op);
    }

    private string GetFilterValuesForFilterType(
      IList<MetadataFilterItem> metadataFilter,
      string filterType)
    {
      foreach (MetadataFilterItem metadataFilterItem in (IEnumerable<MetadataFilterItem>) metadataFilter)
      {
        if (metadataFilterItem.Key.Equals(filterType, StringComparison.OrdinalIgnoreCase))
          return metadataFilterItem.Value;
      }
      return (string) null;
    }

    private int SanitizeIncomingCategoryId(int categoryId) => categoryId != 1 ? categoryId : this.TemplatesCategory.CategoryId;

    private int SanitizeOutgoingCategoryId(int categoryId) => categoryId != this.TemplatesCategory.CategoryId ? categoryId : 1;

    private VsIdeExtensionQuery GetQueryForCategoryTree(
      string vsVersionFromContext,
      string serviceSource,
      string templateType,
      string[] skus,
      string[] subSkus,
      int? parentCategoryId,
      int? programmingLanguageId,
      CultureInfo cultureInfo,
      string productArchitecture = null)
    {
      VsIdeExtensionQuery queryForCategoryTree = this._searchApiQuerybuilder.BuildQueryForCategories(vsVersionFromContext, serviceSource, templateType, skus, subSkus, parentCategoryId, programmingLanguageId, cultureInfo, productArchitecture);
      int? nullable1 = parentCategoryId;
      int categoryId = this.TemplatesCategory.CategoryId;
      if (nullable1.GetValueOrDefault() == categoryId & nullable1.HasValue)
      {
        int? nullable2 = programmingLanguageId;
        int num = 0;
        if (nullable2.GetValueOrDefault() == num & nullable2.HasValue)
        {
          ExtensionQuery extensionQuery = queryForCategoryTree.ExtensionQuery;
          ExtensionQueryResultMetadataFlags? metadataFlags = extensionQuery.MetadataFlags;
          extensionQuery.MetadataFlags = metadataFlags.HasValue ? new ExtensionQueryResultMetadataFlags?(metadataFlags.GetValueOrDefault() | ExtensionQueryResultMetadataFlags.IncludeResultSetProjectType) : new ExtensionQueryResultMetadataFlags?();
        }
      }
      return queryForCategoryTree;
    }

    private IList<ExtensionCategory> GetProgrammingLanguages(
      IVssRequestContext requestContext,
      VsIdeExtensionQuery ideExtensionQuery,
      CultureInfo cultureInfo)
    {
      Func<IList<ExtensionCategory>> run = (Func<IList<ExtensionCategory>>) (() =>
      {
        List<ExtensionCategory> programmingLanguages = new List<ExtensionCategory>();
        IPublishedExtensionService service = requestContext.GetService<IPublishedExtensionService>();
        ExtensionQueryResult extensionQueryResult;
        try
        {
          extensionQueryResult = service.SearchExtensions(requestContext, ideExtensionQuery.ExtensionQuery, (string) null, SearchOverrideFlags.DoNotTranslateCategoryFilter, (IEnumerable<MetadataFilterItem>) ideExtensionQuery.MetadataFilters);
        }
        catch (InvalidExtensionQueryException ex)
        {
          ex.Data.Add((object) "{421AC3F1-A306-4C9B-B3F6-5812F9121FC8}", (object) true);
          throw;
        }
        catch (ArgumentException ex)
        {
          ex.Data.Add((object) "{421AC3F1-A306-4C9B-B3F6-5812F9121FC8}", (object) true);
          throw;
        }
        if (extensionQueryResult == null || extensionQueryResult.Results.IsNullOrEmpty<ExtensionFilterResult>() || extensionQueryResult.Results[0].ResultMetadata == null)
          return (IList<ExtensionCategory>) programmingLanguages;
        IPublishedExtensionService extensionService = service;
        IVssRequestContext requestContext1 = requestContext;
        List<string> languages = new List<string>();
        languages.Add(cultureInfo.Name);
        languages.Add(this.DefaultCategoryCultureName);
        string templatesCategoryName = this.TemplatesCategoryName;
        CategoriesResult categoriesResult = extensionService.QueryAvailableCategories(requestContext1, (IEnumerable<string>) languages, templatesCategoryName, "vs");
        ExtensionCategory extensionCategory1 = this.TemplatesCategory;
        foreach (ExtensionCategory category in categoriesResult.Categories)
        {
          if (string.Equals(category.CategoryName, this.TemplatesCategoryName))
          {
            extensionCategory1 = category;
            break;
          }
        }
        foreach (ExtensionFilterResultMetadata filterResultMetadata in extensionQueryResult.Results[0].ResultMetadata)
        {
          if (filterResultMetadata.MetadataType.Equals(QueryMetadataConstants.ResultSetProjectTypes))
          {
            bool flag = false;
            using (List<MetadataItem>.Enumerator enumerator = filterResultMetadata.MetadataItems.GetEnumerator())
            {
              while (enumerator.MoveNext())
              {
                MetadataItem metadataItem = enumerator.Current;
                if (!metadataItem.Name.IsNullOrEmpty<char>() && ProgrammingLanguage.VsixTemplateLanguageToLanguageName.ContainsKey(metadataItem.Name))
                {
                  ExtensionCategory extensionCategory2 = new ExtensionCategory()
                  {
                    Parent = extensionCategory1,
                    LanguageTitles = new List<CategoryLanguageTitle>()
                    {
                      new CategoryLanguageTitle()
                      {
                        Lang = "",
                        Lcid = 1033,
                        Title = ProgrammingLanguage.VsixTemplateLanguageToLanguageName[metadataItem.Name]
                      }
                    }
                  };
                  extensionCategory2.CategoryId = ProgrammingLanguage.ProgrammingLanguageIdToVsixTemplateLanguageMap.Where<KeyValuePair<int, string>>((Func<KeyValuePair<int, string>, bool>) (x => x.Value.Equals(metadataItem.Name))).First<KeyValuePair<int, string>>().Key;
                  programmingLanguages.Add(extensionCategory2);
                }
                else if (!flag)
                {
                  ExtensionCategory extensionCategory3 = new ExtensionCategory()
                  {
                    Parent = extensionCategory1,
                    LanguageTitles = new List<CategoryLanguageTitle>()
                    {
                      new CategoryLanguageTitle()
                      {
                        Lang = "",
                        Lcid = 1033,
                        Title = "Other"
                      }
                    }
                  };
                  extensionCategory3.CategoryId = ProgrammingLanguage.ProgrammingLanguageIdToVsixTemplateLanguageMap.Where<KeyValuePair<int, string>>((Func<KeyValuePair<int, string>, bool>) (x => x.Value.Equals("Other"))).First<KeyValuePair<int, string>>().Key;
                  programmingLanguages.Add(extensionCategory3);
                  flag = true;
                }
              }
              break;
            }
          }
        }
        return (IList<ExtensionCategory>) programmingLanguages;
      });
      CommandSetter setter = CommandSetter.WithGroupKey((CommandGroupKey) "Marketplace.").AndCommandKey((CommandKey) "GetCategories").AndCommandPropertiesDefaults(this.GetCategoriesCircuitBreakerSettings);
      CommandService<IList<ExtensionCategory>> commandService = new CommandService<IList<ExtensionCategory>>(requestContext, setter, run);
      try
      {
        return commandService.Execute();
      }
      catch (CircuitBreakerException ex)
      {
        int eventId = 160009;
        if (ex is CircuitBreakerShortCircuitException)
          eventId = 160006;
        TeamFoundationEventLog.Default.Log(requestContext, ex.Message, eventId, EventLogEntryType.Error);
        throw;
      }
    }

    private IList<ExtensionCategory> GetCategories(
      IVssRequestContext requestContext,
      VsIdeExtensionQuery ideExtensionQuery,
      CultureInfo cultureInfo,
      int? parentCategoryId)
    {
      Func<IList<ExtensionCategory>> run = (Func<IList<ExtensionCategory>>) (() =>
      {
        List<ExtensionCategory> categories = new List<ExtensionCategory>();
        IPublishedExtensionService service = requestContext.GetService<IPublishedExtensionService>();
        ExtensionQueryResult extensionQueryResult;
        try
        {
          extensionQueryResult = service.SearchExtensions(requestContext, ideExtensionQuery.ExtensionQuery, (string) null, SearchOverrideFlags.DoNotTranslateCategoryFilter, (IEnumerable<MetadataFilterItem>) ideExtensionQuery.MetadataFilters);
        }
        catch (InvalidExtensionQueryException ex)
        {
          ex.Data.Add((object) "{421AC3F1-A306-4C9B-B3F6-5812F9121FC8}", (object) true);
          throw;
        }
        catch (ArgumentException ex)
        {
          ex.Data.Add((object) "{421AC3F1-A306-4C9B-B3F6-5812F9121FC8}", (object) true);
          throw;
        }
        if (extensionQueryResult == null || extensionQueryResult.Results.IsNullOrEmpty<ExtensionFilterResult>() || extensionQueryResult.Results[0].ResultMetadata == null)
          return (IList<ExtensionCategory>) categories;
        ISet<int> intSet = (ISet<int>) new HashSet<int>();
        foreach (ExtensionFilterResultMetadata filterResultMetadata in extensionQueryResult.Results[0].ResultMetadata)
        {
          if (filterResultMetadata.MetadataType.Equals(QueryMetadataConstants.ResultSetCategories))
          {
            using (List<MetadataItem>.Enumerator enumerator = filterResultMetadata.MetadataItems.GetEnumerator())
            {
              while (enumerator.MoveNext())
              {
                int result;
                if (int.TryParse(enumerator.Current.Name, out result))
                  intSet.Add(result);
              }
              break;
            }
          }
        }
        CategoriesResult categoriesResult;
        try
        {
          categoriesResult = service.QueryAvailableCategories(requestContext, (IEnumerable<string>) new List<string>()
          {
            cultureInfo.Name,
            this.DefaultCategoryCultureName
          }, product: "vs");
        }
        catch (ArgumentException ex)
        {
          ex.Data.Add((object) "{421AC3F1-A306-4C9B-B3F6-5812F9121FC8}", (object) true);
          throw;
        }
        foreach (ExtensionCategory category in categoriesResult.Categories)
        {
          if (intSet.Contains(category.CategoryId))
          {
            if (parentCategoryId.HasValue)
            {
              if (category.Parent != null && category.Parent.CategoryId == parentCategoryId.Value)
                categories.Add(category);
            }
            else if (category.ParentCategoryName.IsNullOrEmpty<char>())
              categories.Add(category);
          }
        }
        return (IList<ExtensionCategory>) categories;
      });
      CommandSetter setter = CommandSetter.WithGroupKey((CommandGroupKey) "Marketplace.").AndCommandKey((CommandKey) nameof (GetCategories)).AndCommandPropertiesDefaults(this.GetCategoriesCircuitBreakerSettings);
      CommandService<IList<ExtensionCategory>> commandService = new CommandService<IList<ExtensionCategory>>(requestContext, setter, run);
      try
      {
        return commandService.Execute();
      }
      catch (CircuitBreakerException ex)
      {
        int eventId = 160009;
        if (ex is CircuitBreakerShortCircuitException)
          eventId = 160006;
        TeamFoundationEventLog.Default.Log(requestContext, ex.Message, eventId, EventLogEntryType.Error);
        throw;
      }
    }

    internal CommandPropertiesSetter GetCategoriesCircuitBreakerSettings => new CommandPropertiesSetter().WithExecutionTimeout(new TimeSpan(0, 0, 5)).WithExecutionMaxRequests(100).WithMetricsRollingStatisticalWindowInMilliseconds(60000);

    private Guid GetGuidForCategory(
      ExtensionCategory category,
      int programmingLanguageId = 0,
      bool useCategoryIdAsProgrammingLanguageId = false)
    {
      return category.Parent == null ? VsIdeCategoryIdParser.ConcatIds(this.SanitizeOutgoingCategoryId(category.CategoryId)) : (useCategoryIdAsProgrammingLanguageId ? VsIdeCategoryIdParser.ConcatIds(this.SanitizeOutgoingCategoryId(category.Parent.CategoryId), programmingLanguageId, category.CategoryId) : VsIdeCategoryIdParser.ConcatIds(this.SanitizeOutgoingCategoryId(category.Parent.CategoryId), category.CategoryId, 0));
    }

    private IdeCategory ConvertToIdeCategory(
      ExtensionCategory extensionCategory,
      string cultureName,
      bool hasMore = false,
      int programmingLanguageId = 0,
      bool useProgrammingLanguageId = false)
    {
      return new IdeCategory()
      {
        Id = this.GetGuidForCategory(extensionCategory, programmingLanguageId, useProgrammingLanguageId),
        Title = this.GetTitleForCultureOrFirst(extensionCategory.LanguageTitles, cultureName),
        HasMore = hasMore
      };
    }

    private Category ConvertToCategory(ExtensionCategory extensionCategory, string cultureName) => new Category()
    {
      Id = extensionCategory.CategoryId,
      Title = this.GetTitleForCultureOrFirst(extensionCategory.LanguageTitles, cultureName)
    };

    private string GetTitleForCultureOrFirst(List<CategoryLanguageTitle> titles, string cultureName)
    {
      foreach (CategoryLanguageTitle title in titles)
      {
        if (title.Lang.Equals(cultureName, StringComparison.OrdinalIgnoreCase))
          return title.Title;
      }
      return titles.First<CategoryLanguageTitle>().Title;
    }

    private string GetReportAbuseUrlForVsGallery(string extensionId) => !extensionId.IsNullOrEmpty<char>() ? new Uri(new Uri(this._vsGalleryBaseUrl), extensionId)?.ToString() + "/view/ReportAbuse" : string.Empty;

    private string GetVisualStudioIdeVersionStringFromEnum(VisualStudioIdeVersion ideVersion)
    {
      foreach (KeyValuePair<string, VisualStudioIdeVersion> studioVersionNumbers in (IEnumerable<KeyValuePair<string, VisualStudioIdeVersion>>) ServiceHelper.VisualStudioVersionNumbersMap)
      {
        if (studioVersionNumbers.Value == ideVersion)
          return studioVersionNumbers.Key;
      }
      return (string) null;
    }

    private static bool IsFirstPage(int? skip, int? take)
    {
      int? nullable1 = skip;
      int num1 = 0;
      if (!(nullable1.GetValueOrDefault() == num1 & nullable1.HasValue))
        return false;
      int? nullable2 = take;
      int num2 = 25;
      return nullable2.GetValueOrDefault() == num2 & nullable2.HasValue;
    }

    private string AddQueryParamsToPayloadAssetUri(
      Uri payloadAssetUri,
      bool redirectVsAssetsToCdn,
      string installOrUpdateQueryParam,
      string targetPlatform)
    {
      UriBuilder uriBuilder = new UriBuilder(payloadAssetUri.ToString());
      NameValueCollection queryString = HttpUtility.ParseQueryString("");
      if (!string.IsNullOrWhiteSpace(targetPlatform))
        queryString[nameof (targetPlatform)] = targetPlatform;
      if (redirectVsAssetsToCdn)
        queryString["redirect"] = "true";
      queryString[installOrUpdateQueryParam] = "true";
      uriBuilder.Query = queryString.ToString();
      return uriBuilder.ToString();
    }

    private string GetKeyForRootCategories(string methodName, string cultureOrLcid) => methodName + this.GetKeyPart(cultureOrLcid);

    private string GetKeyForSearchForVsIde(
      string methodName,
      VsIdeExtensionQuery vsIdeExtensionQuery)
    {
      string str = methodName;
      ISet<FilterCriteria> filterCriteriaSet = (ISet<FilterCriteria>) new SortedSet<FilterCriteria>((IComparer<FilterCriteria>) new FilterCriteriaComparer());
      QueryFilter filter = vsIdeExtensionQuery.ExtensionQuery.Filters[0];
      foreach (FilterCriteria criterion in filter.Criteria)
        filterCriteriaSet.Add(criterion);
      ISet<MetadataFilterItem> metadataFilterItemSet = (ISet<MetadataFilterItem>) new SortedSet<MetadataFilterItem>((IComparer<MetadataFilterItem>) new MetadataFilterItemComparer());
      foreach (MetadataFilterItem metadataFilter in vsIdeExtensionQuery.MetadataFilters)
        metadataFilterItemSet.Add(metadataFilter);
      foreach (FilterCriteria filterCriteria in (IEnumerable<FilterCriteria>) filterCriteriaSet)
        str += this.GetKeyPart(filterCriteria.FilterType.ToString() + "-" + filterCriteria.Value);
      foreach (MetadataFilterItem metadataFilterItem in (IEnumerable<MetadataFilterItem>) metadataFilterItemSet)
        str += this.GetKeyPart(metadataFilterItem.Key + "-" + metadataFilterItem.Operator.ToString() + "-" + metadataFilterItem.Value);
      return str + this.GetKeyPart(filter.PageNumber.ToString((IFormatProvider) CultureInfo.InvariantCulture)) + this.GetKeyPart(filter.PageSize.ToString((IFormatProvider) CultureInfo.InvariantCulture)) + this.GetKeyPart(filter.SortBy.ToString((IFormatProvider) CultureInfo.InvariantCulture)) + this.GetKeyPart(filter.SortOrder.ToString((IFormatProvider) CultureInfo.InvariantCulture));
    }

    private string GetKeyForCategoryTree(
      string methodName,
      string cultureOrLcid,
      string vsVersion,
      string categoryIdOrGuid,
      string templateType,
      string serviceSource,
      string[] skus,
      string[] subSkus,
      string productArchitecture)
    {
      string str = methodName + this.GetKeyPart(cultureOrLcid) + this.GetKeyPart(vsVersion) + this.GetKeyPart(categoryIdOrGuid) + this.GetKeyPart(templateType) + this.GetKeyPart(serviceSource);
      ISet<string> stringSet = (ISet<string>) new SortedSet<string>((IComparer<string>) StringComparer.OrdinalIgnoreCase);
      if (!((IEnumerable<string>) skus).IsNullOrEmpty<string>())
        stringSet.AddRange<string, ISet<string>>((IEnumerable<string>) skus);
      if (!((IEnumerable<string>) subSkus).IsNullOrEmpty<string>())
        stringSet.AddRange<string, ISet<string>>((IEnumerable<string>) subSkus);
      string keyForCategoryTree = str + this.GetKeyPart(string.Join("-", (IEnumerable<string>) stringSet));
      if (!productArchitecture.IsNullOrEmpty<char>())
        keyForCategoryTree += this.GetKeyPart(productArchitecture);
      return keyForCategoryTree;
    }

    private string GetKeyForSearchReleases(
      string methodName,
      string cultureOrLcid,
      string whereClause,
      string orderByClause,
      string serviceSource)
    {
      return methodName + this.GetKeyPart(cultureOrLcid) + this.GetKeyPart(whereClause) + this.GetKeyPart(orderByClause) + this.GetKeyPart(serviceSource);
    }

    private string GetKeyPart(string addition) => "::" + addition;

    private void CheckIfIdeApisAllowed(IVssRequestContext requestContext)
    {
      if (!requestContext.IsFeatureEnabled("Microsoft.VisualStudio.Services.Gallery.EnableVsIdeCalls"))
        throw new MethodNotAvailableException("Server Unavailable");
    }

    private string GetVsixVersionValue(PublishedExtension extension)
    {
      if (extension != null && !extension.Metadata.IsNullOrEmpty<ExtensionMetadata>())
      {
        ExtensionMetadata extensionMetadata = extension.Metadata.FirstOrDefault<ExtensionMetadata>((Func<ExtensionMetadata, bool>) (m => m.Key.Equals("VsixVersion")));
        if (extensionMetadata != null)
          return extensionMetadata.Value;
      }
      return (string) null;
    }
  }
}
