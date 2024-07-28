// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.WebApi.Legacy.QueryHelper
// Assembly: Microsoft.VisualStudio.Services.Search.WebApi.Legacy, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 5D4CB2D3-3C08-46C7-B9C5-51E638F57F9E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.WebApi.Legacy.dll

using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Search.WebApi.Legacy
{
  public static class QueryHelper
  {
    public static bool IsAdvancedCodeQuery(string searchText)
    {
      if (!string.IsNullOrWhiteSpace(searchText))
      {
        if (RegularExpressions.WildcardRegex.IsMatch(searchText))
          return true;
        List<string> all = ((IEnumerable<string>) RegularExpressions.WhitespaceRegex.Split(searchText)).ToList<string>().FindAll((Predicate<string>) (s => !string.IsNullOrWhiteSpace(s)));
        int count = all.Count;
        if (count > 2 || count == 2 && !searchText.Contains(":") || count > 0 && all.FindIndex((Predicate<string>) (x => RegularExpressions.SpecialCharRegexForAdvancedQuery.IsMatch(x))) >= 0)
          return true;
      }
      return false;
    }
  }
}
