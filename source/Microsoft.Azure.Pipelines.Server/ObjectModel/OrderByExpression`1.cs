// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Pipelines.Server.ObjectModel.OrderByExpression`1
// Assembly: Microsoft.Azure.Pipelines.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: DC20940E-746B-4985-9936-F8ACD7ADA1DB
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Pipelines.Server.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Microsoft.Azure.Pipelines.Server.ObjectModel
{
  public abstract class OrderByExpression<T> : 
    IReadOnlyList<SortExpression<T>>,
    IReadOnlyCollection<SortExpression<T>>,
    IEnumerable<SortExpression<T>>,
    IEnumerable
    where T : struct, Enum
  {
    private IReadOnlyList<SortExpression<T>> m_expressions;

    protected OrderByExpression(IReadOnlyList<SortExpression<T>> sortExpressions)
    {
      ArgumentUtility.CheckForNull<IReadOnlyList<SortExpression<T>>>(sortExpressions, nameof (sortExpressions));
      this.m_expressions = sortExpressions;
    }

    public SortExpression<T> this[int index] => this.m_expressions[index];

    public int Count => this.m_expressions.Count;

    public IEnumerator<SortExpression<T>> GetEnumerator() => this.m_expressions.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => (IEnumerator) this.m_expressions.GetEnumerator();
  }
}
