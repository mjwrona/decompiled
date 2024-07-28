// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Implementations.ElasticSearch.EntityProviders.EntityProvider
// Assembly: Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EE1DF96-C85D-457F-AAA1-93619829BFD4
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.dll

using Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Implementations.ElasticSearch.EntityProviders
{
  public abstract class EntityProvider
  {
    public virtual bool IsValidEntityAccess(EntitySearchPlatformRequest request) => true;

    public virtual void ValidateQueryRequest(EntitySearchPlatformRequest request)
    {
      if (request == null)
        throw new ArgumentNullException(nameof (request));
      if (request.QueryParseTree == null)
        throw new ArgumentException(FormattableString.Invariant(FormattableStringFactory.Create("{0}.{1} is null", (object) nameof (request), (object) "QueryParseTree")));
      if (request.TakeResults < 0)
        throw new ArgumentOutOfRangeException(nameof (request), FormattableString.Invariant(FormattableStringFactory.Create("{0}.{1} cannot be negative", (object) nameof (request), (object) "TakeResults")));
      if (request.SkipResults < 0)
        throw new ArgumentOutOfRangeException(nameof (request), FormattableString.Invariant(FormattableStringFactory.Create("{0}.{1} cannot be negative", (object) nameof (request), (object) "SkipResults")));
      if (request.SearchFilters == null)
        throw new ArgumentException(FormattableString.Invariant(FormattableStringFactory.Create("{0}.{1} is null", (object) nameof (request), (object) "SearchFilters")));
      foreach (KeyValuePair<string, IEnumerable<string>> searchFilter in (IEnumerable<KeyValuePair<string, IEnumerable<string>>>) request.SearchFilters)
      {
        if (!searchFilter.Value.Any<string>())
          throw new ArgumentException(FormattableString.Invariant(FormattableStringFactory.Create("Filter category [{0}] in {1}.{2} is empty.", (object) searchFilter.Key, (object) nameof (request), (object) "SearchFilters")));
      }
    }

    public virtual void ValidateCountRequest(ResultsCountPlatformRequest request)
    {
      if (request == null)
        throw new ArgumentNullException(nameof (request));
      if (request.QueryParseTree == null)
        throw new ArgumentException(FormattableString.Invariant(FormattableStringFactory.Create("{0}.{1} is null", (object) nameof (request), (object) "QueryParseTree")));
      if (request.SearchFilters == null)
        throw new ArgumentException(FormattableString.Invariant(FormattableStringFactory.Create("{0}.{1} is null", (object) nameof (request), (object) "SearchFilters")));
      foreach (KeyValuePair<string, IEnumerable<string>> searchFilter in (IEnumerable<KeyValuePair<string, IEnumerable<string>>>) request.SearchFilters)
      {
        if (!searchFilter.Value.Any<string>())
          throw new ArgumentException(FormattableString.Invariant(FormattableStringFactory.Create("Filter category [{0}] in {1}.{2} is empty.", (object) searchFilter.Key, (object) nameof (request), (object) "SearchFilters")));
      }
    }

    public virtual void ValidateSuggestRequest(EntitySearchSuggestPlatformRequest request)
    {
      if (request == null)
        throw new ArgumentNullException(nameof (request));
      if (string.IsNullOrWhiteSpace(request.SuggestText))
        throw new ArgumentException(FormattableString.Invariant(FormattableStringFactory.Create("{0}.{1} is null or whitespace", (object) nameof (request), (object) "SuggestText")));
      if (string.IsNullOrWhiteSpace(request.SuggestField))
        throw new ArgumentException(FormattableString.Invariant(FormattableStringFactory.Create("{0}.{1} is null or whitespace", (object) nameof (request), (object) "SuggestField")));
      if (string.IsNullOrWhiteSpace(request.SuggestQueryName))
        throw new ArgumentException(FormattableString.Invariant(FormattableStringFactory.Create("{0}.{1} is null or whitespace", (object) nameof (request), (object) "SuggestQueryName")));
    }
  }
}
