// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.WebApi.PlanEnvironment
// Assembly: Microsoft.TeamFoundation.DistributedTask.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9201F3B5-DEAF-44A3-860C-DB7B277BB5C6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.WebApi.dll

using Microsoft.TeamFoundation.DistributedTask.Pipelines;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.DistributedTask.WebApi
{
  [DataContract]
  public sealed class PlanEnvironment : IOrchestrationEnvironment
  {
    private List<MaskHint> m_maskHints;
    private Dictionary<Guid, JobOption> m_options;
    private VariablesDictionary m_variables;
    [DataMember(Name = "Mask", EmitDefaultValue = false)]
    private List<MaskHint> m_serializedMaskHints;
    [DataMember(Name = "Options", EmitDefaultValue = false)]
    private Dictionary<Guid, JobOption> m_serializedOptions;
    [DataMember(Name = "Variables", EmitDefaultValue = false)]
    private IDictionary<string, string> m_serializedVariables;

    public PlanEnvironment()
    {
    }

    private PlanEnvironment(PlanEnvironment environmentToClone)
    {
      if (environmentToClone.m_options != null)
        this.m_options = this.m_options.ToDictionary<KeyValuePair<Guid, JobOption>, Guid, JobOption>((Func<KeyValuePair<Guid, JobOption>, Guid>) (x => x.Key), (Func<KeyValuePair<Guid, JobOption>, JobOption>) (x => x.Value.Clone()));
      if (environmentToClone.m_maskHints != null)
        this.m_maskHints = environmentToClone.m_maskHints.Select<MaskHint, MaskHint>((Func<MaskHint, MaskHint>) (x => x.Clone())).ToList<MaskHint>();
      if (environmentToClone.m_variables == null)
        return;
      this.m_variables = new VariablesDictionary(environmentToClone.m_variables);
    }

    public List<MaskHint> MaskHints
    {
      get
      {
        if (this.m_maskHints == null)
          this.m_maskHints = new List<MaskHint>();
        return this.m_maskHints;
      }
    }

    public IDictionary<Guid, JobOption> Options
    {
      get
      {
        if (this.m_options == null)
          this.m_options = new Dictionary<Guid, JobOption>();
        return (IDictionary<Guid, JobOption>) this.m_options;
      }
    }

    public IDictionary<string, string> Variables
    {
      get
      {
        if (this.m_variables == null)
          this.m_variables = new VariablesDictionary();
        return (IDictionary<string, string>) this.m_variables;
      }
    }

    OrchestrationProcessType IOrchestrationEnvironment.ProcessType => OrchestrationProcessType.Container;

    IDictionary<string, VariableValue> IOrchestrationEnvironment.Variables
    {
      get
      {
        if (this.m_variables == null)
          this.m_variables = new VariablesDictionary();
        return (IDictionary<string, VariableValue>) this.m_variables;
      }
    }

    [System.Runtime.Serialization.OnDeserialized]
    private void OnDeserialized(StreamingContext context)
    {
      SerializationHelper.Copy<Guid, JobOption>(ref this.m_serializedOptions, ref this.m_options, true);
      SerializationHelper.Copy<MaskHint>(ref this.m_serializedMaskHints, ref this.m_maskHints, true);
      List<MaskHint> maskHints = this.m_maskHints;
      HashSet<string> stringSet = new HashSet<string>((IEnumerable<string>) ((maskHints != null ? (object) maskHints.Where<MaskHint>((Func<MaskHint, bool>) (x => x.Type == MaskType.Variable)).Select<MaskHint, string>((Func<MaskHint, string>) (x => x.Value)) : (object) null) ?? (object) Array.Empty<string>()), (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      if (this.m_serializedVariables != null && this.m_serializedVariables.Count > 0)
      {
        this.m_variables = new VariablesDictionary();
        foreach (KeyValuePair<string, string> serializedVariable in (IEnumerable<KeyValuePair<string, string>>) this.m_serializedVariables)
          this.m_variables[serializedVariable.Key] = new VariableValue(serializedVariable.Value, stringSet.Contains(serializedVariable.Key));
      }
      this.m_serializedVariables = (IDictionary<string, string>) null;
    }

    [System.Runtime.Serialization.OnSerialized]
    private void OnSerialized(StreamingContext context)
    {
      this.m_serializedOptions = (Dictionary<Guid, JobOption>) null;
      this.m_serializedMaskHints = (List<MaskHint>) null;
      this.m_serializedVariables = (IDictionary<string, string>) null;
    }

    [System.Runtime.Serialization.OnSerializing]
    private void OnSerializing(StreamingContext context)
    {
      SerializationHelper.Copy<Guid, JobOption>(ref this.m_options, ref this.m_serializedOptions);
      SerializationHelper.Copy<MaskHint>(ref this.m_maskHints, ref this.m_serializedMaskHints);
      if (this.m_variables == null || this.m_variables.Count <= 0)
        return;
      this.m_serializedVariables = (IDictionary<string, string>) new Dictionary<string, string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      foreach (KeyValuePair<string, VariableValue> variable in this.m_variables)
        this.m_serializedVariables[variable.Key] = variable.Value?.Value;
    }
  }
}
