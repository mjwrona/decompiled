// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Linq.ExpressionSimplifier`1
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using System;
using System.Linq.Expressions;

namespace Microsoft.Azure.Cosmos.Linq
{
  internal sealed class ExpressionSimplifier<T> : ExpressionSimplifier
  {
    public override object EvalBoxed(Expression expr) => (object) this.Eval(expr);

    public T Eval(Expression expr) => Expression.Lambda<Func<T>>(expr).Compile()();
  }
}
