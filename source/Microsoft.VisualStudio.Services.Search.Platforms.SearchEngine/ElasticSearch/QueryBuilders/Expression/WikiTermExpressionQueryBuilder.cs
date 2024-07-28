// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.ElasticSearch.QueryBuilders.Expression.WikiTermExpressionQueryBuilder
// Assembly: Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EE1DF96-C85D-457F-AAA1-93619829BFD4
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Common.Arriba;
using Microsoft.VisualStudio.Services.Search.Common.Arriba.Expressions;
using Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.Entities;
using Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.Entities.Wiki;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.ElasticSearch.QueryBuilders.Expression
{
  internal sealed class WikiTermExpressionQueryBuilder
  {
    public string Build(
      IVssRequestContext requestContext,
      IExpression expression,
      ResultsCountPlatformRequest request)
    {
      TermExpression termExpression = (TermExpression) expression;
      switch (termExpression.Operator)
      {
        case Operator.Equals:
          return this.GetFilteredTermQueryStringForEqualityOperators(termExpression);
        case Operator.Matches:
          WikiSearchPlatformRequest request1 = request as WikiSearchPlatformRequest;
          return this.GetMultiMatchQueryString(requestContext, termExpression, request1);
        default:
          throw new NotSupportedException(FormattableString.Invariant(FormattableStringFactory.Create("Operator [{0}] is not supported in wiki search.", (object) termExpression.Operator)));
      }
    }

    private string GetMultiMatchQueryString(
      IVssRequestContext requestContext,
      TermExpression termExpression,
      WikiSearchPlatformRequest request)
    {
      int num = termExpression.Value.Contains("*") ? 1 : (termExpression.Value.Contains("?") ? 1 : 0);
      IList<string> fieldsToQuery = request?.SearchFields != null ? (IList<string>) request.SearchFields.ToList<string>() : this.GetPlatformFieldNamesToQuery(requestContext);
      return num != 0 ? (termExpression.IsOfType("*") ? this.GetFullTextMultiWildcardQueryString(termExpression, fieldsToQuery) : this.GetFilteredMultiWildcardQueryString(termExpression)) : (termExpression.IsOfType("*") ? this.GetFullTextMultiMatchQueryString(termExpression, fieldsToQuery) : this.GetFilteredMultiMatchQueryString(termExpression));
    }

    private string GetFullTextMultiWildcardQueryString(
      TermExpression termExpression,
      IList<string> fieldsToQuery)
    {
      return FormattableString.Invariant(FormattableStringFactory.Create("{{\r\n                    \"multi_wildcard\": {{\r\n                        \"value\": {0},\r\n                        \"fields\": [\r\n                            {1}\r\n                        ],\"rewrite\":\"top_terms_boost_100\"\r\n                    }}\r\n                }}", (object) JsonConvert.SerializeObject((object) termExpression.Value), (object) string.Join(",", fieldsToQuery.Select<string, string>((Func<string, string>) (f => FormattableString.Invariant(FormattableStringFactory.Create("\"{0}\"", (object) f)))))));
    }

    private string GetFullTextMultiMatchQueryString(
      TermExpression termExpression,
      IList<string> fieldsToQuery)
    {
      return FormattableString.Invariant(FormattableStringFactory.Create("{{\r\n                    \"multi_match\": {{\r\n                        \"query\": {0},\r\n                        \"fields\": [\r\n                            {1}\r\n                        ],\r\n                        \"type\": \"phrase\"\r\n                    }}\r\n                }}", (object) JsonConvert.SerializeObject((object) termExpression.Value), (object) string.Join(",", fieldsToQuery.Select<string, string>((Func<string, string>) (f => FormattableString.Invariant(FormattableStringFactory.Create("\"{0}\"", (object) f)))))));
    }

    private string GetFilteredTermQueryStringForEqualityOperators(TermExpression termExpression)
    {
      if (termExpression.Operator != Operator.Equals)
        return string.Empty;
      return FormattableString.Invariant(FormattableStringFactory.Create("{{\r\n                            \"term\": {{\r\n                                \"{0}\": {1}\r\n                            }}\r\n                        }}", (object) termExpression.Type, (object) JsonConvert.SerializeObject((object) termExpression.Value)));
    }

    private string GetFilteredMultiMatchQueryString(TermExpression termExpression)
    {
      string type = termExpression.Type;
      return FormattableString.Invariant(FormattableStringFactory.Create("{{\r\n                    \"multi_match\": {{\r\n                        \"query\": {0},\r\n                        \"fields\": [\r\n                            \"{1}\"\r\n                        ],\r\n                        \"type\": \"phrase\"\r\n                    }}\r\n                }}", (object) JsonConvert.SerializeObject((object) termExpression.Value), (object) type));
    }

    private string GetFilteredMultiWildcardQueryString(TermExpression termExpression)
    {
      string type = termExpression.Type;
      return FormattableString.Invariant(FormattableStringFactory.Create("{{\r\n                    \"multi_wildcard\": {{\r\n                        \"value\": {0},\r\n                        \"fields\": [\r\n                            \"{1}\"\r\n                        ],\r\n                        \"rewrite\":\"top_terms_boost_100\"\r\n                    }}\r\n                }}", (object) JsonConvert.SerializeObject((object) termExpression.Value), (object) type));
    }

    private IList<string> GetPlatformFieldNamesToQuery(IVssRequestContext requestContext)
    {
      int configValueOrDefault1 = requestContext.GetConfigValueOrDefault("/service/ALMSearch/Settings/WikiDocumentTitleBoostValue", 10);
      int configValueOrDefault2 = requestContext.GetConfigValueOrDefault("/service/ALMSearch/Settings/WikiDocumentContentBoostValue", 1);
      double configValueOrDefault3 = requestContext.GetConfigValueOrDefault("/service/ALMSearch/Settings/WikiStemmedFieldBoostFractionValue", 0.9);
      return (IList<string>) new List<string>()
      {
        FormattableString.Invariant(FormattableStringFactory.Create("{0}^{1}", (object) "fileNames", (object) ((double) configValueOrDefault1 * configValueOrDefault3))),
        FormattableString.Invariant(FormattableStringFactory.Create("{0}.{1}^{2}", (object) "fileNames", (object) "unstemmed", (object) configValueOrDefault1)),
        FormattableString.Invariant(FormattableStringFactory.Create("{0}.{1}^{2}", (object) "fileNames", (object) "lower", (object) configValueOrDefault1)),
        FormattableString.Invariant(FormattableStringFactory.Create("{0}.{1}^{2}", (object) "fileNames", (object) "pattern", (object) configValueOrDefault1)),
        "tags",
        FormattableString.Invariant(FormattableStringFactory.Create("{0}^{1}", (object) "content", (object) ((double) configValueOrDefault2 * configValueOrDefault3))),
        FormattableString.Invariant(FormattableStringFactory.Create("{0}.{1}^{2}", (object) "content", (object) "unstemmed", (object) configValueOrDefault2)),
        FormattableString.Invariant(FormattableStringFactory.Create("{0}.{1}^{2}", (object) "content", (object) "pattern", (object) configValueOrDefault2)),
        "contentLinks"
      };
    }
  }
}
