// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Notifications.Server.NotCondition
// Assembly: Microsoft.VisualStudio.Services.Notifications.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8ED1F52D-E567-4EFA-AAED-F2D9262BE9C2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Notifications.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.CodeDom.Compiler;

namespace Microsoft.VisualStudio.Services.Notifications.Server
{
  [Serializable]
  public class NotCondition : Condition
  {
    public Condition condition;

    public NotCondition(Condition condition) => this.condition = condition;

    public override bool Evaluate(
      IVssRequestContext requestContext,
      EvaluationContext evaluationContext)
    {
      return !this.condition.Evaluate(requestContext, evaluationContext);
    }

    public override void Write(IndentedTextWriter iw)
    {
      iw.WriteLine("NOT");
      ++iw.Indent;
      this.condition.Write(iw);
      --iw.Indent;
    }

    public override string ToString() => "NOT " + this.condition.ToString();

    public override bool Equals(object obj) => obj is NotCondition notCondition && this.condition == notCondition.condition;

    public override int GetHashCode() => this.condition.GetHashCode();
  }
}
