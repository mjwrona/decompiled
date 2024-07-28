// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.ElasticSearch.QueryBuilders.Expression.CodeTermExpressionQueryBuilder
// Assembly: Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EE1DF96-C85D-457F-AAA1-93619829BFD4
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Common.Arriba;
using Microsoft.VisualStudio.Services.Search.Common.Arriba.Expressions;
using Microsoft.VisualStudio.Services.Search.Common.Enums;
using Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.Entities;
using Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.Entities.Code;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.ElasticSearch.QueryBuilders.Expression
{
  internal sealed class CodeTermExpressionQueryBuilder
  {
    [StaticSafe]
    private static readonly IDictionary<DocumentContractType, CodeFileContract> s_documentContractMapping = (IDictionary<DocumentContractType, CodeFileContract>) new FriendlyDictionary<DocumentContractType, CodeFileContract>()
    {
      [DocumentContractType.DedupeFileContractV3] = (CodeFileContract) new DedupeFileContractV3(),
      [DocumentContractType.DedupeFileContractV4] = (CodeFileContract) new DedupeFileContractV4(),
      [DocumentContractType.DedupeFileContractV5] = (CodeFileContract) new DedupeFileContractV5(),
      [DocumentContractType.SourceNoDedupeFileContractV3] = (CodeFileContract) new SourceNoDedupeFileContractV3(),
      [DocumentContractType.SourceNoDedupeFileContractV4] = (CodeFileContract) new SourceNoDedupeFileContractV4(),
      [DocumentContractType.SourceNoDedupeFileContractV5] = (CodeFileContract) new SourceNoDedupeFileContractV5()
    };
    private readonly IDictionary<DocumentContractType, CodeFileContract> m_documentContractMapping;

    public CodeTermExpressionQueryBuilder(
      IDictionary<DocumentContractType, CodeFileContract> documentContractMapping)
    {
      this.m_documentContractMapping = documentContractMapping;
    }

    public CodeTermExpressionQueryBuilder()
      : this(CodeTermExpressionQueryBuilder.s_documentContractMapping)
    {
    }

    public string Build(
      IVssRequestContext requestContext,
      IExpression expression,
      DocumentContractType contractType,
      bool enableRanking,
      string requestId,
      ResultsCountPlatformRequest request)
    {
      TermExpression termExpression1 = expression as TermExpression;
      CodeFileContract documentContract = request.DocumentContract as CodeFileContract;
      if (!this.m_documentContractMapping.TryGetValue(contractType, out documentContract))
        throw new SearchServiceException(FormattableString.Invariant(FormattableStringFactory.Create("Unhandled document contract [{0}] encountered.", (object) contractType.ToString())));
      if (termExpression1.Operator == Operator.Near)
        return documentContract.CreateSpanQueryNearString(termExpression1, requestContext.GetConfigValueOrDefault("/Service/ALMSearch/Settings/SlopValueForProximitySearch", 5));
      if (termExpression1.Operator == Operator.Before)
        return documentContract.CreateSpanQueryBeforeString(termExpression1, requestContext.GetConfigValueOrDefault("/Service/ALMSearch/Settings/SlopValueForProximitySearch", 5));
      if (termExpression1.Operator == Operator.After)
        return documentContract.CreateSpanQueryAfterString(termExpression1, requestContext.GetConfigValueOrDefault("/Service/ALMSearch/Settings/SlopValueForProximitySearch", 5));
      if (CodeSearchFilters.CEFilterIds.Contains(termExpression1.Type))
      {
        string rewriteMethod = CodeFileContract.GetRewriteMethod(requestContext, contractType);
        return documentContract.CreateCodeElementQueryString(termExpression1.Type, termExpression1.Value, CodeSearchFilters.CEFilterAttributeMap[termExpression1.Type].TokenIds, enableRanking, requestId, rewriteMethod);
      }
      if (documentContract.IsOriginalContentAvailable(requestContext))
      {
        if (termExpression1.IsOfType(CodeFileContract.CodeContractQueryableElement.Regex))
          return requestContext.GetConfigValue<bool>("/Service/ALMSearch/Settings/EnableTrigramSearch", TeamFoundationHostType.ProjectCollection) ? documentContract.CreateTermQueryStringForRegexTrigramType(requestContext, termExpression1, enableRanking, requestId) : documentContract.ConvertToRegexpQueryString(requestContext, termExpression1);
        if (termExpression1.IsOfType(CodeFileContract.CodeContractQueryableElement.Default) && termExpression1.Value.ContainsWhitespaceOrSpecialCharacters())
          return documentContract.ConvertToAdvancedPhraseQueryString(requestContext, termExpression1, enableRanking, requestId);
        if (termExpression1.IsOfType(CodeFileContract.CodeContractQueryableElement.Default) && termExpression1.Value.ContainsSubstring() && CodeTermExpressionQueryBuilder.IsSubstringSearchSupported(requestContext))
        {
          TermExpression termExpression2 = new TermExpression(termExpression1.Type, termExpression1.Operator, termExpression1.Value.Trim('*'));
          return documentContract.ConvertToTrigramPhraseQueryString(requestContext, termExpression2, enableRanking, requestId);
        }
      }
      else if (termExpression1.IsOfType(CodeFileContract.CodeContractQueryableElement.Default) && termExpression1.Value.ContainsWhitespace())
        return documentContract.ConvertToPhraseQueryString(termExpression1, enableRanking, requestId);
      if (termExpression1.IsOfType(CodeFileContract.CodeContractQueryableElement.IsDefaultBranch) || termExpression1.IsOfType(CodeFileContract.CodeContractQueryableElement.CollectionId))
        return documentContract.CreateTermQueryString(requestContext, termExpression1, false, requestId);
      if (termExpression1.IsOfType(CodeFileContract.CodeContractQueryableElement.FilePath))
        return documentContract.CreateFilePathQuery(termExpression1, requestId);
      return !termExpression1.IsOfType(CodeFileContract.CodeContractQueryableElement.Default) ? documentContract.CreateFilteredQueryString(termExpression1, requestId) : documentContract.CreateTermQueryStringForDefaultType(requestContext, termExpression1, enableRanking, requestId);
    }

    private static bool IsSubstringSearchSupported(IVssRequestContext requestContext)
    {
      bool flag = false;
      if (requestContext.GetCurrentHostConfigValue<bool>("/Service/ALMSearch/Settings/EnableABTesting") && requestContext.Items.ContainsKey("testQuery"))
        flag = true;
      return ((!requestContext.GetConfigValue<bool>("/Service/ALMSearch/Settings/EnableTrigramSearch", TeamFoundationHostType.ProjectCollection) ? 0 : (!requestContext.IsQueryingNGramsEnabled() ? 1 : 0)) | (flag ? 1 : 0)) != 0;
    }
  }
}
