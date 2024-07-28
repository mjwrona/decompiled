// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Common.Arriba.Expressions.AndExpression
// Assembly: Microsoft.VisualStudio.Services.Search.Common.Arriba, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 29FBF982-8D5A-44EA-8073-2D46D60ABF28
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Common.Arriba.dll

using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Search.Common.Arriba.Expressions
{
  public class AndExpression : RelevanceExpression
  {
    public AndExpression(params IExpression[] set) => this.Children = set;

    public AndExpression(IEnumerable<IExpression> set) => this.Children = set.ToArray<IExpression>();

    public override sealed IExpression[] Children { get; set; }

    public override string ToString() => "(" + string.Join<IExpression>(" AND ", (IEnumerable<IExpression>) this.Children) + ")";
  }
}
