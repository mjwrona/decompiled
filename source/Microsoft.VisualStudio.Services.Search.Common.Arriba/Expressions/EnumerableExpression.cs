// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Common.Arriba.Expressions.EnumerableExpression
// Assembly: Microsoft.VisualStudio.Services.Search.Common.Arriba, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 29FBF982-8D5A-44EA-8073-2D46D60ABF28
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Common.Arriba.dll

using System;
using System.Collections;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Search.Common.Arriba.Expressions
{
  public abstract class EnumerableExpression : IExpression, IEnumerable<IExpression>, IEnumerable
  {
    public virtual IExpression[] Children
    {
      get => EmptyExpression.EmptyEnumerable;
      set => throw new NotSupportedException("Setting this property is not supported");
    }

    public IEnumerator<IExpression> GetEnumerator()
    {
      EnumerableExpression enumerableExpression = this;
      yield return (IExpression) enumerableExpression;
      IExpression[] expressionArray = enumerableExpression.Children;
      for (int index = 0; index < expressionArray.Length; ++index)
      {
        IEnumerator<IExpression> childEnum = expressionArray[index].GetEnumerator();
        while (childEnum.MoveNext())
          yield return childEnum.Current;
        childEnum = (IEnumerator<IExpression>) null;
      }
      expressionArray = (IExpression[]) null;
    }

    IEnumerator IEnumerable.GetEnumerator() => (IEnumerator) this.GetEnumerator();

    public override bool Equals(object obj) => obj is EnumerableExpression enumerableExpression && this.ToString().Equals(enumerableExpression.ToString(), StringComparison.Ordinal);

    public override int GetHashCode() => base.GetHashCode();
  }
}
