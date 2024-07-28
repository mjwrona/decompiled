// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.WebApi.TaskDefinition
// Assembly: Microsoft.TeamFoundation.DistributedTask.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9201F3B5-DEAF-44A3-860C-DB7B277BB5C6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.WebApi.dll

using Microsoft.VisualStudio.Services.WebApi;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.DistributedTask.WebApi
{
  [DataContract]
  [DebuggerDisplay("Id: {Id}, Name: {Name}, Version: {Version}")]
  public class TaskDefinition
  {
    [DataMember(Name = "Visibility", EmitDefaultValue = false)]
    private List<string> m_serializedVisibilities;
    [DataMember(Name = "RunsOn", EmitDefaultValue = false)]
    private List<string> m_serializedRunsOn;
    [DataMember(Name = "OutputVariables", EmitDefaultValue = false)]
    private List<TaskOutputVariable> m_serializedOutputVariables;
    private Dictionary<string, JObject> m_preJobExecution;
    private Dictionary<string, JObject> m_execution;
    private Dictionary<string, JObject> m_postJobExecution;
    private List<Demand> m_demands;
    private Dictionary<string, string> m_buildConfigMapping;
    private List<TaskInputDefinition> m_inputs;
    private List<string> m_satisfies;
    private List<TaskSourceDefinition> m_sourceDefinitions;
    private List<DataSourceBinding> m_dataSourceBindings;
    private List<TaskGroupDefinition> m_groups;
    private List<TaskOutputVariable> m_outputVariables;
    private List<string> m_visibilities;
    private List<string> m_runsOn;

    public TaskDefinition() => this.DefinitionType = "task";

    protected TaskDefinition(TaskDefinition taskDefinitionToClone)
    {
      if (taskDefinitionToClone.AgentExecution != null)
        this.AgentExecution = taskDefinitionToClone.AgentExecution.Clone();
      if (taskDefinitionToClone.PreJobExecution != null)
        this.m_preJobExecution = new Dictionary<string, JObject>((IDictionary<string, JObject>) taskDefinitionToClone.m_preJobExecution);
      if (taskDefinitionToClone.Execution != null)
        this.m_execution = new Dictionary<string, JObject>((IDictionary<string, JObject>) taskDefinitionToClone.m_execution);
      if (taskDefinitionToClone.PostJobExecution != null)
        this.m_postJobExecution = new Dictionary<string, JObject>((IDictionary<string, JObject>) taskDefinitionToClone.m_postJobExecution);
      this.Author = taskDefinitionToClone.Author;
      this.Category = taskDefinitionToClone.Category;
      this.HelpMarkDown = taskDefinitionToClone.HelpMarkDown;
      this.HelpUrl = taskDefinitionToClone.HelpUrl;
      this.ContentsUploaded = taskDefinitionToClone.ContentsUploaded;
      if (taskDefinitionToClone.m_visibilities != null)
        this.m_visibilities = new List<string>((IEnumerable<string>) taskDefinitionToClone.m_visibilities);
      if (taskDefinitionToClone.Restrictions != null)
        this.Restrictions = taskDefinitionToClone.Restrictions.Clone();
      if (taskDefinitionToClone.m_runsOn != null)
        this.m_runsOn = new List<string>((IEnumerable<string>) taskDefinitionToClone.m_runsOn);
      if (this.m_runsOn == null)
        this.m_runsOn = new List<string>((IEnumerable<string>) TaskRunsOnConstants.DefaultValue);
      if (taskDefinitionToClone.m_demands != null)
        this.m_demands = new List<Demand>(taskDefinitionToClone.m_demands.Where<Demand>((Func<Demand, bool>) (x => x != null)).Select<Demand, Demand>((Func<Demand, Demand>) (x => x.Clone())));
      if (taskDefinitionToClone.m_buildConfigMapping != null)
        this.m_buildConfigMapping = new Dictionary<string, string>((IDictionary<string, string>) taskDefinitionToClone.m_buildConfigMapping);
      this.Description = taskDefinitionToClone.Description;
      this.FriendlyName = taskDefinitionToClone.FriendlyName;
      this.HostType = taskDefinitionToClone.HostType;
      this.IconUrl = taskDefinitionToClone.IconUrl;
      this.Id = taskDefinitionToClone.Id;
      if (taskDefinitionToClone.m_inputs != null)
        this.m_inputs = new List<TaskInputDefinition>(taskDefinitionToClone.m_inputs.Where<TaskInputDefinition>((Func<TaskInputDefinition, bool>) (x => x != null)).Select<TaskInputDefinition, TaskInputDefinition>((Func<TaskInputDefinition, TaskInputDefinition>) (x => x.Clone())));
      if (taskDefinitionToClone.m_satisfies != null)
        this.m_satisfies = new List<string>((IEnumerable<string>) taskDefinitionToClone.m_satisfies);
      if (taskDefinitionToClone.m_sourceDefinitions != null)
        this.m_sourceDefinitions = new List<TaskSourceDefinition>(taskDefinitionToClone.m_sourceDefinitions.Where<TaskSourceDefinition>((Func<TaskSourceDefinition, bool>) (x => x != null)).Select<TaskSourceDefinition, TaskSourceDefinition>((Func<TaskSourceDefinition, TaskSourceDefinition>) (x => x.Clone())));
      if (taskDefinitionToClone.m_dataSourceBindings != null)
        this.m_dataSourceBindings = new List<DataSourceBinding>(taskDefinitionToClone.m_dataSourceBindings.Where<DataSourceBinding>((Func<DataSourceBinding, bool>) (x => x != null)).Select<DataSourceBinding, DataSourceBinding>((Func<DataSourceBinding, DataSourceBinding>) (x => x.Clone())));
      if (taskDefinitionToClone.m_groups != null)
        this.m_groups = new List<TaskGroupDefinition>(taskDefinitionToClone.m_groups.Where<TaskGroupDefinition>((Func<TaskGroupDefinition, bool>) (x => x != null)).Select<TaskGroupDefinition, TaskGroupDefinition>((Func<TaskGroupDefinition, TaskGroupDefinition>) (x => x.Clone())));
      if (taskDefinitionToClone.m_outputVariables != null)
        this.m_outputVariables = new List<TaskOutputVariable>(taskDefinitionToClone.m_outputVariables.Where<TaskOutputVariable>((Func<TaskOutputVariable, bool>) (x => x != null)).Select<TaskOutputVariable, TaskOutputVariable>((Func<TaskOutputVariable, TaskOutputVariable>) (x => x.Clone())));
      this.InstanceNameFormat = taskDefinitionToClone.InstanceNameFormat;
      this.MinimumAgentVersion = taskDefinitionToClone.MinimumAgentVersion;
      this.Name = taskDefinitionToClone.Name;
      this.PackageLocation = taskDefinitionToClone.PackageLocation;
      this.PackageType = taskDefinitionToClone.PackageType;
      this.ServerOwned = taskDefinitionToClone.ServerOwned;
      this.SourceLocation = taskDefinitionToClone.SourceLocation;
      this.Version = taskDefinitionToClone.Version.Clone();
      this.ContributionIdentifier = taskDefinitionToClone.ContributionIdentifier;
      this.ContributionVersion = taskDefinitionToClone.ContributionVersion;
      this.Deprecated = taskDefinitionToClone.Deprecated;
      this.Disabled = taskDefinitionToClone.Disabled;
      this.DefinitionType = taskDefinitionToClone.DefinitionType;
      this.ShowEnvironmentVariables = taskDefinitionToClone.ShowEnvironmentVariables;
      this.Preview = taskDefinitionToClone.Preview;
      this.ReleaseNotes = taskDefinitionToClone.ReleaseNotes;
      if (this.DefinitionType != null)
        return;
      this.DefinitionType = "task";
    }

    [DataMember(EmitDefaultValue = false)]
    public Guid Id { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string Name { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public TaskVersion Version { get; set; }

    [Obsolete("Ecosystem property is not currently supported.")]
    [DataMember(EmitDefaultValue = false)]
    public string Ecosystem { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public bool ServerOwned { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public bool ContentsUploaded { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string IconUrl { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string HostType { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string PackageType { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string PackageLocation { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string SourceLocation { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string MinimumAgentVersion { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string FriendlyName { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string Description { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string Category { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string HelpMarkDown { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string HelpUrl { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string ReleaseNotes { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public bool Preview { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public bool Deprecated { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string ContributionIdentifier { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string ContributionVersion { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public bool Disabled { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string DefinitionType { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public bool ShowEnvironmentVariables { get; set; }

    public IList<string> Visibility
    {
      get
      {
        if (this.m_visibilities == null)
          this.m_visibilities = new List<string>();
        return (IList<string>) this.m_visibilities;
      }
    }

    [DataMember(EmitDefaultValue = false)]
    public TaskRestrictions Restrictions { get; set; }

    public IList<string> RunsOn
    {
      get
      {
        if (this.m_runsOn == null)
          this.m_runsOn = new List<string>((IEnumerable<string>) TaskRunsOnConstants.DefaultValue);
        return (IList<string>) this.m_runsOn;
      }
    }

    [DataMember(EmitDefaultValue = false)]
    public string Author { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public IList<Demand> Demands
    {
      get
      {
        if (this.m_demands == null)
          this.m_demands = new List<Demand>();
        return (IList<Demand>) this.m_demands;
      }
    }

    [DataMember(EmitDefaultValue = false)]
    public IList<TaskGroupDefinition> Groups
    {
      get
      {
        if (this.m_groups == null)
          this.m_groups = new List<TaskGroupDefinition>();
        return (IList<TaskGroupDefinition>) this.m_groups;
      }
    }

    [DataMember(EmitDefaultValue = false)]
    public IList<TaskInputDefinition> Inputs
    {
      get
      {
        if (this.m_inputs == null)
          this.m_inputs = new List<TaskInputDefinition>();
        return (IList<TaskInputDefinition>) this.m_inputs;
      }
    }

    [DataMember(EmitDefaultValue = false)]
    public IList<string> Satisfies
    {
      get
      {
        if (this.m_satisfies == null)
          this.m_satisfies = new List<string>();
        return (IList<string>) this.m_satisfies;
      }
    }

    [DataMember(EmitDefaultValue = false)]
    public IList<TaskSourceDefinition> SourceDefinitions
    {
      get
      {
        if (this.m_sourceDefinitions == null)
          this.m_sourceDefinitions = new List<TaskSourceDefinition>();
        return (IList<TaskSourceDefinition>) this.m_sourceDefinitions;
      }
    }

    [DataMember(EmitDefaultValue = false)]
    public IList<DataSourceBinding> DataSourceBindings
    {
      get
      {
        if (this.m_dataSourceBindings == null)
          this.m_dataSourceBindings = new List<DataSourceBinding>();
        return (IList<DataSourceBinding>) this.m_dataSourceBindings;
      }
    }

    [DataMember(EmitDefaultValue = false)]
    public string InstanceNameFormat { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public IDictionary<string, JObject> PreJobExecution
    {
      get
      {
        if (this.m_preJobExecution == null)
          this.m_preJobExecution = new Dictionary<string, JObject>();
        return (IDictionary<string, JObject>) this.m_preJobExecution;
      }
    }

    [DataMember(EmitDefaultValue = false)]
    public IDictionary<string, JObject> Execution
    {
      get
      {
        if (this.m_execution == null)
          this.m_execution = new Dictionary<string, JObject>();
        return (IDictionary<string, JObject>) this.m_execution;
      }
    }

    [DataMember(EmitDefaultValue = false)]
    public IDictionary<string, JObject> PostJobExecution
    {
      get
      {
        if (this.m_postJobExecution == null)
          this.m_postJobExecution = new Dictionary<string, JObject>();
        return (IDictionary<string, JObject>) this.m_postJobExecution;
      }
    }

    [DataMember(EmitDefaultValue = false)]
    public TaskExecution AgentExecution { get; set; }

    public IList<TaskOutputVariable> OutputVariables
    {
      get
      {
        if (this.m_outputVariables == null)
          this.m_outputVariables = new List<TaskOutputVariable>();
        return (IList<TaskOutputVariable>) this.m_outputVariables;
      }
    }

    [DataMember(EmitDefaultValue = false, Name = "_buildConfigMapping", IsRequired = false)]
    [ClientInternalUseOnly(true, OmitFromTypeScriptDeclareFile = true)]
    internal IDictionary<string, string> BuildConfigMapping
    {
      get
      {
        if (this.m_buildConfigMapping == null)
          this.m_buildConfigMapping = new Dictionary<string, string>();
        return (IDictionary<string, string>) this.m_buildConfigMapping;
      }
    }

    internal TaskDefinition Clone() => new TaskDefinition(this);

    [System.Runtime.Serialization.OnDeserialized]
    private void OnDeserialized(StreamingContext context)
    {
      SerializationHelper.Copy<string>(ref this.m_serializedVisibilities, ref this.m_visibilities, true);
      SerializationHelper.Copy<string>(ref this.m_serializedRunsOn, ref this.m_runsOn, true);
      TaskDefinition.RenameLegacyRunsOnValues((IList<string>) this.m_runsOn);
      SerializationHelper.Copy<TaskOutputVariable>(ref this.m_serializedOutputVariables, ref this.m_outputVariables, true);
    }

    [System.Runtime.Serialization.OnSerializing]
    private void OnSerializing(StreamingContext context)
    {
      SerializationHelper.Copy<string>(ref this.m_visibilities, ref this.m_serializedVisibilities);
      TaskDefinition.RenameLegacyRunsOnValues((IList<string>) this.m_runsOn);
      SerializationHelper.Copy<string>(ref this.m_runsOn, ref this.m_serializedRunsOn);
      SerializationHelper.Copy<TaskOutputVariable>(ref this.m_outputVariables, ref this.m_serializedOutputVariables);
    }

    [System.Runtime.Serialization.OnSerialized]
    private void OnSerialized(StreamingContext context)
    {
      this.m_serializedVisibilities = (List<string>) null;
      this.m_serializedRunsOn = (List<string>) null;
      this.m_serializedOutputVariables = (List<TaskOutputVariable>) null;
    }

    private static void RenameLegacyRunsOnValues(IList<string> runsOn)
    {
      int index = 0;
      while (true)
      {
        int num = index;
        int? nullable = runsOn != null ? new int?(runsOn.Count<string>()) : new int?();
        int valueOrDefault = nullable.GetValueOrDefault();
        if (num < valueOrDefault & nullable.HasValue)
        {
          if (runsOn[index].Equals("MachineGroup", StringComparison.OrdinalIgnoreCase))
            runsOn[index] = "DeploymentGroup";
          ++index;
        }
        else
          break;
      }
    }
  }
}
