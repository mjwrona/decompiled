// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Pipelines.Runtime.GraphExecutionContext`1
// Assembly: Microsoft.TeamFoundation.DistributedTask.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9201F3B5-DEAF-44A3-860C-DB7B277BB5C6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.WebApi.dll

using Microsoft.TeamFoundation.DistributedTask.Expressions;
using Microsoft.TeamFoundation.DistributedTask.Logging;
using Microsoft.TeamFoundation.DistributedTask.Pipelines.Expressions;
using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Microsoft.TeamFoundation.DistributedTask.Pipelines.Runtime
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public abstract class GraphExecutionContext<TInstance> : PipelineExecutionContext where TInstance : IGraphNodeInstance
  {
    private Dictionary<string, TInstance> m_dependencies;

    private protected GraphExecutionContext(GraphExecutionContext<TInstance> context)
      : base((PipelineExecutionContext) context)
    {
      this.Node = context.Node;
      Dictionary<string, TInstance> dependencies = context.m_dependencies;
      // ISSUE: explicit non-virtual call
      if ((dependencies != null ? (__nonvirtual (dependencies.Count) > 0 ? 1 : 0) : 0) == 0)
        return;
      this.m_dependencies = context.m_dependencies.ToDictionary<KeyValuePair<string, TInstance>, string, TInstance>((Func<KeyValuePair<string, TInstance>, string>) (x => x.Key), (Func<KeyValuePair<string, TInstance>, TInstance>) (x => x.Value), (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
    }

    private protected GraphExecutionContext(
      TInstance node,
      IDictionary<string, TInstance> dependencies,
      PipelineState state,
      ICounterStore counterStore,
      IPackageStore packageStore,
      IResourceStore resourceStore,
      ITaskStore taskStore,
      IList<IStepProvider> stepProviders,
      IPipelineIdGenerator idGenerator = null,
      IPipelineTraceWriter trace = null,
      EvaluationOptions expressionOptions = null,
      ExecutionOptions executionOptions = null,
      IDictionary<string, bool> featureFlags = null)
      : base(counterStore, packageStore, resourceStore, taskStore, stepProviders, state, idGenerator, trace, expressionOptions, executionOptions, featureFlags)
    {
      ArgumentUtility.CheckGenericForNull((object) node, nameof (node));
      this.Node = node;
      if (dependencies == null || dependencies.Count <= 0)
        return;
      this.m_dependencies = dependencies.ToDictionary<KeyValuePair<string, TInstance>, string, TInstance>((Func<KeyValuePair<string, TInstance>, string>) (x => x.Key), (Func<KeyValuePair<string, TInstance>, TInstance>) (x => x.Value), (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
    }

    public IDictionary<string, TInstance> Dependencies
    {
      get
      {
        if (this.m_dependencies == null)
          this.m_dependencies = new Dictionary<string, TInstance>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
        return (IDictionary<string, TInstance>) this.m_dependencies;
      }
    }

    protected TInstance Node { get; }

    protected override bool SecretsAccessed
    {
      get
      {
        if (base.SecretsAccessed)
          return true;
        Dictionary<string, TInstance> dependencies = this.m_dependencies;
        return dependencies != null && dependencies.Any<KeyValuePair<string, TInstance>>((Func<KeyValuePair<string, TInstance>, bool>) (x => x.Value.SecretsAccessed));
      }
    }

    protected override void ResetSecretsAccessed()
    {
      base.ResetSecretsAccessed();
      Dictionary<string, TInstance> dependencies = this.m_dependencies;
      // ISSUE: explicit non-virtual call
      if ((dependencies != null ? (__nonvirtual (dependencies.Count) > 0 ? 1 : 0) : 0) == 0)
        return;
      foreach (KeyValuePair<string, TInstance> dependency in this.m_dependencies)
        dependency.Value.ResetSecretsAccessed();
    }

    public override ISecretMasker CreateSecretMasker()
    {
      ISecretMasker secretMasker = base.CreateSecretMasker();
      Dictionary<string, TInstance> dependencies = this.m_dependencies;
      // ISSUE: explicit non-virtual call
      if ((dependencies != null ? (__nonvirtual (dependencies.Count) > 0 ? 1 : 0) : 0) != 0)
      {
        foreach (TInstance instance in this.m_dependencies.Values)
        {
          foreach (VariableValue variableValue in instance.Outputs.Values.Where<VariableValue>((Func<VariableValue, bool>) (x => x.IsSecret)))
            secretMasker.AddValue(variableValue.Value);
        }
      }
      return secretMasker;
    }

    protected override IEnumerable<INamedValueInfo> GetSupportedNamedValues()
    {
      foreach (INamedValueInfo supportedNamedValue in base.GetSupportedNamedValues())
        yield return supportedNamedValue;
      yield return (INamedValueInfo) new NamedValueInfo<DependenciesContextNode<TInstance>>("dependencies");
    }
  }
}
