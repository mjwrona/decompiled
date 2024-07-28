// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.ElasticSearch.QueryBuilders.Expression.ProjectRepoExpressionQueryBuilderBase
// Assembly: Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EE1DF96-C85D-457F-AAA1-93619829BFD4
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Search.Common.Arriba.Expressions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.ElasticSearch.QueryBuilders.Expression
{
  internal abstract class ProjectRepoExpressionQueryBuilderBase
  {
    protected string GetMultiMatchQueryString(
      IVssRequestContext requestcontext,
      TermExpression termExpression)
    {
      return termExpression.Value.Contains("*") || termExpression.Value.Contains("?") ? (termExpression.IsOfType("*") ? this.GetFullTextMultiWildcardQueryString(requestcontext, termExpression) : this.GetFilteredMultiWildcardQueryString(termExpression)) : (termExpression.IsOfType("*") ? this.GetFullTextMultiMatchQueryString(requestcontext, termExpression) : this.GetFilteredMultiMatchQueryString(termExpression));
    }

    protected abstract string GetFullTextMultiWildcardQueryString(
      IVssRequestContext requestContext,
      TermExpression termExpression);

    protected abstract string GetFullTextMultiMatchQueryString(
      IVssRequestContext requestContext,
      TermExpression termExpression);

    protected string GetFullTextMultiWildcardQueryString(
      TermExpression termExpression,
      IList<string> fieldsToQuery,
      double tieBreakerFractionValue = 0.0)
    {
      return FormattableString.Invariant(FormattableStringFactory.Create("{{\r\n                    \"query_string\": {{\r\n                        \"query\": {0},\r\n                        \"fields\": [\r\n                            {1}\r\n                        ],\r\n                        \"rewrite\":\"top_terms_boost_100\",\r\n                        \"{2}\": {3}\r\n                    }}\r\n                }}", (object) JsonConvert.SerializeObject((object) termExpression.Value), (object) string.Join(",", fieldsToQuery.Select<string, string>((Func<string, string>) (f => FormattableString.Invariant(FormattableStringFactory.Create("\"{0}\"", (object) f))))), (object) "tie_breaker", (object) tieBreakerFractionValue.ToString((IFormatProvider) CultureInfo.InvariantCulture)));
    }

    protected string GetFullTextMultiMatchQueryString(
      TermExpression termExpression,
      IList<string> fieldsToQuery,
      double tieBreakerFractionValue = 0.0)
    {
      return FormattableString.Invariant(FormattableStringFactory.Create("{{")) + FormattableString.Invariant(FormattableStringFactory.Create("  \"{0}\": {{", (object) "multi_match")) + FormattableString.Invariant(FormattableStringFactory.Create("      \"query\": {0},", (object) JsonConvert.SerializeObject((object) termExpression.Value))) + FormattableString.Invariant(FormattableStringFactory.Create("      \"type\": \"{0}\",", (object) "phrase")) + FormattableString.Invariant(FormattableStringFactory.Create("      \"fields\": [{0}],", (object) string.Join(",", fieldsToQuery.Select<string, string>((Func<string, string>) (f => FormattableString.Invariant(FormattableStringFactory.Create("\"{0}\"", (object) f))))))) + FormattableString.Invariant(FormattableStringFactory.Create("      \"{0}\": {1}", (object) "tie_breaker", (object) tieBreakerFractionValue.ToString((IFormatProvider) CultureInfo.InvariantCulture))) + FormattableString.Invariant(FormattableStringFactory.Create("      }}")) + FormattableString.Invariant(FormattableStringFactory.Create("}}")).ToLowerInvariant();
    }

    protected string CreateTermQueryWithFilterString(TermExpression termExpression) => string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{{\r\n                        \"{0}\": {{\r\n                            \"{1}\": {2}\r\n                        }}\r\n                    }}", (object) "term", (object) termExpression.Type, (object) JsonConvert.SerializeObject((object) termExpression.Value));

    protected string GetFilteredMultiMatchQueryString(TermExpression termExpression)
    {
      string type = termExpression.Type;
      return FormattableString.Invariant(FormattableStringFactory.Create("{{\r\n                    \"multi_match\": {{\r\n                        \"query\": {0},\r\n                        \"fields\": [\r\n                            \"{1}\"\r\n                        ],\r\n                        \"type\": \"phrase\"\r\n                    }}\r\n                }}", (object) JsonConvert.SerializeObject((object) termExpression.Value), (object) type));
    }

    protected string GetFilteredMultiWildcardQueryString(TermExpression termExpression)
    {
      string type = termExpression.Type;
      return FormattableString.Invariant(FormattableStringFactory.Create("{{\r\n                    \"multi_wildcard\": {{\r\n                        \"value\": {0},\r\n                        \"fields\": [\r\n                            \"{1}\"\r\n                        ],\r\n                        \"rewrite\":\"top_terms_boost_100\"\r\n                    }}\r\n                }}", (object) JsonConvert.SerializeObject((object) termExpression.Value), (object) type));
    }

    protected virtual IList<string> GetPlatformFieldNamesToQuery(IVssRequestContext requestContext) => throw new NotImplementedException(FormattableString.Invariant(FormattableStringFactory.Create("Implement in the child class {0}", (object) this.GetType())));
  }
}
