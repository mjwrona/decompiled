// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.Contracts.EntitySearchQuery
// Assembly: Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8C7C9E57-44B4-4654-9458-CC8B59C2B681
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.dll

using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Text;

namespace Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.Contracts
{
  [DataContract]
  public abstract class EntitySearchQuery : SearchSecuredObject
  {
    [DataMember(Name = "searchText")]
    public string SearchText { get; set; }

    [DataMember(Name = "skipResults")]
    public int SkipResults { get; set; }

    [DataMember(Name = "takeResults")]
    public int TakeResults { get; set; }

    [DataMember(Name = "filters")]
    public IEnumerable<SearchFilter> Filters { get; set; }

    [DataMember(Name = "searchFilters")]
    public IDictionary<string, IEnumerable<string>> SearchFilters { get; set; }

    [DataMember(Name = "sortOptions")]
    public IEnumerable<EntitySortOption> SortOptions { get; set; }

    [DataMember(Name = "summarizedHitCountsNeeded", IsRequired = false)]
    public bool SummarizedHitCountsNeeded { get; set; }

    [DataMember(Name = "includeSuggestions", IsRequired = false)]
    public bool IncludeSuggestions { get; set; }

    [ClientInternalUseOnly(true)]
    [DataMember(Name = "isInstantSearch", IsRequired = false)]
    public bool IsInstantSearch { get; set; }

    public virtual void ValidateQuery()
    {
      if (string.IsNullOrEmpty(this.SearchText))
        throw new InvalidQueryException(SearchSharedWebApiResources.NullOrEmptySearchTextMessage);
      if (this.SearchFilters != null)
      {
        List<SearchFilter> searchFilterList = new List<SearchFilter>(this.SearchFilters.Count);
        foreach (KeyValuePair<string, IEnumerable<string>> searchFilter in (IEnumerable<KeyValuePair<string, IEnumerable<string>>>) this.SearchFilters)
          searchFilterList.Add(new SearchFilter()
          {
            Name = searchFilter.Key,
            Values = searchFilter.Value
          });
        this.Filters = (IEnumerable<SearchFilter>) searchFilterList;
      }
      if (this.Filters != null)
      {
        IEnumerable<string> source = this.Filters.Select<SearchFilter, string>((Func<SearchFilter, string>) (f => f.Name));
        if (source.Count<string>() != source.Distinct<string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase).Count<string>())
          throw new InvalidQueryException(SearchSharedWebApiResources.DuplicateFilterNameMessage);
        foreach (SearchFilter filter in this.Filters)
        {
          if (string.IsNullOrWhiteSpace(filter.Name))
            throw new InvalidQueryException(SearchSharedWebApiResources.NullOrEmptyFilterNameMessage);
          if (filter.Values == null)
            throw new InvalidQueryException(string.Format((IFormatProvider) CultureInfo.CurrentUICulture, SearchSharedWebApiResources.NullFilterValuesMessage, (object) filter.Name));
          foreach (string str in filter.Values)
          {
            if (string.IsNullOrWhiteSpace(str))
              throw new InvalidQueryException(string.Format((IFormatProvider) CultureInfo.CurrentUICulture, SearchSharedWebApiResources.InvalidFilterValueMessage, (object) filter.Name));
          }
          if (!this.IsSupportedFilter(filter.Name))
            throw new InvalidQueryException(string.Format((IFormatProvider) CultureInfo.CurrentUICulture, SearchSharedWebApiResources.UnknownFilterMessageFormat, (object) filter.Name));
        }
      }
      if (this.SortOptions == null)
        return;
      HashSet<EntitySortOption> entitySortOptionSet = new HashSet<EntitySortOption>();
      foreach (EntitySortOption sortOption in this.SortOptions)
      {
        if (sortOption == null)
          throw new InvalidQueryException(SearchSharedWebApiResources.EmptySortOptionInOrderByMessage);
        if (sortOption.SortOrder == SortOrder.Undefined)
          throw new InvalidQueryException(string.Format((IFormatProvider) CultureInfo.CurrentUICulture, SearchSharedWebApiResources.EmptySortOrderGivenInSortOptionsMessageFormat, (object) sortOption.Field));
        if (string.IsNullOrWhiteSpace(sortOption.Field))
          throw new InvalidQueryException(SearchSharedWebApiResources.SortOptionFieldNameShouldNotBeEmptyOrNullMessage);
        if (!entitySortOptionSet.Add(sortOption))
          throw new InvalidQueryException(string.Format((IFormatProvider) CultureInfo.CurrentUICulture, SearchSharedWebApiResources.FieldSpecifiedMoreThanOnceInTheSortOptionsMessageFormat, (object) sortOption.Field));
      }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public string GetFilterIfPresentAndSingleValuedElseNull(string filterId)
    {
      IEnumerable<SearchFilter> filters = this.Filters;
      SearchFilter searchFilter = filters != null ? filters.FirstOrDefault<SearchFilter>((Func<SearchFilter, bool>) (x => filterId.Equals(x.Name, StringComparison.OrdinalIgnoreCase))) : (SearchFilter) null;
      if (searchFilter != null)
      {
        IEnumerable<string> values = searchFilter.Values;
        int? nullable = values != null ? new int?(values.Count<string>()) : new int?();
        int num = 1;
        if (nullable.GetValueOrDefault() == num & nullable.HasValue)
          return searchFilter.Values.First<string>();
      }
      return (string) null;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public IEnumerable<string> GetAllFilters(string filterId)
    {
      if (string.IsNullOrWhiteSpace(filterId))
        return (IEnumerable<string>) new List<string>();
      IEnumerable<SearchFilter> filters = this.Filters;
      return (filters != null ? filters.FirstOrDefault<SearchFilter>((Func<SearchFilter, bool>) (x => filterId.Equals(x.Name, StringComparison.OrdinalIgnoreCase))) : (SearchFilter) null)?.Values;
    }

    protected void ValidateFilterHierarchy(IReadOnlyDictionary<string, string> parentOf)
    {
      if (this.Filters == null)
        return;
      foreach (KeyValuePair<string, string> keyValuePair in (IEnumerable<KeyValuePair<string, string>>) parentOf)
      {
        KeyValuePair<string, string> childAndParentFilters = keyValuePair;
        if (this.Filters.FirstOrDefault<SearchFilter>((Func<SearchFilter, bool>) (f => f.Name.Equals(childAndParentFilters.Key, StringComparison.OrdinalIgnoreCase))) != null)
        {
          SearchFilter searchFilter = this.Filters.FirstOrDefault<SearchFilter>((Func<SearchFilter, bool>) (f => f.Name.Equals(childAndParentFilters.Value, StringComparison.OrdinalIgnoreCase)));
          if (searchFilter == null)
            throw new InvalidQueryException(string.Format((IFormatProvider) CultureInfo.CurrentUICulture, SearchSharedWebApiResources.ParentFilterNotFoundMessageFormat, (object) childAndParentFilters.Key, (object) childAndParentFilters.Value));
          if (searchFilter.Values.Count<string>() != 1)
            throw new InvalidQueryException(string.Format((IFormatProvider) CultureInfo.CurrentUICulture, SearchSharedWebApiResources.ParentFilterHasMultipleValuesMessageFormat, (object) childAndParentFilters.Value, (object) childAndParentFilters.Key));
        }
      }
    }

    public string ToString(int indentLevel)
    {
      StringBuilder sb = new StringBuilder();
      string indentSpacing = Extensions.GetIndentSpacing(indentLevel);
      sb.Append(indentSpacing, "Skip: ").AppendLine(this.SkipResults.ToString());
      sb.Append(indentSpacing, "Take: ").AppendLine(this.TakeResults.ToString());
      sb.Append(indentSpacing, "Filters:").AppendLine();
      if (this.Filters != null)
      {
        foreach (SearchFilter filter in this.Filters)
          sb.AppendLine(filter.ToString(indentLevel + 1));
      }
      return sb.ToString();
    }

    public override string ToString() => this.ToString(0);

    public abstract bool IsSupportedFilter(string filterName);

    internal override void SetSecuredObject(
      Guid namespaceId,
      int requiredPermissions,
      string token)
    {
      base.SetSecuredObject(namespaceId, requiredPermissions, token);
      IEnumerable<SearchFilter> filters = this.Filters;
      this.Filters = filters != null ? (IEnumerable<SearchFilter>) filters.Select<SearchFilter, SearchFilter>((Func<SearchFilter, SearchFilter>) (i =>
      {
        i.SetSecuredObject(namespaceId, requiredPermissions, token);
        return i;
      })).ToList<SearchFilter>() : (IEnumerable<SearchFilter>) null;
      IEnumerable<EntitySortOption> sortOptions = this.SortOptions;
      this.SortOptions = sortOptions != null ? (IEnumerable<EntitySortOption>) sortOptions.Select<EntitySortOption, EntitySortOption>((Func<EntitySortOption, EntitySortOption>) (i =>
      {
        i.SetSecuredObject(namespaceId, requiredPermissions, token);
        return i;
      })).ToList<EntitySortOption>() : (IEnumerable<EntitySortOption>) null;
    }
  }
}
