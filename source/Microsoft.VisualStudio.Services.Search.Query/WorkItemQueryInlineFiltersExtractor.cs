// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Query.WorkItemQueryInlineFiltersExtractor
// Assembly: Microsoft.VisualStudio.Services.Search.Query, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 71E00698-03D3-4C67-B313-A550333DA80C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Query.dll

using Microsoft.VisualStudio.Services.Search.Common.Arriba.Expressions;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Search.Query
{
  internal static class WorkItemQueryInlineFiltersExtractor
  {
    public static IEnumerable<string> Extract(IExpression queryExpression)
    {
      HashSet<string> stringSet = new HashSet<string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      foreach (IExpression expression in (IEnumerable<IExpression>) queryExpression)
      {
        if (expression is TermExpression termExpression && !termExpression.IsOfType("*"))
          stringSet.Add(termExpression.Type);
      }
      return (IEnumerable<string>) stringSet;
    }
  }
}
