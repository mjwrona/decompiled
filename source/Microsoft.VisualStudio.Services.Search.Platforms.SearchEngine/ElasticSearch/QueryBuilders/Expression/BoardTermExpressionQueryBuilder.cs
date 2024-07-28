// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.ElasticSearch.QueryBuilders.Expression.BoardTermExpressionQueryBuilder
// Assembly: Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EE1DF96-C85D-457F-AAA1-93619829BFD4
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.dll

using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Common.Arriba;
using Microsoft.VisualStudio.Services.Search.Common.Arriba.Expressions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.ElasticSearch.QueryBuilders.Expression
{
  internal sealed class BoardTermExpressionQueryBuilder
  {
    public string Build(IExpression expression)
    {
      TermExpression termExpression = (TermExpression) expression;
      return termExpression.Operator == Operator.Matches ? this.GetMultiMatchQueryString(termExpression) : throw new NotSupportedException(FormattableString.Invariant(FormattableStringFactory.Create("Operator [{0}] is not supported in Board search.", (object) termExpression.Operator)));
    }

    private string GetMultiMatchQueryString(TermExpression termExpression)
    {
      int num = termExpression.Value.Contains("*") ? 1 : (termExpression.Value.Contains("?") ? 1 : 0);
      IList<string> namesToWildcardQuery = this.GetPlatformFieldNamesToWildcardQuery();
      return num != 0 ? this.GetFullTextMultiWildcardQueryString(termExpression, namesToWildcardQuery) : this.GetFullTextMultiMatchQueryString(termExpression, namesToWildcardQuery);
    }

    private IList<string> GetPlatformFieldNamesToWildcardQuery()
    {
      IList<string> namesToWildcardQuery = (IList<string>) new List<string>();
      namesToWildcardQuery.Add(FormattableString.Invariant(FormattableStringFactory.Create("{0}", (object) "teamName")));
      namesToWildcardQuery.Add(FormattableString.Invariant(FormattableStringFactory.Create("{0}", (object) "boardType")));
      return namesToWildcardQuery;
    }

    private string GetFullTextMultiMatchQueryString(
      TermExpression termExpression,
      IList<string> fieldsToQuery)
    {
      return FormattableString.Invariant(FormattableStringFactory.Create("{{\r\n                    \"multi_match\": {{\r\n                        \"query\": {0},\r\n                        \"fields\": [\r\n                            {1}\r\n                        ],\r\n                        \"type\": \"phrase\"\r\n                    }}\r\n                }}", (object) JsonConvert.SerializeObject((object) termExpression.Value.NormalizePath()), (object) string.Join(",", fieldsToQuery.Select<string, string>((Func<string, string>) (f => FormattableString.Invariant(FormattableStringFactory.Create("\"{0}\"", (object) f)))))));
    }

    private string GetFullTextMultiWildcardQueryString(
      TermExpression termExpression,
      IList<string> fieldsToQuery)
    {
      return FormattableString.Invariant(FormattableStringFactory.Create("{{\r\n                    \"multi_wildcard\": {{\r\n                        \"value\": {0},\r\n                        \"fields\": [\r\n                            {1}\r\n                        ],\r\n                        \"rewrite\":\"top_terms_boost_100\"\r\n                    }}\r\n                }}", (object) JsonConvert.SerializeObject((object) termExpression.Value.NormalizePath()), (object) string.Join(",", fieldsToQuery.Select<string, string>((Func<string, string>) (f => FormattableString.Invariant(FormattableStringFactory.Create("\"{0}\"", (object) f)))))));
    }
  }
}
