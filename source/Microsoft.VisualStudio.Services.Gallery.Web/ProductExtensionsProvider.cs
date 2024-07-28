// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Web.ProductExtensionsProvider
// Assembly: Microsoft.VisualStudio.Services.Gallery.Web, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 17D36576-2EF3-4ABC-94BA-AF7891D15A3A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Web.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Gallery.Server;
using Microsoft.VisualStudio.Services.Gallery.Web.Models;
using Microsoft.VisualStudio.Services.Gallery.WebApi;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Web;

namespace Microsoft.VisualStudio.Services.Gallery.Web
{
  public class ProductExtensionsProvider : IProductExtensionsDataProvider
  {
    private IPublishedExtensionService _extensionService;
    internal const string OTHER_CATEGORY_NAME = "Other";
    internal const string DEVELOPER_SAMPLES = "Developer samples";
    internal const string FEATURED_CATEGORY_NAME = "Featured";
    internal const string RECENTLY_ADDED_ITEMS_NAME = "Recently Added";
    internal const string MOST_POPULAR_ITEMS_NAME = "Most Popular";
    internal const string TOP_PAID_ITEMS_NAME = "Top Paid";
    internal const string TOP_FREE_ITEMS_NAME = "Top Free";
    internal const string HIGHEST_RATED_NAME = "Highest Rated";
    internal const string TRENDING_WEEKLY = "TrendingWeekly";
    internal const string TRENDING_DAILY = "TrendingDaily";
    internal const string TRENDING_MONTHLY = "TrendingMonthly";
    internal static readonly List<string> SPECIAL_CATEGORIES = new List<string>()
    {
      "Featured",
      "Most Popular",
      "Top Paid",
      "Top Free",
      "TrendingWeekly",
      "TrendingDaily",
      "TrendingMonthly",
      "Recently Added",
      "Highest Rated"
    };

    public ProductExtensionsProvider(IVssRequestContext tfsRequestContext) => this._extensionService = tfsRequestContext.GetService<IPublishedExtensionService>();

    public object GetProductExtensions(IVssRequestContext requestContext, string product)
    {
      if (this._isProductVSSubs(product))
        return (object) this.PopulateOffers(requestContext);
      Stopwatch stopwatch = Stopwatch.StartNew();
      ProductExtensions extensionsFromDb = this.GetProductExtensionsFromDb(requestContext, product);
      stopwatch.Stop();
      CustomerIntelligenceData properties = new CustomerIntelligenceData();
      properties.Add("eventType", "GetProductExtensionsRefresh");
      properties.Add(nameof (product), product);
      properties.Add("TimeTaken", (double) stopwatch.ElapsedMilliseconds);
      requestContext.GetService<CustomerIntelligenceService>().Publish(requestContext, "Microsoft.VisualStudio.Services.Gallery", "HomepageCache", properties);
      return (object) extensionsFromDb;
    }

    private ProductExtensions GetProductExtensionsFromDb(
      IVssRequestContext requestContext,
      string product)
    {
      ProductExtensions extensionsFromDb = new ProductExtensions();
      Dictionary<string, List<PublishedExtension>> categoryDirectDb = this.GetAllExtensionsPerCategoryDirectDB(requestContext, product);
      List<ExtensionPerCategory> extensionPerCategoryList = new List<ExtensionPerCategory>();
      foreach (string key in categoryDirectDb.Keys)
      {
        if (this.ShouldAddCategoryToResult(product, key))
        {
          if (requestContext.IsFeatureEnabled("Microsoft.VisualStudio.Services.Gallery.DedupeHomepageExtensions") && (key.Equals("TrendingWeekly", StringComparison.InvariantCultureIgnoreCase) || key.Equals("TrendingMonthly", StringComparison.InvariantCultureIgnoreCase) || key.Equals("TrendingDaily", StringComparison.InvariantCultureIgnoreCase)))
          {
            string statType = this._getTrendingStatType(key);
            List<PublishedExtension> list = categoryDirectDb[key].OrderByDescending<PublishedExtension, double>((Func<PublishedExtension, double>) (extension => this.GetTrendingScoreForExtension(extension, statType))).ThenByDescending<PublishedExtension, double>((Func<PublishedExtension, double>) (extension => this.GetInstallCountForExtension(extension))).Where<PublishedExtension>((Func<PublishedExtension, bool>) (extension => this.GetTrendingScoreForExtension(extension, statType) > 0.0)).ToList<PublishedExtension>().Distinct<PublishedExtension>().ToList<PublishedExtension>();
            if (list != null && list.Count > 0)
            {
              bool flag = this.HasMoreExtensions(list.Count);
              extensionPerCategoryList.Add(new ExtensionPerCategory()
              {
                CategoryName = key,
                Extensions = this.ConvertToExtensionList(requestContext, list),
                HasMoreExtensions = flag
              });
            }
            else
              extensionPerCategoryList.Add(new ExtensionPerCategory()
              {
                CategoryName = key,
                Extensions = new List<BaseExtensionItem>(),
                HasMoreExtensions = false
              });
          }
          else if (key.Equals("TrendingWeekly", StringComparison.InvariantCultureIgnoreCase))
          {
            List<PublishedExtension> list = categoryDirectDb[key].OrderByDescending<PublishedExtension, double>((Func<PublishedExtension, double>) (extension => this.GetTrendingScoreForExtension(extension, "trendingweekly"))).ThenByDescending<PublishedExtension, double>((Func<PublishedExtension, double>) (extension => this.GetInstallCountForExtension(extension))).Where<PublishedExtension>((Func<PublishedExtension, bool>) (extension => this.GetTrendingScoreForExtension(extension, "trendingweekly") > 0.0)).ToList<PublishedExtension>().Distinct<PublishedExtension>().ToList<PublishedExtension>();
            if (list != null && list.Count > 0)
            {
              bool flag = this.HasMoreExtensions(list.Count);
              extensionPerCategoryList.Add(new ExtensionPerCategory()
              {
                CategoryName = key,
                Extensions = this.ConvertToExtensionList(requestContext, list),
                HasMoreExtensions = flag
              });
            }
            else
              extensionPerCategoryList.Add(new ExtensionPerCategory()
              {
                CategoryName = key,
                Extensions = new List<BaseExtensionItem>(),
                HasMoreExtensions = false
              });
          }
          else
            extensionPerCategoryList.Add(new ExtensionPerCategory()
            {
              CategoryName = key,
              Extensions = this.ConvertToExtensionList(requestContext, categoryDirectDb[key]),
              HasMoreExtensions = this.HasMoreExtensions(categoryDirectDb[key].Count)
            });
        }
      }
      extensionsFromDb.ExtensionsPerCategory = extensionPerCategoryList;
      extensionsFromDb.Categories = this._isProductVS(product) ? this.PopulateVSCategories() : this.OrderCategoryNames(((IEnumerable<string>) this.GetNonEmptyCategories(requestContext, product, categoryDirectDb)).ToList<string>());
      return extensionsFromDb;
    }

    private string _getTrendingStatType(string categoryName)
    {
      if (categoryName.Equals("TrendingDaily", StringComparison.OrdinalIgnoreCase))
        return "trendingdaily";
      return categoryName.Equals("TrendingMonthly", StringComparison.OrdinalIgnoreCase) ? "trendingmonthly" : "trendingweekly";
    }

    private string[] PopulateVSCategories() => new string[6]
    {
      GalleryResources.VSCategories_Coding_MigratedTitle,
      GalleryResources.VSCategories_Framework_MigratedTitle,
      GalleryResources.VSCategories_Language_MigratedTitle,
      GalleryResources.VSCategories_WinForms_MigratedTitle,
      GalleryResources.VSCategories_TeamDevelopment_MigratedTitle,
      GalleryResources.VSCategories_SeeAll_Title
    };

    private bool ShouldAddCategoryToResult(string product, string category)
    {
      bool result = true;
      if ((this._isProductVSCode(product) || this._isProductVS(product) || this._isProductVSForMac(product)) && !ProductExtensionsProvider.SPECIAL_CATEGORIES.Contains(category))
        result = false;
      return result;
    }

    private string[] GetNonEmptyCategories(
      IVssRequestContext requestContext,
      string product,
      Dictionary<string, List<PublishedExtension>> categoryMap)
    {
      List<string> stringList = new List<string>();
      foreach (string key in categoryMap.Keys)
      {
        if (categoryMap[key].Where<PublishedExtension>((Func<PublishedExtension, bool>) (extension => this.ShouldIncludeExtension(requestContext, extension))).Count<PublishedExtension>() > 0 && !ProductExtensionsProvider.SPECIAL_CATEGORIES.Contains(key))
          stringList.Add(key);
      }
      return stringList.ToArray();
    }

    private List<BaseExtensionItem> ConvertToExtensionList(
      IVssRequestContext requestContext,
      List<PublishedExtension> extensions)
    {
      List<BaseExtensionItem> extensionList = new List<BaseExtensionItem>();
      extensions = GalleryServerUtil.SetEffectiveDisplayedStarRating(requestContext, extensions);
      foreach (PublishedExtension extension in extensions)
        extensionList.Add((BaseExtensionItem) new VSTSExtensionItem(extension));
      return extensionList;
    }

    private bool _isProductVSTS(string product) => product.Equals("vsts", StringComparison.InvariantCultureIgnoreCase);

    private bool _isProductVSForMac(string product) => product.Equals("vsformac", StringComparison.InvariantCultureIgnoreCase);

    private bool _isProductVSCode(string product) => product.Equals("vscode", StringComparison.InvariantCultureIgnoreCase);

    private bool _isProductVS(string product) => product.Equals("vs", StringComparison.InvariantCultureIgnoreCase);

    private bool _isProductVSSubs(string product) => product.Equals("subscriptions", StringComparison.InvariantCultureIgnoreCase);

    private bool HasMoreExtensions(int extensionListSize)
    {
      bool flag = false;
      if (extensionListSize > 6)
        flag = true;
      return flag;
    }

    internal double GetInstallCountForExtension(PublishedExtension extension)
    {
      double countForExtension = 0.0;
      if (extension.Statistics != null && extension.Statistics.Count > 0)
      {
        foreach (ExtensionStatistic statistic in extension.Statistics)
        {
          if (statistic.StatisticName.Equals("install", StringComparison.OrdinalIgnoreCase))
            countForExtension = statistic.Value;
        }
      }
      return countForExtension;
    }

    private Dictionary<string, List<PublishedExtension>> GetAllExtensionsPerCategoryDirectDB(
      IVssRequestContext requestContext,
      string product)
    {
      List<string> categoriesForProduct = this.GetAllCategoriesForProduct(requestContext, product);
      Dictionary<string, List<PublishedExtension>> categoryDirectDb = new Dictionary<string, List<PublishedExtension>>();
      ExtensionQuery andAllCategories = this.GetBatchQueryForProductAndAllCategories(product, categoriesForProduct, requestContext);
      ExtensionQueryResult extensionQueryResult = this._extensionService.QueryExtensions(requestContext, andAllCategories, (string) null);
      if (extensionQueryResult.Results == null || extensionQueryResult.Results.Count == 0)
        return categoryDirectDb;
      for (int index = 0; index < categoriesForProduct.Count; ++index)
      {
        if (extensionQueryResult.Results[index] != null && extensionQueryResult.Results[index].Extensions.Count != 0)
          categoryDirectDb[categoriesForProduct[index]] = extensionQueryResult.Results[index].Extensions;
      }
      return categoryDirectDb;
    }

    private List<string> GetAllCategoriesForProduct(
      IVssRequestContext requestContext,
      string product)
    {
      List<string> categoriesForProduct = new List<string>();
      if (requestContext.ExecutionEnvironment.IsHostedDeployment)
      {
        categoriesForProduct.Add("Featured");
        if (this._isProductVS(product))
        {
          if (requestContext.IsFeatureEnabled("Microsoft.VisualStudio.Services.Gallery.VsTrendingHomepage"))
          {
            categoriesForProduct.Add("TrendingWeekly");
            if (requestContext.IsFeatureEnabled("Microsoft.VisualStudio.Services.Gallery.DedupeHomepageExtensions"))
            {
              categoriesForProduct.Add("TrendingDaily");
              categoriesForProduct.Add("TrendingMonthly");
            }
          }
          categoriesForProduct.Add("Most Popular");
          categoriesForProduct.Add("Highest Rated");
        }
        else
        {
          if (this._isProductVSTS(product))
          {
            categoriesForProduct.Add("Most Popular");
            categoriesForProduct.Add("TrendingWeekly");
            if (requestContext.IsFeatureEnabled("Microsoft.VisualStudio.Services.Gallery.DedupeHomepageExtensions"))
            {
              categoriesForProduct.Add("TrendingDaily");
              categoriesForProduct.Add("TrendingMonthly");
            }
          }
          else if (this._isProductVSCode(product))
          {
            categoriesForProduct.Add("TrendingWeekly");
            if (requestContext.IsFeatureEnabled("Microsoft.VisualStudio.Services.Gallery.DedupeHomepageExtensions"))
            {
              categoriesForProduct.Add("TrendingDaily");
              categoriesForProduct.Add("TrendingMonthly");
            }
            categoriesForProduct.Add("Most Popular");
          }
          else if (this._isProductVSForMac(product))
            categoriesForProduct.Add("Most Popular");
          categoriesForProduct.Add("Recently Added");
        }
      }
      if (!this._isProductVS(product))
      {
        string b = "en-us";
        List<string> source = new List<string>();
        IPublishedExtensionService extensionService = this._extensionService;
        IVssRequestContext requestContext1 = requestContext;
        List<string> languages = new List<string>();
        languages.Add(b);
        string product1 = product;
        foreach (ExtensionCategory category in extensionService.QueryAvailableCategories(requestContext1, (IEnumerable<string>) languages, product: product1).Categories)
        {
          foreach (CategoryLanguageTitle languageTitle in category.LanguageTitles)
          {
            if (string.Equals(languageTitle.Lang, b))
              source.Add(languageTitle.Title);
          }
        }
        List<string> list = source.OrderBy<string, string>((Func<string, string>) (q => q)).ToList<string>();
        categoriesForProduct.AddRange((IEnumerable<string>) list);
        if (categoriesForProduct.Contains("Developer samples"))
        {
          categoriesForProduct.Remove("Developer samples");
          categoriesForProduct.Add("Developer samples");
        }
        if (categoriesForProduct.Contains("Other"))
        {
          categoriesForProduct.Remove("Other");
          categoriesForProduct.Add("Other");
        }
      }
      return categoriesForProduct;
    }

    private ExtensionQuery GetBatchQueryForProductAndAllCategories(
      string product,
      List<string> categoryList,
      IVssRequestContext requestContext,
      bool featured = false)
    {
      ExtensionQuery andAllCategories = new ExtensionQuery()
      {
        Filters = new List<QueryFilter>(),
        Flags = this.GetExtensionQueryCommonFlags(),
        AssetTypes = new List<string>()
        {
          "Microsoft.VisualStudio.Services.Icons.Small",
          "Microsoft.VisualStudio.Services.Icons.Default"
        }
      };
      foreach (string category in categoryList)
      {
        List<FilterCriteria> filterCriteria1 = new List<FilterCriteria>();
        FilterCriteria filterCriteria2 = new FilterCriteria();
        filterCriteria2.FilterType = 12;
        PublishedExtensionFlags publishedExtensionFlags = PublishedExtensionFlags.BuiltIn | PublishedExtensionFlags.System | PublishedExtensionFlags.Unpublished;
        if (requestContext.IsFeatureEnabled("Microsoft.VisualStudio.Services.Gallery.EnableQueriesBasedOnHiddenFlags"))
          publishedExtensionFlags = PublishedExtensionFlags.System | PublishedExtensionFlags.Unpublished | PublishedExtensionFlags.Hidden;
        if (category == "Top Free")
          publishedExtensionFlags |= PublishedExtensionFlags.Paid;
        FilterCriteria filterCriteria3 = filterCriteria2;
        int num = (int) publishedExtensionFlags;
        string str = num.ToString();
        filterCriteria3.Value = str;
        filterCriteria1.Add(filterCriteria2);
        if (category == "Top Paid")
        {
          FilterCriteria filterCriteria4 = new FilterCriteria();
          filterCriteria4.FilterType = 13;
          num = 16;
          filterCriteria4.Value = num.ToString();
          FilterCriteria filterCriteria5 = filterCriteria4;
          filterCriteria1.Add(filterCriteria5);
        }
        if (category == "Featured")
          filterCriteria1.AddRange((IEnumerable<FilterCriteria>) this.GetInstallationTargetCriteria(product, requestContext, true));
        else
          filterCriteria1.AddRange((IEnumerable<FilterCriteria>) this.GetInstallationTargetCriteria(product, requestContext));
        this.ApplyCategoryFilterOnExtensionQuery(category, filterCriteria1);
        andAllCategories.Filters.Add(this.GetQueryFiltersOnExtensionQuery(category, filterCriteria1, requestContext));
      }
      return andAllCategories;
    }

    private ExtensionQueryFlags GetExtensionQueryCommonFlags() => ExtensionQueryFlags.IncludeFiles | ExtensionQueryFlags.IncludeCategoryAndTags | ExtensionQueryFlags.ExcludeNonValidated | ExtensionQueryFlags.IncludeInstallationTargets | ExtensionQueryFlags.IncludeStatistics | ExtensionQueryFlags.IncludeLatestVersionOnly;

    internal QueryFilter GetQueryFiltersOnExtensionQuery(
      string category,
      List<FilterCriteria> filterCriteria,
      IVssRequestContext requestContext)
    {
      int num1 = 4;
      int num2 = 0;
      int num3 = 18;
      if (requestContext.IsFeatureEnabled("Microsoft.VisualStudio.Services.Gallery.DedupeHomepageExtensions") && (category == "Most Popular" || category == "TrendingDaily" || category == "TrendingWeekly" || category == "TrendingMonthly"))
        num3 = 24;
      switch (category)
      {
        case "Recently Added":
          num1 = requestContext.ExecutionEnvironment.IsHostedDeployment ? 10 : 5;
          break;
        case "TrendingWeekly":
          num1 = 8;
          break;
        case "TrendingDaily":
          num1 = 7;
          break;
        case "TrendingMonthly":
          num1 = 9;
          break;
        case "Highest Rated":
          num1 = 12;
          break;
      }
      return new QueryFilter()
      {
        Criteria = filterCriteria,
        Direction = PagingDirection.Forward,
        PageNumber = 1,
        SortBy = num1,
        SortOrder = num2,
        PageSize = num3
      };
    }

    private void ApplyCategoryFilterOnExtensionQuery(
      string category,
      List<FilterCriteria> filterCriteria)
    {
      if (ProductExtensionsProvider.SPECIAL_CATEGORIES.Any<string>((Func<string, bool>) (s => category.Contains(s))))
        return;
      filterCriteria.Add(new FilterCriteria()
      {
        FilterType = 5,
        Value = category
      });
    }

    private List<FilterCriteria> GetInstallationTargetCriteria(
      string product,
      IVssRequestContext requestContext,
      bool featured = false)
    {
      List<FilterCriteria> installationTargetCriteria = new List<FilterCriteria>();
      if (this._isProductVSCode(product))
        installationTargetCriteria.AddRange((IEnumerable<FilterCriteria>) ProductExtensionsProvider.GetFilterCriteriaList("Microsoft.VisualStudio.Code", featured));
      else if (this._isProductVS(product))
      {
        if (!requestContext.IsFeatureEnabled("Microsoft.VisualStudio.Services.Gallery.Vs2019Homepage"))
          installationTargetCriteria.AddRange((IEnumerable<FilterCriteria>) ProductExtensionsProvider.GetFilterCriteriaList("Microsoft.VisualStudio.Ide", featured));
        else if (!featured)
        {
          installationTargetCriteria.AddRange((IEnumerable<FilterCriteria>) ProductExtensionsProvider.GetFilterCriteriaList("Microsoft.VisualStudio.VSWinDesktopExpress"));
          installationTargetCriteria.AddRange((IEnumerable<FilterCriteria>) ProductExtensionsProvider.GetFilterCriteriaList("Microsoft.VisualStudio.VSWinExpress"));
          installationTargetCriteria.AddRange((IEnumerable<FilterCriteria>) ProductExtensionsProvider.GetFilterCriteriaList("Microsoft.VisualStudio.VWDExpress"));
          installationTargetCriteria.AddRange((IEnumerable<FilterCriteria>) ProductExtensionsProvider.GetFilterCriteriaList("Microsoft.VisualStudio.Community"));
          installationTargetCriteria.AddRange((IEnumerable<FilterCriteria>) ProductExtensionsProvider.GetFilterCriteriaList("Microsoft.VisualStudio.Pro"));
          installationTargetCriteria.AddRange((IEnumerable<FilterCriteria>) ProductExtensionsProvider.GetFilterCriteriaList("Microsoft.VisualStudio.Enterprise"));
          installationTargetCriteria.AddRange((IEnumerable<FilterCriteria>) ProductExtensionsProvider.GetFilterCriteriaList("Microsoft.VisualStudio.IntegratedShell"));
          installationTargetCriteria.AddRange((IEnumerable<FilterCriteria>) ProductExtensionsProvider.GetFilterCriteriaList("Microsoft.VisualStudio.Isolated"));
          installationTargetCriteria.AddRange((IEnumerable<FilterCriteria>) ProductExtensionsProvider.GetFilterCriteriaList("Microsoft.VisualStudio.Test"));
          installationTargetCriteria.AddRange((IEnumerable<FilterCriteria>) ProductExtensionsProvider.GetFilterCriteriaList("Microsoft.VisualStudio.Ultimate"));
          installationTargetCriteria.AddRange((IEnumerable<FilterCriteria>) ProductExtensionsProvider.GetFilterCriteriaList("Microsoft.VisualStudio.Premium"));
          installationTargetCriteria.AddRange((IEnumerable<FilterCriteria>) ProductExtensionsProvider.GetFilterCriteriaList("Microsoft.VisualStudio.VST_All"));
          installationTargetCriteria.AddRange((IEnumerable<FilterCriteria>) ProductExtensionsProvider.GetFilterCriteriaList("Microsoft.VisualStudio.VSLS"));
          installationTargetCriteria.AddRange((IEnumerable<FilterCriteria>) ProductExtensionsProvider.GetFilterCriteriaList("Microsoft.VisualStudio.VPDExpress"));
          installationTargetCriteria.Add(new FilterCriteria()
          {
            FilterType = 15,
            Value = "16.0"
          });
        }
        else
          installationTargetCriteria.AddRange((IEnumerable<FilterCriteria>) ProductExtensionsProvider.GetFilterCriteriaList("Microsoft.VisualStudio.Ide", featured));
      }
      else if (this._isProductVSTS(product))
      {
        installationTargetCriteria.AddRange((IEnumerable<FilterCriteria>) ProductExtensionsProvider.GetFilterCriteriaList("Microsoft.VisualStudio.Services", featured));
        installationTargetCriteria.AddRange((IEnumerable<FilterCriteria>) ProductExtensionsProvider.GetFilterCriteriaList("Microsoft.VisualStudio.Services.Cloud"));
        installationTargetCriteria.AddRange((IEnumerable<FilterCriteria>) ProductExtensionsProvider.GetFilterCriteriaList("Microsoft.VisualStudio.Services.Integration"));
        installationTargetCriteria.AddRange((IEnumerable<FilterCriteria>) ProductExtensionsProvider.GetFilterCriteriaList("Microsoft.TeamFoundation.Server"));
        installationTargetCriteria.AddRange((IEnumerable<FilterCriteria>) ProductExtensionsProvider.GetFilterCriteriaList("Microsoft.TeamFoundation.Server.Integration"));
        installationTargetCriteria.AddRange((IEnumerable<FilterCriteria>) ProductExtensionsProvider.GetFilterCriteriaList("Microsoft.VisualStudio.Services.Cloud.Integration"));
        installationTargetCriteria.AddRange((IEnumerable<FilterCriteria>) ProductExtensionsProvider.GetFilterCriteriaList("Microsoft.VisualStudio.Services.Resource.Cloud"));
      }
      else
      {
        if (!this._isProductVSForMac(product))
          return (List<FilterCriteria>) null;
        installationTargetCriteria.AddRange((IEnumerable<FilterCriteria>) ProductExtensionsProvider.GetFilterCriteriaList("Microsoft.VisualStudio.Mac", featured));
      }
      return installationTargetCriteria;
    }

    private static List<FilterCriteria> GetFilterCriteriaList(
      string installationTarget,
      bool featured = false)
    {
      List<FilterCriteria> filterCriteriaList = new List<FilterCriteria>();
      filterCriteriaList.Add(new FilterCriteria()
      {
        FilterType = 8,
        Value = installationTarget
      });
      if (featured)
        filterCriteriaList.Add(new FilterCriteria()
        {
          FilterType = 9
        });
      return filterCriteriaList;
    }

    private bool ShouldIncludeExtension(
      IVssRequestContext requestContext,
      PublishedExtension extension)
    {
      return requestContext.IsFeatureEnabled("Microsoft.VisualStudio.Services.Gallery.EnableQueriesBasedOnHiddenFlags") ? (extension.Flags & PublishedExtensionFlags.System) == PublishedExtensionFlags.None && (extension.Flags & PublishedExtensionFlags.Hidden) == PublishedExtensionFlags.None : (extension.Flags & PublishedExtensionFlags.System) == PublishedExtensionFlags.None && (extension.Flags & PublishedExtensionFlags.BuiltIn) == PublishedExtensionFlags.None;
    }

    internal string[] OrderCategoryNames(List<string> categoryNames)
    {
      categoryNames.Sort();
      if (categoryNames.Contains("Other"))
      {
        categoryNames.Remove("Other");
        categoryNames.Add("Other");
      }
      return categoryNames.ToArray();
    }

    internal double GetTrendingScoreForExtension(
      PublishedExtension extension,
      string trendingStatisticType)
    {
      double scoreForExtension = 0.0;
      if (extension.Statistics != null && extension.Statistics.Count > 0)
      {
        foreach (ExtensionStatistic statistic in extension.Statistics)
        {
          if (statistic.StatisticName.Equals(trendingStatisticType, StringComparison.OrdinalIgnoreCase))
            scoreForExtension = statistic.Value;
        }
      }
      return scoreForExtension;
    }

    private Dictionary<string, List<VSSOfferViewDataKO>> PopulateOffers(
      IVssRequestContext requestContext)
    {
      Dictionary<string, List<VSSOfferViewDataKO>> dictionary = new Dictionary<string, List<VSSOfferViewDataKO>>();
      PublishedExtension publishedExtension = this.GetPublishedExtension(requestContext, "ms.xamarin-university");
      List<VSSOfferViewDataKO> vssOfferViewDataKoList1 = new List<VSSOfferViewDataKO>();
      int index1 = 0;
      string str1 = "";
      SubscriptionButton[] subscriptionButtonArray1;
      if (!requestContext.IsFeatureEnabled("Microsoft.VisualStudio.Services.Gallery.VsAnnualDisable"))
      {
        subscriptionButtonArray1 = new SubscriptionButton[2];
        subscriptionButtonArray1[index1++] = new SubscriptionButton()
        {
          itemName = "vs-professional-annual",
          buttonLink = VSTSExtensionItem.GetItemDetailsURL("ms.vs-professional-annual"),
          buttonCost = GalleryResources.VSProfessionalAnnualCost,
          buttonQuantity = "",
          buttonLabel = GalleryResources.SubscriptionBuyProfessionalAnnual
        };
        str1 = string.Format((IFormatProvider) CultureInfo.CurrentCulture, GalleryResources.VSProfessionalNote, (object) "ux-item-offers-cost-link");
      }
      else
        subscriptionButtonArray1 = new SubscriptionButton[1];
      subscriptionButtonArray1[index1] = new SubscriptionButton()
      {
        itemName = "vs-professional-monthly",
        buttonLink = VSTSExtensionItem.GetItemDetailsURL("ms.vs-professional-monthly"),
        buttonCost = GalleryResources.VSProfessionalMonthlyCost,
        buttonQuantity = "",
        buttonLabel = GalleryResources.SubscriptionBuyProfessionalMonthly
      };
      vssOfferViewDataKoList1.Add(new VSSOfferViewDataKO()
      {
        SubscriptionTypeCss = "vs-professional",
        SubscriptionName = GalleryResources.VSProfessionalSubscriptionName,
        SubscriptionDescription = GalleryResources.VSProfessionalSubscriptionDesc,
        Note = str1,
        Buttons = subscriptionButtonArray1
      });
      int index2 = 0;
      SubscriptionButton[] subscriptionButtonArray2;
      string str2;
      if (!requestContext.IsFeatureEnabled("Microsoft.VisualStudio.Services.Gallery.VsAnnualDisable"))
      {
        subscriptionButtonArray2 = new SubscriptionButton[2];
        subscriptionButtonArray2[index2++] = new SubscriptionButton()
        {
          itemName = "vs-enterprise-annual",
          buttonLink = VSTSExtensionItem.GetItemDetailsURL("ms.vs-enterprise-annual"),
          buttonCost = GalleryResources.VSEnterpriseAnnualCost,
          buttonQuantity = "",
          buttonLabel = GalleryResources.SubscriptionBuyEnterpriseAnnual
        };
        str2 = string.Format((IFormatProvider) CultureInfo.CurrentCulture, GalleryResources.VSEnterpriseNote, (object) "ux-item-offers-cost-link");
      }
      else
      {
        subscriptionButtonArray2 = new SubscriptionButton[1];
        str2 = "";
      }
      subscriptionButtonArray2[index2] = new SubscriptionButton()
      {
        itemName = "vs-enterprise-monthly",
        buttonLink = VSTSExtensionItem.GetItemDetailsURL("ms.vs-enterprise-monthly"),
        buttonCost = GalleryResources.VSEnterpriseMonthlyCost,
        buttonQuantity = "",
        buttonLabel = GalleryResources.SubscriptionBuyEnterpriseMonthly
      };
      vssOfferViewDataKoList1.Add(new VSSOfferViewDataKO()
      {
        SubscriptionTypeCss = "vs-enterprise",
        SubscriptionName = GalleryResources.VSEnterpriseSubscriptionName,
        SubscriptionDescription = GalleryResources.VSEnterpriseSubscriptionDesc,
        Note = str2,
        Buttons = subscriptionButtonArray2
      });
      dictionary.Add("vsoffers", vssOfferViewDataKoList1);
      if (publishedExtension != null && HttpContext.Current?.Request.Cookies["XamarinUniversityDisable"]?.Value == null && !requestContext.IsFeatureEnabled("Microsoft.VisualStudio.Services.Gallery.XamarinUniversityDisable"))
      {
        List<VSSOfferViewDataKO> vssOfferViewDataKoList2 = new List<VSSOfferViewDataKO>();
        SubscriptionButton[] subscriptionButtonArray3 = new SubscriptionButton[1]
        {
          new SubscriptionButton()
          {
            itemName = "xamarin-university",
            buttonLink = VSTSExtensionItem.GetItemDetailsURL("ms.xamarin-university"),
            buttonCost = GalleryResources.XamarinUniversityTileCostDuration,
            buttonQuantity = GalleryResources.XamarinUniversityTileCostText,
            buttonLabel = GalleryResources.SubscriptionBuyXamarinUniversity
          }
        };
        vssOfferViewDataKoList2.Add(new VSSOfferViewDataKO()
        {
          SubscriptionTypeCss = "xamarin-university",
          SubscriptionName = GalleryResources.XamarinUniversitySubscriptionName,
          SubscriptionDescription = GalleryResources.XamarinUniversityTileDescription,
          Note = "",
          TileIconSource = new PageContextProvider().GetResourcesPath(requestContext) + "XamarinUniv.svg",
          Buttons = subscriptionButtonArray3
        });
        dictionary.Add("xuoffers", vssOfferViewDataKoList2);
      }
      return dictionary;
    }

    private PublishedExtension GetPublishedExtension(
      IVssRequestContext requestContext,
      string extensionName)
    {
      PublishedExtension publishedExtension = (PublishedExtension) null;
      if (!string.IsNullOrEmpty(extensionName))
      {
        string[] strArray = extensionName.Split(new char[1]
        {
          '.'
        }, 2);
        if (strArray.Length == 2)
        {
          requestContext.GetUserIdentity();
          IPublishedExtensionService service = requestContext.GetService<IPublishedExtensionService>();
          ExtensionQueryFlags flags = this.GetExtensionQueryCommonFlags() | ExtensionQueryFlags.IncludeInstallationTargets;
          bool useCache = requestContext.IsFeatureEnabled("Microsoft.VisualStudio.Services.Gallery.QueryExtensionCacheExtended");
          try
          {
            publishedExtension = service.QueryExtension(requestContext, strArray[0], strArray[1], (string) null, flags, (string) null, useCache);
          }
          catch (ExtensionDoesNotExistException ex)
          {
          }
        }
      }
      return publishedExtension;
    }
  }
}
