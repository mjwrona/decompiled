// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.Linq.ExpressionSimplifier`1
// Assembly: Microsoft.Azure.DocumentDB.Core, Version=2.10.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 20BB0EB7-1465-494C-8F4E-F898448B85D9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DocumentDB.Core.dll

using System;
using System.Linq.Expressions;

namespace Microsoft.Azure.Documents.Linq
{
  internal sealed class ExpressionSimplifier<T> : ExpressionSimplifier
  {
    public override object EvalBoxed(Expression expr) => (object) this.Eval(expr);

    public T Eval(Expression expr) => Expression.Lambda<Func<T>>(expr).Compile()();
  }
}
