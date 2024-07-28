// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Implementations.ElasticSearch.QueryBuilders.Relevance.TermBoostQueryBuilder
// Assembly: Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EE1DF96-C85D-457F-AAA1-93619829BFD4
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.dll

using Microsoft.VisualStudio.Services.Search.Common.Arriba.Expressions.Relevance;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Implementations.ElasticSearch.QueryBuilders.Relevance
{
  internal class TermBoostQueryBuilder : IRelevanceQueryBuilder
  {
    public string Build(string baseQuery, IRelevanceExpression rule)
    {
      TermBoostExpression termBoostExpression = rule as TermBoostExpression;
      List<string> values = new List<string>();
      foreach (TermBoostExpression.TermExpression termExpression in termBoostExpression.TermsDescriptor)
        values.Add(FormattableString.Invariant(FormattableStringFactory.Create("{{\r\n                    \"filter\": {{ \"terms\": {{ {0}: {1} }} }}, \r\n                    \"weight\": {2} \r\n                    }}", (object) JsonConvert.SerializeObject((object) termExpression.FieldName), (object) JsonConvert.SerializeObject((object) termExpression.Terms), (object) termExpression.Boost)));
      return FormattableString.Invariant(FormattableStringFactory.Create("{{\r\n                \"function_score\": {{\r\n                \"query\": {0},\r\n                \"functions\": [\r\n                    {1}\r\n                ],\r\n                \"score_mode\": \"multiply\"\r\n              }}\r\n           }}", (object) baseQuery, (object) string.Join(FormattableString.Invariant(FormattableStringFactory.Create(",{0}", (object) Environment.NewLine)), (IEnumerable<string>) values)));
    }
  }
}
