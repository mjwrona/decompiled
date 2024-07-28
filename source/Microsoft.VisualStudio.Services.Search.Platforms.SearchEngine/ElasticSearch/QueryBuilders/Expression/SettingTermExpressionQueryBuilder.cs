// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.ElasticSearch.QueryBuilders.Expression.SettingTermExpressionQueryBuilder
// Assembly: Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EE1DF96-C85D-457F-AAA1-93619829BFD4
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.dll

using Microsoft.VisualStudio.Services.Search.Common.Arriba;
using Microsoft.VisualStudio.Services.Search.Common.Arriba.Expressions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.ElasticSearch.QueryBuilders.Expression
{
  internal sealed class SettingTermExpressionQueryBuilder
  {
    public string Build(IExpression expression)
    {
      TermExpression termExpression = (TermExpression) expression;
      switch (termExpression.Operator)
      {
        case Operator.Equals:
          return this.GetFilteredTermQueryStringForEqualityOperators(termExpression);
        case Operator.Matches:
          return this.GetMultiMatchQueryString(termExpression);
        default:
          throw new NotSupportedException(FormattableString.Invariant(FormattableStringFactory.Create("Operator [{0}] is not supported in Setting search.", (object) termExpression.Operator)));
      }
    }

    private string GetMultiMatchQueryString(TermExpression termExpression)
    {
      int num = termExpression.Value.Contains("*") ? 1 : (termExpression.Value.Contains("?") ? 1 : 0);
      IList<string> fieldsToQuery = num != 0 ? this.GetPlatformFieldNamesToWildcardQuery() : this.GetPlatformFieldNamesToQuery();
      return num != 0 ? (termExpression.IsOfType("*") ? this.GetFullTextMultiWildcardQueryString(termExpression, fieldsToQuery) : this.GetFilteredMultiWildcardQueryString(termExpression)) : (termExpression.IsOfType("*") ? this.GetFullTextMultiMatchQueryString(termExpression, fieldsToQuery) : this.GetFilteredMultiMatchQueryString(termExpression));
    }

    private string GetFilteredTermQueryStringForEqualityOperators(TermExpression termExpression)
    {
      if (termExpression.Operator != Operator.Equals)
        return string.Empty;
      return FormattableString.Invariant(FormattableStringFactory.Create("{{\r\n                            \"term\": {{\r\n                                \"{0}\": {1}\r\n                            }}\r\n                        }}", (object) termExpression.Type, (object) JsonConvert.SerializeObject((object) termExpression.Value)));
    }

    private IList<string> GetPlatformFieldNamesToQuery()
    {
      IList<string> fieldNamesToQuery = (IList<string>) new List<string>();
      fieldNamesToQuery.Add(FormattableString.Invariant(FormattableStringFactory.Create("{0}", (object) "description")));
      fieldNamesToQuery.Add(FormattableString.Invariant(FormattableStringFactory.Create("{0}", (object) "scope")));
      fieldNamesToQuery.Add(FormattableString.Invariant(FormattableStringFactory.Create("{0}", (object) "tags")));
      fieldNamesToQuery.Add(FormattableString.Invariant(FormattableStringFactory.Create("{0}", (object) "title")));
      return fieldNamesToQuery;
    }

    private IList<string> GetPlatformFieldNamesToWildcardQuery()
    {
      IList<string> namesToWildcardQuery = (IList<string>) new List<string>();
      namesToWildcardQuery.Add(FormattableString.Invariant(FormattableStringFactory.Create("{0}", (object) "description")));
      namesToWildcardQuery.Add(FormattableString.Invariant(FormattableStringFactory.Create("{0}", (object) "scope")));
      namesToWildcardQuery.Add(FormattableString.Invariant(FormattableStringFactory.Create("{0}", (object) "tags")));
      namesToWildcardQuery.Add(FormattableString.Invariant(FormattableStringFactory.Create("{0}", (object) "title")));
      return namesToWildcardQuery;
    }

    private string GetFullTextMultiWildcardQueryString(
      TermExpression termExpression,
      IList<string> fieldsToQuery)
    {
      return FormattableString.Invariant(FormattableStringFactory.Create("{{\r\n                    \"query_string\": {{\r\n                        \"query\": {0},\r\n                        \"fields\": [\r\n                            {1}\r\n                        ],\r\n                        \"rewrite\":\"top_terms_boost_100\"\r\n                    }}\r\n                }}", (object) JsonConvert.SerializeObject((object) termExpression.Value), (object) string.Join(",", fieldsToQuery.Select<string, string>((Func<string, string>) (f => FormattableString.Invariant(FormattableStringFactory.Create("\"{0}\"", (object) f)))))));
    }

    private string GetFullTextMultiMatchQueryString(
      TermExpression termExpression,
      IList<string> fieldsToQuery)
    {
      return FormattableString.Invariant(FormattableStringFactory.Create("{{\r\n                    \"multi_match\": {{\r\n                        \"query\": {0},\r\n                        \"fields\": [\r\n                            {1}\r\n                        ],\r\n                        \"type\": \"phrase\"\r\n                    }}\r\n                }}", (object) JsonConvert.SerializeObject((object) termExpression.Value), (object) string.Join(",", fieldsToQuery.Select<string, string>((Func<string, string>) (f => FormattableString.Invariant(FormattableStringFactory.Create("\"{0}\"", (object) f)))))));
    }

    private string GetFilteredMultiWildcardQueryString(TermExpression termExpression)
    {
      string type = termExpression.Type;
      return FormattableString.Invariant(FormattableStringFactory.Create("{{\r\n                    \"multi_wildcard\": {{\r\n                        \"value\": {0},\r\n                        \"fields\": [\r\n                            \"{1}\"\r\n                        ],\r\n                        \"rewrite\":\"top_terms_boost_100\"\r\n                    }}\r\n                }}", (object) JsonConvert.SerializeObject((object) termExpression.Value), (object) type));
    }

    private string GetFilteredMultiMatchQueryString(TermExpression termExpression)
    {
      string type = termExpression.Type;
      return FormattableString.Invariant(FormattableStringFactory.Create("{{\r\n                    \"multi_match\": {{\r\n                        \"query\": {0},\r\n                        \"fields\": [\r\n                            \"{1}\"\r\n                        ],\r\n                        \"type\": \"phrase\"\r\n                    }}\r\n                }}", (object) JsonConvert.SerializeObject((object) termExpression.Value), (object) type));
    }
  }
}
