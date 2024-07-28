// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Query.Correctors.CodeNormalizationTermCorrector
// Assembly: Microsoft.VisualStudio.Services.Search.Query, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 71E00698-03D3-4C67-B313-A550333DA80C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Query.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Search.Common.Arriba.Correctors;
using Microsoft.VisualStudio.Services.Search.Common.Arriba.Expressions;
using Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.Entities.Code;
using System;

namespace Microsoft.VisualStudio.Services.Search.Query.Correctors
{
  internal class CodeNormalizationTermCorrector : TermCorrector
  {
    public override IExpression CorrectTerm(
      IVssRequestContext requestContext,
      TermExpression termExpression)
    {
      if (termExpression == null)
        throw new ArgumentNullException(nameof (termExpression));
      if (!termExpression.IsOfType(CodeFileContract.CodeContractQueryableElement.Default))
        termExpression.Type = termExpression.Type.NormalizeString();
      termExpression.Value = termExpression.IsOfType(CodeFileContract.CodeContractQueryableElement.FilePath) || termExpression.IsOfType(CodeFileContract.CodeContractQueryableElement.FileName) || termExpression.IsOfType(CodeFileContract.CodeContractQueryableElement.BranchName) ? termExpression.Value.NormalizePath() : (termExpression.IsOfType(CodeFileContract.CodeContractQueryableElement.Default) || CodeSearchFilters.CEFilterNameToIdMap.ContainsKey(termExpression.Type) ? termExpression.Value.NormalizeStringAndReplaceTurkishDottedI() : termExpression.Value.NormalizeString());
      return (IExpression) termExpression;
    }
  }
}
