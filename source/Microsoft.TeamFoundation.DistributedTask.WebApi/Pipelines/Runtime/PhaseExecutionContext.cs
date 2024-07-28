// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Pipelines.Runtime.PhaseExecutionContext
// Assembly: Microsoft.TeamFoundation.DistributedTask.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9201F3B5-DEAF-44A3-860C-DB7B277BB5C6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.WebApi.dll

using Microsoft.TeamFoundation.DistributedTask.Expressions;
using Microsoft.TeamFoundation.DistributedTask.Logging;
using Microsoft.TeamFoundation.DistributedTask.Pipelines.Expressions;
using Microsoft.TeamFoundation.DistributedTask.WebApi;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Microsoft.TeamFoundation.DistributedTask.Pipelines.Runtime
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public class PhaseExecutionContext : GraphExecutionContext<PhaseInstance>
  {
    private IList<PhaseAttempt> m_phaseAttempts;
    private IDictionary<string, IDictionary<string, PhaseInstance>> m_stageDependencies;

    public PhaseExecutionContext(
      StageInstance stage = null,
      PhaseInstance phase = null,
      IList<PhaseInstance> dependencies = null,
      EvaluationOptions expressionOptions = null,
      ExecutionOptions executionOptions = null)
      : this(stage, phase, dependencies != null ? (IDictionary<string, PhaseInstance>) dependencies.ToDictionary<PhaseInstance, string, PhaseInstance>((Func<PhaseInstance, string>) (x => x.Name), (Func<PhaseInstance, PhaseInstance>) (x => x), (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase) : (IDictionary<string, PhaseInstance>) null, expressionOptions, executionOptions)
    {
    }

    public PhaseExecutionContext(
      StageInstance stage,
      PhaseInstance phase,
      IDictionary<string, PhaseInstance> dependencies,
      EvaluationOptions expressionOptions,
      ExecutionOptions executionOptions)
      : this(stage, phase, dependencies, (IDictionary<string, IDictionary<string, PhaseInstance>>) null, PipelineState.InProgress, (ICounterStore) new Microsoft.TeamFoundation.DistributedTask.Pipelines.CounterStore(), (IPackageStore) new Microsoft.TeamFoundation.DistributedTask.Pipelines.PackageStore((IEnumerable<PackageMetadata>) null, (IPackageResolver) null), (IResourceStore) new Microsoft.TeamFoundation.DistributedTask.Pipelines.ResourceStore(), (ITaskStore) new Microsoft.TeamFoundation.DistributedTask.Pipelines.TaskStore(Array.Empty<TaskDefinition>()), (IList<IStepProvider>) null, (IPipelineIdGenerator) null, (IPipelineTraceWriter) null, expressionOptions, executionOptions)
    {
    }

    public PhaseExecutionContext(
      StageInstance stage,
      PhaseInstance phase,
      IDictionary<string, PhaseInstance> dependencies,
      IDictionary<string, IDictionary<string, PhaseInstance>> stageDependencies,
      PipelineState state,
      ICounterStore counterStore,
      IPackageStore packageStore,
      IResourceStore resourceStore,
      ITaskStore taskStore,
      IList<IStepProvider> stepProviders,
      IPipelineIdGenerator idGenerator,
      IPipelineTraceWriter trace,
      EvaluationOptions expressionOptions,
      ExecutionOptions executionOptions,
      IDictionary<string, bool> featureFlags = null)
      : base(phase, dependencies, state, counterStore, packageStore, resourceStore, taskStore, stepProviders, idGenerator, trace, expressionOptions, executionOptions, featureFlags)
    {
      this.Stage = stage;
      if (this.Stage != null)
        this.Stage.Identifier = this.IdGenerator.GetStageIdentifier(this.Stage.Name);
      this.Phase.Identifier = this.IdGenerator.GetPhaseIdentifier(this.Stage?.Name, this.Phase.Name);
      this.m_stageDependencies = stageDependencies;
    }

    public StageInstance Stage { get; }

    public PhaseInstance Phase => this.Node;

    public IList<PhaseAttempt> PreviousAttempts
    {
      get
      {
        if (this.m_phaseAttempts == null)
          this.m_phaseAttempts = (IList<PhaseAttempt>) new List<PhaseAttempt>();
        return this.m_phaseAttempts;
      }
    }

    public IDictionary<string, IDictionary<string, PhaseInstance>> StageDependencies
    {
      get
      {
        if (this.m_stageDependencies == null)
          this.m_stageDependencies = (IDictionary<string, IDictionary<string, PhaseInstance>>) new Dictionary<string, IDictionary<string, PhaseInstance>>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
        return this.m_stageDependencies;
      }
    }

    public JobExecutionContext CreateJobContext(
      string name,
      int attempt,
      int positionInPhase = 0,
      int totalJobsInPhase = 0)
    {
      return this.CreateJobContext(new JobInstance(name, attempt), positionInPhase, totalJobsInPhase);
    }

    public JobExecutionContext CreateJobContext(
      JobInstance jobInstance,
      int positionInPhase = 0,
      int totalJobsInPhase = 0)
    {
      return new JobExecutionContext(this, jobInstance, (IDictionary<string, VariableValue>) null, positionInPhase, totalJobsInPhase);
    }

    internal override string GetInstanceName() => this.IdGenerator.GetPhaseInstanceName(this.Stage?.Name, this.Phase.Name, this.Phase.Attempt);

    protected override bool SecretsAccessed
    {
      get
      {
        if (base.SecretsAccessed)
          return true;
        IDictionary<string, IDictionary<string, PhaseInstance>> stageDependencies = this.m_stageDependencies;
        return stageDependencies != null && stageDependencies.Any<KeyValuePair<string, IDictionary<string, PhaseInstance>>>((Func<KeyValuePair<string, IDictionary<string, PhaseInstance>>, bool>) (x => x.Value.Any<KeyValuePair<string, PhaseInstance>>((Func<KeyValuePair<string, PhaseInstance>, bool>) (y => y.Value.SecretsAccessed))));
      }
    }

    protected override void ResetSecretsAccessed()
    {
      base.ResetSecretsAccessed();
      IDictionary<string, IDictionary<string, PhaseInstance>> stageDependencies = this.m_stageDependencies;
      if ((stageDependencies != null ? (stageDependencies.Count > 0 ? 1 : 0) : 0) == 0)
        return;
      foreach (KeyValuePair<string, IDictionary<string, PhaseInstance>> stageDependency in (IEnumerable<KeyValuePair<string, IDictionary<string, PhaseInstance>>>) this.m_stageDependencies)
      {
        foreach (KeyValuePair<string, PhaseInstance> keyValuePair in (IEnumerable<KeyValuePair<string, PhaseInstance>>) stageDependency.Value)
          keyValuePair.Value.ResetSecretsAccessed();
      }
    }

    public override ISecretMasker CreateSecretMasker()
    {
      ISecretMasker secretMasker = base.CreateSecretMasker();
      IDictionary<string, IDictionary<string, PhaseInstance>> stageDependencies = this.m_stageDependencies;
      if ((stageDependencies != null ? (stageDependencies.Count > 0 ? 1 : 0) : 0) != 0)
      {
        foreach (IDictionary<string, PhaseInstance> dictionary in (IEnumerable<IDictionary<string, PhaseInstance>>) this.m_stageDependencies.Values)
        {
          foreach (GraphNodeInstance<PhaseNode> graphNodeInstance in (IEnumerable<PhaseInstance>) dictionary.Values)
          {
            foreach (VariableValue variableValue in graphNodeInstance.Outputs.Values.Where<VariableValue>((Func<VariableValue, bool>) (x => x.IsSecret)))
              secretMasker.AddValue(variableValue.Value);
          }
        }
      }
      return secretMasker;
    }

    protected override IEnumerable<INamedValueInfo> GetSupportedNamedValues()
    {
      foreach (INamedValueInfo supportedNamedValue in base.GetSupportedNamedValues())
        yield return supportedNamedValue;
      yield return (INamedValueInfo) new NamedValueInfo<StageDependenciesContextNode>("stageDependencies");
    }
  }
}
