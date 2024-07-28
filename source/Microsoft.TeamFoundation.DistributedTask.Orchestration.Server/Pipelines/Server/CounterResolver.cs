// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Pipelines.Server.CounterResolver
// Assembly: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07FD5059-3D25-415E-AA3A-5372051D7E71
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.dll

using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using System;

namespace Microsoft.TeamFoundation.DistributedTask.Pipelines.Server
{
  internal sealed class CounterResolver : ICounterResolver
  {
    private readonly IVssRequestContext m_requestContext;

    public CounterResolver(IVssRequestContext requestContext) => this.m_requestContext = requestContext;

    public int Increment(IPipelineContext context, string prefix, int seed)
    {
      Guid planId = CounterResolver.GetValue<Guid>(context, WellKnownDistributedTaskVariables.PlanId);
      string planType = CounterResolver.GetValue<string>(context, WellKnownDistributedTaskVariables.System);
      Guid projectId = CounterResolver.GetValue<Guid>(context, WellKnownDistributedTaskVariables.TeamProjectId);
      int definitionId = CounterResolver.GetValue<int>(context, WellKnownDistributedTaskVariables.DefinitionId);
      return string.IsNullOrEmpty(planType) || definitionId == 0 ? seed : this.m_requestContext.GetService<IPipelineCounterService>().IncrementCounter(this.m_requestContext, projectId, planType, definitionId, planId, prefix, seed);
    }

    private static T GetValue<T>(IPipelineContext context, string name)
    {
      VariableValue variableValue;
      if (!context.Variables.TryGetValue(name, out variableValue))
        return default (T);
      if (typeof (T).Equals(typeof (string)))
        return (T) variableValue.Value;
      if (typeof (T).Equals(typeof (int)))
        return (T) (ValueType) int.Parse(variableValue.Value);
      if (typeof (T).Equals(typeof (Guid)))
        return (T) (ValueType) Guid.Parse(variableValue.Value);
      throw new NotSupportedException();
    }
  }
}
