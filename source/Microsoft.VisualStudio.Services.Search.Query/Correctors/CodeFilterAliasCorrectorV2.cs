// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Query.Correctors.CodeFilterAliasCorrectorV2
// Assembly: Microsoft.VisualStudio.Services.Search.Query, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 71E00698-03D3-4C67-B313-A550333DA80C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Query.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Search.Common.Arriba.Expressions;
using Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.Entities.Code;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Search.Query.Correctors
{
  internal class CodeFilterAliasCorrectorV2 : CodeFilterAliasCorrector
  {
    [StaticSafe]
    private static IReadOnlyDictionary<string, List<string>> s_filterAliasMap = (IReadOnlyDictionary<string, List<string>>) new Dictionary<string, List<string>>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase)
    {
      {
        "caller",
        new List<string>() { "ref" }
      },
      {
        "method",
        new List<string>() { "func" }
      },
      {
        "filename",
        new List<string>()
        {
          CodeFileContract.CodeContractQueryableElement.FileName.InlineFilterName()
        }
      },
      {
        "function",
        new List<string>() { "func" }
      },
      {
        "funcdecl",
        new List<string>() { "func" }
      },
      {
        "funcdef",
        new List<string>() { "func" }
      },
      {
        "methoddecl",
        new List<string>() { "func" }
      },
      {
        "Method Declaration",
        new List<string>() { "func" }
      },
      {
        "methoddef",
        new List<string>() { "func" }
      },
      {
        "Method Definition",
        new List<string>() { "func" }
      },
      {
        "string",
        new List<string>() { "strlit" }
      },
      {
        "Type Definition",
        new List<string>() { "type" }
      },
      {
        "arg",
        new List<string>() { "decl" }
      },
      {
        "classdecl",
        new List<string>() { "class" }
      },
      {
        "classdef",
        new List<string>() { "class" }
      },
      {
        "ctor",
        new List<string>() { "func" }
      },
      {
        "dtor",
        new List<string>() { "func" }
      },
      {
        "extern",
        new List<string>() { "decl" }
      },
      {
        "friend",
        new List<string>() { "ref" }
      },
      {
        "global",
        new List<string>() { "decl", "def" }
      },
      {
        "header",
        new List<string>() { "ref" }
      },
      {
        "macrodef",
        new List<string>() { "macro" }
      },
      {
        "macroref",
        new List<string>() { "macro" }
      },
      {
        "prop",
        new List<string>() { "field" }
      },
      {
        "struct",
        new List<string>() { "type" }
      },
      {
        "structdecl",
        new List<string>() { "type" }
      },
      {
        "structdef",
        new List<string>() { "type" }
      },
      {
        "tmplarg",
        new List<string>() { "decl" }
      },
      {
        "tmplspec",
        new List<string>() { "decl" }
      },
      {
        "typedef",
        new List<string>() { "type" }
      },
      {
        "union",
        new List<string>() { "type" }
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
      IExpression expression = (IExpression) termExpression;
      TermExpression termExpression1 = termExpression;
      List<IExpression> expressionList = new List<IExpression>();
      string str;
      if (CodeSearchFilters.CEFilterNameToIdMap.TryGetValue(termExpression.Type, out str))
        termExpression1.Type = str;
      List<string> stringList;
      if (this.GetFilterAliasMap().TryGetValue(termExpression1.Type, out stringList))
      {
        foreach (string type in stringList)
          expressionList.Add((IExpression) new TermExpression(type, termExpression1.Operator, termExpression1.Value));
        expression = (IExpression) new OrExpression(expressionList.ToArray());
      }
      return expression;
    }

    protected IReadOnlyDictionary<string, List<string>> GetFilterAliasMap() => CodeFilterAliasCorrectorV2.s_filterAliasMap;
  }
}
