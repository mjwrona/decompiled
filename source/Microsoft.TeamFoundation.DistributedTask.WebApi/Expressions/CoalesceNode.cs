// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Expressions.CoalesceNode
// Assembly: Microsoft.TeamFoundation.DistributedTask.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9201F3B5-DEAF-44A3-860C-DB7B277BB5C6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.WebApi.dll

using System.Collections.Generic;

namespace Microsoft.TeamFoundation.DistributedTask.Expressions
{
  internal sealed class CoalesceNode : FunctionNode
  {
    protected override sealed bool TraceFullyRealized => false;

    protected override sealed object EvaluateCore(EvaluationContext context)
    {
      EvaluationResult evaluationResult = (EvaluationResult) null;
      foreach (ExpressionNode parameter in (IEnumerable<ExpressionNode>) this.Parameters)
      {
        evaluationResult = parameter.Evaluate(context);
        if (evaluationResult.Kind != ValueKind.Null)
        {
          if (evaluationResult.Kind == ValueKind.String)
          {
            if (!string.IsNullOrEmpty(evaluationResult.Value as string))
              break;
          }
          else
            break;
        }
      }
      return evaluationResult?.Value;
    }
  }
}
