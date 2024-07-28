// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.Relevance.Rules.FieldBoostRule
// Assembly: Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EE1DF96-C85D-457F-AAA1-93619829BFD4
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.dll

using Microsoft.VisualStudio.Services.Search.Common.Arriba.Expressions;
using Microsoft.VisualStudio.Services.Search.Common.Arriba.Expressions.Relevance;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.Relevance.Rules
{
  public class FieldBoostRule : RelevanceRule
  {
    public IDictionary<string, double> FieldsBoost { get; set; }

    public override IRelevanceExpression Evaluate(IExpression expression)
    {
      IRelevanceExpression relevanceExpression = base.Evaluate(expression);
      if (relevanceExpression != null)
        return relevanceExpression;
      return (IRelevanceExpression) new FieldBoostExpression()
      {
        FieldsBoost = this.FieldsBoost
      };
    }
  }
}
