// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Pipelines.Runtime.JobExecutionContext
// Assembly: Microsoft.TeamFoundation.DistributedTask.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9201F3B5-DEAF-44A3-860C-DB7B277BB5C6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.WebApi.dll

using Microsoft.TeamFoundation.DistributedTask.WebApi;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Microsoft.TeamFoundation.DistributedTask.Pipelines.Runtime
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class JobExecutionContext : PipelineExecutionContext
  {
    public JobExecutionContext(PipelineState state, IPipelineIdGenerator idGenerator = null)
      : base((ICounterStore) null, (IPackageStore) null, (IResourceStore) null, (ITaskStore) null, (IList<IStepProvider>) null, state, idGenerator)
    {
    }

    public JobExecutionContext(
      PhaseExecutionContext context,
      JobInstance job,
      IDictionary<string, VariableValue> variables,
      int positionInPhase = 0,
      int totalJobsInPhase = 0)
      : base((PipelineExecutionContext) context)
    {
      this.Stage = context.Stage;
      this.Phase = context.Phase;
      this.Job = job;
      this.Job.Identifier = this.IdGenerator.GetJobIdentifier(this.Stage?.Name, this.Phase.Name, this.Job.Name, job.CheckRerunAttempt);
      Microsoft.TeamFoundation.DistributedTask.Pipelines.Job definition = job.Definition;
      int num1;
      if (definition == null)
      {
        num1 = 0;
      }
      else
      {
        int? count = definition.Variables?.Count;
        int num2 = 0;
        num1 = count.GetValueOrDefault() > num2 & count.HasValue ? 1 : 0;
      }
      if (num1 != 0)
        this.SetUserVariables(job.Definition.Variables.OfType<Variable>());
      this.SetSystemVariables(variables);
      List<Variable> variableList = new List<Variable>()
      {
        new Variable()
        {
          Name = WellKnownDistributedTaskVariables.JobIdentifier,
          Value = job.Identifier,
          Readonly = true
        },
        new Variable()
        {
          Name = WellKnownDistributedTaskVariables.JobAttempt,
          Value = job.Attempt.ToString(),
          Readonly = true
        }
      };
      if (positionInPhase != 0)
        variableList.Add(new Variable()
        {
          Name = WellKnownDistributedTaskVariables.JobPositionInPhase,
          Value = positionInPhase.ToString(),
          Readonly = true
        });
      if (totalJobsInPhase != 0)
        variableList.Add(new Variable()
        {
          Name = WellKnownDistributedTaskVariables.TotalJobsInPhase,
          Value = totalJobsInPhase.ToString(),
          Readonly = true
        });
      this.SetSystemVariables((IEnumerable<Variable>) variableList);
      VariableValue variableValue;
      if (!string.IsNullOrEmpty(this.ExecutionOptions.SystemTokenScope) || !this.Variables.TryGetValue(WellKnownDistributedTaskVariables.AccessTokenScope, out variableValue))
        return;
      this.ExecutionOptions.SystemTokenScope = variableValue?.Value;
    }

    public StageInstance Stage { get; }

    public PhaseInstance Phase { get; }

    public JobInstance Job { get; }

    internal override string GetInstanceName() => this.IdGenerator.GetJobInstanceName(this.Stage?.Name, this.Phase.Name, this.Job.Name, this.Job.Attempt, this.Job.CheckRerunAttempt);
  }
}
