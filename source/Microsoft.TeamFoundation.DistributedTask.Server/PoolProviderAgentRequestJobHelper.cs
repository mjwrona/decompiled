// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.PoolProviderAgentRequestJobHelper
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.Azure.Pipelines.PoolProvider.Contracts;
using Microsoft.TeamFoundation.DistributedTask.Pipelines;
using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.DistributedTask.Server
{
  internal static class PoolProviderAgentRequestJobHelper
  {
    internal static AgentRequestJob GetAgentRequestJob(
      IVssRequestContext requestContext,
      TaskAgentJobRequest request,
      Job job)
    {
      return new AgentRequestJob(request.PoolId, request.ScopeId, request.PlanId, request.PlanType, PoolProviderAgentRequestJobHelper.GetRun(request.Owner), PoolProviderAgentRequestJobHelper.GetDefinition(request.Definition), job.Id, job.Name, job.Container, PoolProviderAgentRequestJobHelper.GetSteps(job.Steps), job.SidecarContainers, PoolProviderAgentRequestJobHelper.GetVariables(job.Variables));
    }

    public static AgentRequestJob ConvertJobContract(
      IVssRequestContext requestContext,
      TaskAgentJobRequest request,
      TaskAgentJob job)
    {
      return new AgentRequestJob(request.PoolId, request.ScopeId, request.PlanId, request.PlanType, PoolProviderAgentRequestJobHelper.GetRun(request.Owner), PoolProviderAgentRequestJobHelper.GetDefinition(request.Definition), job.Id, job.Name, job.Container, PoolProviderAgentRequestJobHelper.ConvertJobSteps(job.Steps), job.SidecarContainers, PoolProviderAgentRequestJobHelper.ConvertVariables(job.Variables));
    }

    private static IList<AgentRequestJobVariable> ConvertVariables(
      IList<TaskAgentJobVariable> variables)
    {
      List<AgentRequestJobVariable> requestJobVariableList = new List<AgentRequestJobVariable>();
      foreach (TaskAgentJobVariable variable in (IEnumerable<TaskAgentJobVariable>) variables)
        requestJobVariableList.Add(new AgentRequestJobVariable()
        {
          Name = variable.Name,
          Value = variable.Secret ? (string) null : variable.Value,
          Secret = variable.Secret
        });
      return (IList<AgentRequestJobVariable>) requestJobVariableList;
    }

    private static IList<AgentRequestJobStep> ConvertJobSteps(IList<TaskAgentJobStep> steps)
    {
      List<AgentRequestJobStep> agentRequestJobStepList = new List<AgentRequestJobStep>();
      foreach (TaskAgentJobStep step in (IEnumerable<TaskAgentJobStep>) steps)
      {
        AgentRequestJobStep agentRequestJobStep = new AgentRequestJobStep()
        {
          Id = step.Id,
          Name = step.Name,
          Enabled = step.Enabled,
          Condition = step.Condition,
          ContinueOnError = step.ContinueOnError,
          TimeoutInMinutes = step.TimeoutInMinutes,
          RetryCountOnTaskFailure = step.RetryCountOnTaskFailure,
          Task = new AgentRequestJobTask()
          {
            Id = step.Task.Id,
            Name = step.Task.Name,
            Version = step.Task.Version
          },
          Env = (IDictionary<string, string>) new Dictionary<string, string>(step.Env, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase),
          Inputs = (IDictionary<string, string>) new Dictionary<string, string>(step.Inputs, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase)
        };
        agentRequestJobStepList.Add(agentRequestJobStep);
      }
      return (IList<AgentRequestJobStep>) agentRequestJobStepList;
    }

    private static IList<AgentRequestJobVariable> GetVariables(IList<IVariable> variables)
    {
      List<AgentRequestJobVariable> variables1 = new List<AgentRequestJobVariable>();
      if (variables != null && variables.Count > 0)
      {
        foreach (IVariable variable1 in (IEnumerable<IVariable>) variables)
        {
          if (variable1 is Variable variable2)
            variables1.Add(new AgentRequestJobVariable()
            {
              Name = variable2.Name,
              Value = variable2.Secret ? (string) null : variable2.Value,
              Secret = variable2.Secret
            });
        }
      }
      return (IList<AgentRequestJobVariable>) variables1;
    }

    private static IList<AgentRequestJobStep> GetSteps(IList<JobStep> steps)
    {
      List<AgentRequestJobStep> steps1 = new List<AgentRequestJobStep>();
      if (steps != null && steps.Count > 0)
      {
        foreach (JobStep step in (IEnumerable<JobStep>) steps)
        {
          AgentRequestJobStep agentRequestJobStep = new AgentRequestJobStep()
          {
            Id = step.Id,
            Name = step.Name,
            Enabled = step.Enabled,
            Condition = step.Condition,
            ContinueOnError = step.ContinueOnError,
            TimeoutInMinutes = step.TimeoutInMinutes,
            RetryCountOnTaskFailure = step.RetryCountOnTaskFailure
          };
          if (!(step is TaskStep taskStep))
            throw new NotSupportedException(step.GetType().FullName);
          agentRequestJobStep.Type = AgentRequestJobStep.TaskAgentJobStepType.Task;
          AgentRequestJobTask agentRequestJobTask = new AgentRequestJobTask()
          {
            Id = taskStep.Reference.Id,
            Name = taskStep.Reference.Name,
            Version = taskStep.Reference.Version
          };
          agentRequestJobStep.Task = agentRequestJobTask;
          agentRequestJobStep.Env = (IDictionary<string, string>) new Dictionary<string, string>(taskStep.Environment, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
          agentRequestJobStep.Inputs = (IDictionary<string, string>) new Dictionary<string, string>(taskStep.Inputs, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
          steps1.Add(agentRequestJobStep);
        }
      }
      return (IList<AgentRequestJobStep>) steps1;
    }

    private static AgentRequestJobOwnerReference GetDefinition(TaskOrchestrationOwner definition)
    {
      AgentRequestJobOwnerReference definition1 = new AgentRequestJobOwnerReference();
      object obj;
      if (definition.Links.Links.TryGetValue("self", out obj) && obj is ReferenceLink referenceLink)
      {
        definition1.Id = definition.Id;
        definition1.Name = definition.Name;
        definition1.Ref = referenceLink.Href;
      }
      return definition1;
    }

    private static AgentRequestJobOwnerReference GetRun(TaskOrchestrationOwner owner)
    {
      AgentRequestJobOwnerReference run = new AgentRequestJobOwnerReference();
      object obj;
      if (owner.Links.Links.TryGetValue("self", out obj) && obj is ReferenceLink referenceLink)
      {
        run.Id = owner.Id;
        run.Name = owner.Name;
        run.Ref = referenceLink.Href;
      }
      return run;
    }
  }
}
