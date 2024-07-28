// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.ElasticSearch.QueryBuilders.Expression.PackageTermExpressionQueryBuilder
// Assembly: Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EE1DF96-C85D-457F-AAA1-93619829BFD4
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Search.Common.Arriba;
using Microsoft.VisualStudio.Services.Search.Common.Arriba.Expressions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.ElasticSearch.QueryBuilders.Expression
{
  internal sealed class PackageTermExpressionQueryBuilder
  {
    public string Build(IVssRequestContext requestContext, IExpression expression)
    {
      TermExpression termExpression = (TermExpression) expression;
      switch (termExpression.Operator)
      {
        case Operator.Equals:
          return this.GetFilteredTermQueryStringForEqualityOperators(termExpression);
        case Operator.Matches:
          return this.GetMultiMatchQueryString(requestContext, termExpression);
        default:
          throw new NotSupportedException(FormattableString.Invariant(FormattableStringFactory.Create("Operator [{0}] is not supported in Package search.", (object) termExpression.Operator)));
      }
    }

    private string GetMultiMatchQueryString(
      IVssRequestContext requestContext,
      TermExpression termExpression)
    {
      int num = termExpression.Value.Contains("*") ? 1 : (termExpression.Value.Contains("?") ? 1 : 0);
      IList<string> fieldsToQuery = num != 0 ? this.GetPlatformFieldNamesToWildcardQuery(requestContext) : this.GetPlatformFieldNamesToQuery(requestContext);
      return num != 0 ? (termExpression.IsOfType("*") ? this.GetFullTextMultiWildcardQueryString(termExpression, fieldsToQuery) : this.GetFilteredMultiWildcardQueryString(termExpression)) : (termExpression.IsOfType("*") ? this.GetFullTextMultiMatchQueryString(termExpression, fieldsToQuery) : this.GetFilteredMultiMatchQueryString(termExpression));
    }

    private string GetFilteredTermQueryStringForEqualityOperators(TermExpression termExpression)
    {
      if (termExpression.Operator != Operator.Equals)
        return string.Empty;
      return FormattableString.Invariant(FormattableStringFactory.Create("{{\r\n                            \"term\": {{\r\n                                \"{0}\": {1}\r\n                            }}\r\n                        }}", (object) termExpression.Type, (object) JsonConvert.SerializeObject((object) termExpression.Value)));
    }

    private IList<string> GetPlatformFieldNamesToQuery(IVssRequestContext requestContext)
    {
      IList<string> fieldNamesToQuery = (IList<string>) new List<string>();
      fieldNamesToQuery.Add(FormattableString.Invariant(FormattableStringFactory.Create("{0}^8", (object) "name")));
      fieldNamesToQuery.Add(FormattableString.Invariant(FormattableStringFactory.Create("{0}^8", (object) "name.casechangeanalyzed")));
      fieldNamesToQuery.Add(FormattableString.Invariant(FormattableStringFactory.Create("{0}^1", (object) "description")));
      if (requestContext.ServiceHost.Is(TeamFoundationHostType.Application))
        fieldNamesToQuery.Add(FormattableString.Invariant(FormattableStringFactory.Create("{0}^3", (object) "version")));
      return fieldNamesToQuery;
    }

    private IList<string> GetPlatformFieldNamesToWildcardQuery(IVssRequestContext requestContext)
    {
      IList<string> namesToWildcardQuery = (IList<string>) new List<string>();
      namesToWildcardQuery.Add(FormattableString.Invariant(FormattableStringFactory.Create("{0}^8", (object) "name")));
      namesToWildcardQuery.Add(FormattableString.Invariant(FormattableStringFactory.Create("{0}^8", (object) "name.casechangeanalyzed")));
      namesToWildcardQuery.Add(FormattableString.Invariant(FormattableStringFactory.Create("{0}^1", (object) "description")));
      if (requestContext.ServiceHost.Is(TeamFoundationHostType.Application))
        namesToWildcardQuery.Add(FormattableString.Invariant(FormattableStringFactory.Create("{0}^3", (object) "version")));
      return namesToWildcardQuery;
    }

    private string GetFullTextMultiWildcardQueryString(
      TermExpression termExpression,
      IList<string> fieldsToQuery)
    {
      return FormattableString.Invariant(FormattableStringFactory.Create("{{\r\n                    \"query_string\": {{\r\n                        \"query\": {0},\r\n                        \"fields\": [\r\n                            {1}\r\n                        ],\"rewrite\":\"top_terms_boost_100\"\r\n                    }}\r\n                }}", (object) JsonConvert.SerializeObject((object) termExpression.Value), (object) string.Join(",", fieldsToQuery.Select<string, string>((Func<string, string>) (f => FormattableString.Invariant(FormattableStringFactory.Create("\"{0}\"", (object) f)))))));
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
