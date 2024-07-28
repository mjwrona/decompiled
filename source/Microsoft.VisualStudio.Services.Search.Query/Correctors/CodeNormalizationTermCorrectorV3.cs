// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Query.Correctors.CodeNormalizationTermCorrectorV3
// Assembly: Microsoft.VisualStudio.Services.Search.Query, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 71E00698-03D3-4C67-B313-A550333DA80C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Query.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Search.Common.Arriba.Expressions;
using Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.Entities.Code;
using System;

namespace Microsoft.VisualStudio.Services.Search.Query.Correctors
{
  internal class CodeNormalizationTermCorrectorV3 : CodeNormalizationTermCorrectorV2
  {
    [StaticSafe]
    internal static readonly NoPayloadContractUtils noPayloadContractUtils = new NoPayloadContractUtils();

    public override IExpression CorrectTerm(
      IVssRequestContext requestContext,
      TermExpression termExpression)
    {
      if (termExpression == null)
        throw new ArgumentNullException(nameof (termExpression));
      if (!termExpression.IsOfType(CodeFileContract.CodeContractQueryableElement.Default))
        termExpression.Type = termExpression.Type.NormalizeString();
      termExpression.Value = !termExpression.IsOfType(CodeFileContract.CodeContractQueryableElement.FilePath) ? (!termExpression.IsOfType(CodeFileContract.CodeContractQueryableElement.BranchName) ? (CodeSearchFilters.CEFilterNameToIdMap.ContainsKey(termExpression.Type) || termExpression.IsOfType(CodeFileContract.CodeContractQueryableElement.Default) && !termExpression.Value.ContainsWhitespaceOrSpecialCharacters() && !termExpression.Value.Contains("?") && !termExpression.Value.Contains("*") ? termExpression.Value.NormalizeStringAndReplaceTurkishDottedI() : termExpression.Value.NormalizeString()) : termExpression.Value.NormalizePath()) : CodeNormalizationTermCorrectorV3.noPayloadContractUtils.CorrectFilePathFilter(termExpression.Value);
      return (IExpression) termExpression;
    }
  }
}
