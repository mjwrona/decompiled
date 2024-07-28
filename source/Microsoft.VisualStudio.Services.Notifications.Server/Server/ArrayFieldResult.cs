// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Notifications.Server.ArrayFieldResult
// Assembly: Microsoft.VisualStudio.Services.Notifications.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8ED1F52D-E567-4EFA-AAED-F2D9262BE9C2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Notifications.Server.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Notifications.WebApi;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Notifications.Server
{
  public class ArrayFieldResult : FieldResult
  {
    public object[] Results { get; }

    public ArrayFieldResult(object[] results) => this.Results = results;

    public bool UnderPath(string target) => this.Results is string[] results && UnderPathComparer.IsTargetUnder(results, target);

    private bool BinarySearch(string target, out bool evalResult)
    {
      if (this.Results is string[] results)
      {
        evalResult = Array.BinarySearch<string>(results, target, (IComparer<string>) VssStringComparer.StringFieldConditionEquality) >= 0;
        return true;
      }
      evalResult = false;
      return false;
    }

    public bool BinarySearch(
      EvaluationContext evaluationContext,
      byte op,
      string target,
      out bool evalResult)
    {
      bool flag = false;
      evalResult = false;
      switch (op)
      {
        case 12:
          flag = this.BinarySearch(target, out evalResult);
          break;
        case 13:
          flag = this.BinarySearch(target, out evalResult);
          if (flag)
          {
            evalResult = !evalResult;
            break;
          }
          break;
      }
      return flag;
    }

    public override bool CompareResult(
      byte op,
      EvaluationContext evaluationContext,
      object expectedValue,
      SubscriptionFieldType type)
    {
      FieldComparer.GetComparer(type);
      foreach (object result in this.Results)
      {
        if (this.CompareSingleValue(op, evaluationContext, expectedValue, result, type))
          return true;
      }
      return false;
    }

    public override void TraceEvaluationString(
      EvaluationContext evaluationContext,
      bool result,
      byte op,
      object expectedValue)
    {
      FieldResult.TraceEvaluationString(evaluationContext, result, op, expectedValue, (object) this.Results);
    }
  }
}
