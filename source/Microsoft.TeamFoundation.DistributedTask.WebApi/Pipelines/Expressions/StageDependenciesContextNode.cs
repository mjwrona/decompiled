// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Pipelines.Expressions.StageDependenciesContextNode
// Assembly: Microsoft.TeamFoundation.DistributedTask.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9201F3B5-DEAF-44A3-860C-DB7B277BB5C6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.WebApi.dll

using Microsoft.TeamFoundation.DistributedTask.Expressions;
using Microsoft.TeamFoundation.DistributedTask.Pipelines.Runtime;
using System.Collections.Generic;
using System.ComponentModel;

namespace Microsoft.TeamFoundation.DistributedTask.Pipelines.Expressions
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  internal sealed class StageDependenciesContextNode : NamedValueNode
  {
    protected override object EvaluateCore(EvaluationContext context)
    {
      PhaseExecutionContext state = context.State as PhaseExecutionContext;
      Dictionary<string, object> core = new Dictionary<string, object>();
      foreach (KeyValuePair<string, IDictionary<string, PhaseInstance>> stageDependency in (IEnumerable<KeyValuePair<string, IDictionary<string, PhaseInstance>>>) state.StageDependencies)
        core.Add(stageDependency.Key, (object) stageDependency.Value);
      return (object) core;
    }
  }
}
