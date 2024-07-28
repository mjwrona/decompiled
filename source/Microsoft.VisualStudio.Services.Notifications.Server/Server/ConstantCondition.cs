// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Notifications.Server.ConstantCondition
// Assembly: Microsoft.VisualStudio.Services.Notifications.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8ED1F52D-E567-4EFA-AAED-F2D9262BE9C2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Notifications.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Notifications.WebApi;
using System;

namespace Microsoft.VisualStudio.Services.Notifications.Server
{
  [Serializable]
  public class ConstantCondition : FieldCondition
  {
    private bool m_value;

    public ConstantCondition(Token fieldName, byte op, bool value)
    {
      this.FieldName = fieldName;
      this.m_operation = op;
      this.m_value = value;
    }

    public override bool Evaluate(
      IVssRequestContext requestContext,
      EvaluationContext evaluationContext)
    {
      return base.Evaluate(requestContext, evaluationContext);
    }

    public override bool EvaluateOneValue(
      IVssRequestContext requestContext,
      EvaluationContext evaluationContext,
      object fieldValue)
    {
      ++evaluationContext.EvaluationsCount;
      bool result1 = false;
      bool flag = this.m_value;
      if (!bool.TryParse(fieldValue.ToString(), out result1))
        throw new InvalidFieldValueException(CoreRes.EventConditionFieldNotFound((object) this.FieldName));
      bool result2;
      if (this.m_operation == (byte) 12)
      {
        result2 = result1 == flag;
      }
      else
      {
        if (this.m_operation != (byte) 13)
          throw new InvalidOperationException(string.Format("Operation {0} not valid for ConstantCondition", (object) this.m_operation));
        result2 = result1 != flag;
      }
      evaluationContext.Tracer.EvaluationTraceClause(result2, "'{0}' '{1}' {2} '{3}'", (object) this.FieldName, (object) result1, (object) this.GetOperandString(), (object) flag, evaluationContext, nameof (EvaluateOneValue));
      return result2;
    }

    public override bool Equals(object obj) => obj is ConstantCondition constantCondition && this.m_value == constantCondition.m_value;

    public override int GetHashCode() => this.m_value ? 1 : 0;

    public override string GetOperandString() => this.m_value.ToString();
  }
}
