// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.Linq.Nominator
// Assembly: Microsoft.Azure.DocumentDB.Core, Version=2.10.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 20BB0EB7-1465-494C-8F4E-F898448B85D9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DocumentDB.Core.dll

using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Microsoft.Azure.Documents.Linq
{
  internal static class Nominator
  {
    public static HashSet<Expression> Nominate(
      Expression expression,
      Func<Expression, bool> fnCanBeEvaluated)
    {
      return new Nominator.NominatorVisitor(fnCanBeEvaluated).Nominate(expression);
    }

    private sealed class NominatorVisitor : ExpressionVisitor
    {
      private readonly Func<Expression, bool> fnCanBeEvaluated;
      private HashSet<Expression> candidates;
      private bool canBeEvaluated;

      public NominatorVisitor(Func<Expression, bool> fnCanBeEvaluated) => this.fnCanBeEvaluated = fnCanBeEvaluated;

      public HashSet<Expression> Nominate(Expression expression)
      {
        this.candidates = new HashSet<Expression>();
        this.Visit(expression);
        return this.candidates;
      }

      public override Expression Visit(Expression expression)
      {
        if (expression != null)
        {
          bool canBeEvaluated = this.canBeEvaluated;
          this.canBeEvaluated = true;
          base.Visit(expression);
          if (this.canBeEvaluated)
          {
            this.canBeEvaluated = this.fnCanBeEvaluated(expression);
            if (this.canBeEvaluated)
              this.candidates.Add(expression);
          }
          this.canBeEvaluated &= canBeEvaluated;
        }
        return expression;
      }
    }
  }
}
