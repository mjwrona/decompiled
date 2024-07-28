// Decompiled with JetBrains decompiler
// Type: Nest.MemberInfoResolver
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace Nest
{
  public class MemberInfoResolver : ExpressionVisitor
  {
    public MemberInfoResolver(Expression expression) => this.Visit(expression);

    public IList<MemberInfo> Members { get; } = (IList<MemberInfo>) new List<MemberInfo>();

    protected override Expression VisitMember(MemberExpression expression)
    {
      this.Members.Add(expression.Member);
      return base.VisitMember(expression);
    }
  }
}
