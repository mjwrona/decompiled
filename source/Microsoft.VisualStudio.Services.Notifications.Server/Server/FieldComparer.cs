// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Notifications.Server.FieldComparer
// Assembly: Microsoft.VisualStudio.Services.Notifications.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8ED1F52D-E567-4EFA-AAED-F2D9262BE9C2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Notifications.Server.dll

using Microsoft.VisualStudio.Services.Notifications.WebApi;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Notifications.Server
{
  public abstract class FieldComparer
  {
    private static Dictionary<SubscriptionFieldType, FieldComparer> m_comparers = new Dictionary<SubscriptionFieldType, FieldComparer>();

    static FieldComparer()
    {
      StringFieldComparer stringFieldComparer = new StringFieldComparer();
      DateFieldComparer dateFieldComparer = new DateFieldComparer();
      BooleanFieldComparer booleanFieldComparer = new BooleanFieldComparer();
      NumericFieldComparer numericFieldComparer = new NumericFieldComparer();
      FieldComparer.m_comparers.Add(SubscriptionFieldType.String, (FieldComparer) stringFieldComparer);
      FieldComparer.m_comparers.Add(SubscriptionFieldType.DateTime, (FieldComparer) dateFieldComparer);
      FieldComparer.m_comparers.Add(SubscriptionFieldType.Boolean, (FieldComparer) booleanFieldComparer);
      FieldComparer.m_comparers.Add(SubscriptionFieldType.Integer, (FieldComparer) numericFieldComparer);
    }

    public static FieldComparer GetComparer(SubscriptionFieldType fieldType)
    {
      FieldComparer fieldComparer;
      return FieldComparer.m_comparers.TryGetValue(fieldType, out fieldComparer) ? fieldComparer : FieldComparer.m_comparers[SubscriptionFieldType.String];
    }

    protected abstract IComparable FromObject(object o);

    public virtual bool CompareObject(
      byte op,
      EvaluationContext evaluationContext,
      object a,
      object b)
    {
      return this.Compare(op, evaluationContext, this.FromObject(a), this.FromObject(b));
    }

    public virtual bool Compare(
      byte op,
      EvaluationContext evaluationContext,
      IComparable a,
      IComparable b)
    {
      switch (op)
      {
        case 8:
          return a.CompareTo((object) b) < 0;
        case 9:
          return a.CompareTo((object) b) > 0;
        case 10:
          return a.CompareTo((object) b) <= 0;
        case 11:
          return a.CompareTo((object) b) >= 0;
        case 12:
          return a.Equals((object) b);
        case 13:
          return !a.Equals((object) b);
        default:
          throw new InvalidOperationException("Operator " + SubscriptionFilterOperators.GetLocalizedOperator(op) + " is not valid for type " + a.GetType().ToString());
      }
    }
  }
}
