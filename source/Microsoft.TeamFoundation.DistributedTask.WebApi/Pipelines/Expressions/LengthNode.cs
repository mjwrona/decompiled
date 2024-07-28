// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Pipelines.Expressions.LengthNode
// Assembly: Microsoft.TeamFoundation.DistributedTask.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9201F3B5-DEAF-44A3-860C-DB7B277BB5C6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.WebApi.dll

using Microsoft.TeamFoundation.DistributedTask.Expressions;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;

namespace Microsoft.TeamFoundation.DistributedTask.Pipelines.Expressions
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class LengthNode : FunctionNode
  {
    public static int minParameters = 1;
    public static int maxParameters = 1;

    protected override sealed bool TraceFullyRealized => false;

    protected override sealed object EvaluateCore(EvaluationContext context)
    {
      EvaluationResult evaluationResult = this.Parameters[0].Evaluate(context);
      bool flag = false;
      int num = -1;
      switch (evaluationResult.Kind)
      {
        case ValueKind.Array:
          num = ((JContainer) evaluationResult.Value).Count;
          break;
        case ValueKind.Boolean:
        case ValueKind.Null:
        case ValueKind.Number:
        case ValueKind.Version:
          flag = true;
          break;
        case ValueKind.Object:
          if (evaluationResult.Value is IReadOnlyDictionary<string, object>)
          {
            num = ((IReadOnlyCollection<KeyValuePair<string, object>>) evaluationResult.Value).Count;
            break;
          }
          if (evaluationResult.Value is ICollection)
          {
            num = ((ICollection) evaluationResult.Value).Count;
            break;
          }
          flag = true;
          break;
        case ValueKind.String:
          num = ((string) evaluationResult.Value).Length;
          break;
      }
      if (flag)
        throw new NotSupportedException(PipelineStrings.InvalidTypeForLengthFunction((object) evaluationResult.Kind));
      return (object) new Decimal(num);
    }
  }
}
