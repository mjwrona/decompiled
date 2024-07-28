// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.ElasticSearch.QueryBuilders.Expression.ProjectTermExpressionQueryBuilder
// Assembly: Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EE1DF96-C85D-457F-AAA1-93619829BFD4
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Common.Arriba;
using Microsoft.VisualStudio.Services.Search.Common.Arriba.Expressions;
using Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Implementations.ElasticSearch.QueryBuilders;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.ElasticSearch.QueryBuilders.Expression
{
  internal sealed class ProjectTermExpressionQueryBuilder : ProjectRepoExpressionQueryBuilderBase
  {
    public string Build(IVssRequestContext requestcontext, IExpression expression)
    {
      TermExpression termExpression = expression as TermExpression;
      switch (termExpression.Operator)
      {
        case Operator.Equals:
          return this.CreateTermQueryWithFilterString(termExpression);
        case Operator.Matches:
          return this.GetMultiMatchQueryString(requestcontext, termExpression);
        default:
          throw new NotSupportedException(FormattableString.Invariant(FormattableStringFactory.Create("Operator [{0}] is not supported in project search.", (object) termExpression.Operator)));
      }
    }

    protected override string GetFullTextMultiWildcardQueryString(
      IVssRequestContext requestcontext,
      TermExpression termExpression)
    {
      double configValueOrDefault = requestcontext.GetConfigValueOrDefault("/Service/OrgSearch/Settings/ProjectRepoTieBreakerFractionValue", 0.0);
      IList<string> fieldNamesToQuery = this.GetPlatformFieldNamesToQuery(requestcontext);
      return this.GetFullTextMultiWildcardQueryString(termExpression, fieldNamesToQuery, configValueOrDefault);
    }

    protected override string GetFullTextMultiMatchQueryString(
      IVssRequestContext requestcontext,
      TermExpression termExpression)
    {
      double configValueOrDefault = requestcontext.GetConfigValueOrDefault("/Service/OrgSearch/Settings/ProjectRepoTieBreakerFractionValue", 0.0);
      IList<string> fieldNamesToQuery = this.GetPlatformFieldNamesToQuery(requestcontext);
      return this.GetFullTextMultiMatchQueryString(termExpression, fieldNamesToQuery, configValueOrDefault);
    }

    protected override IList<string> GetPlatformFieldNamesToQuery(IVssRequestContext requestContext) => ProjectRepoQueryFields.ConvertToRawString(ProjectRepoQueryFields.GetProjectQueryFieldsBoostValueMap(requestContext));
  }
}
