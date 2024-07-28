// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.SoapService.Galleries.SearchApiQueryBuilder
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Gallery.Server.SoapService.Models;
using Microsoft.VisualStudio.Services.Gallery.WebApi;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

namespace Microsoft.VisualStudio.Services.Gallery.Server.SoapService.Galleries
{
  internal class SearchApiQueryBuilder
  {
    private readonly IFilterTranslator filterTranslator;
    [StaticSafe("Grandfathered")]
    private static Regex percentSemiReplace = new Regex(";%$", RegexOptions.Compiled);
    private const string DefaultLcidFilterValue = "1033";

    public SearchApiQueryBuilder(IFilterTranslator filterTranslator)
    {
      this.filterTranslator = filterTranslator;
      this.filterTranslator.AddFilterExpressionTranslation(new Regex("Project\\.Metadata\\[\\'([a-zA-Z0-9_-]+)\\'\\]"), (Func<FilterExpression, string[], string>) ((expression, captures) => string.Format("PMD::{0}::{1}::{2}|", (object) captures[1], (object) expression.Comparison, (object) expression.Value)));
      this.filterTranslator.AddFilterExpressionTranslation("Category.Id", (Func<FilterExpression, string>) (expression =>
      {
        return !(expression.Comparison != "=") ? string.Format("CAT::{0}|", (object) expression.Value) : throw new InvalidOperationException("Category.ID may only be used with the = operator.");
      }));
    }

    public virtual VsIdeExtensionQuery BuildQueryFromExtensionQuery(ExtensionQuery extensionQuery)
    {
      VsIdeExtensionQuery ideExtensionQuery = new VsIdeExtensionQuery(extensionQuery);
      QueryFilter filter = extensionQuery.Filters[0];
      bool flag = false;
      FilterCriteria filterCriteria = (FilterCriteria) null;
      List<string> stringList = new List<string>();
      foreach (FilterCriteria criterion in filter.Criteria)
      {
        if (criterion.FilterType == 14)
        {
          if (criterion.Value.Equals("1033", StringComparison.OrdinalIgnoreCase))
            flag = true;
        }
        else if (criterion.FilterType == 10)
          filterCriteria = criterion;
        else if (criterion.FilterType == 8)
          stringList.Add(criterion.Value);
      }
      if (!flag)
        filter.Criteria.Add(new FilterCriteria()
        {
          FilterType = 14,
          Value = "1033"
        });
      string str = filterCriteria != null ? filterCriteria.Value : "";
      foreach (string sku in stringList)
        str = str + " " + this.ConvertSkuToInstallationTargetEql(sku, false);
      if (filterCriteria != null)
        filterCriteria.Value = str;
      else
        filter.Criteria.Add(new FilterCriteria()
        {
          FilterType = 10,
          Value = str
        });
      filter.Criteria.RemoveAll((Predicate<FilterCriteria>) (x => x.FilterType == 8));
      extensionQuery.Flags = ExtensionQueryFlags.IncludeFiles | ExtensionQueryFlags.ExcludeNonValidated | ExtensionQueryFlags.IncludeInstallationTargets | ExtensionQueryFlags.IncludeAssetUri | ExtensionQueryFlags.IncludeStatistics | ExtensionQueryFlags.IncludeLatestVersionOnly | ExtensionQueryFlags.IncludeMetadata;
      extensionQuery.MetadataFlags = new ExtensionQueryResultMetadataFlags?(ExtensionQueryResultMetadataFlags.IncludeResultCount);
      if (filter.PageSize <= 0)
        filter.PageSize = 25;
      if (filter.PageNumber <= 0)
        filter.PageNumber = 1;
      if (filter.SortBy == 11)
        filter.SortBy = 3;
      return ideExtensionQuery;
    }

    public virtual VsIdeExtensionQuery BuildQuery(
      string searchText,
      string whereClause,
      string orderByClause,
      IDictionary<string, string> context,
      int? skip,
      int? take)
    {
      VsIdeExtensionQuery query = new VsIdeExtensionQuery();
      ExtensionQuery extensionQuery = query.ExtensionQuery;
      if (searchText.IsNullOrEmpty<char>())
        searchText = "";
      extensionQuery.Flags = ExtensionQueryFlags.IncludeFiles | ExtensionQueryFlags.ExcludeNonValidated | ExtensionQueryFlags.IncludeInstallationTargets | ExtensionQueryFlags.IncludeAssetUri | ExtensionQueryFlags.IncludeStatistics | ExtensionQueryFlags.IncludeLatestVersionOnly | ExtensionQueryFlags.IncludeMetadata;
      extensionQuery.MetadataFlags = new ExtensionQueryResultMetadataFlags?(ExtensionQueryResultMetadataFlags.IncludeResultCount);
      extensionQuery.Filters = new List<QueryFilter>();
      int num1 = take ?? 25;
      int num2 = skip.HasValue ? (skip.Value + num1) / num1 : 1;
      extensionQuery.Filters.Add(new QueryFilter()
      {
        Criteria = new List<FilterCriteria>()
        {
          new FilterCriteria() { FilterType = 14, Value = "1033" }
        },
        PageSize = num1,
        PageNumber = num2
      });
      if (context["LCID"] != "1033")
        extensionQuery.Filters[0].Criteria.Add(new FilterCriteria()
        {
          FilterType = 14,
          Value = int.Parse(context["LCID"], (IFormatProvider) CultureInfo.InvariantCulture).ToString((IFormatProvider) CultureInfo.InvariantCulture)
        });
      this.GetSortFieldAndOrder(extensionQuery.Filters[0], orderByClause);
      this.GetFilterConstraint(query, whereClause, searchText);
      return query;
    }

    protected VsIdeExtensionQuery BuildQueryForCategories(
      string vsVersion,
      string[] skus,
      string[] subSkus,
      int? parentCategoryId)
    {
      VsIdeExtensionQuery ideExtensionQuery = new VsIdeExtensionQuery();
      ExtensionQuery extensionQuery = ideExtensionQuery.ExtensionQuery;
      extensionQuery.Filters = new List<QueryFilter>()
      {
        new QueryFilter()
        {
          ForcePageSize = true,
          PageSize = 0,
          PageNumber = 1,
          Criteria = new List<FilterCriteria>()
          {
            new FilterCriteria() { FilterType = 14, Value = "1033" }
          }
        }
      };
      ideExtensionQuery.ExtensionQuery.Flags = ExtensionQueryFlags.ExcludeNonValidated;
      ideExtensionQuery.ExtensionQuery.MetadataFlags = new ExtensionQueryResultMetadataFlags?(ExtensionQueryResultMetadataFlags.IncludeResultSetCategories);
      List<FilterCriteria> criteria = ideExtensionQuery.ExtensionQuery.Filters[0].Criteria;
      this.AddInstallationTargetFilters(extensionQuery.Filters[0].Criteria, vsVersion, skus, subSkus, parentCategoryId);
      if (!parentCategoryId.HasValue)
        return ideExtensionQuery;
      criteria.Add(new FilterCriteria()
      {
        FilterType = 5,
        Value = parentCategoryId.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture)
      });
      return ideExtensionQuery;
    }

    private void GetFilterConstraint(
      VsIdeExtensionQuery query,
      string whereClause,
      string searchText)
    {
      QueryFilter filter = query.ExtensionQuery.Filters[0];
      if (string.IsNullOrEmpty(whereClause))
        return;
      string str1 = this.filterTranslator.Translate(whereClause);
      if (string.IsNullOrEmpty(str1))
        return;
      string[] strArray1 = str1.Replace("(", "").Replace(")", "").Replace("AND", "").Replace("OR", "").Split('|');
      List<string> enumerable1 = new List<string>();
      string enumerable2 = (string) null;
      foreach (string str2 in strArray1)
      {
        string[] separator = new string[1]{ "::" };
        string[] strArray2 = str2.Split(separator, StringSplitOptions.None);
        strArray2[0] = strArray2[0].Trim();
        if (strArray2[0] == "PMD")
        {
          string str3 = SearchApiQueryBuilder.percentSemiReplace.Replace(strArray2[3].Trim(), "%");
          if (string.Equals(strArray2[1], "SupportedVSEditions", StringComparison.OrdinalIgnoreCase) && strArray2[2] == "LIKE")
          {
            string[] strArray3 = str3.Replace("%", "").Trim().Split(new string[1]
            {
              ","
            }, StringSplitOptions.RemoveEmptyEntries);
            if (strArray3.Length == 2)
            {
              enumerable1.Add(strArray3[1]);
              if (enumerable2.IsNullOrEmpty<char>())
                enumerable2 = strArray3[0];
            }
          }
          else
          {
            MetadataFilterItem metadataFilterItem = new MetadataFilterItem(strArray2[1], str3, MetadataFilterItem.ComparisonOperator.Equal);
            if (strArray2[2] == "!=")
              metadataFilterItem.Operator = MetadataFilterItem.ComparisonOperator.NotEqual;
            query.MetadataFilters.Add(metadataFilterItem);
          }
        }
        else if (strArray2[0] == "CAT")
          filter.Criteria.Add(new FilterCriteria()
          {
            FilterType = 5,
            Value = strArray2[1]
          });
      }
      string str4 = searchText;
      if (enumerable1.IsNullOrEmpty<string>())
      {
        str4 = str4 + " " + this.ConvertSkuToInstallationTargetEql("Microsoft.VisualStudio.Ide", false);
      }
      else
      {
        foreach (string sku in enumerable1)
          str4 = str4 + " " + this.ConvertSkuToInstallationTargetEql(sku);
        filter.Criteria.Add(new FilterCriteria()
        {
          FilterType = 15,
          Value = enumerable2
        });
      }
      filter.Criteria.Add(new FilterCriteria()
      {
        FilterType = 10,
        Value = str4.Trim()
      });
    }

    private void GetSortFieldAndOrder(QueryFilter queryFilter, string orderByClause)
    {
      if (string.IsNullOrEmpty(orderByClause))
        return;
      string[] strArray = orderByClause.Split(' ');
      if (strArray.Length != 2)
        return;
      queryFilter.SortBy = (int) SearchApiQueryBuilder.GetSortField(strArray[0]);
      queryFilter.SortOrder = (int) SearchApiQueryBuilder.IsAscendingSort(strArray[1]);
    }

    private void AddInstallationTargetFilters(
      List<FilterCriteria> filterCriteria,
      string vsVersion,
      string[] skus,
      string[] subSkus,
      int? parentCategoryId)
    {
      string str = "";
      bool flag = false;
      string enumerable;
      if ((skus == null || skus.Length == 0) && (subSkus == null || subSkus.Length == 0))
      {
        enumerable = str + this.ConvertSkuToInstallationTargetEql("Microsoft.VisualStudio.Ide", false);
        flag = true;
      }
      else
        enumerable = str + this.GetTargetStringFromSkuAndSubSku(skus, subSkus);
      if (parentCategoryId.HasValue && !flag && (skus == null || !((IEnumerable<string>) skus).Any<string>((Func<string, bool>) (x => x != null && x.ToUpperInvariant() == "VSLS"))) && (subSkus == null || !((IEnumerable<string>) subSkus).Any<string>((Func<string, bool>) (x => x != null && x.ToUpperInvariant() == "VSLS"))))
        enumerable = enumerable + " " + this.ConvertSkuToInstallationTargetEql("VSLS");
      if (!enumerable.IsNullOrEmpty<char>())
        filterCriteria.Add(new FilterCriteria()
        {
          FilterType = 10,
          Value = enumerable.Trim()
        });
      if (flag || !parentCategoryId.HasValue || vsVersion.IsNullOrEmpty<char>())
        return;
      filterCriteria.Add(new FilterCriteria()
      {
        FilterType = 15,
        Value = vsVersion
      });
    }

    private string GetTargetStringFromSkuAndSubSku(string[] skus, string[] subSkus)
    {
      string fromSkuAndSubSku = "";
      if (skus != null && skus.Length != 0)
      {
        foreach (string sku in skus)
          fromSkuAndSubSku = fromSkuAndSubSku + " " + this.ConvertSkuToInstallationTargetEql(sku);
      }
      if (subSkus != null && subSkus.Length != 0)
      {
        foreach (string subSku in subSkus)
          fromSkuAndSubSku = fromSkuAndSubSku + " " + this.ConvertSkuToInstallationTargetEql(subSku);
      }
      return fromSkuAndSubSku;
    }

    private string ConvertSkuToInstallationTargetEql(string sku, bool usePrefix = true)
    {
      if (sku.IsNullOrEmpty<char>())
        return (string) null;
      return usePrefix ? "target:\"Microsoft.VisualStudio." + sku + "\"" : "target:\"" + sku + "\"";
    }

    private static SortOrderType IsAscendingSort(string order) => !order.Equals("asc", StringComparison.OrdinalIgnoreCase) ? SortOrderType.Descending : SortOrderType.Ascending;

    private static SortByType GetSortField(string sortString)
    {
      SortByType sortField = SortByType.Title;
      string lowerInvariant = sortString.ToLowerInvariant();
      if (lowerInvariant != null)
      {
        switch (lowerInvariant.Length)
        {
          case 9:
            if (lowerInvariant == "relevance")
              goto label_19;
            else
              goto label_20;
          case 13:
            if (lowerInvariant == "project.title")
            {
              sortField = SortByType.Title;
              goto label_20;
            }
            else
              goto label_20;
          case 14:
            if (lowerInvariant == "release.rating")
            {
              sortField = SortByType.WeightedRating;
              goto label_20;
            }
            else
              goto label_20;
          case 15:
            if (lowerInvariant == "release.ranking")
            {
              sortField = SortByType.WeightedRating;
              goto label_20;
            }
            else
              goto label_20;
          case 20:
            if (lowerInvariant == "project.modifieddate")
              break;
            goto label_20;
          case 25:
            if (lowerInvariant == "releasefile.downloadcount")
            {
              sortField = SortByType.InstallCount;
              goto label_20;
            }
            else
              goto label_20;
          case 26:
            if (lowerInvariant == "project.metadata['author']")
            {
              sortField = SortByType.Publisher;
              goto label_20;
            }
            else
              goto label_20;
          case 27:
            if (lowerInvariant == "project.metadata['ranking']")
            {
              sortField = SortByType.WeightedRating;
              goto label_20;
            }
            else
              goto label_20;
          case 29:
            if (lowerInvariant == "project.metadata['relevance']")
              goto label_19;
            else
              goto label_20;
          case 32:
            if (lowerInvariant == "project.metadata['lastmodified']")
              break;
            goto label_20;
          default:
            goto label_20;
        }
        sortField = SortByType.LastUpdatedDate;
        goto label_20;
label_19:
        sortField = SortByType.Relevance;
      }
label_20:
      return sortField;
    }
  }
}
