// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.ElasticSearch.QueryBuilders.Expression.TermExpressionQueryBuilder
// Assembly: Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EE1DF96-C85D-457F-AAA1-93619829BFD4
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Search.Common.Arriba.Expressions;
using Microsoft.VisualStudio.Services.Search.Common.Entities;
using Microsoft.VisualStudio.Services.Search.Common.Enums;
using Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions;
using Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.Entities;
using System;
using System.Runtime.CompilerServices;

namespace Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.ElasticSearch.QueryBuilders.Expression
{
  internal class TermExpressionQueryBuilder : IPlatformQueryBuilder
  {
    public string Build(
      IVssRequestContext requestContext,
      IExpression expression,
      IEntityType entityType,
      DocumentContractType contractType,
      bool enableRanking,
      bool allowSpellingErrors,
      string requestId,
      ResultsCountPlatformRequest request)
    {
      string name = entityType.Name;
      if (name != null)
      {
        switch (name.Length)
        {
          case 4:
            switch (name[0])
            {
              case 'C':
                if (name == "Code")
                  return new CodeTermExpressionQueryBuilder().Build(requestContext, expression, contractType, enableRanking, requestId, request);
                break;
              case 'W':
                if (name == "Wiki")
                  return new WikiTermExpressionQueryBuilder().Build(requestContext, expression, request);
                break;
            }
            break;
          case 5:
            if (name == "Board")
              return new BoardTermExpressionQueryBuilder().Build(expression);
            break;
          case 7:
            switch (name[0])
            {
              case 'P':
                if (name == "Package")
                  return new PackageTermExpressionQueryBuilder().Build(requestContext, expression);
                break;
              case 'S':
                if (name == "Setting")
                  return new SettingTermExpressionQueryBuilder().Build(expression);
                break;
            }
            break;
          case 8:
            if (name == "WorkItem")
            {
              WorkItemTermExpressionQueryBuilder expressionQueryBuilder = new WorkItemTermExpressionQueryBuilder();
              if (allowSpellingErrors)
                throw new NotSupportedException(FormattableString.Invariant(FormattableStringFactory.Create("Spelling errors are not supported for work item.")));
              IVssRequestContext requestContext1 = requestContext;
              IExpression expression1 = expression;
              return expressionQueryBuilder.Build(requestContext1, expression1);
            }
            break;
          case 11:
            if (name == "ProjectRepo")
            {
              switch (contractType)
              {
                case DocumentContractType.ProjectContract:
                  return new ProjectTermExpressionQueryBuilder().Build(requestContext, expression);
                case DocumentContractType.RepositoryContract:
                  return (contractType != request.ContractType ? (RepositoryTermExpressionQueryBuilderBase) new RepositoryChildContractTermExpressionQueryBuilder() : (RepositoryTermExpressionQueryBuilderBase) new RepositoryTermExpressionQueryBuilder()).Build(requestContext, expression);
                default:
                  throw new NotSupportedException(FormattableString.Invariant(FormattableStringFactory.Create("ContractType [{0}] is not supported.", (object) contractType)));
              }
            }
            else
              break;
        }
      }
      throw new NotSupportedException(FormattableString.Invariant(FormattableStringFactory.Create("EntityType [{0}] is not supported.", (object) entityType.Name)));
    }
  }
}
