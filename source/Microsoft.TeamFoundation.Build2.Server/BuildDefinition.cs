// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Server.BuildDefinition
// Assembly: Microsoft.TeamFoundation.Build2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 680FF5F5-CB5D-4078-8EFA-56C292BFDBB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.Server.dll

using Microsoft.TeamFoundation.Build.WebApi;
using Microsoft.TeamFoundation.DistributedTask.Common.Contracts;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Build2.Server
{
  [DataContract]
  public class BuildDefinition : MinimalBuildDefinition, IReadOnlyBuildDefinition
  {
    private List<string> m_tags;
    private List<BuildMetric> m_metrics;
    private PropertiesCollection m_properties;
    private List<BuildOption> m_buildOptions;
    private BuildRepository m_repository;
    private List<BuildTrigger> m_triggers;
    private Dictionary<string, BuildDefinitionVariable> m_variables;
    private List<Demand> m_demands;
    private List<VariableGroup> m_variableGroups;
    private ProcessParameters m_processParameters;
    private List<BuildDefinition> m_drafts;
    private List<BuildCompletionTrigger> m_buildCompletionTriggers;

    [IgnoreDataMember]
    public string ProjectName { get; set; }

    [DataMember]
    public Microsoft.TeamFoundation.Build2.Server.DefinitionQuality? DefinitionQuality { get; set; }

    [DataMember]
    public AgentPoolQueue Queue { get; set; }

    [IgnoreDataMember]
    public Guid AuthoredBy { get; set; }

    [DataMember]
    public DateTime CreatedDate { get; set; }

    [DataMember]
    public string Description { get; set; }

    [DataMember]
    public string BuildNumberFormat { get; set; }

    [DataMember]
    public int JobTimeoutInMinutes { get; set; }

    [DataMember]
    public int JobCancelTimeoutInMinutes { get; set; }

    [DataMember]
    public bool BadgeEnabled { get; set; }

    [DataMember]
    public string Comment { get; set; }

    [DataMember]
    public BuildDefinition ParentDefinition { get; set; }

    [DataMember]
    public string RepositoryString { get; set; }

    [DataMember]
    public BuildProcess Process { get; set; }

    [DataMember]
    public string ProcessParametersString { get; set; }

    [DataMember]
    public string BuildOptionsString { get; set; }

    [IgnoreDataMember]
    public BuildData LatestCompletedBuild { get; set; }

    [IgnoreDataMember]
    public BuildData LatestBuild { get; set; }

    [DataMember]
    public string DropLocation { get; set; }

    [DataMember]
    public string Url { get; set; }

    [DataMember]
    public List<string> Tags
    {
      get
      {
        if (this.m_tags == null)
          this.m_tags = new List<string>();
        return this.m_tags;
      }
      set => this.m_tags = value;
    }

    [DataMember]
    public List<BuildMetric> Metrics
    {
      get
      {
        if (this.m_metrics == null)
          this.m_metrics = new List<BuildMetric>();
        return this.m_metrics;
      }
      set => this.m_metrics = value;
    }

    [DataMember]
    public PropertiesCollection Properties
    {
      get
      {
        if (this.m_properties == null)
          this.m_properties = new PropertiesCollection();
        return this.m_properties;
      }
      set => this.m_properties = value;
    }

    [DataMember]
    public List<BuildOption> Options
    {
      get
      {
        if (this.m_buildOptions == null)
          this.SummonBuildOptions();
        return this.m_buildOptions;
      }
      set => this.m_buildOptions = value;
    }

    private void SummonBuildOptions()
    {
      if (!string.IsNullOrEmpty(this.BuildOptionsString))
        this.m_buildOptions = JsonUtility.FromString<List<BuildOption>>(this.BuildOptionsString);
      else
        this.m_buildOptions = new List<BuildOption>();
    }

    [DataMember]
    public BuildRepository Repository
    {
      get
      {
        if (this.m_repository == null)
          this.SummonBuildRepository();
        return this.m_repository;
      }
      set => this.m_repository = value;
    }

    private void SummonBuildRepository()
    {
      if (string.IsNullOrEmpty(this.RepositoryString))
        return;
      this.m_repository = JsonUtility.FromString<BuildRepository>(this.RepositoryString);
    }

    public string TriggersString { get; set; }

    [DataMember]
    public List<BuildTrigger> Triggers
    {
      get
      {
        if (this.m_triggers == null)
          this.SummonTriggers();
        return this.m_triggers;
      }
      set => this.m_triggers = value;
    }

    private void SummonTriggers()
    {
      if (!string.IsNullOrEmpty(this.TriggersString))
        this.m_triggers = JsonUtility.FromString<List<BuildTrigger>>(this.TriggersString);
      else
        this.m_triggers = new List<BuildTrigger>();
    }

    public string VariablesString { get; set; }

    [DataMember]
    public Dictionary<string, BuildDefinitionVariable> Variables
    {
      get
      {
        if (this.m_variables == null)
          this.SummonVariables();
        return this.m_variables;
      }
      set => this.m_variables = value;
    }

    private void SummonVariables()
    {
      if (!string.IsNullOrEmpty(this.VariablesString))
        this.m_variables = JsonUtility.FromString<Dictionary<string, BuildDefinitionVariable>>(this.VariablesString);
      else
        this.m_variables = new Dictionary<string, BuildDefinitionVariable>();
    }

    public string DemandsString { get; set; }

    [DataMember]
    public List<Demand> Demands
    {
      get
      {
        if (this.m_demands == null)
          this.SummonDemands();
        return this.m_demands;
      }
      set
      {
        if (value == null)
          return;
        this.m_demands = new List<Demand>((IEnumerable<Demand>) value);
      }
    }

    private void SummonDemands()
    {
      if (!string.IsNullOrEmpty(this.DemandsString))
        this.m_demands = JsonUtility.FromString<List<Demand>>(this.DemandsString);
      else
        this.m_demands = new List<Demand>();
    }

    public string VariableGroupsString { get; set; }

    [DataMember]
    public List<VariableGroup> VariableGroups
    {
      get
      {
        if (this.m_variableGroups == null)
          this.SummonVariableGroups();
        return this.m_variableGroups;
      }
      set => this.m_variableGroups = value;
    }

    private void SummonVariableGroups()
    {
      if (!string.IsNullOrEmpty(this.VariableGroupsString))
        this.m_variableGroups = JsonUtility.FromString<List<VariableGroup>>(this.VariableGroupsString);
      else
        this.m_variableGroups = new List<VariableGroup>();
    }

    [DataMember]
    public ProcessParameters ProcessParameters
    {
      get
      {
        if (this.m_processParameters == null)
          this.SummonProcessParameters();
        return this.m_processParameters;
      }
      set => this.m_processParameters = value;
    }

    public BuildDefinition Clone()
    {
      BuildDefinition buildDefinition = new BuildDefinition();
      buildDefinition.ProjectId = this.ProjectId;
      buildDefinition.ProjectName = this.ProjectName;
      buildDefinition.Id = this.Id;
      buildDefinition.Revision = this.Revision;
      buildDefinition.Name = this.Name;
      buildDefinition.Path = this.Path;
      buildDefinition.Type = this.Type;
      buildDefinition.DefinitionQuality = this.DefinitionQuality;
      buildDefinition.Queue = this.Queue.Clone();
      buildDefinition.QueueStatus = this.QueueStatus;
      buildDefinition.AuthoredBy = this.AuthoredBy;
      buildDefinition.CreatedDate = this.CreatedDate;
      buildDefinition.Description = this.Description;
      buildDefinition.BuildNumberFormat = this.BuildNumberFormat;
      buildDefinition.JobAuthorizationScope = this.JobAuthorizationScope;
      buildDefinition.JobTimeoutInMinutes = this.JobTimeoutInMinutes;
      buildDefinition.JobCancelTimeoutInMinutes = this.JobCancelTimeoutInMinutes;
      buildDefinition.BadgeEnabled = this.BadgeEnabled;
      buildDefinition.Comment = this.Comment;
      buildDefinition.ParentDefinition = this.ParentDefinition;
      buildDefinition.RepositoryString = this.RepositoryString;
      buildDefinition.Process = this.Process;
      buildDefinition.ProcessParametersString = this.ProcessParametersString;
      buildDefinition.BuildOptionsString = this.BuildOptionsString;
      buildDefinition.LatestCompletedBuild = this.LatestCompletedBuild;
      buildDefinition.LatestBuild = this.LatestBuild;
      buildDefinition.DropLocation = this.DropLocation;
      buildDefinition.Url = this.Url;
      buildDefinition.Uri = this.Uri;
      buildDefinition.Tags = this.Tags.ConvertAll<string>((Converter<string, string>) (tag => tag));
      buildDefinition.Metrics = this.Metrics.ConvertAll<BuildMetric>((Converter<BuildMetric, BuildMetric>) (metric => metric.Clone()));
      buildDefinition.Properties = new PropertiesCollection((IDictionary<string, object>) this.Properties);
      buildDefinition.Options = this.Options.ConvertAll<BuildOption>((Converter<BuildOption, BuildOption>) (option => option.Clone()));
      buildDefinition.Repository = this.Repository.Clone();
      buildDefinition.TriggersString = this.TriggersString;
      buildDefinition.Triggers = this.Triggers.ConvertAll<BuildTrigger>((Converter<BuildTrigger, BuildTrigger>) (trigger => trigger.Clone()));
      buildDefinition.VariablesString = this.VariablesString;
      buildDefinition.Variables = this.Variables.ToDictionary<KeyValuePair<string, BuildDefinitionVariable>, string, BuildDefinitionVariable>((Func<KeyValuePair<string, BuildDefinitionVariable>, string>) (entry => entry.Key), (Func<KeyValuePair<string, BuildDefinitionVariable>, BuildDefinitionVariable>) (entry => entry.Value.Clone()));
      buildDefinition.DemandsString = this.DemandsString;
      buildDefinition.Demands = this.Demands.ConvertAll<Demand>((Converter<Demand, Demand>) (demand => demand.Clone()));
      buildDefinition.VariableGroupsString = this.VariableGroupsString;
      buildDefinition.VariableGroups = this.VariableGroups.ConvertAll<VariableGroup>((Converter<VariableGroup, VariableGroup>) (group => group.Clone()));
      buildDefinition.ProcessParameters = this.ProcessParameters.Clone();
      buildDefinition.BuildCompletionTriggersString = this.BuildCompletionTriggersString;
      buildDefinition.BuildCompletionTriggers = this.BuildCompletionTriggers.ConvertAll<BuildCompletionTrigger>((Converter<BuildCompletionTrigger, BuildCompletionTrigger>) (trigger => trigger.Clone()));
      return buildDefinition;
    }

    public static string GetToken(Guid projectId, string path, int definitionId) => projectId.ToString("D") + Security.GetSecurityTokenPath(path ?? string.Empty) + (object) definitionId;

    private void SummonProcessParameters()
    {
      if (string.IsNullOrEmpty(this.ProcessParametersString))
        return;
      this.m_processParameters = JsonUtility.FromString<ProcessParameters>(this.ProcessParametersString);
    }

    internal string BuildCompletionTriggersString { get; set; }

    [DataMember]
    public List<BuildCompletionTrigger> BuildCompletionTriggers
    {
      get
      {
        if (this.m_buildCompletionTriggers == null)
          this.SummonBuildCompletionTriggers();
        return this.m_buildCompletionTriggers;
      }
      set => this.m_buildCompletionTriggers = value;
    }

    private void SummonBuildCompletionTriggers()
    {
      if (!string.IsNullOrEmpty(this.BuildCompletionTriggersString))
        this.m_buildCompletionTriggers = JsonUtility.FromString<List<BuildCompletionTrigger>>(this.BuildCompletionTriggersString);
      else
        this.m_buildCompletionTriggers = new List<BuildCompletionTrigger>();
    }

    public bool HasDesignerSchedules() => this.Triggers.OfType<ScheduleTrigger>().Any<ScheduleTrigger>((Func<ScheduleTrigger, bool>) (sch => sch.Schedules.Count > 0));

    [IgnoreDataMember]
    public List<BuildDefinition> Drafts => this.m_drafts ?? (this.m_drafts = new List<BuildDefinition>());

    IReadOnlyList<BuildCompletionTrigger> IReadOnlyBuildDefinition.BuildCompletionTriggers => (IReadOnlyList<BuildCompletionTrigger>) this.BuildCompletionTriggers;

    IReadOnlyList<IReadOnlyBuildDefinition> IReadOnlyBuildDefinition.Drafts => (IReadOnlyList<IReadOnlyBuildDefinition>) this.Drafts;

    IReadOnlyList<BuildMetric> IReadOnlyBuildDefinition.Metrics => (IReadOnlyList<BuildMetric>) this.Metrics;

    IReadOnlyList<BuildOption> IReadOnlyBuildDefinition.Options => (IReadOnlyList<BuildOption>) this.Options;

    IReadOnlyList<string> IReadOnlyBuildDefinition.Tags => (IReadOnlyList<string>) this.Tags;

    IReadOnlyList<BuildTrigger> IReadOnlyBuildDefinition.Triggers => (IReadOnlyList<BuildTrigger>) this.Triggers;

    IReadOnlyList<VariableGroup> IReadOnlyBuildDefinition.VariableGroups => (IReadOnlyList<VariableGroup>) this.VariableGroups;

    IReadOnlyDictionary<string, BuildDefinitionVariable> IReadOnlyBuildDefinition.Variables => (IReadOnlyDictionary<string, BuildDefinitionVariable>) this.Variables;

    IReadOnlyBuildData IReadOnlyBuildDefinition.LatestBuild => (IReadOnlyBuildData) this.LatestBuild;

    IReadOnlyBuildData IReadOnlyBuildDefinition.LatestCompletedBuild => (IReadOnlyBuildData) this.LatestCompletedBuild;

    IReadOnlyBuildDefinition IReadOnlyBuildDefinition.ParentDefinition => (IReadOnlyBuildDefinition) this.ParentDefinition;

    private struct CounterNameAndId
    {
      public string Name { get; set; }

      public int Id { get; set; }
    }
  }
}
