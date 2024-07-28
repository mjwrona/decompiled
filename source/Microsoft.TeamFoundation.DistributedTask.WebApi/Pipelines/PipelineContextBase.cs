// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Pipelines.PipelineContextBase
// Assembly: Microsoft.TeamFoundation.DistributedTask.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9201F3B5-DEAF-44A3-860C-DB7B277BB5C6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.WebApi.dll

using Microsoft.TeamFoundation.DistributedTask.Expressions;
using Microsoft.TeamFoundation.DistributedTask.Logging;
using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.VisualStudio.Services.Common;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Microsoft.TeamFoundation.DistributedTask.Pipelines
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public abstract class PipelineContextBase : IPipelineContext
  {
    private VariablesDictionary m_variables;
    private HashSet<string> m_systemVariables;
    private Lazy<ISecretMasker> m_secretMasker;
    private PipelineResources m_referencedResources;
    private IDictionary<string, bool> m_featureFlags;
    private Dictionary<string, IExpressionNode> m_parsedExpressions = new Dictionary<string, IExpressionNode>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);

    private protected PipelineContextBase(IPipelineContext context)
    {
      this.EnvironmentVersion = context.EnvironmentVersion;
      this.SystemVariableNames.UnionWith((IEnumerable<string>) context.SystemVariableNames);
      this.Variables.AddRange<KeyValuePair<string, VariableValue>, IDictionary<string, VariableValue>>((IEnumerable<KeyValuePair<string, VariableValue>>) context.Variables);
      this.m_referencedResources = context.ReferencedResources?.Clone();
      this.CounterStore = context.CounterStore;
      this.ExpressionOptions = context.ExpressionOptions ?? new EvaluationOptions();
      this.IdGenerator = context.IdGenerator ?? (IPipelineIdGenerator) new PipelineIdGenerator();
      this.PackageStore = context.PackageStore;
      this.ResourceStore = context.ResourceStore;
      this.TaskStore = context.TaskStore;
      this.Trace = context.Trace;
      this.m_secretMasker = new Lazy<ISecretMasker>((Func<ISecretMasker>) (() => this.CreateSecretMasker()));
      this.StepProviders = context.StepProviders;
      this.FeatureFlags.AddRange<KeyValuePair<string, bool>, IDictionary<string, bool>>((IEnumerable<KeyValuePair<string, bool>>) context.FeatureFlags);
    }

    private protected PipelineContextBase(
      ICounterStore counterStore,
      IPackageStore packageStore,
      IResourceStore resourceStore,
      ITaskStore taskStore,
      IList<IStepProvider> stepProviders,
      IPipelineIdGenerator idGenerator = null,
      IPipelineTraceWriter trace = null,
      EvaluationOptions expressionOptions = null,
      IDictionary<string, bool> featureFlags = null)
    {
      this.CounterStore = counterStore;
      this.ExpressionOptions = expressionOptions ?? new EvaluationOptions();
      this.IdGenerator = idGenerator ?? (IPipelineIdGenerator) new PipelineIdGenerator();
      this.PackageStore = packageStore;
      this.ResourceStore = resourceStore;
      this.TaskStore = taskStore;
      this.Trace = trace;
      if (featureFlags != null)
        this.FeatureFlags.AddRange<KeyValuePair<string, bool>, IDictionary<string, bool>>((IEnumerable<KeyValuePair<string, bool>>) featureFlags);
      this.m_secretMasker = new Lazy<ISecretMasker>((Func<ISecretMasker>) (() => this.CreateSecretMasker()));
      List<IStepProvider> stepProviderList = new List<IStepProvider>();
      if (this.ResourceStore != null)
        stepProviderList.Add((IStepProvider) this.ResourceStore);
      if (stepProviders != null)
        stepProviderList.AddRange((IEnumerable<IStepProvider>) stepProviders);
      this.StepProviders = (IReadOnlyList<IStepProvider>) stepProviderList;
    }

    public ICounterStore CounterStore { get; }

    public int EnvironmentVersion { get; internal set; }

    public EvaluationOptions ExpressionOptions { get; }

    public IPipelineIdGenerator IdGenerator { get; }

    public IPackageStore PackageStore { get; }

    public PipelineResources ReferencedResources
    {
      get
      {
        if (this.m_referencedResources == null)
          this.m_referencedResources = new PipelineResources();
        return this.m_referencedResources;
      }
    }

    public IResourceStore ResourceStore { get; }

    public IReadOnlyList<IStepProvider> StepProviders { get; }

    public ISecretMasker SecretMasker => this.m_secretMasker.Value;

    public ITaskStore TaskStore { get; }

    public IPipelineTraceWriter Trace { get; }

    public ISet<string> SystemVariableNames
    {
      get
      {
        if (this.m_systemVariables == null)
          this.m_systemVariables = new HashSet<string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
        return (ISet<string>) this.m_systemVariables;
      }
    }

    public IDictionary<string, VariableValue> Variables
    {
      get
      {
        if (this.m_variables == null)
          this.m_variables = new VariablesDictionary();
        return (IDictionary<string, VariableValue>) this.m_variables;
      }
    }

    protected virtual bool SecretsAccessed
    {
      get
      {
        VariablesDictionary variables = this.m_variables;
        return variables != null && variables.SecretsAccessed.Count > 0;
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

    public virtual ISecretMasker CreateSecretMasker()
    {
      Microsoft.TeamFoundation.DistributedTask.Logging.SecretMasker secretMasker = new Microsoft.TeamFoundation.DistributedTask.Logging.SecretMasker();
      VariablesDictionary variables = this.m_variables;
      if ((variables != null ? (variables.Count > 0 ? 1 : 0) : 0) != 0)
      {
        foreach (VariableValue variableValue in this.m_variables.Values.Where<VariableValue>((Func<VariableValue, bool>) (x => x.IsSecret)))
          secretMasker.AddValue(variableValue.Value);
      }
      return (ISecretMasker) secretMasker;
    }

    public string ExpandVariables(string value, bool maskSecrets = false)
    {
      if (!string.IsNullOrEmpty(value))
      {
        VariablesDictionary variables = this.m_variables;
        if ((variables != null ? (variables.Count > 0 ? 1 : 0) : 0) != 0)
          return VariableUtility.ExpandVariables(value, this.m_variables, false, maskSecrets);
      }
      return value;
    }

    public JToken ExpandVariables(JToken value)
    {
      if (value != null)
      {
        VariablesDictionary variables = this.m_variables;
        if ((variables != null ? (variables.Count > 0 ? 1 : 0) : 0) != 0)
          return VariableUtility.ExpandVariables(value, this.m_variables, false);
      }
      return value;
    }

    public ExpressionResult<JObject> Evaluate(JObject value)
    {
      if (value == null)
        return (ExpressionResult<JObject>) null;
      bool containsSecrets = false;
      JObject jobject = new JObject();
      foreach (KeyValuePair<string, JToken> keyValuePair in value)
      {
        JToken jtoken = keyValuePair.Value;
        switch (jtoken.Type)
        {
          case JTokenType.Object:
            ExpressionResult<JObject> expressionResult = this.Evaluate(jtoken.Value<JObject>());
            containsSecrets |= expressionResult.ContainsSecrets;
            jobject[keyValuePair.Key] = (JToken) expressionResult.Value;
            continue;
          case JTokenType.String:
            jobject[keyValuePair.Key] = (JToken) this.ExpandVariables(ResolveExpression(jtoken.Value<string>()), false);
            continue;
          default:
            jobject[keyValuePair.Key] = jtoken;
            continue;
        }
      }
      return new ExpressionResult<JObject>(jobject, containsSecrets);

      string ResolveExpression(string s)
      {
        if (!ExpressionValue.IsExpression(s))
          return s;
        string str;
        try
        {
          ExpressionResult<string> expressionResult = this.Evaluate<string>(ExpressionValue.TrimExpression(s));
          containsSecrets |= expressionResult.ContainsSecrets;
          str = expressionResult.Value;
        }
        catch (ExpressionException ex)
        {
          return s;
        }
        return !string.IsNullOrEmpty(str) ? str : s;
      }
    }

    public ExpressionResult<T> Evaluate<T>(string expression)
    {
      IExpressionNode tree;
      if (!this.m_parsedExpressions.TryGetValue(expression, out tree))
      {
        tree = new ExpressionParser().CreateTree(expression, (ITraceWriter) this.Trace, this.GetSupportedNamedValues(), this.GetSupportedFunctions());
        this.m_parsedExpressions.Add(expression, tree);
      }
      this.ResetSecretsAccessed();
      return new ExpressionResult<T>(tree.Evaluate<T>((ITraceWriter) this.Trace, this.SecretMasker, (object) this, this.ExpressionOptions), this.SecretsAccessed);
    }

    protected virtual void ResetSecretsAccessed() => this.m_variables?.SecretsAccessed.Clear();

    protected virtual IEnumerable<IFunctionInfo> GetSupportedFunctions()
    {
      yield return Microsoft.TeamFoundation.DistributedTask.Pipelines.Expressions.ExpressionConstants.CounterFunction;
    }

    protected virtual IEnumerable<INamedValueInfo> GetSupportedNamedValues()
    {
      yield return Microsoft.TeamFoundation.DistributedTask.Pipelines.Expressions.ExpressionConstants.PipelineNamedValue;
      yield return Microsoft.TeamFoundation.DistributedTask.Pipelines.Expressions.ExpressionConstants.ResourcesNamedValue;
      yield return Microsoft.TeamFoundation.DistributedTask.Pipelines.Expressions.ExpressionConstants.VariablesNamedValue;
    }

    internal void SetSystemVariables(IEnumerable<Variable> variables)
    {
      foreach (Variable variable in variables)
      {
        this.SystemVariableNames.Add(variable.Name);
        this.Variables[variable.Name] = new VariableValue(variable.Value, variable.Secret, variable.Readonly);
      }
    }

    internal void SetUserVariables(IEnumerable<Variable> variables)
    {
      foreach (Variable variable in variables.Where<Variable>((Func<Variable, bool>) (x => !x.Name.StartsWith("system.", StringComparison.OrdinalIgnoreCase) && !this.SystemVariableNames.Contains(x.Name))))
        this.Variables[variable.Name] = new VariableValue(variable.Value, variable.Secret, variable.Readonly);
    }

    internal void SetUserVariables(IDictionary<string, string> variables)
    {
      foreach (KeyValuePair<string, string> keyValuePair in variables.Where<KeyValuePair<string, string>>((Func<KeyValuePair<string, string>, bool>) (x => !x.Key.StartsWith("system.", StringComparison.OrdinalIgnoreCase) && !this.SystemVariableNames.Contains(x.Key))))
        this.Variables[keyValuePair.Key] = (VariableValue) keyValuePair.Value;
    }

    internal void SetSystemVariables(IDictionary<string, VariableValue> variables)
    {
      if (variables == null || variables.Count <= 0)
        return;
      foreach (KeyValuePair<string, VariableValue> variable in (IEnumerable<KeyValuePair<string, VariableValue>>) variables)
      {
        this.SystemVariableNames.Add(variable.Key);
        IDictionary<string, VariableValue> variables1 = this.Variables;
        string key = variable.Key;
        VariableValue variableValue1 = variable.Value;
        VariableValue variableValue2 = variableValue1 != null ? variableValue1.Clone() : (VariableValue) null;
        variables1[key] = variableValue2;
      }
    }
  }
}
