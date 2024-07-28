// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Pipelines.Expressions.PipelineContextNode
// Assembly: Microsoft.TeamFoundation.DistributedTask.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9201F3B5-DEAF-44A3-860C-DB7B277BB5C6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.WebApi.dll

using Microsoft.TeamFoundation.DistributedTask.Expressions;
using Microsoft.TeamFoundation.DistributedTask.WebApi;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Microsoft.TeamFoundation.DistributedTask.Pipelines.Expressions
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  internal sealed class PipelineContextNode : NamedValueNode
  {
    protected override object EvaluateCore(EvaluationContext context)
    {
      IPipelineContext state = context.State as IPipelineContext;
      Dictionary<string, object> core = new Dictionary<string, object>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      VariableValue variableValue;
      DateTimeOffset result;
      if (state.Variables.TryGetValue(WellKnownDistributedTaskVariables.PipelineStartTime, out variableValue) && !string.IsNullOrEmpty(variableValue.Value) && EvaluationResult.CreateIntermediateResult(context, (object) variableValue.Value, out ResultMemory _).TryConvertToDateTime(context, out result))
        core["startTime"] = (object) result;
      return (object) core;
    }
  }
}
