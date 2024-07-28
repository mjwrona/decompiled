// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Query.Correctors.PackageInlineFieldCorrector
// Assembly: Microsoft.VisualStudio.Services.Search.Query, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 71E00698-03D3-4C67-B313-A550333DA80C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Query.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Search.Common.Arriba;
using Microsoft.VisualStudio.Services.Search.Common.Arriba.Correctors;
using Microsoft.VisualStudio.Services.Search.Common.Arriba.Expressions;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Search.Query.Correctors
{
  internal class PackageInlineFieldCorrector : TermCorrector
  {
    private static readonly IReadOnlyDictionary<string, string> s_inlineFieldMap = (IReadOnlyDictionary<string, string>) new Dictionary<string, string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase)
    {
      {
        "organization",
        "collectionName"
      },
      {
        "type",
        "protocol.lower"
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
      if (PackageInlineFieldCorrector.s_inlineFieldMap.TryGetValue(termExpression.Type, out str))
        termExpression1.Type = str;
      else
        termExpression1 = new TermExpression("*", Operator.Matches, termExpression.Type + ":" + termExpression.Value);
      return (IExpression) termExpression1;
    }
  }
}
