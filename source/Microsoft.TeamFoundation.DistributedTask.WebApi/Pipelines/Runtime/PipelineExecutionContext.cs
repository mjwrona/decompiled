// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Pipelines.Runtime.PipelineExecutionContext
// Assembly: Microsoft.TeamFoundation.DistributedTask.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9201F3B5-DEAF-44A3-860C-DB7B277BB5C6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.WebApi.dll

using Microsoft.TeamFoundation.DistributedTask.Expressions;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Microsoft.TeamFoundation.DistributedTask.Pipelines.Runtime
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public abstract class PipelineExecutionContext : PipelineContextBase
  {
    private protected PipelineExecutionContext(PipelineExecutionContext context)
      : base((IPipelineContext) context)
    {
      this.State = context.State;
      this.ExecutionOptions = context.ExecutionOptions;
    }

    private protected PipelineExecutionContext(
      ICounterStore counterStore,
      IPackageStore packageStore,
      IResourceStore resourceStore,
      ITaskStore taskStore,
      IList<IStepProvider> stepProviders,
      PipelineState state,
      IPipelineIdGenerator idGenerator = null,
      IPipelineTraceWriter trace = null,
      EvaluationOptions expressionOptions = null,
      ExecutionOptions executionOptions = null,
      IDictionary<string, bool> featureFlags = null)
      : base(counterStore, packageStore, resourceStore, taskStore, stepProviders, idGenerator, trace, expressionOptions, featureFlags)
    {
      this.State = state;
      this.ExecutionOptions = executionOptions ?? new ExecutionOptions();
    }

    public PipelineState State { get; }

    public ExecutionOptions ExecutionOptions { get; }

    internal Guid GetInstanceId() => this.IdGenerator.GetInstanceId(this.GetInstanceName());

    internal abstract string GetInstanceName();
  }
}
