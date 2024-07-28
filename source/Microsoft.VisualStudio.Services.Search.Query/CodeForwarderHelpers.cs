// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Query.CodeForwarderHelpers
// Assembly: Microsoft.VisualStudio.Services.Search.Query, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 71E00698-03D3-4C67-B313-A550333DA80C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Query.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Common.Arriba.Expressions;
using Microsoft.VisualStudio.Services.Search.Common.Enums;
using Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.Entities.Code;
using Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy;
using Microsoft.VisualStudio.Services.Search.WebApi.Legacy;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Search.Query
{
  public static class CodeForwarderHelpers
  {
    internal static void CheckIfCodeQuerySupported(
      IVssRequestContext requestContext,
      DocumentContractType contractType,
      IExpression correctedQueryParseTree,
      IDictionary<string, IEnumerable<string>> searchFilters,
      bool isWildcardOnlySearch,
      out List<ErrorData> errorList,
      out CodeSearchQueryTagger tagger)
    {
      tagger = new CodeSearchQueryTagger(correctedQueryParseTree, searchFilters, requestContext);
      tagger.Compute();
      tagger.Publish();
      errorList = new List<ErrorData>();
      bool flag1 = requestContext.GetCurrentHostConfigValue<bool>("/Service/ALMSearch/Settings/EnableCodePrefixWildcardSearch", true) && contractType.IsDedupeFileContract() || requestContext.IsQueryingNGramsEnabled();
      bool flag2 = requestContext.GetCurrentHostConfigValue<bool>("/Service/ALMSearch/Settings/EnableSubStringSearch", true) || requestContext.IsQueryingNGramsEnabled();
      int num = !requestContext.IsWildcardConstantScoreRewriteFeatureEnabled() ? 0 : (CodeFileContract.IsContractTypeSupportedByConstantScoreRewrite(contractType) ? 1 : 0);
      EmptyExpression emptyExpression = correctedQueryParseTree as EmptyExpression;
      if ((num == 0 || !flag1) && tagger.Tags.HasFlag((Enum) CodeSearchTag.CodeElementPrefixWildcard) || !flag1 && tagger.Tags.HasFlag((Enum) CodeSearchTag.CodePrefixWildcard) && !tagger.Tags.HasFlag((Enum) CodeSearchTag.CodePrefixSuffixWildcardTerm) || flag1 && tagger.Tags.HasFlag((Enum) CodeSearchTag.CodePrefixWildcard) && tagger.Tags.HasFlag((Enum) CodeSearchTag.CEFacet))
        errorList.Add(new ErrorData()
        {
          ErrorCode = "PrefixWildcardQueryNotSupported",
          ErrorType = ErrorType.Warning
        });
      else if (!requestContext.IsQueryingNGramsEnabled() & flag2 && !tagger.Tags.HasFlag((Enum) CodeSearchTag.CodeSubStringTerm) && tagger.Tags.HasFlag((Enum) CodeSearchTag.CodePrefixSuffixWildcardTerm))
      {
        if (!flag1)
          errorList.Add(new ErrorData()
          {
            ErrorCode = "PrefixWildcardQueryNotSupported",
            ErrorType = ErrorType.Warning
          });
        else if (tagger.Tags.HasFlag((Enum) CodeSearchTag.CodeSubstringTooShort))
          errorList.Add(new ErrorData()
          {
            ErrorCode = "PrefixSuffixSubStringTooShort",
            ErrorType = ErrorType.Warning
          });
        else if (tagger.Tags.HasFlag((Enum) CodeSearchTag.CodeSubstringWithInfixWildcard))
          errorList.Add(new ErrorData()
          {
            ErrorCode = "SubstringWithInfixWildcard",
            ErrorType = ErrorType.Warning
          });
        else if (tagger.Tags.HasFlag((Enum) CodeSearchTag.CodeSubstringWithQuestionMarkWildcard))
          errorList.Add(new ErrorData()
          {
            ErrorCode = "QuestionMarkWildcardSubstring",
            ErrorType = ErrorType.Warning
          });
        else if (tagger.Tags.HasFlag((Enum) CodeSearchTag.CodeSubstringWithMixedWildcard))
          errorList.Add(new ErrorData()
          {
            ErrorCode = "MixedWildcardSubstring",
            ErrorType = ErrorType.Warning
          });
      }
      else if (tagger.Tags.HasFlag((Enum) CodeSearchTag.CodePrefixSuffixWildcardTerm) && !flag2)
        errorList.Add(new ErrorData()
        {
          ErrorCode = "PrefixWildcardQueryNotSupported",
          ErrorType = ErrorType.Warning
        });
      else if (isWildcardOnlySearch && emptyExpression != null)
        errorList.Add(new ErrorData()
        {
          ErrorCode = "OnlyWildcardQueryNotSupported",
          ErrorType = ErrorType.Warning
        });
      else if (emptyExpression != null)
        errorList.Add(new ErrorData()
        {
          ErrorCode = "EmptyQueryNotSupported",
          ErrorType = ErrorType.Warning
        });
      else if (tagger.Tags.HasFlag((Enum) CodeSearchTag.CodeWildcardSubstringTooShort))
        errorList.Add(new ErrorData()
        {
          ErrorCode = "WildcardSubstringTooShort",
          ErrorType = ErrorType.Warning
        });
      else if (tagger.Tags.HasFlag((Enum) CodeSearchTag.CodeElementWildcardSubstring) || tagger.Tags.HasFlag((Enum) CodeSearchTag.CodeElementPrefixSuffixWildcardSubstring) || tagger.Tags.HasFlag((Enum) CodeSearchTag.CodeElementSubStringWithInfixWildcard))
      {
        if (!flag1)
          errorList.Add(new ErrorData()
          {
            ErrorCode = "PrefixWildcardQueryNotSupported",
            ErrorType = ErrorType.Warning
          });
        else if (tagger.Tags.HasFlag((Enum) CodeSearchTag.CodeElementSubStringWithInfixWildcard))
          errorList.Add(new ErrorData()
          {
            ErrorCode = "SubstringWithInfixWildcardCEFFacets",
            ErrorType = ErrorType.Warning
          });
        else
          errorList.Add(new ErrorData()
          {
            ErrorCode = "SubstringSearchCEFFacets",
            ErrorType = ErrorType.Warning
          });
      }
      if (tagger.Tags.HasFlag((Enum) CodeSearchTag.MultiWord) && tagger.Tags.HasFlag((Enum) CodeSearchTag.CEFacet))
        errorList.Add(new ErrorData()
        {
          ErrorCode = "MultiWordWithCodeFacetNotSupported",
          ErrorType = ErrorType.Warning
        });
      else if (tagger.Tags.HasFlag((Enum) CodeSearchTag.SingleWord) && tagger.Tags.HasFlag((Enum) CodeSearchTag.Phrase) && tagger.Tags.HasFlag((Enum) CodeSearchTag.CEFacet))
        errorList.Add(new ErrorData()
        {
          ErrorCode = "PhraseQueriesWithCEFacetsNotSupported",
          ErrorType = ErrorType.Warning
        });
      else if (tagger.Tags.HasFlag((Enum) CodeSearchTag.SingleWord) && tagger.Tags.HasFlag((Enum) CodeSearchTag.CodeInfixSuffixWildcardTerm) && tagger.Tags.HasFlag((Enum) CodeSearchTag.CEFacet))
      {
        errorList.Add(new ErrorData()
        {
          ErrorCode = "WildcardQueriesWithCEFacetsNotSupported",
          ErrorType = ErrorType.Warning
        });
      }
      else
      {
        if (!tagger.Tags.HasFlag((Enum) CodeSearchTag.UnsupportedProximityTerm))
          return;
        errorList.Add(new ErrorData()
        {
          ErrorCode = "UnsupportedProximitySearchTerm",
          ErrorType = ErrorType.Warning
        });
      }
    }

    internal static bool IsWildCardOnlySearch_Code(IExpression expression)
    {
      bool flag = false;
      if (!(expression is EmptyExpression))
      {
        foreach (IExpression expression1 in (IEnumerable<IExpression>) expression)
        {
          if (expression1 is TermExpression termExpression && !string.IsNullOrWhiteSpace(termExpression.Value))
          {
            if (!RegularExpressions.WildcardOnlyRegex.IsMatch(termExpression.Value))
              return false;
            flag = true;
          }
        }
        if (flag)
          return true;
      }
      return false;
    }
  }
}
