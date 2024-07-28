// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Query.CodeSearchQueryTagger
// Assembly: Microsoft.VisualStudio.Services.Search.Query, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 71E00698-03D3-4C67-B313-A550333DA80C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Query.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Common.Arriba;
using Microsoft.VisualStudio.Services.Search.Common.Arriba.Expressions;
using Microsoft.VisualStudio.Services.Search.Common.Entities;
using Microsoft.VisualStudio.Services.Search.Common.Enums;
using Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.Entities.Code;
using Microsoft.VisualStudio.Services.Search.WebApi.Legacy;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Search.Query
{
  internal class CodeSearchQueryTagger
  {
    private int m_numDefaultCodeTokens;
    private int m_numCodeElementTokens;
    private int m_pathFilterCount;
    private int m_fileExtFilterCount;
    private int m_fileNameFilterCount;
    private int m_projFilterCount;
    private int m_repoFilterCount;
    private int m_branchFilterCount;
    private int m_specialCharTermCount;
    private int m_phraseCount;
    private int m_prefixWildcardTermCount;
    private int m_infixSuffixWildcardTermCount;
    private int m_questionMarkWildcard;
    private int m_mixedWildcardCount;
    private int m_substringTooShort;
    private int m_prefixInfixSuffixWildcardCount;
    private int m_prefixWildcardCodeElementTermCount;
    private int m_prefixSuffixWildcardCodeElementTermCount;
    private int m_prefixInfixSuffixWildcardCodeElementTermCount;
    private int m_substringCodeElementTermCount;
    private int m_infixSuffixWildcardCodeElementTermCount;
    private int m_substringSupportedTermCount;
    private int m_substringTermCount;
    private int m_nearTermCount;
    private int m_beforeTermCount;
    private int m_afterTermCount;
    private readonly IExpression m_expression;
    private readonly IDictionary<string, IEnumerable<string>> m_searchFacets;
    private IVssRequestContext m_requestContext;
    private Dictionary<string, int> m_numPerCodeElementToken;

    public CodeSearchQueryTagger(
      IExpression expression,
      IDictionary<string, IEnumerable<string>> searchFacets,
      IVssRequestContext requestContext)
    {
      this.m_expression = expression;
      this.m_searchFacets = searchFacets;
      this.m_requestContext = requestContext;
      this.m_numPerCodeElementToken = new Dictionary<string, int>();
      foreach (string ceFilterId in (IEnumerable<string>) CodeSearchFilters.CEFilterIds)
        this.m_numPerCodeElementToken.Add(ceFilterId, 0);
    }

    public CodeSearchTag Tags { get; private set; }

    public void Compute()
    {
      this.TagFacets(this.m_searchFacets);
      this.TagExpression(this.m_expression);
      this.TagForCodeTokenCount();
    }

    public void Publish()
    {
      CustomerIntelligenceData properties = new CustomerIntelligenceData();
      properties.Add("SearchRequestTags", this.Tags.ToString());
      properties.Add("NumberOfPathFilters", (double) this.m_pathFilterCount);
      properties.Add("NumberOfFileExtFilters", (double) this.m_fileExtFilterCount);
      properties.Add("NumberOfFileNameFilters", (double) this.m_fileNameFilterCount);
      properties.Add("NumberOfProjFilters", (double) this.m_projFilterCount);
      properties.Add("NumberOfRepoFilters", (double) this.m_repoFilterCount);
      properties.Add("NumberOfBranchFilters", (double) this.m_branchFilterCount);
      properties.Add("TotalNumberOfDefaultCodeTerms", (double) this.m_numDefaultCodeTokens);
      properties.Add("NumberOfSpecialCharTerms", (double) this.m_specialCharTermCount);
      properties.Add("NumberOfPhrases", (double) this.m_phraseCount);
      properties.Add("NumberOfPrefixWildcardTerms", (double) this.m_prefixWildcardTermCount);
      properties.Add("NumberOfInfixSuffixWildcardTerms", (double) this.m_infixSuffixWildcardTermCount);
      properties.Add("NumberOfSupportedSubStringTerms", (double) this.m_substringSupportedTermCount);
      properties.Add("NumberOfSubStringTerms", (double) this.m_substringTermCount);
      properties.Add("NumberOfAlphaNumericTerms", (double) (this.m_numDefaultCodeTokens - (this.m_specialCharTermCount + this.m_phraseCount + this.m_prefixWildcardTermCount + this.m_infixSuffixWildcardTermCount)));
      properties.Add("NumberOfQuestionMarkWildcards", (double) this.m_questionMarkWildcard);
      properties.Add("NumberOfMixedWildcards", (double) this.m_mixedWildcardCount);
      properties.Add("NumberOfSubstringTooShort", (double) this.m_substringTooShort);
      properties.Add("NumberOfPrefixInfixSuffixWildcard", (double) this.m_prefixInfixSuffixWildcardCount);
      properties.Add("TotalNumberOfCodeElementTokens", (double) this.m_numCodeElementTokens);
      properties.Add("DistributionOfCodeElementFilters", this.m_numPerCodeElementToken.Serialize<Dictionary<string, int>>());
      properties.Add("NumberOfPrefixWildcardCodeElementTerms", (double) this.m_prefixWildcardCodeElementTermCount);
      properties.Add("NumberOfPrefixSuffixWildcardCodeElementTerms", (double) this.m_prefixSuffixWildcardCodeElementTermCount);
      properties.Add("NumberOfPrefixInfixSuffixWildcardCodeElementTerms", (double) this.m_prefixInfixSuffixWildcardCodeElementTermCount);
      properties.Add("NumberOfInfixSuffixWildcardCodeElementTerms", (double) this.m_infixSuffixWildcardCodeElementTermCount);
      properties.Add("NumberOfSubstringSearchCodeElementTerms", (double) this.m_substringCodeElementTermCount);
      properties.Add("NumberOfNonWildcardCodeElementTerms", (double) (this.m_numCodeElementTokens - (this.m_prefixWildcardCodeElementTermCount + this.m_infixSuffixWildcardCodeElementTermCount + this.m_prefixSuffixWildcardCodeElementTermCount)));
      properties.Add("NumberOfTermsWithNearOperator", (double) this.m_nearTermCount);
      properties.Add("NumberOfTermsWithBeforeOperator", (double) this.m_beforeTermCount);
      properties.Add("NumberOfTermsWithAfterOperator", (double) this.m_afterTermCount);
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.PublishCi("Query Pipeline", "Query Pipeline", properties);
    }

    private void TagFacets(
      IDictionary<string, IEnumerable<string>> searchFacets)
    {
      int num = 0;
      if (searchFacets.ContainsKey("AccountFilters"))
      {
        this.Tags |= CodeSearchTag.AccountFacet;
        ++num;
      }
      if (searchFacets.ContainsKey("CollectionFilters"))
      {
        this.Tags |= CodeSearchTag.CollectionFacet;
        ++num;
      }
      if (searchFacets.ContainsKey("ProjectFilters"))
      {
        this.Tags |= CodeSearchTag.ProjFacet;
        ++num;
      }
      if (searchFacets.ContainsKey("RepositoryFilters"))
      {
        this.Tags |= CodeSearchTag.RepoFacet;
        ++num;
      }
      if (searchFacets.ContainsKey("CodeElementFilters"))
      {
        this.Tags |= CodeSearchTag.CEFacet;
        ++num;
      }
      if (searchFacets.ContainsKey("BranchFilters"))
      {
        this.Tags |= CodeSearchTag.BranchFacet;
        ++num;
        foreach (string str in searchFacets["BranchFilters"])
        {
          switch (str)
          {
            case "#Default#":
              this.Tags |= CodeSearchTag.DefaultBranch;
              continue;
            case "#All#":
              this.Tags |= CodeSearchTag.AllBranch;
              continue;
            default:
              continue;
          }
        }
      }
      if (searchFacets.ContainsKey("PathFilters"))
      {
        this.Tags |= CodeSearchTag.PathFacet;
        ++num;
      }
      if (searchFacets.Count == num)
        return;
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceError(1081332, "Query Pipeline", "Query", "[" + string.Join(", ", (IEnumerable<string>) searchFacets.Keys) + "] contains unrecognized search facet category(ies) observed in SearchQueryTagger class.");
    }

    private void TagExpression(IExpression expression)
    {
      foreach (IExpression expression1 in (IEnumerable<IExpression>) expression)
      {
        switch (expression1)
        {
          case TermExpression termExpression:
            this.TagTermExpression(termExpression);
            continue;
          case AndExpression _:
            this.Tags |= CodeSearchTag.And;
            continue;
          case OrExpression _:
            this.Tags |= CodeSearchTag.Or;
            continue;
          case NotExpression _:
            this.Tags |= CodeSearchTag.Not;
            continue;
          case EmptyExpression _:
            this.Tags |= CodeSearchTag.None;
            continue;
          default:
            Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceError(1081330, "Query Pipeline", "Query", "IExpression type [" + expression1.GetType()?.ToString() + "] is not handled in SearchQueryTagger class. Please add support for the same.");
            continue;
        }
      }
    }

    private void TagTermExpression(TermExpression termExpression)
    {
      bool flag = false;
      int asteriskWildcardCharacterIndex = termExpression.Value.IndexOf('*');
      if (asteriskWildcardCharacterIndex >= 0)
        this.Tags |= CodeSearchTag.WildcardAsterisk;
      int questionWildcardCharacterIndex = termExpression.Value.IndexOf('?');
      if (questionWildcardCharacterIndex >= 0)
        this.Tags |= CodeSearchTag.WildcardQuestion;
      if (termExpression.IsOfType(CodeFileContract.CodeContractQueryableElement.Default))
      {
        flag = true;
        this.TagDefaultTermExpression(termExpression, asteriskWildcardCharacterIndex, questionWildcardCharacterIndex);
      }
      else if (termExpression.IsOfType(CodeFileContract.CodeContractQueryableElement.FilePath))
      {
        this.Tags |= CodeSearchTag.PathFilter;
        ++this.m_pathFilterCount;
      }
      else if (termExpression.IsOfType(CodeFileContract.CodeContractQueryableElement.FileExtension))
      {
        this.Tags |= CodeSearchTag.ExtFilter;
        ++this.m_fileExtFilterCount;
      }
      else if (termExpression.IsOfType(CodeFileContract.CodeContractQueryableElement.FileName))
      {
        this.Tags |= CodeSearchTag.FileFilter;
        ++this.m_fileNameFilterCount;
      }
      else if (CodeSearchFilters.CEFilterIds.Contains(termExpression.Type) || termExpression.Operator == Operator.Near || termExpression.Operator == Operator.After || termExpression.Operator == Operator.Before)
      {
        if (termExpression.Operator != Operator.Near && termExpression.Operator != Operator.After && termExpression.Operator != Operator.Before)
        {
          flag = true;
          this.Tags |= CodeSearchTag.CEFilter;
          this.m_numPerCodeElementToken[termExpression.Type]++;
          ++this.m_numCodeElementTokens;
          if (questionWildcardCharacterIndex >= 0 || asteriskWildcardCharacterIndex >= 0)
          {
            if (RegularExpressions.SupportedSubStringRegex.Match(termExpression.Value).Success)
            {
              ++this.m_substringCodeElementTermCount;
              this.Tags |= CodeSearchTag.CodeElementWildcardSubstring;
            }
            else if (RegularExpressions.PrefixWildcardRegex.Match(termExpression.Value).Success)
            {
              if (RegularExpressions.PrefixInfixSuffixWildcardRegex.Match(termExpression.Value).Success)
              {
                ++this.m_prefixInfixSuffixWildcardCodeElementTermCount;
                this.Tags |= CodeSearchTag.CodeElementSubStringWithInfixWildcard;
              }
              else if (RegularExpressions.PostfixWildcardRegex.Match(termExpression.Value).Success)
              {
                ++this.m_prefixSuffixWildcardCodeElementTermCount;
                this.Tags |= CodeSearchTag.CodeElementPrefixSuffixWildcardSubstring;
              }
              else
              {
                ++this.m_prefixWildcardCodeElementTermCount;
                this.Tags |= CodeSearchTag.CodeElementPrefixWildcard;
              }
            }
            else
            {
              ++this.m_infixSuffixWildcardCodeElementTermCount;
              this.Tags |= CodeSearchTag.CodeElementInfixSuffixWildcardTerm;
            }
            if (this.IsGramSizeTooSmall(termExpression.Value))
              this.Tags |= CodeSearchTag.CodeElementWildcardSubstringTooShort;
          }
        }
        else if (termExpression.Operator == Operator.Near)
        {
          this.Tags |= CodeSearchTag.CodeNearTerm;
          ++this.m_nearTermCount;
          this.CheckIfProximityQuerySupported(termExpression);
        }
        else if (termExpression.Operator == Operator.Before)
        {
          this.Tags |= CodeSearchTag.CodeBeforeTerm;
          ++this.m_beforeTermCount;
          this.CheckIfProximityQuerySupported(termExpression);
        }
        else if (termExpression.Operator == Operator.After)
        {
          this.Tags |= CodeSearchTag.CodeAfterTerm;
          ++this.m_afterTermCount;
          this.CheckIfProximityQuerySupported(termExpression);
        }
      }
      else if (termExpression.IsOfType(CodeFileContract.CodeContractQueryableElement.ProjectName))
      {
        this.Tags |= CodeSearchTag.ProjFilter;
        ++this.m_projFilterCount;
      }
      else if (termExpression.IsOfType(CodeFileContract.CodeContractQueryableElement.RepoName))
      {
        this.Tags |= CodeSearchTag.RepoFilter;
        ++this.m_repoFilterCount;
      }
      else if (termExpression.IsOfType(CodeFileContract.CodeContractQueryableElement.BranchName))
      {
        this.Tags |= CodeSearchTag.BranchFilter;
        ++this.m_branchFilterCount;
      }
      else
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceError(1081331, "Query Pipeline", "Query", "Code search filter [" + termExpression.Type + "] is not handled in SearchQueryTagger class. Please add support for the same.");
      if (!flag || asteriskWildcardCharacterIndex < 0 && questionWildcardCharacterIndex < 0)
        return;
      if (RegularExpressions.PostfixWildcardRegex.Match(termExpression.Value).Success)
        this.Tags |= CodeSearchTag.CodePostfixWildcard;
      if (asteriskWildcardCharacterIndex >= 0)
        this.Tags |= CodeSearchTag.CodeWildcardAsterisk;
      if (questionWildcardCharacterIndex < 0)
        return;
      this.Tags |= CodeSearchTag.CodeWildcardQuestion;
    }

    private void CheckIfProximityQuerySupported(TermExpression termExpression)
    {
      if (RegularExpressions.ProximityOperandRegex.Match(termExpression.Type).Success && RegularExpressions.ProximityOperandRegex.Match(termExpression.Value).Success)
        return;
      this.Tags |= CodeSearchTag.UnsupportedProximityTerm;
    }

    private void TagForCodeTokenCount()
    {
      int num = this.m_numDefaultCodeTokens + this.m_numCodeElementTokens;
      if (num == 1)
      {
        this.Tags |= CodeSearchTag.SingleWord;
      }
      else
      {
        if (num <= 1)
          return;
        this.Tags |= CodeSearchTag.MultiWord;
      }
    }

    private void TagDefaultTermExpression(
      TermExpression termExpression,
      int asteriskWildcardCharacterIndex,
      int questionWildcardCharacterIndex)
    {
      ++this.m_numDefaultCodeTokens;
      bool success = RegularExpressions.PrefixWildcardRegex.Match(termExpression.Value).Success;
      DocumentContractType documentContractType = new IndexMapper((IEntityType) CodeEntityType.GetInstance()).GetDocumentContractType(this.m_requestContext);
      if ((this.m_requestContext.IsFTSEnabled() || documentContractType.IsNoPayloadContract()) && RegularExpressions.SpecialCharRegexWithoutWildcardAndSpace.Match(termExpression.Value).Success)
      {
        if (termExpression.Value.StartsWith("\"", StringComparison.Ordinal) && termExpression.Value.EndsWith("\"", StringComparison.Ordinal) && termExpression.Value.Length > 1 && !RegularExpressions.SpecialCharRegexWithoutWildcardAndSpace.Match(termExpression.Value.Substring(1, termExpression.Value.Length - 2)).Success)
        {
          this.Tags |= CodeSearchTag.Phrase;
          ++this.m_phraseCount;
        }
        else
        {
          this.Tags |= CodeSearchTag.Phrase;
          ++this.m_specialCharTermCount;
        }
      }
      else if (termExpression.Value.ContainsWhitespace())
      {
        this.Tags |= CodeSearchTag.Phrase;
        ++this.m_phraseCount;
      }
      else if (success)
      {
        this.Tags |= CodeSearchTag.CodePrefixWildcard;
        ++this.m_prefixWildcardTermCount;
        if (RegularExpressions.PostfixWildcardRegex.Match(termExpression.Value).Success)
          this.Tags |= CodeSearchTag.CodePrefixSuffixWildcardTerm;
        if (RegularExpressions.PrefixInfixSuffixWildcardRegex.Match(termExpression.Value).Success)
        {
          this.Tags |= CodeSearchTag.CodeSubstringWithInfixWildcard;
          ++this.m_prefixInfixSuffixWildcardCount;
        }
        if (RegularExpressions.SubstringTooShortRegex.Match(termExpression.Value).Success)
        {
          this.Tags |= CodeSearchTag.CodeSubstringTooShort;
          ++this.m_substringTooShort;
        }
        if (RegularExpressions.QuestionMarkWildcardRegex.Match(termExpression.Value).Success)
        {
          this.Tags |= CodeSearchTag.CodeSubstringWithQuestionMarkWildcard;
          ++this.m_questionMarkWildcard;
        }
        if (RegularExpressions.MixedWildcardRegex.Match(termExpression.Value).Success)
        {
          this.Tags |= CodeSearchTag.CodeSubstringWithMixedWildcard;
          ++this.m_mixedWildcardCount;
        }
        if (RegularExpressions.SubStringRegex.Match(termExpression.Value).Success)
        {
          ++this.m_substringTermCount;
          if (RegularExpressions.SupportedSubStringRegex.Match(termExpression.Value).Success)
          {
            ++this.m_substringSupportedTermCount;
            this.Tags |= CodeSearchTag.CodeSubStringTerm;
          }
        }
      }
      else if (asteriskWildcardCharacterIndex >= 0 || questionWildcardCharacterIndex >= 0)
      {
        this.Tags |= CodeSearchTag.CodeInfixSuffixWildcardTerm;
        ++this.m_infixSuffixWildcardTermCount;
      }
      if (!RegularExpressions.WildcardRegex.IsMatch(termExpression.Value) || !this.IsGramSizeTooSmall(termExpression.Value))
        return;
      this.Tags |= CodeSearchTag.CodeWildcardSubstringTooShort;
    }

    private bool IsGramSizeTooSmall(string value)
    {
      value = value.Trim();
      if (value.ContainsWhitespace() || !this.m_requestContext.IsQueryingNGramsEnabled())
        return false;
      int currentHostConfigValue1 = this.m_requestContext.GetCurrentHostConfigValue<int>("/Service/ALMSearch/Settings/CodeMinEdgeGramSizeForQuery");
      int currentHostConfigValue2 = this.m_requestContext.GetCurrentHostConfigValue<int>("/Service/ALMSearch/Settings/CodeInlineGramSizeForQuery");
      string[] strArray = value.Split(new char[2]
      {
        '*',
        '?'
      }, StringSplitOptions.RemoveEmptyEntries);
      int num1 = 0;
      int num2 = strArray.Length - 1;
      if (value.StartsWith(strArray[0], StringComparison.Ordinal))
      {
        if (strArray[0].Length < currentHostConfigValue1)
          return true;
        ++num1;
      }
      if (value.EndsWith(strArray[strArray.Length - 1], StringComparison.Ordinal))
      {
        if (strArray[strArray.Length - 1].Length < currentHostConfigValue1)
          return true;
        --num2;
      }
      for (int index = num1; index <= num2; ++index)
      {
        if (strArray[index].Length < currentHostConfigValue2)
          return true;
      }
      return false;
    }
  }
}
