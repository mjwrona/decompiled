// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Notifications.Server.NumericCondition
// Assembly: Microsoft.VisualStudio.Services.Notifications.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8ED1F52D-E567-4EFA-AAED-F2D9262BE9C2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Notifications.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Notifications.WebApi;
using System;

namespace Microsoft.VisualStudio.Services.Notifications.Server
{
  public class NumericCondition : FieldCondition
  {
    public NumericCondition(Token fieldName, byte op, int target)
    {
      this.FieldName = fieldName;
      this.m_operation = op;
      this.Target = target;
    }

    public override bool EvaluateOneValue(
      IVssRequestContext requestContext,
      EvaluationContext evaluationContext,
      object fieldValue)
    {
      ++evaluationContext.EvaluationsCount;
      int result = -1;
      int target = this.Target;
      if (!int.TryParse(fieldValue.ToString(), out result))
        throw new InvalidFieldValueException(CoreRes.EventConditionFieldNotFound((object) this.FieldName));
      return NumericCondition.CompareNumeric(evaluationContext, this.FieldName, result, target, this.m_operation);
    }

    public static bool CompareNumeric(
      EvaluationContext evaluationContext,
      Token fieldName,
      int val,
      int target,
      byte operation)
    {
      bool result;
      switch (operation)
      {
        case 8:
          result = val < target;
          break;
        case 9:
          result = val > target;
          break;
        case 10:
          result = val <= target;
          break;
        case 11:
          result = val >= target;
          break;
        case 12:
          result = val == target;
          break;
        case 13:
          result = val != target;
          break;
        default:
          throw new InvalidOperationException(string.Format("'{0}' '{1}' {2} '{3}' ({4})", (object) fieldName, (object) val, (object) Token.GetOperatorString(operation), (object) target, (object) evaluationContext?.Subscription?.ID));
      }
      evaluationContext.Tracer.EvaluationTraceClause(result, "'{0}' '{1}' {2} '{3}'", (object) fieldName, (object) val, (object) Token.GetOperatorString(operation), (object) target, evaluationContext, nameof (CompareNumeric));
      return result;
    }

    public override string GetOperandString() => this.Target.ToString();

    public override bool Equals(object obj) => obj is NumericCondition numericCondition && this.FieldName == numericCondition.FieldName && this.Target == numericCondition.Target && (int) this.m_operation == (int) numericCondition.Operation;

    public override int GetHashCode() => this.FieldName.GetHashCode() ^ this.Target.GetHashCode() ^ this.m_operation.GetHashCode();

    public int Target { get; set; }
  }
}
