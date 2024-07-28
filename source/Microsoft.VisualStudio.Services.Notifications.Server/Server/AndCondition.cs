// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Notifications.Server.AndCondition
// Assembly: Microsoft.VisualStudio.Services.Notifications.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8ED1F52D-E567-4EFA-AAED-F2D9262BE9C2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Notifications.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.CodeDom.Compiler;

namespace Microsoft.VisualStudio.Services.Notifications.Server
{
  [Serializable]
  public class AndCondition : Condition
  {
    public bool hasParens;

    public Condition condition1 { get; set; }

    public Condition condition2 { get; set; }

    public AndCondition()
    {
    }

    public AndCondition(Condition condition1, Condition condition2)
    {
      this.condition1 = condition1;
      this.condition2 = condition2;
    }

    public override bool Evaluate(
      IVssRequestContext requestContext,
      EvaluationContext evaluationContext)
    {
      return this.condition1.Evaluate(requestContext, evaluationContext) && this.condition2.Evaluate(requestContext, evaluationContext);
    }

    public override bool EvaluateAllConditions(
      IVssRequestContext requestContext,
      EvaluationContext evaluationContext)
    {
      return this.condition1.Evaluate(requestContext, evaluationContext) & this.condition2.Evaluate(requestContext, evaluationContext);
    }

    public override void Write(IndentedTextWriter iw)
    {
      iw.WriteLine("AND");
      ++iw.Indent;
      this.condition1.Write(iw);
      this.condition2.Write(iw);
      --iw.Indent;
    }

    public override string ToString()
    {
      if (!this.hasParens)
        return this.condition1.ToString() + " AND " + this.condition2.ToString();
      return "(" + this.condition1.ToString() + " AND " + this.condition2.ToString() + ")";
    }

    public override bool Equals(object obj) => obj is AndCondition andCondition && this.condition1.Equals((object) andCondition.condition1) && this.condition2.Equals((object) andCondition.condition2);

    public override int GetHashCode() => this.condition1.GetHashCode() ^ this.condition2.GetHashCode();
  }
}
