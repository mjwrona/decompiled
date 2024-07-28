// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Notifications.Server.BooleanFieldCompare
// Assembly: Microsoft.VisualStudio.Services.Notifications.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8ED1F52D-E567-4EFA-AAED-F2D9262BE9C2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Notifications.Server.dll

using System;

namespace Microsoft.VisualStudio.Services.Notifications.Server
{
  public class BooleanFieldCompare : FieldCompare<bool>
  {
    public override bool Compare(byte op, EvaluationContext evaluationContext, bool a, bool b) => base.Compare(op, evaluationContext, a, b);

    public override bool GetComparable(object o)
    {
      bool result;
      if (bool.TryParse(o.ToString(), out result))
        throw new InvalidOperationException(string.Format("{0} is not a valid Boolean", o));
      return result;
    }
  }
}
