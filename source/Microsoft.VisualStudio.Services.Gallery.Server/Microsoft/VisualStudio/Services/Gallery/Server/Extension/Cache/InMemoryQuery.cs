// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.Extension.Cache.InMemoryQuery
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Gallery.Server.Components;
using Microsoft.VisualStudio.Services.Gallery.WebApi;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Microsoft.VisualStudio.Services.Gallery.Server.Extension.Cache
{
  internal class InMemoryQuery
  {
    internal const int DbCallPageSize = 2000;
    private readonly ComparerFactory comparerFactory = new ComparerFactory();
    private readonly List<int> allowedFilterTypesForInMemoryQuery = new List<int>()
    {
      8,
      7,
      4,
      12
    };
    private readonly List<int> extendedAllowedFilterTypesForInMemoryQuery = new List<int>()
    {
      1,
      5,
      8,
      7,
      4,
      12,
      23
    };
    private readonly List<int> restrictingFilters = new List<int>()
    {
      7,
      4
    };

    public virtual ExtensionQueryResult QueryExtensions(
      IVssRequestContext requestContext,
      CachedExtensionData inMemoryData,
      List<QueryFilter> filters,
      List<QueryFilterValue> filterValues,
      ExtensionQueryFlags flags)
    {
      IDictionary<string, PublishedExtension> dictionary = (IDictionary<string, PublishedExtension>) new Dictionary<string, PublishedExtension>();
      bool isAllQuery = false;
      bool flag1 = false;
      bool excludeUsingFlags = false;
      int excludedFlags = 0;
      QueryFilter filter = filters[0];
      foreach (QueryFilterValue filterValue in filterValues)
      {
        if (filterValue.QueryIndex == 0 && this.restrictingFilters.Contains(filterValue.FilterValueType))
          flag1 = true;
      }
      if (!flag1)
        isAllQuery = true;
      foreach (QueryFilterValue filterValue in filterValues)
      {
        if (filterValue.QueryIndex == 0)
        {
          switch (filterValue.FilterValueType)
          {
            case 4:
            case 7:
              PublishedExtension extension = inMemoryData.GetExtension(filterValue.FilterValue);
              if (extension != null)
              {
                dictionary.TryAdd<string, PublishedExtension>(extension.ExtensionId.ToString(), extension);
                continue;
              }
              continue;
            case 12:
              excludeUsingFlags = true;
              int.TryParse(filterValue.FilterValue, out excludedFlags);
              continue;
            default:
              continue;
          }
        }
      }
      bool flag2 = false;
      List<PublishedExtension> matchingExtensions = new List<PublishedExtension>();
      if (dictionary.IsNullOrEmpty<KeyValuePair<string, PublishedExtension>>())
      {
        if (isAllQuery)
        {
          matchingExtensions.AddRange((IEnumerable<PublishedExtension>) inMemoryData.GetAllExtensions());
          if (filter.SortBy == 4 && (filter.SortOrder == 0 || filter.SortOrder == 2))
            flag2 = true;
        }
      }
      else
        matchingExtensions.AddRange((IEnumerable<PublishedExtension>) dictionary.Values);
      if (flags.HasFlag((Enum) ExtensionQueryFlags.ExcludeNonValidated) | excludeUsingFlags)
        matchingExtensions.RemoveAll((Predicate<PublishedExtension>) (ext =>
        {
          if (flags.HasFlag((Enum) ExtensionQueryFlags.ExcludeNonValidated) && !ext.Flags.HasFlag((Enum) PublishedExtensionFlags.Validated))
            return true;
          return excludeUsingFlags && (ext.Flags & (PublishedExtensionFlags) excludedFlags) != 0;
        }));
      if (!flag2)
        this.SortMatchingExtensions(matchingExtensions, filter, inMemoryData);
      ExtensionQueryResult andPrepareResult = this.ExtractPageAndPrepareResult(matchingExtensions, filter, flags);
      this.PublishCustomerIntelligenceEventForInMemoryQuery(requestContext, isAllQuery, matchingExtensions.Count, filter.PageNumber, filter.PageSize, andPrepareResult.Results[0].Extensions.Count);
      return andPrepareResult;
    }

    public virtual ExtensionQueryResult QueryExtensionsNew(
      IVssRequestContext requestContext,
      CachedExtensionData inMemoryData,
      List<QueryFilter> filters,
      List<QueryFilterValue> filterValues,
      ExtensionQueryFlags flags,
      string accountToken)
    {
      Stopwatch stopwatch1 = Stopwatch.StartNew();
      IDictionary<string, PublishedExtension> dictionary = (IDictionary<string, PublishedExtension>) new Dictionary<string, PublishedExtension>();
      HashSet<InstallationTarget> installationTargetFilterValues = new HashSet<InstallationTarget>((IEqualityComparer<InstallationTarget>) new GalleryInstallationTargets.InstallationTargetComparer());
      bool flag1 = false;
      bool excludeUsingFlags = false;
      bool flag2 = false;
      int excludedFlags = 0;
      QueryFilter filter = filters[0];
      List<PublishedExtension> publishedExtensionList = new List<PublishedExtension>();
      foreach (QueryFilterValue filterValue in filterValues)
      {
        if (filterValue.QueryIndex == 0)
        {
          switch (filterValue.FilterValueType)
          {
            case 1:
              flag1 = true;
              publishedExtensionList = inMemoryData.GetExtensionsWithTag(filterValue.FilterValue);
              continue;
            case 4:
            case 7:
              flag1 = true;
              flag2 = true;
              PublishedExtension extension = inMemoryData.GetExtension(filterValue.FilterValue);
              if (extension != null && dictionary.TryAdd<string, PublishedExtension>(extension.ExtensionId.ToString(), extension))
              {
                publishedExtensionList.Add(extension);
                continue;
              }
              continue;
            case 5:
              flag1 = true;
              publishedExtensionList = inMemoryData.GetExtensionsInCategory(filterValue.FilterValue);
              continue;
            case 8:
              installationTargetFilterValues.Add(new InstallationTarget()
              {
                Target = filterValue.FilterValue
              });
              continue;
            case 12:
              excludeUsingFlags = true;
              int.TryParse(filterValue.FilterValue, out excludedFlags);
              continue;
            case 23:
              flag1 = true;
              publishedExtensionList = inMemoryData.GetExtensionsInTargetPlatform(filterValue.FilterValue);
              int count = publishedExtensionList.IsNullOrEmpty<PublishedExtension>() ? 0 : publishedExtensionList.Count;
              requestContext.Trace(12062086, TraceLevel.Info, "gallery", nameof (InMemoryQuery), string.Format("FilterVsCodeExtensionsByTargetPlatform | TargetPlatform:{0} | ExtensionsCount={1} | AccountToken={2}", (object) filterValue.FilterValue, (object) count, (object) accountToken));
              continue;
            default:
              continue;
          }
        }
      }
      bool flag3 = false;
      if (publishedExtensionList.IsNullOrEmpty<PublishedExtension>())
      {
        if (publishedExtensionList == null)
          publishedExtensionList = new List<PublishedExtension>();
        if (installationTargetFilterValues.Count > 0 && !flag1)
        {
          publishedExtensionList.AddRange((IEnumerable<PublishedExtension>) inMemoryData.GetAllExtensions());
          if (installationTargetFilterValues.Count <= 1)
          {
            if (installationTargetFilterValues.Contains(new InstallationTarget()
            {
              Target = "Microsoft.VisualStudio.Code"
            }))
              goto label_20;
          }
          publishedExtensionList.RemoveAll((Predicate<PublishedExtension>) (ext => !ext.Flags.HasFlag((Enum) PublishedExtensionFlags.Public)));
label_20:
          if (filter.SortBy == 4 && filter.SortOrder != 1)
            flag3 = true;
        }
      }
      if (flag2)
        this.FilterPrivateExtensionsPostCacheRetrieval(requestContext, publishedExtensionList, accountToken);
      stopwatch1.Stop();
      Stopwatch stopwatch2 = Stopwatch.StartNew();
      if (installationTargetFilterValues.Overlaps((IEnumerable<InstallationTarget>) GalleryInstallationTargets.VstsInstallationTargets) && installationTargetFilterValues.Count != GalleryInstallationTargets.VstsInstallationTargets.Count)
        publishedExtensionList.RemoveAll((Predicate<PublishedExtension>) (ext => ext.InstallationTargets.IsNullOrEmpty<InstallationTarget>() || !installationTargetFilterValues.Overlaps((IEnumerable<InstallationTarget>) ext.InstallationTargets)));
      if (flags.HasFlag((Enum) ExtensionQueryFlags.ExcludeNonValidated) | excludeUsingFlags)
        publishedExtensionList.RemoveAll((Predicate<PublishedExtension>) (ext =>
        {
          if (flags.HasFlag((Enum) ExtensionQueryFlags.ExcludeNonValidated) && !ext.Flags.HasFlag((Enum) PublishedExtensionFlags.Validated))
            return true;
          return excludeUsingFlags && (ext.Flags & (PublishedExtensionFlags) excludedFlags) != 0;
        }));
      stopwatch2.Stop();
      Stopwatch stopwatch3 = Stopwatch.StartNew();
      if (!flag3)
        this.SortMatchingExtensions(publishedExtensionList, filter, inMemoryData);
      stopwatch3.Stop();
      Stopwatch stopwatch4 = Stopwatch.StartNew();
      ExtensionQueryResult andPrepareResult = this.ExtractPageAndPrepareResult(publishedExtensionList, filter, flags);
      stopwatch4.Stop();
      this.PublishCustomerIntelligenceEventForInMemoryQuery(requestContext, installationTargetFilterValues.Count > 0 && !flag1, publishedExtensionList.Count, filter.PageNumber, filter.PageSize, andPrepareResult.Results[0].Extensions.Count, stopwatch1.ElapsedMilliseconds, stopwatch2.ElapsedMilliseconds, stopwatch3.ElapsedMilliseconds, stopwatch4.ElapsedMilliseconds);
      return andPrepareResult;
    }

    private void FilterPrivateExtensionsPostCacheRetrieval(
      IVssRequestContext requestContext,
      List<PublishedExtension> extensions,
      string accountToken)
    {
      for (int index = 0; index < extensions.Count; ++index)
      {
        PublishedExtension extension = extensions[index];
        if (!extensions[index].IsPublic() && !GallerySecurity.HasExtensionPermission(requestContext, extension, accountToken, PublisherPermissions.Read, true) && !GallerySecurity.HasExtensionPermission(requestContext, extension, accountToken, PublisherPermissions.PrivateRead, true))
        {
          requestContext.Trace(12061034, TraceLevel.Info, "gallery", "QueryExtensions", string.Format("FilterExtensionsPostCacheRetrieval| Removing Extension (no-permission) | ExtensionId={0}, ExtensionName:{1}, ExtensionFlags={2}, PublisherId={3}, PublisherName={4}, PublisherFlags={5}, AccountToken:{6}", (object) extension.ExtensionId, (object) extension.DisplayName, (object) extension.Flags, (object) extension.Publisher.PublisherId, (object) extension.Publisher.PublisherName, (object) extension.Publisher.Flags, (object) accountToken));
          extensions.RemoveAt(index--);
        }
      }
    }

    private void SortMatchingExtensions(
      List<PublishedExtension> matchingExtensions,
      QueryFilter filter,
      CachedExtensionData inMemoryData)
    {
      IComparer<PublishedExtension> comparer = this.comparerFactory.GetComparer((SortByType) filter.SortBy, (SortOrderType) filter.SortOrder, inMemoryData.AverageRating, inMemoryData.MinVotesRequired);
      matchingExtensions.Sort(comparer);
    }

    private ExtensionQueryResult ExtractPageAndPrepareResult(
      List<PublishedExtension> matchingExtensions,
      QueryFilter queryFilter,
      ExtensionQueryFlags flags)
    {
      ExtensionQueryResult andPrepareResult = new ExtensionQueryResult();
      andPrepareResult.Results = new List<ExtensionFilterResult>();
      ExtensionFilterResult extensionFilterResult = new ExtensionFilterResult();
      andPrepareResult.Results.Add(extensionFilterResult);
      extensionFilterResult.ResultMetadata = new List<ExtensionFilterResultMetadata>();
      extensionFilterResult.ResultMetadata.Add(new ExtensionFilterResultMetadata()
      {
        MetadataType = QueryMetadataConstants.ResultCount,
        MetadataItems = new List<MetadataItem>()
        {
          new MetadataItem()
          {
            Name = QueryMetadataConstants.TotalCount,
            Count = matchingExtensions.Count
          }
        }
      });
      List<PublishedExtension> publishedExtensionList = new List<PublishedExtension>();
      int num1 = (queryFilter.PageNumber - 1) * queryFilter.PageSize;
      int num2 = num1 + queryFilter.PageSize;
      for (int index = num1; index < matchingExtensions.Count && index < num2; ++index)
      {
        PublishedExtension publishedExtension = GalleryUtil.CloneExtension(matchingExtensions[index], flags);
        publishedExtensionList.Add(publishedExtension);
      }
      extensionFilterResult.Extensions = publishedExtensionList;
      return andPrepareResult;
    }

    private void PublishCustomerIntelligenceEventForInMemoryQuery(
      IVssRequestContext requestContext,
      bool isAllQuery,
      int totalCount,
      int pageNumber,
      int pageSize,
      int resultCount,
      long cacheRetrievalTime = -1,
      long filteringTime = -1,
      long sortingTime = -1,
      long resultPreparationTime = -1)
    {
      CustomerIntelligenceData intelligenceData = new CustomerIntelligenceData();
      intelligenceData.Add(CustomerIntelligenceProperty.Action, nameof (InMemoryQuery));
      intelligenceData.Add("QueryType", isAllQuery ? "All" : "Ids");
      intelligenceData.Add("TotalCount", (double) totalCount);
      intelligenceData.Add("PageNumber", (double) pageNumber);
      intelligenceData.Add("PageSize", (double) pageSize);
      intelligenceData.Add("ResultCount", (double) resultCount);
      intelligenceData.Add("CacheRetrievalTimeMs", (double) cacheRetrievalTime);
      intelligenceData.Add("FilteringTimeMs", (double) filteringTime);
      intelligenceData.Add("SortingTimeMs", (double) sortingTime);
      intelligenceData.Add("ResultPreparationTimeMs", (double) resultPreparationTime);
      intelligenceData.AddGalleryUserIdentifier(requestContext);
      requestContext.GetService<CustomerIntelligenceService>().Publish(requestContext, "Microsoft.VisualStudio.Services.Gallery", "Extension", intelligenceData);
    }

    public virtual bool IsQueryApplicableForCache(
      IVssRequestContext requestContext,
      ExtensionQuery extensionQuery,
      Uri referrer = null)
    {
      bool flag = false;
      if (requestContext.UserAgent != null)
      {
        if ((requestContext.UserAgent.StartsWith("VSCode", StringComparison.OrdinalIgnoreCase) || this.IsRequestFromVsCodeDocumentation(requestContext, referrer)) && extensionQuery.Filters.Count == 1)
        {
          QueryFilter filter = extensionQuery.Filters[0];
          if (filter.PageNumber > 0 && filter.PageSize > 0 && filter.Criteria != null)
          {
            foreach (FilterCriteria criterion in filter.Criteria)
            {
              if (!this.allowedFilterTypesForInMemoryQuery.Contains(criterion.FilterType))
                return false;
              if (criterion.FilterType == 8)
              {
                if (!string.Equals(criterion.Value, "Microsoft.VisualStudio.Code"))
                  return false;
                flag = true;
              }
            }
            return flag;
          }
        }
        else if (requestContext.UserAgent.StartsWith("VSMarketplaceBadge", StringComparison.OrdinalIgnoreCase))
          return true;
      }
      return false;
    }

    public virtual ApplicableMemoryCacheType GetApplicableMemoryCacheType(
      IVssRequestContext requestContext,
      ExtensionQuery extensionQuery)
    {
      ApplicableMemoryCacheType applicableMemoryCacheType = ApplicableMemoryCacheType.None;
      bool flag1 = false;
      bool flag2 = false;
      bool flag3 = false;
      int num1 = 0;
      int num2 = 0;
      int num3 = 0;
      bool flag4 = requestContext.IsFeatureEnabled("Microsoft.VisualStudio.Services.Gallery.EnableQueryExtensionsCacheForTagsAndCategoriesForVSTS");
      bool flag5 = requestContext.IsFeatureEnabled("Microsoft.VisualStudio.Services.Gallery.EnableQueryExtensionsCacheForTagsAndCategoriesForVSCode");
      if (extensionQuery.Filters.Count != 1)
        return ApplicableMemoryCacheType.None;
      QueryFilter filter = extensionQuery.Filters[0];
      if (filter.PageNumber <= 0 || filter.PageSize <= 0 || filter.Criteria == null)
        return ApplicableMemoryCacheType.None;
      foreach (FilterCriteria criterion in filter.Criteria)
      {
        if (!this.extendedAllowedFilterTypesForInMemoryQuery.Contains(criterion.FilterType))
        {
          flag1 = true;
          break;
        }
        switch (criterion.FilterType)
        {
          case 1:
          case 5:
            ++num1;
            continue;
          case 4:
          case 7:
            ++num2;
            continue;
          case 8:
            flag3 = true;
            if (string.Equals(criterion.Value, "Microsoft.VisualStudio.Code", StringComparison.OrdinalIgnoreCase))
            {
              applicableMemoryCacheType |= ApplicableMemoryCacheType.VSCode;
              continue;
            }
            if (requestContext.IsFeatureEnabled("Microsoft.VisualStudio.Services.Gallery.EnableQueryExtensionsCacheForVSTS"))
            {
              if (GalleryInstallationTargets.VstsInstallationTargets.Contains(new InstallationTarget()
              {
                Target = criterion.Value
              }))
              {
                applicableMemoryCacheType |= ApplicableMemoryCacheType.VSTS;
                continue;
              }
            }
            flag2 = true;
            continue;
          case 23:
            ++num3;
            continue;
          default:
            continue;
        }
      }
      if (flag1 | flag2 || num1 > 1 || num1 > 0 && num2 > 0)
        applicableMemoryCacheType = ApplicableMemoryCacheType.None;
      else if (num3 > 1 || num1 > 0 && num3 > 0 || num3 > 0 && num2 > 0)
        applicableMemoryCacheType = ApplicableMemoryCacheType.None;
      else if (!flag3)
      {
        applicableMemoryCacheType = ApplicableMemoryCacheType.VSCode;
        if (requestContext.IsFeatureEnabled("Microsoft.VisualStudio.Services.Gallery.EnableQueryExtensionsCacheForVSTS"))
          applicableMemoryCacheType |= ApplicableMemoryCacheType.VSTS;
      }
      if (num1 > 0)
      {
        if (!flag3)
        {
          if (!flag5 || !flag4)
            applicableMemoryCacheType = ApplicableMemoryCacheType.None;
        }
        else
        {
          if (!flag5)
            applicableMemoryCacheType &= ~ApplicableMemoryCacheType.VSCode;
          if (!flag4)
            applicableMemoryCacheType &= ~ApplicableMemoryCacheType.VSTS;
        }
      }
      return applicableMemoryCacheType;
    }

    private bool IsRequestFromVsCodeDocumentation(IVssRequestContext requestContext, Uri referrer) => requestContext.IsFeatureEnabled("Microsoft.VisualStudio.Services.Gallery.EnableQueryExtensionsCacheForVsCodeDoc") && referrer != (Uri) null && referrer.Host.Equals("code.visualstudio.com", StringComparison.InvariantCultureIgnoreCase);
  }
}
