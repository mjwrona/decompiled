// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Query.QueryForwarderHelpers
// Assembly: Microsoft.VisualStudio.Services.Search.Query, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 71E00698-03D3-4C67-B313-A550333DA80C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Query.dll

using Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Search.Query
{
  public static class QueryForwarderHelpers
  {
    public static IDictionary<string, IEnumerable<string>> ConvertToDictionary(
      IEnumerable<SearchFilter> searchFilters)
    {
      Dictionary<string, IEnumerable<string>> dictionary = new Dictionary<string, IEnumerable<string>>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      if (searchFilters == null)
        return (IDictionary<string, IEnumerable<string>>) dictionary;
      foreach (SearchFilter searchFilter in searchFilters)
      {
        if (searchFilter.Values.Any<string>())
          dictionary.Add(searchFilter.Name, searchFilter.Values);
      }
      return (IDictionary<string, IEnumerable<string>>) dictionary;
    }
  }
}
