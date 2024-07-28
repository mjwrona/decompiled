// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.OutputVariableScope
// Assembly: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07FD5059-3D25-415E-AA3A-5372051D7E71
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.dll

using Microsoft.TeamFoundation.DistributedTask.Pipelines;
using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.VisualStudio.Services.Common;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.DistributedTask.Orchestration.Server
{
  [DataContract]
  public sealed class OutputVariableScope
  {
    private List<OutputVariableScope> m_childScopes;
    private Dictionary<string, VariableValue> m_variables;

    [JsonConstructor]
    public OutputVariableScope()
    {
    }

    public OutputVariableScope(Guid scopeId, string scopeType, string name)
      : this(scopeId, scopeType, name, (IDictionary<string, VariableValue>) null)
    {
    }

    public OutputVariableScope(
      Guid scopeId,
      string scopeType,
      string name,
      IDictionary<string, VariableValue> variables)
    {
      this.Id = scopeId;
      this.ScopeType = scopeType;
      this.Name = name;
      if (variables == null)
        return;
      this.m_variables = new Dictionary<string, VariableValue>(variables, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
    }

    [DataMember]
    public Guid Id { get; internal set; }

    [DataMember]
    public string Name { get; internal set; }

    [DataMember]
    public string ScopeType { get; internal set; }

    [DataMember(EmitDefaultValue = false)]
    public TaskResult? Result { get; internal set; }

    public List<OutputVariableScope> ChildScopes
    {
      get
      {
        if (this.m_childScopes == null)
          this.m_childScopes = new List<OutputVariableScope>();
        return this.m_childScopes;
      }
    }

    public IDictionary<string, VariableValue> Variables
    {
      get
      {
        if (this.m_variables == null)
          this.m_variables = new Dictionary<string, VariableValue>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
        return (IDictionary<string, VariableValue>) this.m_variables;
      }
    }

    public IEnumerable<OutputVariableScope> FindAll(Predicate<OutputVariableScope> matches)
    {
      OutputVariableScope outputVariableScope1 = this;
      ArgumentUtility.CheckForNull<Predicate<OutputVariableScope>>(matches, nameof (matches));
      if (matches(outputVariableScope1))
        yield return outputVariableScope1;
      if (outputVariableScope1.ChildScopes.Count > 0)
      {
        Queue<OutputVariableScope> scopesToSearch = new Queue<OutputVariableScope>((IEnumerable<OutputVariableScope>) outputVariableScope1.ChildScopes);
        while (scopesToSearch.Count > 0)
        {
          OutputVariableScope outputVariableScope2 = scopesToSearch.Dequeue();
          if (matches(outputVariableScope2))
          {
            yield return outputVariableScope2;
          }
          else
          {
            foreach (OutputVariableScope childScope in outputVariableScope2.ChildScopes)
              scopesToSearch.Enqueue(childScope);
          }
        }
        scopesToSearch = (Queue<OutputVariableScope>) null;
      }
    }

    public IDictionary<string, VariableValue> Flatten(bool fullyQualified = true, bool includeSecrets = true)
    {
      Dictionary<string, VariableValue> variables = new Dictionary<string, VariableValue>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      this.Populate((string) null, fullyQualified, includeSecrets, (IDictionary<string, VariableValue>) variables);
      return (IDictionary<string, VariableValue>) variables;
    }

    internal bool IsValidChild(string childScopeType)
    {
      if (this.ScopeType.Equals("Plan", StringComparison.OrdinalIgnoreCase))
        return childScopeType.Equals("Job", StringComparison.OrdinalIgnoreCase) || childScopeType.Equals("Phase", StringComparison.OrdinalIgnoreCase) || childScopeType.Equals("Stage", StringComparison.OrdinalIgnoreCase);
      if (this.ScopeType.Equals("Stage", StringComparison.OrdinalIgnoreCase))
        return childScopeType.Equals("Phase", StringComparison.OrdinalIgnoreCase);
      return this.ScopeType.Equals("Phase", StringComparison.OrdinalIgnoreCase) ? childScopeType.Equals("Job", StringComparison.OrdinalIgnoreCase) || childScopeType.Equals("Task", StringComparison.OrdinalIgnoreCase) : (this.ScopeType.Equals("Job", StringComparison.OrdinalIgnoreCase) || this.ScopeType.Equals("Task", StringComparison.OrdinalIgnoreCase)) && childScopeType.Equals("Task", StringComparison.OrdinalIgnoreCase);
    }

    private void Populate(
      string parentScope,
      bool fullyQualified,
      bool includeSecrets,
      IDictionary<string, VariableValue> variables)
    {
      string str = string.Empty;
      if (fullyQualified)
      {
        if (string.IsNullOrEmpty(parentScope) || parentScope.Equals(PipelineConstants.DefaultJobName, StringComparison.OrdinalIgnoreCase))
        {
          if (!string.Equals(this.Name, PipelineConstants.DefaultJobName, StringComparison.OrdinalIgnoreCase))
            str = this.Name;
        }
        else
          str = !string.Equals(this.Name, PipelineConstants.DefaultJobName, StringComparison.OrdinalIgnoreCase) ? string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}.{1}", (object) parentScope, (object) this.Name) : parentScope;
      }
      foreach (KeyValuePair<string, VariableValue> keyValuePair in this.Variables.Where<KeyValuePair<string, VariableValue>>((Func<KeyValuePair<string, VariableValue>, bool>) (x =>
      {
        if (x.Value == null)
          return false;
        return includeSecrets || !x.Value.IsSecret;
      })))
        variables[OutputVariableScope.QualifyName(str, keyValuePair.Key)] = keyValuePair.Value;
      foreach (OutputVariableScope childScope in this.ChildScopes)
        childScope.Populate(str, fullyQualified, includeSecrets, variables);
    }

    private static string QualifyName(string scope, string name) => string.IsNullOrEmpty(scope) ? name : string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}.{1}", (object) scope, (object) name);
  }
}
