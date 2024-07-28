// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Notifications.Server.Condition
// Assembly: Microsoft.VisualStudio.Services.Notifications.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8ED1F52D-E567-4EFA-AAED-F2D9262BE9C2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Notifications.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.CodeDom.Compiler;

namespace Microsoft.VisualStudio.Services.Notifications.Server
{
  [Serializable]
  public abstract class Condition
  {
    public const string Area = "Notifications";
    public const string Layer = "Condition";

    public abstract bool Evaluate(
      IVssRequestContext requestContext,
      EvaluationContext evaluationContext);

    public abstract void Write(IndentedTextWriter writer);

    public virtual bool EvaluateAllConditions(
      IVssRequestContext requestContext,
      EvaluationContext evaluationContext)
    {
      return this.Evaluate(requestContext, evaluationContext);
    }
  }
}
