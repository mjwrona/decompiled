// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Controllers.TaskHubJobInstancesController
// Assembly: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07FD5059-3D25-415E-AA3A-5372051D7E71
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.dll

using Microsoft.TeamFoundation.DistributedTask.Pipelines;
using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Web.Http;

namespace Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Controllers
{
  [ClientInternalUseOnly(true)]
  [ControllerApiVersion(5.0)]
  [VersionedApiControllerCustomName(Area = "distributedtask", ResourceName = "jobinstances")]
  public sealed class TaskHubJobInstancesController : TaskHubApiController
  {
    [HttpGet]
    public TaskAgentJob GetJobInstance(string orchestrationId)
    {
      Job agentRequestJob = this.Hub.GetAgentRequestJob(this.TfsRequestContext, this.ScopeIdentifier, orchestrationId);
      return new TaskAgentJob(agentRequestJob.Id, agentRequestJob.Name, agentRequestJob.Container, this.ConvertSteps(agentRequestJob.Steps), agentRequestJob.SidecarContainers, this.ConvertVariables(agentRequestJob.Variables));
    }

    private IList<TaskAgentJobStep> ConvertSteps(IList<JobStep> steps)
    {
      List<TaskAgentJobStep> taskAgentJobStepList = new List<TaskAgentJobStep>();
      if (steps != null && steps.Count > 0)
      {
        foreach (JobStep step in (IEnumerable<JobStep>) steps)
        {
          TaskAgentJobStep taskAgentJobStep = new TaskAgentJobStep()
          {
            Id = step.Id,
            Name = step.Name,
            Enabled = step.Enabled,
            Condition = step.Condition,
            ContinueOnError = step.ContinueOnError,
            TimeoutInMinutes = step.TimeoutInMinutes
          };
          if (!(step is TaskStep taskStep))
            throw new System.NotSupportedException(step.GetType().FullName);
          taskAgentJobStep.Type = TaskAgentJobStep.TaskAgentJobStepType.Task;
          TaskAgentJobTask taskAgentJobTask = new TaskAgentJobTask()
          {
            Id = taskStep.Reference.Id,
            Name = taskStep.Reference.Name,
            Version = taskStep.Reference.Version
          };
          taskAgentJobStep.Task = taskAgentJobTask;
          taskAgentJobStep.Env = (IDictionary<string, string>) new Dictionary<string, string>(taskStep.Environment, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
          taskAgentJobStep.Inputs = (IDictionary<string, string>) new Dictionary<string, string>(taskStep.Inputs, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
          taskAgentJobStepList.Add(taskAgentJobStep);
        }
      }
      return (IList<TaskAgentJobStep>) taskAgentJobStepList;
    }

    private Exception NotSupportedException() => throw new NotImplementedException();

    private IList<TaskAgentJobVariable> ConvertVariables(IList<IVariable> variables)
    {
      List<TaskAgentJobVariable> agentJobVariableList = new List<TaskAgentJobVariable>();
      if (variables != null && variables.Count > 0)
      {
        foreach (IVariable variable1 in (IEnumerable<IVariable>) variables)
        {
          if (variable1 is Variable variable2)
            agentJobVariableList.Add(new TaskAgentJobVariable()
            {
              Name = variable2.Name,
              Value = variable2.Secret ? (string) null : variable2.Value,
              Secret = variable2.Secret
            });
        }
      }
      return (IList<TaskAgentJobVariable>) agentJobVariableList;
    }
  }
}
