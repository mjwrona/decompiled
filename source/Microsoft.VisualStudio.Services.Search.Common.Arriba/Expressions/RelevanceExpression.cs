// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Common.Arriba.Expressions.RelevanceExpression
// Assembly: Microsoft.VisualStudio.Services.Search.Common.Arriba, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 29FBF982-8D5A-44EA-8073-2D46D60ABF28
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Common.Arriba.dll

using Microsoft.VisualStudio.Services.Search.Common.Arriba.Expressions.Relevance;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Search.Common.Arriba.Expressions
{
  public class RelevanceExpression : EnumerableExpression
  {
    private List<IRelevanceExpression> m_relevanceExpressions;
    private List<IRelevanceExpression> m_childDocumentRelevanceExpressions;

    public RelevanceExpression()
    {
      this.m_relevanceExpressions = new List<IRelevanceExpression>();
      this.m_childDocumentRelevanceExpressions = new List<IRelevanceExpression>();
    }

    public void AddRelevanceExpression(IRelevanceExpression rule)
    {
      if (rule == null)
        return;
      this.m_relevanceExpressions.Add(rule);
    }

    public void AddChildDocumentRelevanceExpression(IRelevanceExpression rule)
    {
      if (rule == null)
        return;
      this.m_childDocumentRelevanceExpressions.Add(rule);
    }

    public bool IsLeafNode() => this.Children.Length == 0;

    public IEnumerable<IRelevanceExpression> GetRelevanceExpressions() => (IEnumerable<IRelevanceExpression>) this.m_relevanceExpressions;

    public IEnumerable<IRelevanceExpression> GetChildDocumentRelevanceExpressions() => (IEnumerable<IRelevanceExpression>) this.m_childDocumentRelevanceExpressions;

    public bool HasRelevanceExpressions() => this.m_relevanceExpressions.Count > 0;

    public bool HasChildDocumentRelevanceExpressions() => this.m_childDocumentRelevanceExpressions.Count > 0;
  }
}
