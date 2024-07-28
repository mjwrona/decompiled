// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Notifications.Server.FieldCompare`1
// Assembly: Microsoft.VisualStudio.Services.Notifications.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8ED1F52D-E567-4EFA-AAED-F2D9262BE9C2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Notifications.Server.dll

using Microsoft.VisualStudio.Services.Notifications.WebApi;
using System;

namespace Microsoft.VisualStudio.Services.Notifications.Server
{
  public abstract class FieldCompare<T> where T : IComparable
  {
    public abstract T GetComparable(object o);

    public bool CompareObject(byte op, EvaluationContext evaluationContext, object a, object b) => this.Compare(op, evaluationContext, this.GetComparable(a), this.GetComparable(b));

    public virtual bool Compare(byte op, EvaluationContext evaluationContext, T a, T b)
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
