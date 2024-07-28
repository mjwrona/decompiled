// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Pipelines.PipelineContextBuilder
// Assembly: Microsoft.TeamFoundation.DistributedTask.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9201F3B5-DEAF-44A3-860C-DB7B277BB5C6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.WebApi.dll

using Microsoft.TeamFoundation.DistributedTask.Expressions;
using Microsoft.TeamFoundation.DistributedTask.Pipelines.Runtime;
using Microsoft.TeamFoundation.DistributedTask.Pipelines.Validation;
using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Microsoft.TeamFoundation.DistributedTask.Pipelines
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  internal sealed class PipelineContextBuilder
  {
    private IList<IVariable> m_userVariables;
    private VariablesDictionary m_systemVariables;
    private IDictionary<string, bool> m_featureFlags;
    private readonly IPipelineContext m_context;
    private static readonly Lazy<PipelineContextBuilder.VariableGroupComparer> s_comparer = new Lazy<PipelineContextBuilder.VariableGroupComparer>((Func<PipelineContextBuilder.VariableGroupComparer>) (() => new PipelineContextBuilder.VariableGroupComparer()));

    public PipelineContextBuilder()
      : this((IDictionary<string, bool>) null)
    {
    }

    public PipelineContextBuilder(
      IDictionary<string, bool> featureFlags,
      ICounterStore counterStore = null,
      IPackageStore packageStore = null,
      IResourceStore resourceStore = null,
      IList<IStepProvider> stepProviders = null,
      ITaskStore taskStore = null,
      IPipelineIdGenerator idGenerator = null,
      EvaluationOptions expressionOptions = null,
      IList<IPhaseProvider> phaseProviders = null)
    {
      this.m_featureFlags = featureFlags;
      this.EnvironmentVersion = 2;
      this.CounterStore = counterStore ?? (ICounterStore) new Microsoft.TeamFoundation.DistributedTask.Pipelines.CounterStore();
      this.IdGenerator = idGenerator ?? (IPipelineIdGenerator) new PipelineIdGenerator();
      this.PackageStore = packageStore ?? (IPackageStore) new Microsoft.TeamFoundation.DistributedTask.Pipelines.PackageStore((IEnumerable<PackageMetadata>) null, (IPackageResolver) null);
      this.ResourceStore = resourceStore ?? (IResourceStore) new Microsoft.TeamFoundation.DistributedTask.Pipelines.ResourceStore();
      this.StepProviders = stepProviders ?? (IList<IStepProvider>) new List<IStepProvider>();
      this.TaskStore = taskStore ?? (ITaskStore) new Microsoft.TeamFoundation.DistributedTask.Pipelines.TaskStore(Array.Empty<TaskDefinition>());
      this.ExpressionOptions = expressionOptions ?? new EvaluationOptions();
      this.PhaseProviders = phaseProviders ?? (IList<IPhaseProvider>) new List<IPhaseProvider>();
    }

    internal PipelineContextBuilder(IPipelineContext context)
      : this(context.FeatureFlags, context.CounterStore, context.PackageStore, context.ResourceStore, (IList<IStepProvider>) context.StepProviders.ToList<IStepProvider>(), context.TaskStore, context.IdGenerator, context.ExpressionOptions)
    {
      this.m_context = context;
      List<IVariable> variableList = new List<IVariable>();
      VariablesDictionary variablesDictionary = new VariablesDictionary();
      foreach (KeyValuePair<string, VariableValue> variable1 in (IEnumerable<KeyValuePair<string, VariableValue>>) context.Variables)
      {
        if (context.SystemVariableNames.Contains(variable1.Key))
        {
          variablesDictionary[variable1.Key] = variable1.Value.Clone();
        }
        else
        {
          Variable variable2 = new Variable();
          variable2.Name = variable1.Key;
          VariableValue variableValue1 = variable1.Value;
          variable2.Secret = variableValue1 != null && variableValue1.IsSecret;
          variable2.Value = variable1.Value?.Value;
          VariableValue variableValue2 = variable1.Value;
          variable2.Readonly = variableValue2 != null && variableValue2.IsReadOnly;
          Variable variable3 = variable2;
          variableList.Add((IVariable) variable3);
        }
      }
      this.m_userVariables = (IList<IVariable>) variableList.AsReadOnly();
      this.m_systemVariables = variablesDictionary.AsReadOnly();
    }

    public ICounterStore CounterStore { get; }

    public IEnvironmentStore EnvironmentStore { get; }

    public int EnvironmentVersion { get; set; }

    public IPipelineIdGenerator IdGenerator { get; }

    public IPackageStore PackageStore { get; }

    public IResourceStore ResourceStore { get; }

    public IList<IStepProvider> StepProviders { get; }

    public IList<IPhaseProvider> PhaseProviders { get; }

    public ITaskStore TaskStore { get; }

    public EvaluationOptions ExpressionOptions { get; }

    public IList<IVariable> UserVariables
    {
      get
      {
        if (this.m_userVariables == null)
          this.m_userVariables = (IList<IVariable>) new List<IVariable>();
        return this.m_userVariables;
      }
    }

    public IDictionary<string, VariableValue> SystemVariables
    {
      get
      {
        if (this.m_systemVariables == null)
          this.m_systemVariables = new VariablesDictionary();
        return (IDictionary<string, VariableValue>) this.m_systemVariables;
      }
    }

    public IDictionary<string, bool> FeatureFlags
    {
      get
      {
        if (this.m_featureFlags == null)
          this.m_featureFlags = (IDictionary<string, bool>) new Dictionary<string, bool>();
        return this.m_featureFlags;
      }
    }

    public PipelineBuildContext CreateBuildContext(
      BuildOptions options,
      IPackageStore packageStore = null,
      bool includeSecrets = false)
    {
      PipelineBuildContext context;
      if (this.m_context == null)
      {
        context = new PipelineBuildContext(options, this.CounterStore, this.ResourceStore, this.StepProviders, this.TaskStore, packageStore, (IInputValidator) new InputValidator(), expressionOptions: this.ExpressionOptions, phaseProviders: this.PhaseProviders, featureFlags: this.FeatureFlags);
        this.SetVariables((IPipelineContext) context, includeSecrets: includeSecrets);
        context.EnvironmentVersion = this.EnvironmentVersion;
      }
      else
        context = new PipelineBuildContext(this.m_context, options);
      return context;
    }

    public StageExecutionContext CreateStageExecutionContext(
      StageInstance stage,
      IDictionary<string, StageInstance> dependencies = null,
      PipelineState state = PipelineState.InProgress,
      bool includeSecrets = false,
      IPipelineTraceWriter trace = null,
      ExecutionOptions executionOptions = null)
    {
      if (this.m_context != null)
        throw new NotSupportedException();
      StageExecutionContext context = new StageExecutionContext(stage, dependencies, state, this.CounterStore, this.PackageStore, this.ResourceStore, this.TaskStore, this.StepProviders, this.IdGenerator, trace, this.ExpressionOptions, executionOptions, this.FeatureFlags);
      this.SetVariables((IPipelineContext) context, stage, includeSecrets: includeSecrets);
      context.EnvironmentVersion = this.EnvironmentVersion;
      return context;
    }

    public PhaseExecutionContext CreatePhaseExecutionContext(
      StageInstance stage,
      PhaseInstance phase,
      IDictionary<string, PhaseInstance> dependencies = null,
      IDictionary<string, IDictionary<string, PhaseInstance>> stageDependencies = null,
      PipelineState state = PipelineState.InProgress,
      bool includeSecrets = false,
      IPipelineTraceWriter trace = null,
      ExecutionOptions executionOptions = null,
      IDictionary<string, bool> featureFlags = null)
    {
      if (this.m_context != null)
        throw new NotSupportedException();
      PhaseExecutionContext context = new PhaseExecutionContext(stage, phase, dependencies, stageDependencies, state, this.CounterStore, this.PackageStore, this.ResourceStore, this.TaskStore, this.StepProviders, this.IdGenerator, trace, this.ExpressionOptions, executionOptions, featureFlags);
      this.SetVariables((IPipelineContext) context, stage, phase, includeSecrets);
      if (context.FeatureFlags.GetValueOrDefault<string, bool>("DistributedTask.SetSecretDependenciesOutputs"))
      {
        PipelineContextBuilder.SetSecretJobDependenciesOutputs((IPipelineContext) context, dependencies);
        PipelineContextBuilder.SetSecretStageDependenciesOutputs((IPipelineContext) context, stageDependencies);
      }
      context.EnvironmentVersion = this.EnvironmentVersion;
      return context;
    }

    private static void SetSecretJobDependenciesOutputs(
      IPipelineContext context,
      IDictionary<string, PhaseInstance> jobDependencies)
    {
      if (jobDependencies == null)
        return;
      foreach (GraphNodeInstance<PhaseNode> graphNodeInstance in (IEnumerable<PhaseInstance>) jobDependencies.Values)
      {
        foreach (VariableValue variableValue in (IEnumerable<VariableValue>) graphNodeInstance.Outputs.Values)
        {
          if (variableValue.IsSecret)
          {
            variableValue.IsReadOnly = true;
            context.Variables[string.Format("SecretOutput-{0}", (object) Guid.NewGuid())] = variableValue;
          }
        }
      }
    }

    private static void SetSecretStageDependenciesOutputs(
      IPipelineContext context,
      IDictionary<string, IDictionary<string, PhaseInstance>> stageDependencies)
    {
      if (stageDependencies == null)
        return;
      foreach (IDictionary<string, PhaseInstance> jobDependencies in (IEnumerable<IDictionary<string, PhaseInstance>>) stageDependencies.Values)
        PipelineContextBuilder.SetSecretJobDependenciesOutputs(context, jobDependencies);
    }

    private void SetVariables(
      IPipelineContext context,
      StageInstance stage = null,
      PhaseInstance phase = null,
      bool includeSecrets = false)
    {
      Dictionary<string, VariableGroup> source1 = new Dictionary<string, VariableGroup>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      Dictionary<string, ExpressionValue<string>> dictionary = new Dictionary<string, ExpressionValue<string>>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      IList<IVariable> userVariables = this.m_userVariables;
      if ((userVariables != null ? (userVariables.Count > 0 ? 1 : 0) : 0) == 0)
      {
        if (stage != null)
        {
          int? count = stage.Definition?.Variables.Count;
          int num = 0;
          if (count.GetValueOrDefault() > num & count.HasValue)
            goto label_5;
        }
        if (phase != null)
        {
          int? count = phase.Definition?.Variables.Count;
          int num = 0;
          if (!(count.GetValueOrDefault() > num & count.HasValue))
            goto label_40;
        }
        else
          goto label_40;
      }
label_5:
      foreach (IVariable variable1 in this.UserVariables.Concat<IVariable>((IEnumerable<IVariable>) stage?.Definition?.Variables ?? Enumerable.Empty<IVariable>()).Concat<IVariable>((IEnumerable<IVariable>) phase?.Definition?.Variables ?? Enumerable.Empty<IVariable>()))
      {
        if (variable1 is Variable variable2)
        {
          ExpressionValue<string> expressionValue;
          if (ExpressionValue.TryParse<string>(variable2.Value, out expressionValue))
            dictionary[variable2.Name] = expressionValue;
          VariableValue variableValue;
          if (context.Variables.TryGetValue(variable2.Name, out variableValue))
          {
            variableValue.Value = variable2.Value;
            variableValue.IsSecret |= variable2.Secret;
            source1.Remove(variable2.Name);
          }
          else
            context.Variables[variable2.Name] = new VariableValue(variable2.Value, variable2.Secret, variable2.Readonly);
        }
        else if (variable1 is VariableGroupReference queue)
        {
          VariableGroup variableGroup = this.ResourceStore.VariableGroups.Get(queue);
          if (variableGroup == null)
            throw new ResourceNotFoundException(PipelineStrings.VariableGroupNotFound((object) variableGroup));
          SecretStoreConfiguration secretStore = queue.SecretStore;
          if ((secretStore != null ? (secretStore.Keys.Count > 0 ? 1 : 0) : 0) != 0)
          {
            foreach (string key in (IEnumerable<string>) queue.SecretStore.Keys)
            {
              VariableValue variableValue1;
              if (variableGroup.Variables.TryGetValue(key, out variableValue1))
              {
                VariableValue variableValue2;
                if (context.Variables.TryGetValue(key, out variableValue2))
                {
                  variableValue2.Value = variableValue1.Value;
                  variableValue2.IsSecret |= variableValue1.IsSecret;
                  source1[key] = variableGroup;
                }
                else
                {
                  VariableValue variableValue3 = variableValue1.Clone();
                  variableValue3.Value = variableValue1.Value;
                  context.Variables[key] = variableValue3;
                  source1[key] = variableGroup;
                }
              }
            }
          }
          else
          {
            foreach (KeyValuePair<string, VariableValue> keyValuePair in variableGroup.Variables.Where<KeyValuePair<string, VariableValue>>((Func<KeyValuePair<string, VariableValue>, bool>) (v => v.Value != null)))
            {
              VariableValue variableValue4;
              if (context.Variables.TryGetValue(keyValuePair.Key, out variableValue4))
              {
                variableValue4.Value = keyValuePair.Value.Value;
                variableValue4.IsSecret |= keyValuePair.Value.IsSecret;
                source1[keyValuePair.Key] = variableGroup;
              }
              else
              {
                VariableValue variableValue5 = keyValuePair.Value.Clone();
                variableValue5.Value = keyValuePair.Value.Value;
                context.Variables[keyValuePair.Key] = variableValue5;
                source1[keyValuePair.Key] = variableGroup;
              }
            }
          }
        }
      }
label_40:
      VariablesDictionary systemVariables = this.m_systemVariables;
      if ((systemVariables != null ? (systemVariables.Count > 0 ? 1 : 0) : 0) != 0 || stage != null || phase != null)
      {
        VariablesDictionary variablesDictionary = this.m_systemVariables == null ? new VariablesDictionary() : new VariablesDictionary(this.m_systemVariables);
        if (stage != null)
        {
          variablesDictionary[WellKnownDistributedTaskVariables.StageDisplayName] = (VariableValue) (stage.Definition?.DisplayName ?? stage.Name);
          variablesDictionary[WellKnownDistributedTaskVariables.StageId] = (VariableValue) this.IdGenerator.GetStageInstanceId(stage.Name, stage.Attempt).ToString("D");
          variablesDictionary[WellKnownDistributedTaskVariables.StageName] = (VariableValue) stage.Name;
          variablesDictionary[WellKnownDistributedTaskVariables.StageAttempt] = (VariableValue) stage.Attempt.ToString();
        }
        if (phase != null)
        {
          variablesDictionary[WellKnownDistributedTaskVariables.PhaseDisplayName] = (VariableValue) (phase.Definition?.DisplayName ?? phase.Name);
          variablesDictionary[WellKnownDistributedTaskVariables.PhaseId] = (VariableValue) this.IdGenerator.GetPhaseInstanceId(stage?.Name, phase.Name, phase.Attempt).ToString("D");
          variablesDictionary[WellKnownDistributedTaskVariables.PhaseName] = (VariableValue) phase.Name;
          variablesDictionary[WellKnownDistributedTaskVariables.PhaseAttempt] = (VariableValue) phase.Attempt.ToString();
        }
        foreach (KeyValuePair<string, VariableValue> keyValuePair in variablesDictionary)
        {
          source1.Remove(keyValuePair.Key);
          IDictionary<string, VariableValue> variables = context.Variables;
          string key = keyValuePair.Key;
          VariableValue variableValue6 = keyValuePair.Value;
          VariableValue variableValue7 = variableValue6 != null ? variableValue6.Clone() : (VariableValue) null;
          variables[key] = variableValue7;
          ExpressionValue<string> expressionValue;
          if (ExpressionValue.TryParse<string>(keyValuePair.Value?.Value, out expressionValue))
            dictionary[keyValuePair.Key] = expressionValue;
          context.SystemVariableNames.Add(keyValuePair.Key);
        }
      }
      if (source1.Count > 0 || dictionary.Count > 0)
        context.Trace?.EnterProperty("Variables");
      if (source1.Count > 0)
      {
        foreach (IGrouping<VariableGroup, string> source2 in source1.GroupBy<KeyValuePair<string, VariableGroup>, VariableGroup, string>((Func<KeyValuePair<string, VariableGroup>, VariableGroup>) (x => x.Value), (Func<KeyValuePair<string, VariableGroup>, string>) (x => x.Key), (IEqualityComparer<VariableGroup>) PipelineContextBuilder.s_comparer.Value))
        {
          VariableGroupReference groupReference = PipelineContextBuilder.ToGroupReference(source2.Key, (IList<string>) source2.ToList<string>());
          if (groupReference?.SecretStore != null)
          {
            context.ReferencedResources.VariableGroups.Add(groupReference);
            if (groupReference.SecretStore.Endpoint != null)
            {
              this.ResourceStore.Endpoints.Authorize(groupReference.SecretStore.Endpoint);
              context.ReferencedResources.Endpoints.Add(groupReference.SecretStore.Endpoint);
            }
            if (groupReference.SecretStore.Keys.Count != 0)
            {
              IVariableValueProvider valueProvider = this.ResourceStore.VariableGroups.GetValueProvider(groupReference);
              if (valueProvider != null)
              {
                VariableGroup variableGroup = this.ResourceStore.GetVariableGroup(groupReference);
                ServiceEndpoint endpoint = (ServiceEndpoint) null;
                if (groupReference.SecretStore.Endpoint != null)
                {
                  endpoint = this.ResourceStore.GetEndpoint(groupReference.SecretStore.Endpoint);
                  if (endpoint == null)
                    throw new DistributedTaskException(PipelineStrings.ServiceConnectionUsedInVariableGroupNotValid((object) groupReference.SecretStore.Endpoint, (object) groupReference.Name));
                }
                if (!valueProvider.ShouldGetValues(context))
                {
                  foreach (string key in (IEnumerable<string>) groupReference.SecretStore.Keys)
                  {
                    context.Trace?.Info(key + ": $[ variablegroups." + variableGroup.Name + "." + key + " ]");
                    dictionary.Remove(key);
                    context.Variables[key] = new VariableValue((string) null, true);
                  }
                }
                else
                {
                  IDictionary<string, VariableValue> values = valueProvider.GetValues(variableGroup, endpoint, (IEnumerable<string>) groupReference.SecretStore.Keys, includeSecrets);
                  if (values != null)
                  {
                    foreach (KeyValuePair<string, VariableValue> keyValuePair in (IEnumerable<KeyValuePair<string, VariableValue>>) values)
                    {
                      context.Trace?.Info(keyValuePair.Key + ": $[ variablegroups." + variableGroup.Name + "." + keyValuePair.Key + " ]");
                      dictionary.Remove(keyValuePair.Key);
                      if (includeSecrets || !keyValuePair.Value.IsSecret)
                        context.Variables[keyValuePair.Key] = keyValuePair.Value;
                      else
                        context.Variables[keyValuePair.Key].Value = (string) null;
                    }
                  }
                }
              }
            }
          }
        }
      }
      if (dictionary.Count > 0)
      {
        foreach (KeyValuePair<string, ExpressionValue<string>> keyValuePair in dictionary)
        {
          context.Trace?.EnterProperty(keyValuePair.Key);
          ExpressionResult<string> expressionResult = keyValuePair.Value.GetValue(context);
          context.Trace?.LeaveProperty(keyValuePair.Key);
          VariableValue variableValue;
          if (context.Variables.TryGetValue(keyValuePair.Key, out variableValue))
          {
            variableValue.Value = expressionResult.Value;
            variableValue.IsSecret |= expressionResult.ContainsSecrets;
          }
          else
            context.Variables[keyValuePair.Key] = new VariableValue(expressionResult.Value, expressionResult.ContainsSecrets);
        }
      }
      if (!includeSecrets)
      {
        foreach (VariableValue variableValue in context.Variables.Values.Where<VariableValue>((Func<VariableValue, bool>) (x => x.IsSecret)))
          variableValue.Value = (string) null;
      }
      if (source1.Count <= 0 && dictionary.Count <= 0)
        return;
      context.Trace?.LeaveProperty("Variables");
    }

    private static VariableGroupReference ToGroupReference(VariableGroup group, IList<string> keys)
    {
      if (group == null || keys == null || keys.Count == 0)
        return (VariableGroupReference) null;
      SecretStoreConfiguration storeConfiguration = PipelineContextBuilder.ToSecretStoreConfiguration(group, keys);
      VariableGroupReference groupReference = new VariableGroupReference();
      groupReference.Id = group.Id;
      groupReference.Name = (ExpressionValue<string>) group.Name;
      groupReference.GroupType = group.Type;
      groupReference.SecretStore = storeConfiguration;
      return groupReference;
    }

    private static SecretStoreConfiguration ToSecretStoreConfiguration(
      VariableGroup group,
      IList<string> keys)
    {
      if (keys.Count == 0)
        return (SecretStoreConfiguration) null;
      AzureKeyVaultVariableGroupProviderData providerData = group.ProviderData as AzureKeyVaultVariableGroupProviderData;
      SecretStoreConfiguration storeConfiguration = new SecretStoreConfiguration()
      {
        StoreName = providerData?.Vault ?? group.Name
      };
      if (providerData != null && providerData.ServiceEndpointId != Guid.Empty)
        storeConfiguration.Endpoint = new ServiceEndpointReference()
        {
          Id = providerData.ServiceEndpointId
        };
      storeConfiguration.Keys.AddRange<string, IList<string>>((IEnumerable<string>) keys);
      return storeConfiguration;
    }

    private sealed class VariableGroupComparer : IEqualityComparer<VariableGroup>
    {
      public bool Equals(VariableGroup x, VariableGroup y)
      {
        int? id1 = x?.Id;
        int? id2 = y?.Id;
        return id1.GetValueOrDefault() == id2.GetValueOrDefault() & id1.HasValue == id2.HasValue;
      }

      public int GetHashCode(VariableGroup obj) => obj.Id.GetHashCode();
    }
  }
}
