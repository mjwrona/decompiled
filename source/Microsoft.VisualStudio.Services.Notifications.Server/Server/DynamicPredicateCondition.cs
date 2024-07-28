// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Notifications.Server.DynamicPredicateCondition
// Assembly: Microsoft.VisualStudio.Services.Notifications.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8ED1F52D-E567-4EFA-AAED-F2D9262BE9C2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Notifications.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Notifications.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Notifications.Server
{
  [Serializable]
  public class DynamicPredicateCondition : FieldCondition
  {
    private Token m_dynamicPredicateName;
    private IDynamicEventPredicate m_dynamicEventPredicate;

    public DynamicPredicateCondition(
      IReadOnlyList<IDynamicEventPredicate> dynamicEventPredicates,
      Token fieldName,
      Token dynamicPredicateName)
    {
      this.m_operation = (byte) 17;
      this.m_dynamicPredicateName = dynamicPredicateName;
      this.FieldName = fieldName;
      if (dynamicEventPredicates == null)
        return;
      this.m_dynamicEventPredicate = dynamicEventPredicates.FirstOrDefault<IDynamicEventPredicate>((Func<IDynamicEventPredicate, bool>) (x => x.DynamicPredicateName.Equals(dynamicPredicateName.Spelling, StringComparison.OrdinalIgnoreCase)));
    }

    public override bool EvaluateOneValue(
      IVssRequestContext requestContext,
      EvaluationContext evaluationContext,
      object fieldValue)
    {
      string str = fieldValue as string;
      bool result = false;
      ++evaluationContext.EvaluationsCount;
      if (this.m_dynamicEventPredicate == null)
        throw new DynamicEventPredicateException(CoreRes.EventConditionDynamicPredicateNotFound((object) this.m_dynamicPredicateName));
      try
      {
        if (str == null)
          throw new DynamicEventPredicateException(CoreRes.EventConditionFieldNotFound((object) this.FieldName));
        result = this.m_dynamicEventPredicate.Evaluate(requestContext, evaluationContext.Subscription, str);
        return result;
      }
      catch (Exception ex)
      {
        throw new DynamicEventPredicateException(CoreRes.EventConditionDynamicPredicateThrewException((object) this.m_dynamicPredicateName), ex);
      }
      finally
      {
        evaluationContext.Tracer.EvaluationTraceClause(result, "dyn({0}) '{1}' '{2}'", (object) this.FieldName, (object) this.GetOperandString(), (object) str, evaluationContext, nameof (EvaluateOneValue));
      }
    }

    public override string GetOperandString()
    {
      string str = "'";
      if (this.m_dynamicPredicateName.Spelling.Contains(str))
        str = "\"";
      return str + this.m_dynamicPredicateName.Spelling + str;
    }
  }
}
