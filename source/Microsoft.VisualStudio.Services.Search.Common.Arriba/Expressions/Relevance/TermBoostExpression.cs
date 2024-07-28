// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Common.Arriba.Expressions.Relevance.TermBoostExpression
// Assembly: Microsoft.VisualStudio.Services.Search.Common.Arriba, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 29FBF982-8D5A-44EA-8073-2D46D60ABF28
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Common.Arriba.dll

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Microsoft.VisualStudio.Services.Search.Common.Arriba.Expressions.Relevance
{
  public class TermBoostExpression : IRelevanceExpression
  {
    public List<TermBoostExpression.TermExpression> TermsDescriptor { get; set; }

    public class TermExpression
    {
      private double m_boost;

      public string FieldName { get; set; }

      public List<string> Terms { get; set; }

      public double Boost
      {
        get => this.m_boost;
        set => this.m_boost = value >= 0.0 && value <= 10.0 ? value : throw new ArgumentException(FormattableString.Invariant(FormattableStringFactory.Create("Boost can not be set to {0}. Boost should be between 0 - 10", (object) value)));
      }
    }
  }
}
