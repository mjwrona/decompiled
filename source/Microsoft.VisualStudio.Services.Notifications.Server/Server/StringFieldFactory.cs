// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Notifications.Server.StringFieldFactory
// Assembly: Microsoft.VisualStudio.Services.Notifications.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8ED1F52D-E567-4EFA-AAED-F2D9262BE9C2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Notifications.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Notifications.WebApi;
using System;
using System.Diagnostics;

namespace Microsoft.VisualStudio.Services.Notifications.Server
{
  public static class StringFieldFactory
  {
    public static Condition GetCondition(
      IVssRequestContext requestContext,
      Subscription subscription,
      StringFieldMode stringFieldMode,
      EventSerializerType serializerType,
      Token fieldName,
      byte op,
      Token target)
    {
      Condition condition = (Condition) null;
      if (stringFieldMode != StringFieldMode.Legacy)
      {
        ISubscriptionAdapter defaultAdapter = requestContext == null || subscription == null ? (ISubscriptionAdapter) null : subscription.GetDefaultAdapter(requestContext, false);
        if (defaultAdapter != null)
        {
          Condition optimizedCondition = (Condition) null;
          try
          {
            optimizedCondition = defaultAdapter.GetOptimizedCondition(fieldName, op, target);
          }
          catch (Exception ex)
          {
            requestContext.TraceException(1002014, "Notifications", "Condition", ex);
          }
          if (optimizedCondition != null)
          {
            if (optimizedCondition is StringFieldCondition stringFieldCondition)
              requestContext.Trace(1002014, TraceLevel.Info, "Notifications", "Condition", "Legacy {0} {1} {2} Optimized {3} {4} {5}", (object) fieldName.Spelling, (object) Token.GetOperatorString(op), (object) target.Spelling, (object) stringFieldCondition.FieldName.Spelling, (object) Token.GetOperatorString(stringFieldCondition.Operation), (object) stringFieldCondition.Target.Spelling);
            else
              requestContext.TraceSerializedConditionally(1002014, TraceLevel.Info, "Notifications", "Condition", "Legacy {0} {1} {2} Optimized {3}", (object) fieldName.Spelling, (object) Token.GetOperatorString(op), (object) target.Spelling, (object) optimizedCondition.ToString());
            switch (stringFieldMode)
            {
              case StringFieldMode.Optimized:
                condition = optimizedCondition;
                break;
              case StringFieldMode.Dual:
                condition = (Condition) new DualStringFieldCondition(new StringFieldCondition(fieldName, op, target), optimizedCondition);
                break;
            }
          }
        }
      }
      return condition ?? (Condition) new StringFieldCondition(fieldName, op, target);
    }
  }
}
