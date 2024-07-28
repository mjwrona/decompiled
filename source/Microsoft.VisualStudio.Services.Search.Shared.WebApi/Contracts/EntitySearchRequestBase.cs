// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Shared.WebApi.Contracts.EntitySearchRequestBase
// Assembly: Microsoft.VisualStudio.Services.Search.Shared.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 504F400B-CBC4-4007-9816-31A8DED1C3FC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Shared.WebApi.dll

using Microsoft.VisualStudio.Services.Search.Shared.WebApi.Exceptions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Search.Shared.WebApi.Contracts
{
  [DataContract]
  public abstract class EntitySearchRequestBase : SearchSecuredV2Object
  {
    [DataMember(Name = "searchText")]
    public string SearchText { get; set; }

    [DataMember(Name = "filters", IsRequired = false)]
    public IDictionary<string, IEnumerable<string>> Filters { get; set; }

    public override string ToString() => JsonConvert.SerializeObject((object) this, Formatting.None, new JsonSerializerSettings()
    {
      NullValueHandling = NullValueHandling.Ignore
    });

    public virtual void ValidateQuery() => this.ValidateQueryRequest();

    public virtual void ValidateQuery(IReadOnlyDictionary<string, string> ParentOf)
    {
      this.ValidateQueryRequest();
      this.ValidateFilterHierarchy(ParentOf);
    }

    public abstract bool IsSupportedFilter(string filterName);

    private void ValidateQueryRequest()
    {
      if (string.IsNullOrWhiteSpace(this.SearchText))
        throw new InvalidQueryException(SearchSharedWebApiResources.NullOrEmptySearchTextMessage);
      if (this.Filters == null)
        return;
      foreach (KeyValuePair<string, IEnumerable<string>> filter in (IEnumerable<KeyValuePair<string, IEnumerable<string>>>) this.Filters)
      {
        string key = filter.Key;
        IEnumerable<string> strings = filter.Value;
        if (string.IsNullOrWhiteSpace(key))
          throw new InvalidQueryException(SearchSharedWebApiResources.NullOrEmptyFilterNameMessage);
        if (strings == null)
          throw new InvalidQueryException(string.Format((IFormatProvider) CultureInfo.CurrentUICulture, SearchSharedWebApiResources.NullFilterValuesMessage, (object) key));
        if (!this.IsSupportedFilter(key))
          throw new InvalidQueryException(string.Format((IFormatProvider) CultureInfo.CurrentUICulture, SearchSharedWebApiResources.UnknownFilterMessageFormat, (object) key));
      }
    }

    private void ValidateFilterHierarchy(IReadOnlyDictionary<string, string> parentOf)
    {
      if (this.Filters == null)
        return;
      foreach (KeyValuePair<string, string> keyValuePair in (IEnumerable<KeyValuePair<string, string>>) parentOf)
      {
        KeyValuePair<string, string> childAndParentFilters = keyValuePair;
        if (this.Filters.Any<KeyValuePair<string, IEnumerable<string>>>((Func<KeyValuePair<string, IEnumerable<string>>, bool>) (f => f.Key.Equals(childAndParentFilters.Key, StringComparison.OrdinalIgnoreCase))))
        {
          KeyValuePair<string, IEnumerable<string>>? nullable = this.Filters.Where<KeyValuePair<string, IEnumerable<string>>>((Func<KeyValuePair<string, IEnumerable<string>>, bool>) (f => f.Key.Equals(childAndParentFilters.Value, StringComparison.OrdinalIgnoreCase))).Select<KeyValuePair<string, IEnumerable<string>>, KeyValuePair<string, IEnumerable<string>>?>((Func<KeyValuePair<string, IEnumerable<string>>, KeyValuePair<string, IEnumerable<string>>?>) (f => new KeyValuePair<string, IEnumerable<string>>?(f))).FirstOrDefault<KeyValuePair<string, IEnumerable<string>>?>();
          if (!nullable.HasValue)
            throw new InvalidQueryException(string.Format((IFormatProvider) CultureInfo.CurrentUICulture, SearchSharedWebApiResources.ParentFilterNotFoundMessageFormat, (object) childAndParentFilters.Key, (object) childAndParentFilters.Value));
          if (nullable.Value.Value.Count<string>() > 1)
            throw new InvalidQueryException(string.Format((IFormatProvider) CultureInfo.CurrentUICulture, SearchSharedWebApiResources.ParentFilterHasMultipleValuesMessageFormat, (object) childAndParentFilters.Value, (object) childAndParentFilters.Key));
        }
      }
    }
  }
}
