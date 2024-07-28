// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Notifications.Server.IdentityListFieldResult
// Assembly: Microsoft.VisualStudio.Services.Notifications.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8ED1F52D-E567-4EFA-AAED-F2D9262BE9C2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Notifications.Server.dll

using Microsoft.VisualStudio.Services.Notifications.WebApi;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Notifications.Server
{
  public class IdentityListFieldResult : FieldResult
  {
    public IdentityListFieldResult() => this.Identities = new List<string>();

    public IdentityListFieldResult(string key, string value)
    {
      this.Identities = new List<string>();
      this.Identities.Add(key);
      this.Identities.Add(value);
    }

    public List<string> Identities { get; }

    public override bool CompareResult(
      byte op,
      EvaluationContext evaluationContext,
      object expectedValue,
      SubscriptionFieldType type)
    {
      bool flag = op != (byte) 13;
      FieldComparer.GetComparer(type);
      foreach (object identity in this.Identities)
      {
        if (this.CompareSingleValue(op, evaluationContext, expectedValue, identity, type) == flag)
          return flag;
      }
      return !flag;
    }

    public override void TraceEvaluationString(
      EvaluationContext evaluationContext,
      bool result,
      byte op,
      object expectedValue)
    {
      FieldResult.TraceEvaluationString(evaluationContext, result, op, expectedValue, (object) this.Identities);
    }
  }
}
