// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.WebApi.Internals.BuildDefinition3_2
// Assembly: Microsoft.TeamFoundation.Build2.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0683407D-7C61-4505-8CA6-86AD7E3B6BCA
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Build2.WebApi.dll

using Microsoft.TeamFoundation.DistributedTask.Common.Contracts;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Build.WebApi.Internals
{
  [DataContract]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public class BuildDefinition3_2 : BuildDefinitionReference3_2
  {
    [DataMember(Name = "Build", EmitDefaultValue = false)]
    private List<BuildDefinitionStep> m_serializedSteps;
    [DataMember(Name = "Options", EmitDefaultValue = false)]
    private List<BuildOption> m_serializedOptions;
    [DataMember(Name = "Triggers", EmitDefaultValue = false)]
    private List<BuildTrigger> m_serializedTriggers;
    [DataMember(Name = "Variables", EmitDefaultValue = false)]
    private IDictionary<string, BuildDefinitionVariable> m_serializedVariables;
    [DataMember(Name = "Demands", EmitDefaultValue = false)]
    private List<Demand> m_serializedDemands;
    [DataMember(Name = "RetentionRules", EmitDefaultValue = false)]
    private List<RetentionPolicy> m_serializedRetentionRules;
    [DataMember(IsRequired = false, EmitDefaultValue = false, Name = "Properties")]
    private PropertiesCollection m_properties;
    [DataMember(EmitDefaultValue = false, Name = "Tags")]
    private List<string> m_tags;
    [DataMember(EmitDefaultValue = false, Name = "LatestBuild")]
    private Microsoft.TeamFoundation.Build.WebApi.Build m_latestBuild;
    [DataMember(EmitDefaultValue = false, Name = "LatestCompletedBuild")]
    private Microsoft.TeamFoundation.Build.WebApi.Build m_latestCompletedBuild;
    private List<Demand> m_demands;
    private List<BuildOption> m_options;
    private List<BuildTrigger> m_triggers;
    private List<RetentionPolicy> m_retentionRules;
    private List<BuildDefinitionStep> m_steps;
    private IDictionary<string, BuildDefinitionVariable> m_variables;

    public BuildDefinition3_2() => this.JobAuthorizationScope = BuildAuthorizationScope.ProjectCollection;

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

    public List<BuildDefinitionStep> Steps
    {
      get
      {
        if (this.m_steps == null)
          this.m_steps = new List<BuildDefinitionStep>();
        return this.m_steps;
      }
    }

    public List<BuildOption> Options
    {
      get
      {
        if (this.m_options == null)
          this.m_options = new List<BuildOption>();
        return this.m_options;
      }
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
    }

    public IDictionary<string, BuildDefinitionVariable> Variables
    {
      get
      {
        if (this.m_variables == null)
          this.m_variables = (IDictionary<string, BuildDefinitionVariable>) new Dictionary<string, BuildDefinitionVariable>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
        return this.m_variables;
      }
    }

    public List<Demand> Demands
    {
      get
      {
        if (this.m_demands == null)
          this.m_demands = new List<Demand>();
        return this.m_demands;
      }
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
    }

    public Microsoft.TeamFoundation.Build.WebApi.Build LatestBuild
    {
      get => this.m_latestBuild;
      internal set => this.m_latestBuild = value;
    }

    public Microsoft.TeamFoundation.Build.WebApi.Build LatestCompletedBuild
    {
      get => this.m_latestCompletedBuild;
      internal set => this.m_latestCompletedBuild = value;
    }

    [System.Runtime.Serialization.OnDeserialized]
    private void OnDeserialized(StreamingContext context)
    {
      Microsoft.TeamFoundation.Build.WebApi.SerializationHelper.Copy<BuildOption>(ref this.m_serializedOptions, ref this.m_options, true);
      Microsoft.TeamFoundation.Build.WebApi.SerializationHelper.Copy<BuildDefinitionStep>(ref this.m_serializedSteps, ref this.m_steps, true);
      Microsoft.TeamFoundation.Build.WebApi.SerializationHelper.Copy<BuildTrigger>(ref this.m_serializedTriggers, ref this.m_triggers, true);
      Microsoft.TeamFoundation.Build.WebApi.SerializationHelper.Copy<string, BuildDefinitionVariable>(ref this.m_serializedVariables, ref this.m_variables, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase, true);
      Microsoft.TeamFoundation.Build.WebApi.SerializationHelper.Copy<Demand>(ref this.m_serializedDemands, ref this.m_demands, true);
      Microsoft.TeamFoundation.Build.WebApi.SerializationHelper.Copy<RetentionPolicy>(ref this.m_serializedRetentionRules, ref this.m_retentionRules, true);
    }

    [System.Runtime.Serialization.OnSerializing]
    private void OnSerializing(StreamingContext context)
    {
      Microsoft.TeamFoundation.Build.WebApi.SerializationHelper.Copy<BuildOption>(ref this.m_options, ref this.m_serializedOptions);
      Microsoft.TeamFoundation.Build.WebApi.SerializationHelper.Copy<BuildDefinitionStep>(ref this.m_steps, ref this.m_serializedSteps);
      Microsoft.TeamFoundation.Build.WebApi.SerializationHelper.Copy<BuildTrigger>(ref this.m_triggers, ref this.m_serializedTriggers);
      Microsoft.TeamFoundation.Build.WebApi.SerializationHelper.Copy<string, BuildDefinitionVariable>(ref this.m_variables, ref this.m_serializedVariables, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      Microsoft.TeamFoundation.Build.WebApi.SerializationHelper.Copy<Demand>(ref this.m_demands, ref this.m_serializedDemands);
      Microsoft.TeamFoundation.Build.WebApi.SerializationHelper.Copy<RetentionPolicy>(ref this.m_retentionRules, ref this.m_serializedRetentionRules);
    }

    [System.Runtime.Serialization.OnSerialized]
    private void OnSerialized(StreamingContext context)
    {
      this.m_serializedSteps = (List<BuildDefinitionStep>) null;
      this.m_serializedOptions = (List<BuildOption>) null;
      this.m_serializedTriggers = (List<BuildTrigger>) null;
      this.m_serializedVariables = (IDictionary<string, BuildDefinitionVariable>) null;
      this.m_serializedRetentionRules = (List<RetentionPolicy>) null;
    }
  }
}
