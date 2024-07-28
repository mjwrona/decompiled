// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Pipelines.Runtime.StageExecutionContext
// Assembly: Microsoft.TeamFoundation.DistributedTask.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9201F3B5-DEAF-44A3-860C-DB7B277BB5C6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.WebApi.dll

using Microsoft.TeamFoundation.DistributedTask.Expressions;
using Microsoft.TeamFoundation.DistributedTask.Pipelines.Expressions;
using Microsoft.TeamFoundation.DistributedTask.WebApi;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Microsoft.TeamFoundation.DistributedTask.Pipelines.Runtime
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public class StageExecutionContext : GraphExecutionContext<StageInstance>
  {
    public StageExecutionContext(StageInstance stage = null, IList<StageInstance> dependencies = null)
      : this(stage, dependencies != null ? (IDictionary<string, StageInstance>) dependencies.ToDictionary<StageInstance, string, StageInstance>((Func<StageInstance, string>) (x => x.Name), (Func<StageInstance, StageInstance>) (x => x), (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase) : (IDictionary<string, StageInstance>) null)
    {
    }

    public StageExecutionContext(
      StageInstance stage,
      IDictionary<string, StageInstance> dependencies)
      : this(stage, dependencies, PipelineState.InProgress, (ICounterStore) new Microsoft.TeamFoundation.DistributedTask.Pipelines.CounterStore(), (IPackageStore) new Microsoft.TeamFoundation.DistributedTask.Pipelines.PackageStore((IEnumerable<PackageMetadata>) null, (IPackageResolver) null), (IResourceStore) new Microsoft.TeamFoundation.DistributedTask.Pipelines.ResourceStore(), (ITaskStore) new Microsoft.TeamFoundation.DistributedTask.Pipelines.TaskStore(Array.Empty<TaskDefinition>()), (IList<IStepProvider>) null, (IPipelineIdGenerator) null, (IPipelineTraceWriter) null, (EvaluationOptions) null, (ExecutionOptions) null)
    {
    }

    public StageExecutionContext(
      StageInstance stage,
      IDictionary<string, StageInstance> dependencies,
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
      : base(stage, dependencies, state, counterStore, packageStore, resourceStore, taskStore, stepProviders, idGenerator, trace, expressionOptions, executionOptions, featureFlags)
    {
      this.Stage.Identifier = this.IdGenerator.GetStageIdentifier(stage.Name);
    }

    public StageInstance Stage => this.Node;

    public StageAttempt PreviousAttempt { get; }

    protected override IEnumerable<INamedValueInfo> GetSupportedNamedValues()
    {
      foreach (INamedValueInfo supportedNamedValue in base.GetSupportedNamedValues())
        yield return supportedNamedValue;
      yield return (INamedValueInfo) new NamedValueInfo<DependenciesContextNode<StageInstance>>(Microsoft.TeamFoundation.DistributedTask.Pipelines.Expressions.ExpressionConstants.StageDependencies);
    }

    internal override string GetInstanceName() => this.IdGenerator.GetStageInstanceName(this.Stage.Name, this.Stage.Attempt);
  }
}
