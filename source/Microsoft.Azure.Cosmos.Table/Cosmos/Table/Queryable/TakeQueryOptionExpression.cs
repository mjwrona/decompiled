// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Table.Queryable.TakeQueryOptionExpression
// Assembly: Microsoft.Azure.Cosmos.Table, Version=1.0.7.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 461D0B3A-0B96-4D42-B330-3A8E714FC39A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Table.dll

using System;
using System.Diagnostics;
using System.Linq.Expressions;

namespace Microsoft.Azure.Cosmos.Table.Queryable
{
  [DebuggerDisplay("TakeQueryOptionExpression {TakeAmount}")]
  internal class TakeQueryOptionExpression : QueryOptionExpression
  {
    private ConstantExpression takeAmount;

    internal TakeQueryOptionExpression(Type type, ConstantExpression takeAmount)
      : base((ExpressionType) 10003, type)
    {
      this.takeAmount = takeAmount;
    }

    internal ConstantExpression TakeAmount => this.takeAmount;

    internal override QueryOptionExpression ComposeMultipleSpecification(
      QueryOptionExpression previous)
    {
      return (int) this.takeAmount.Value >= (int) ((TakeQueryOptionExpression) previous).takeAmount.Value ? previous : (QueryOptionExpression) this;
    }
  }
}
