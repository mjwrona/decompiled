// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Pipelines.PipelineEnvironment
// Assembly: Microsoft.TeamFoundation.DistributedTask.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9201F3B5-DEAF-44A3-860C-DB7B277BB5C6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.WebApi.dll

using Microsoft.TeamFoundation.DistributedTask.WebApi;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.DistributedTask.Pipelines
{
  [DataContract]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class PipelineEnvironment : IOrchestrationEnvironment
  {
    [DataMember(Name = "Counters", EmitDefaultValue = false)]
    private Dictionary<string, int> m_counters;
    [DataMember(Name = "Options")]
    private ExecutionOptions m_options = new ExecutionOptions();
    [DataMember(Name = "ProcessType")]
    private OrchestrationProcessType m_processType = OrchestrationProcessType.Pipeline;
    [DataMember(Name = "Resources", EmitDefaultValue = false)]
    private PipelineResources m_resources;
    [DataMember(Name = "SystemVariables", EmitDefaultValue = false)]
    private VariablesDictionary m_systemVariables;
    [DataMember(Name = "UserVariables", EmitDefaultValue = false)]
    private IList<IVariable> m_userVariables;
    [DataMember(Name = "Variables", EmitDefaultValue = false)]
    private VariablesDictionary m_variables;
    [DataMember(Name = "YamlTemplateReferences", EmitDefaultValue = false)]
    private IList<YamlTemplateReference> m_yamlTemplateReferences;

    public PipelineEnvironment() => this.Version = 1;

    public IList<YamlTemplateReference> YamlTemplateReferences
    {
      get
      {
        if (this.m_yamlTemplateReferences == null)
          this.m_yamlTemplateReferences = (IList<YamlTemplateReference>) new List<YamlTemplateReference>();
        return this.m_yamlTemplateReferences;
      }
    }

    public PipelineResources Resources
    {
      get
      {
        if (this.m_resources == null)
          this.m_resources = new PipelineResources();
        return this.m_resources;
      }
      set => this.m_resources = value;
    }

    public IDictionary<string, int> Counters
    {
      get
      {
        if (this.m_counters == null)
          this.m_counters = new Dictionary<string, int>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
        return (IDictionary<string, int>) this.m_counters;
      }
    }

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

    [Obsolete("This property is obsolete. Use UserVariables and/or SystemVariables instead")]
    public IDictionary<string, VariableValue> Variables
    {
      get
      {
        if (this.m_variables == null)
          this.m_variables = new VariablesDictionary();
        return (IDictionary<string, VariableValue>) this.m_variables;
      }
    }

    public ExecutionOptions Options => this.m_options;

    [DefaultValue(1)]
    [DataMember(Name = "Version", EmitDefaultValue = false)]
    public int Version { get; set; }

    OrchestrationProcessType IOrchestrationEnvironment.ProcessType => this.m_processType;

    [System.Runtime.Serialization.OnSerializing]
    private void OnSerializing(StreamingContext context)
    {
      PipelineResources resources = this.m_resources;
      if ((resources != null ? (resources.Count == 0 ? 1 : 0) : 0) != 0)
        this.m_resources = (PipelineResources) null;
      Dictionary<string, int> counters = this.m_counters;
      // ISSUE: explicit non-virtual call
      if ((counters != null ? (__nonvirtual (counters.Count) == 0 ? 1 : 0) : 0) != 0)
        this.m_counters = (Dictionary<string, int>) null;
      IList<IVariable> userVariables = this.m_userVariables;
      if ((userVariables != null ? (userVariables.Count == 0 ? 1 : 0) : 0) != 0)
        this.m_userVariables = (IList<IVariable>) null;
      VariablesDictionary systemVariables = this.m_systemVariables;
      if ((systemVariables != null ? (systemVariables.Count == 0 ? 1 : 0) : 0) != 0)
        this.m_systemVariables = (VariablesDictionary) null;
      VariablesDictionary variables = this.m_variables;
      if ((variables != null ? (variables.Count == 0 ? 1 : 0) : 0) != 0)
        this.m_variables = (VariablesDictionary) null;
      IList<YamlTemplateReference> templateReferences = this.m_yamlTemplateReferences;
      if ((templateReferences != null ? (templateReferences.Count == 0 ? 1 : 0) : 0) == 0)
        return;
      this.m_yamlTemplateReferences = (IList<YamlTemplateReference>) null;
    }
  }
}
