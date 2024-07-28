// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Linq.Nominator
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Microsoft.Azure.Cosmos.Linq
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
