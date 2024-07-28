// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Notifications.Server.FieldCondition
// Assembly: Microsoft.VisualStudio.Services.Notifications.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8ED1F52D-E567-4EFA-AAED-F2D9262BE9C2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Notifications.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Notifications.WebApi;
using System;
using System.CodeDom.Compiler;

namespace Microsoft.VisualStudio.Services.Notifications.Server
{
  [Serializable]
  public abstract class FieldCondition : Condition
  {
    protected byte m_operation;

    public override void Write(IndentedTextWriter writer) => writer.WriteLine("{0} {1}", (object) this.FieldName, (object) Token.spellings[(int) this.m_operation]);

    public override string ToString()
    {
      string str = "\"";
      if (this.FieldName.Spelling.Contains(str))
        str = "'";
      return str + this.FieldName.Spelling + str + " " + Token.spellings[(int) this.m_operation] + " " + this.GetOperandString();
    }

    public FieldResult GetFieldResult(object fieldResult) => FieldResult.GetFieldResult(fieldResult);

    public override bool Evaluate(
      IVssRequestContext requestContext,
      EvaluationContext evaluationContext)
    {
      string token = this.FieldName.EvaluateToken(evaluationContext.MacrosLookup);
      object fieldValue = evaluationContext.Event.GetFieldContainer().GetFieldValue(token);
      object fieldResult = (object) this.GetFieldResult(fieldValue);
      evaluationContext.Tracer.EvaluationTraceNoisy("FieldCondition.Evaluate: EvaluatedFieldName = {0}", (object) token, evaluationContext.Subscription, nameof (Evaluate));
      ++evaluationContext.EvaluationsCount;
      return fieldValue != null && this.EvaluateResult(requestContext, evaluationContext, fieldResult);
    }

    protected virtual bool EvaluateArrayResult(
      IVssRequestContext requestContext,
      EvaluationContext evaluationContext,
      ArrayFieldResult arrFieldResult)
    {
      bool arrayStopCondition = this.GetArrayStopCondition();
      foreach (object result in arrFieldResult.Results)
      {
        if (this.EvaluateOneValue(requestContext, evaluationContext, result) == arrayStopCondition)
          return arrayStopCondition;
      }
      return !arrayStopCondition;
    }

    private bool EvaluateResult(
      IVssRequestContext requestContext,
      EvaluationContext evaluationContext,
      object FieldResult)
    {
      switch (FieldResult)
      {
        case ArrayFieldResult arrFieldResult:
          return this.EvaluateArrayResult(requestContext, evaluationContext, arrFieldResult);
        case IdentityListFieldResult _:
          IdentityListFieldResult identityListFieldResult = FieldResult as IdentityListFieldResult;
          bool arrayStopCondition = this.GetArrayStopCondition();
          foreach (object identity in identityListFieldResult.Identities)
          {
            if (this.EvaluateOneValue(requestContext, evaluationContext, identity) == arrayStopCondition)
              return arrayStopCondition;
          }
          return !arrayStopCondition;
        case ConstantFieldResult _:
          ConstantFieldResult constantFieldResult = FieldResult as ConstantFieldResult;
          return this.EvaluateOneValue(requestContext, evaluationContext, constantFieldResult.Result);
        default:
          return this.EvaluateOneValue(requestContext, evaluationContext, FieldResult);
      }
    }

    protected bool GetArrayStopCondition()
    {
      switch (this.m_operation)
      {
        case 13:
        case 25:
        case 28:
          return false;
        default:
          return true;
      }
    }

    public abstract bool EvaluateOneValue(
      IVssRequestContext requestContext,
      EvaluationContext evaluationContext,
      object val);

    public abstract string GetOperandString();

    public Token FieldName { get; protected internal set; }

    public void SetFieldNameExternal(Token fieldName) => this.FieldName = fieldName;

    public byte Operation => this.m_operation;
  }
}
