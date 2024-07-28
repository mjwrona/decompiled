// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.Relevance.Rules.RelevanceRule
// Assembly: Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EE1DF96-C85D-457F-AAA1-93619829BFD4
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.dll

using Microsoft.VisualStudio.Services.Search.Common.Arriba.Expressions;
using Microsoft.VisualStudio.Services.Search.Common.Arriba.Expressions.Relevance;
using System;

namespace Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.Relevance.Rules
{
  public class RelevanceRule
  {
    public string Description { get; set; }

    public Target Target { get; set; }

    public Func<IExpression, RelevanceRule, IRelevanceExpression> Initializer { get; set; } = (Func<IExpression, RelevanceRule, IRelevanceExpression>) ((expression, rule) => (IRelevanceExpression) null);

    public virtual IRelevanceExpression Evaluate(IExpression expression) => this.Initializer(expression, this);
  }
}
