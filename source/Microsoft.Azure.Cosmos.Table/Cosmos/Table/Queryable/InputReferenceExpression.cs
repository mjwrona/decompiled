// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Table.Queryable.InputReferenceExpression
// Assembly: Microsoft.Azure.Cosmos.Table, Version=1.0.7.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 461D0B3A-0B96-4D42-B330-3A8E714FC39A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Table.dll

using System.Diagnostics;
using System.Linq.Expressions;

namespace Microsoft.Azure.Cosmos.Table.Queryable
{
  [DebuggerDisplay("InputReferenceExpression -> {Type}")]
  internal sealed class InputReferenceExpression : Expression
  {
    private ResourceExpression target;

    internal InputReferenceExpression(ResourceExpression target)
      : base((ExpressionType) 10007, target.ResourceType)
    {
      this.target = target;
    }

    internal ResourceExpression Target => this.target;

    internal void OverrideTarget(ResourceSetExpression newTarget) => this.target = (ResourceExpression) newTarget;
  }
}
