// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Query.Correctors.CodeFilterAliasCorrector
// Assembly: Microsoft.VisualStudio.Services.Search.Query, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 71E00698-03D3-4C67-B313-A550333DA80C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Query.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Search.Common.Arriba.Correctors;
using Microsoft.VisualStudio.Services.Search.Common.Arriba.Expressions;
using Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.Entities.Code;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Search.Query.Correctors
{
  internal class CodeFilterAliasCorrector : TermCorrector
  {
    [StaticSafe]
    private static IReadOnlyDictionary<string, string> s_filterAliasMap = (IReadOnlyDictionary<string, string>) new Dictionary<string, string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase)
    {
      {
        "caller",
        "ref"
      },
      {
        "method",
        "func"
      },
      {
        "filename",
        CodeFileContract.CodeContractQueryableElement.FileName.InlineFilterName()
      },
      {
        "function",
        "func"
      },
      {
        "methoddecl",
        "funcdecl"
      },
      {
        "Method Declaration",
        "funcdecl"
      },
      {
        "methoddef",
        "funcdef"
      },
      {
        "Method Definition",
        "funcdef"
      },
      {
        "string",
        "strlit"
      },
      {
        "Type Definition",
        "typedef"
      }
    };

    public override IExpression CorrectTerm(
      IVssRequestContext requestContext,
      TermExpression termExpression)
    {
      if (termExpression == null)
        throw new ArgumentNullException(nameof (termExpression));
      if (termExpression.IsOfType("*"))
        return (IExpression) termExpression;
      TermExpression termExpression1 = termExpression;
      string str;
      if (CodeFilterAliasCorrector.s_filterAliasMap.TryGetValue(termExpression.Type, out str))
        termExpression1.Type = str;
      else if (CodeSearchFilters.CEFilterNameToIdMap.TryGetValue(termExpression.Type, out str))
        termExpression1.Type = str;
      return (IExpression) termExpression1;
    }
  }
}
