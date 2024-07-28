// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.SoapService.Galleries.ExtensionsSearchApiQueryBuilder
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Gallery.Server.SoapService.Models;
using Microsoft.VisualStudio.Services.Gallery.WebApi;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Microsoft.VisualStudio.Services.Gallery.Server.SoapService.Galleries
{
  internal class ExtensionsSearchApiQueryBuilder : 
    SearchApiQueryBuilder,
    IExtensionSearchApiQueryBuilder
  {
    public ExtensionsSearchApiQueryBuilder(IFilterTranslator filterTranslator)
      : base(filterTranslator)
    {
    }

    public override VsIdeExtensionQuery BuildQueryFromExtensionQuery(ExtensionQuery extensionQuery)
    {
      int[] categoryIds = this.ExtractCategoryIds(extensionQuery);
      VsIdeExtensionQuery query = base.BuildQueryFromExtensionQuery(extensionQuery);
      if (categoryIds.Length > 1 && categoryIds[1] != 0)
      {
        int programmingLanguageId = categoryIds[1];
        this.AddProgramingLanguageFilter(query, programmingLanguageId);
      }
      return query;
    }

    public override VsIdeExtensionQuery BuildQuery(
      string searchText,
      string whereClause,
      string orderByClause,
      IDictionary<string, string> context,
      int? skip,
      int? take)
    {
      int[] categoryIds = this.ExtractCategoryIds(ref whereClause);
      VsIdeExtensionQuery query = base.BuildQuery(searchText, whereClause, orderByClause, context, skip, take);
      if (categoryIds.Length > 1 && categoryIds[1] != 0)
      {
        int programmingLanguageId = categoryIds[1];
        this.AddProgramingLanguageFilter(query, programmingLanguageId);
      }
      return query;
    }

    public VsIdeExtensionQuery BuildQueryForCategories(
      string vsVersion,
      string serviceSource,
      string templateType,
      string[] skus,
      string[] subSkus,
      int? parentCategoryId,
      int? programmingLanguageId,
      CultureInfo cultureInfo,
      string productArchitecture = null)
    {
      VsIdeExtensionQuery query = this.BuildQueryForCategories(vsVersion, skus, subSkus, parentCategoryId);
      List<FilterCriteria> criteria = query.ExtensionQuery.Filters[0].Criteria;
      if (!serviceSource.IsNullOrEmpty<char>())
        this.BuildMetadataFiltersBasedOnServiceSource(query, serviceSource);
      else if (!templateType.IsNullOrEmpty<char>())
        query.MetadataFilters.Add(new MetadataFilterItem("Type", templateType, MetadataFilterItem.ComparisonOperator.Equal));
      if (cultureInfo != null && cultureInfo.LCID != 1033)
        criteria.Add(new FilterCriteria()
        {
          FilterType = 14,
          Value = cultureInfo.LCID.ToString((IFormatProvider) CultureInfo.InvariantCulture)
        });
      if (programmingLanguageId.HasValue)
      {
        int? nullable = programmingLanguageId;
        int num = 0;
        if (nullable.GetValueOrDefault() > num & nullable.HasValue)
          this.AddProgramingLanguageFilter(query, programmingLanguageId.Value);
      }
      if (productArchitecture != null)
        criteria.Add(new FilterCriteria()
        {
          FilterType = 22,
          Value = productArchitecture
        });
      return query;
    }

    private void AddProgramingLanguageFilter(VsIdeExtensionQuery query, int programmingLanguageId)
    {
      string str;
      if (!ProgrammingLanguage.ProgrammingLanguageIdToVsixTemplateLanguageMap.TryGetValue(programmingLanguageId, out str))
        throw new ArgumentException("Invalid programming language Id provided", nameof (programmingLanguageId)).Expected("Gallery");
      if (str.Equals("Other"))
      {
        foreach (string programmingLanguageName in ProgrammingLanguage.DefinedProgrammingLanguageNames)
          query.MetadataFilters.Add(new MetadataFilterItem("ProjectType", programmingLanguageName, MetadataFilterItem.ComparisonOperator.NotEqual));
      }
      else
        query.MetadataFilters.Add(new MetadataFilterItem("ProjectType", str, MetadataFilterItem.ComparisonOperator.Equal));
    }

    private int[] ExtractCategoryIds(ref string whereClause)
    {
      int[] categoryIds = Array.Empty<int>();
      string pattern = "Category.Id = (\\{){0,1}[0-9a-fA-F]{8}\\-[0-9a-fA-F]{4}\\-[0-9a-fA-F]{4}\\-[0-9a-fA-F]{4}\\-[0-9a-fA-F]{12}(\\}){0,1}";
      Match match = Regex.Match(whereClause, pattern);
      if (match.Success)
      {
        categoryIds = VsIdeCategoryIdParser.ParseIds(Guid.Parse(match.ToString().Replace("Category.Id = ", "")));
        whereClause = whereClause.Replace(match.ToString(), "Category.Id = " + (categoryIds[2] != 0 ? categoryIds[2] : categoryIds[0]).ToString());
      }
      return categoryIds;
    }

    private int[] ExtractCategoryIds(ExtensionQuery extensionQuery)
    {
      int[] categoryIds = Array.Empty<int>();
      QueryFilter filter = extensionQuery.Filters[0];
      if (filter != null && !filter.Criteria.IsNullOrEmpty<FilterCriteria>())
      {
        foreach (FilterCriteria criterion in filter.Criteria)
        {
          Guid result;
          if (criterion.FilterType == 5 && Guid.TryParse(criterion.Value, out result))
          {
            categoryIds = VsIdeCategoryIdParser.ParseIds(result);
            criterion.Value = (categoryIds[2] != 0 ? categoryIds[2] : categoryIds[0]).ToString((IFormatProvider) CultureInfo.InvariantCulture);
          }
        }
      }
      return categoryIds;
    }

    private static bool IsWin8OrGreater(string versionString)
    {
      if (string.IsNullOrEmpty(versionString))
        return false;
      Version version = new Version(versionString);
      if (version.Major > 6)
        return true;
      return version.Major == 6 && version.Minor >= 2;
    }

    private void BuildMetadataFiltersBasedOnServiceSource(
      VsIdeExtensionQuery query,
      string serviceSource)
    {
      if (serviceSource.Equals("NewProjectDialog", StringComparison.OrdinalIgnoreCase))
      {
        query.MetadataFilters.Add(new MetadataFilterItem("Type", "ProjectTemplate", MetadataFilterItem.ComparisonOperator.Equal));
        query.MetadataFilters.Add(new MetadataFilterItem("ProjectType", "Web", MetadataFilterItem.ComparisonOperator.NotEqual));
      }
      else if (serviceSource.Equals("NewWebSiteDialog", StringComparison.OrdinalIgnoreCase))
      {
        query.MetadataFilters.Add(new MetadataFilterItem("Type", "ProjectTemplate", MetadataFilterItem.ComparisonOperator.Equal));
        query.MetadataFilters.Add(new MetadataFilterItem("ProjectType", "Web", MetadataFilterItem.ComparisonOperator.Equal));
      }
      else if (serviceSource.Equals("NewItemDialog", StringComparison.OrdinalIgnoreCase))
      {
        query.MetadataFilters.Add(new MetadataFilterItem("Type", "ItemTemplate", MetadataFilterItem.ComparisonOperator.Equal));
      }
      else
      {
        if (!serviceSource.Equals("NewWebItemDialog", StringComparison.OrdinalIgnoreCase))
          return;
        query.MetadataFilters.Add(new MetadataFilterItem("Type", "ItemTemplate", MetadataFilterItem.ComparisonOperator.Equal));
        query.MetadataFilters.Add(new MetadataFilterItem("ProjectType", "Web", MetadataFilterItem.ComparisonOperator.Equal));
      }
    }
  }
}
