// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.WebApi.BuildDefinition
// Assembly: Microsoft.TeamFoundation.Build2.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0683407D-7C61-4505-8CA6-86AD7E3B6BCA
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Build2.WebApi.dll

using Microsoft.TeamFoundation.DistributedTask.Common.Contracts;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Build.WebApi
{
  [DataContract]
  public class BuildDefinition : BuildDefinitionReference
  {
    [DataMember(Name = "Options", EmitDefaultValue = false)]
    private List<BuildOption> m_serializedOptions;
    [DataMember(Name = "Triggers", EmitDefaultValue = false)]
    private List<BuildTrigger> m_serializedTriggers;
    [DataMember(Name = "Variables", EmitDefaultValue = false)]
    private IDictionary<string, BuildDefinitionVariable> m_serializedVariables;
    [DataMember(Name = "VariableGroups", EmitDefaultValue = false)]
    private List<VariableGroup> m_serializedVariableGroups;
    [DataMember(Name = "Demands", EmitDefaultValue = false)]
    private List<Demand> m_serializedDemands;
    [DataMember(Name = "RetentionRules", EmitDefaultValue = false)]
    private List<RetentionPolicy> m_serializedRetentionRules;
    [DataMember(IsRequired = false, EmitDefaultValue = false, Name = "Properties")]
    private PropertiesCollection m_properties;
    [DataMember(EmitDefaultValue = false, Name = "Tags")]
    private List<string> m_tags;
    private List<Demand> m_demands;
    private List<BuildOption> m_options;
    private List<BuildTrigger> m_triggers;
    private List<RetentionPolicy> m_retentionRules;
    private List<VariableGroup> m_variableGroups;
    private IDictionary<string, BuildDefinitionVariable> m_variables;

    public BuildDefinition() => this.JobAuthorizationScope = BuildAuthorizationScope.ProjectCollection;

    [DataMember(EmitDefaultValue = false)]
    public string BuildNumberFormat { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string Comment { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string Description { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string DropLocation { get; set; }

    [DataMember]
    public BuildAuthorizationScope JobAuthorizationScope { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public int JobTimeoutInMinutes { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public int JobCancelTimeoutInMinutes { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public bool BadgeEnabled { get; set; }

    [Obsolete]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public List<BuildDefinitionStep> Steps { get; }

    [DataMember(EmitDefaultValue = false)]
    public BuildProcess Process { get; set; }

    public List<BuildOption> Options
    {
      get
      {
        if (this.m_options == null)
          this.m_options = new List<BuildOption>();
        return this.m_options;
      }
      internal set => this.m_options = value;
    }

    [DataMember(EmitDefaultValue = false)]
    public BuildRepository Repository { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public ProcessParameters ProcessParameters { get; set; }

    public List<BuildTrigger> Triggers
    {
      get
      {
        if (this.m_triggers == null)
          this.m_triggers = new List<BuildTrigger>();
        return this.m_triggers;
      }
      internal set => this.m_triggers = value;
    }

    public IDictionary<string, BuildDefinitionVariable> Variables
    {
      get
      {
        if (this.m_variables == null)
          this.m_variables = (IDictionary<string, BuildDefinitionVariable>) new Dictionary<string, BuildDefinitionVariable>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
        return this.m_variables;
      }
      internal set => this.m_variables = (IDictionary<string, BuildDefinitionVariable>) new Dictionary<string, BuildDefinitionVariable>(value, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
    }

    public List<VariableGroup> VariableGroups
    {
      get
      {
        if (this.m_variableGroups == null)
          this.m_variableGroups = new List<VariableGroup>();
        return this.m_variableGroups;
      }
      internal set => this.m_variableGroups = value;
    }

    public List<Demand> Demands
    {
      get
      {
        if (this.m_demands == null)
          this.m_demands = new List<Demand>();
        return this.m_demands;
      }
      internal set => this.m_demands = value;
    }

    [Obsolete]
    public List<RetentionPolicy> RetentionRules
    {
      get
      {
        if (this.m_retentionRules == null)
          this.m_retentionRules = new List<RetentionPolicy>();
        return this.m_retentionRules;
      }
      internal set => this.m_retentionRules = value;
    }

    public PropertiesCollection Properties
    {
      get
      {
        if (this.m_properties == null)
          this.m_properties = new PropertiesCollection();
        return this.m_properties;
      }
      internal set => this.m_properties = value;
    }

    public List<string> Tags
    {
      get
      {
        if (this.m_tags == null)
          this.m_tags = new List<string>();
        return this.m_tags;
      }
      internal set => this.m_tags = value;
    }

    [System.Runtime.Serialization.OnDeserialized]
    private void OnDeserialized(StreamingContext context)
    {
      SerializationHelper.Copy<BuildOption>(ref this.m_serializedOptions, ref this.m_options, true);
      SerializationHelper.Copy<BuildTrigger>(ref this.m_serializedTriggers, ref this.m_triggers, true);
      SerializationHelper.Copy<string, BuildDefinitionVariable>(ref this.m_serializedVariables, ref this.m_variables, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase, true);
      SerializationHelper.Copy<VariableGroup>(ref this.m_serializedVariableGroups, ref this.m_variableGroups, true);
      SerializationHelper.Copy<Demand>(ref this.m_serializedDemands, ref this.m_demands, true);
      SerializationHelper.Copy<RetentionPolicy>(ref this.m_serializedRetentionRules, ref this.m_retentionRules, true);
    }

    [System.Runtime.Serialization.OnSerializing]
    private void OnSerializing(StreamingContext context)
    {
      SerializationHelper.Copy<BuildOption>(ref this.m_options, ref this.m_serializedOptions);
      SerializationHelper.Copy<BuildTrigger>(ref this.m_triggers, ref this.m_serializedTriggers);
      SerializationHelper.Copy<string, BuildDefinitionVariable>(ref this.m_variables, ref this.m_serializedVariables, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      SerializationHelper.Copy<VariableGroup>(ref this.m_variableGroups, ref this.m_serializedVariableGroups);
      SerializationHelper.Copy<Demand>(ref this.m_demands, ref this.m_serializedDemands);
      SerializationHelper.Copy<RetentionPolicy>(ref this.m_retentionRules, ref this.m_serializedRetentionRules);
    }

    [System.Runtime.Serialization.OnSerialized]
    private void OnSerialized(StreamingContext context)
    {
      this.m_serializedOptions = (List<BuildOption>) null;
      this.m_serializedTriggers = (List<BuildTrigger>) null;
      this.m_serializedVariables = (IDictionary<string, BuildDefinitionVariable>) null;
      this.m_serializedVariableGroups = (List<VariableGroup>) null;
      this.m_serializedRetentionRules = (List<RetentionPolicy>) null;
    }
  }
}
