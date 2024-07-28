// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Notifications.Server.OrCondition
// Assembly: Microsoft.VisualStudio.Services.Notifications.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8ED1F52D-E567-4EFA-AAED-F2D9262BE9C2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Notifications.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.CodeDom.Compiler;

namespace Microsoft.VisualStudio.Services.Notifications.Server
{
  [Serializable]
  public class OrCondition : Condition
  {
    public bool m_hasParens;

    public OrCondition()
    {
    }

    public OrCondition(Condition condition1, Condition condition2)
    {
      this.Condition1 = condition1;
      this.Condition2 = condition2;
    }

    public override bool Evaluate(
      IVssRequestContext requestContext,
      EvaluationContext evaluationContext)
    {
      return this.Condition1.Evaluate(requestContext, evaluationContext) || this.Condition2.Evaluate(requestContext, evaluationContext);
    }

    public override bool EvaluateAllConditions(
      IVssRequestContext requestContext,
      EvaluationContext evaluationContext)
    {
      return this.Condition1.Evaluate(requestContext, evaluationContext) | this.Condition2.Evaluate(requestContext, evaluationContext);
    }

    public override void Write(IndentedTextWriter iw)
    {
      iw.WriteLine("OR");
      ++iw.Indent;
      this.Condition1.Write(iw);
      this.Condition2.Write(iw);
      --iw.Indent;
    }

    public override string ToString()
    {
      if (!this.m_hasParens)
        return this.Condition1.ToString() + " OR " + this.Condition2.ToString();
      return "(" + this.Condition1.ToString() + " OR " + this.Condition2.ToString() + ")";
    }

    public override bool Equals(object obj) => obj is OrCondition orCondition && this.Condition1.Equals((object) orCondition.Condition1) && this.Condition2.Equals((object) orCondition.Condition2);

    public override int GetHashCode() => this.Condition1.GetHashCode() ^ this.Condition2.GetHashCode();

    public Condition Condition1 { get; set; }

    public Condition Condition2 { get; set; }
  }
}
