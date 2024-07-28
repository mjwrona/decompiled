// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Notifications.Server.StringFieldCondition
// Assembly: Microsoft.VisualStudio.Services.Notifications.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8ED1F52D-E567-4EFA-AAED-F2D9262BE9C2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Notifications.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Notifications.WebApi;
using Newtonsoft.Json;
using System;
using System.Xml.XPath;

namespace Microsoft.VisualStudio.Services.Notifications.Server
{
  public class StringFieldCondition : FieldCondition
  {
    public StringFieldCondition(Token fieldName, byte op, Token target)
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
      int result1 = 0;
      int result2 = 0;
      int num = fieldValue == null || this.Target?.Spelling == null || !int.TryParse(fieldValue.ToString(), out result1) ? 0 : (int.TryParse(this.Target.Spelling, out result2) ? 1 : 0);
      ++evaluationContext.EvaluationsCount;
      if (num != 0)
      {
        try
        {
          return NumericCondition.CompareNumeric(evaluationContext, this.FieldName, result1, result2, this.m_operation);
        }
        catch (InvalidOperationException ex)
        {
        }
      }
      if (!(fieldValue is string expectedValue))
        throw new InvalidFieldValueException(CoreRes.EventConditionFieldNotFound((object) this.FieldName));
      string str1 = this.Target.EvaluateToken(evaluationContext.MacrosLookup);
      object obj = (object) null;
      if (evaluationContext.Event != null && this.Target.IsXPathExpression)
      {
        if (this.IsPossiblyValidXPathTarget(str1))
        {
          try
          {
            object fieldValue1 = evaluationContext.Event.GetFieldContainer().GetFieldValue(str1);
            obj = (object) this.GetFieldResult(fieldValue1);
            if (fieldValue1 is string str2)
              str1 = str2;
          }
          catch (JsonException ex)
          {
          }
          catch (XPathException ex)
          {
          }
        }
      }
      bool result3;
      if (obj is IdentityListFieldResult identityListFieldResult)
      {
        bool flag = false;
        bool arrayStopCondition = this.GetArrayStopCondition();
        foreach (object identity in identityListFieldResult.Identities)
        {
          if (ConditionEvalHelper.EvaluateStringCondition(requestContext, evaluationContext, expectedValue, identity as string, this.m_operation) == arrayStopCondition)
            flag = true;
        }
        result3 = flag ? arrayStopCondition : !arrayStopCondition;
        identityListFieldResult.TraceEvaluationString(evaluationContext, result3, this.m_operation, (object) expectedValue);
      }
      else
      {
        result3 = ConditionEvalHelper.EvaluateStringCondition(requestContext, evaluationContext, expectedValue, str1, this.m_operation);
        evaluationContext.Tracer.EvaluationTraceClause(result3, "'{0}' {1} {2} '{3}'", (object) this.FieldName, (object) expectedValue, (object) Token.GetOperatorString(this.m_operation), (object) str1, evaluationContext, nameof (EvaluateOneValue));
      }
      return result3;
    }

    protected override bool EvaluateArrayResult(
      IVssRequestContext requestContext,
      EvaluationContext evaluationContext,
      ArrayFieldResult arrFieldResult)
    {
      switch (this.m_operation)
      {
        case 12:
        case 13:
          bool evalResult;
          if (arrFieldResult.BinarySearch(evaluationContext, this.m_operation, this.Target.EvaluateToken(evaluationContext.MacrosLookup), out evalResult))
            return evalResult;
          break;
        case 26:
          return arrFieldResult.UnderPath(this.Target.EvaluateToken(evaluationContext.MacrosLookup));
      }
      return base.EvaluateArrayResult(requestContext, evaluationContext, arrFieldResult);
    }

    private bool IsPossiblyValidXPathTarget(string target) => !string.IsNullOrEmpty(target) && target[0] != '\'' && target[0] != '"';

    public override string GetOperandString()
    {
      string str = "'";
      if (this.Target.Spelling.Contains(str))
        str = "\"";
      return str + this.Target?.ToString() + str;
    }

    public override bool Equals(object obj) => obj is StringFieldCondition stringFieldCondition && this.FieldName == stringFieldCondition.FieldName && this.Target == stringFieldCondition.Target && (int) this.m_operation == (int) stringFieldCondition.Operation;

    public override int GetHashCode() => this.FieldName.GetHashCode() ^ this.Target.GetHashCode() ^ this.m_operation.GetHashCode();

    public Token Target { get; set; }
  }
}
