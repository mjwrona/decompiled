// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Notifications.Server.ConstantFieldResult
// Assembly: Microsoft.VisualStudio.Services.Notifications.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8ED1F52D-E567-4EFA-AAED-F2D9262BE9C2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Notifications.Server.dll

using Microsoft.VisualStudio.Services.Notifications.WebApi;

namespace Microsoft.VisualStudio.Services.Notifications.Server
{
  public class ConstantFieldResult : FieldResult
  {
    public ConstantFieldResult(object result) => this.Result = result;

    public object Result { get; }

    public override bool CompareResult(
      byte op,
      EvaluationContext context,
      object expectedValue,
      SubscriptionFieldType type)
    {
      return this.CompareSingleValue(op, context, expectedValue, this.Result, type);
    }

    public override void TraceEvaluationString(
      EvaluationContext evaluationContext,
      bool result,
      byte op,
      object expectedValue)
    {
      FieldResult.TraceEvaluationString(evaluationContext, result, op, expectedValue, this.Result);
    }
  }
}
