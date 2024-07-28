// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Server.BuildData
// Assembly: Microsoft.TeamFoundation.Build2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 680FF5F5-CB5D-4078-8EFA-56C292BFDBB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.Server.dll

using Microsoft.Azure.Pipelines.Server.ObjectModel;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace Microsoft.TeamFoundation.Build2.Server
{
  public class BuildData : IReadOnlyBuildData
  {
    private List<TaskOrchestrationPlanReference> m_plans;
    private List<string> m_tags;
    private Uri m_uri;
    private PropertiesCollection m_properties;
    private SourceVersionInfo m_sourceVersionInfo;
    private Dictionary<string, string> m_triggerInfo;
    private List<BuildRequestValidationResult> m_validationResults;
    private HashSet<string> m_stagesToSkip;
    private Dictionary<string, object> m_templateParameters;
    private List<RetentionLease> m_retentionLeases;

    public BuildData()
    {
      this.Reason = BuildReason.Manual;
      this.Priority = QueuePriority.Normal;
    }

    public int Id { get; set; }

    public Guid ProjectId { get; set; }

    public string BuildNumber { get; set; }

    public int? BuildNumberRevision { get; set; }

    public string SourceBranch { get; set; }

    public string SourceVersion { get; set; }

    public string Parameters { get; set; }

    public BuildStatus? Status { get; set; }

    public QueuePriority Priority { get; set; }

    public DateTime? StartTime { get; set; }

    public DateTime? FinishTime { get; set; }

    public DateTime? QueueTime { get; set; }

    public BuildReason Reason { get; set; }

    public Guid RequestedFor { get; set; }

    public Guid RequestedBy { get; set; }

    public DateTime LastChangedDate { get; set; }

    public Guid LastChangedBy { get; set; }

    public bool Deleted { get; set; }

    public Guid? DeletedBy { get; set; }

    public DateTime? DeletedDate { get; set; }

    public string DeletedReason { get; set; }

    public BuildResult? Result { get; set; }

    public MinimalBuildRepository Repository { get; set; }

    public MinimalBuildDefinition Definition { get; set; }

    public int? QueueId { get; set; }

    public AgentSpecification AgentSpecification { get; set; }

    public LogReference Logs { get; set; }

    public QueueOptions QueueOptions { get; set; }

    public bool ChangesCalculated { get; set; }

    public List<Demand> Demands { get; set; }

    internal bool? LegacyInputKeepForever { get; set; }

    internal bool? LegacyInputRetainedByRelease { get; set; }

    public List<RetentionLease> RetentionLeases
    {
      get
      {
        if (this.m_retentionLeases == null)
          this.m_retentionLeases = new List<RetentionLease>();
        return this.m_retentionLeases;
      }
      set
      {
        if (value == null)
          return;
        this.m_retentionLeases = new List<RetentionLease>((IEnumerable<RetentionLease>) value);
      }
    }

    public List<string> Tags
    {
      get
      {
        if (this.m_tags == null)
          this.m_tags = new List<string>();
        return this.m_tags;
      }
      set
      {
        if (value == null)
          return;
        this.m_tags = new List<string>((IEnumerable<string>) value);
      }
    }

    public Uri Uri
    {
      get
      {
        if (this.m_uri == (Uri) null)
          this.m_uri = new Uri(LinkingUtilities.EncodeUri(new ArtifactId()
          {
            ToolSpecificId = this.Id.ToString((IFormatProvider) CultureInfo.InvariantCulture),
            ArtifactType = "Build",
            Tool = "Build"
          }), UriKind.Absolute);
        return this.m_uri;
      }
      set => this.m_uri = value;
    }

    internal JustInTimeSettings JustInTime { get; } = new JustInTimeSettings();

    public TaskOrchestrationPlanReference OrchestrationPlan { get; set; }

    public List<TaskOrchestrationPlanReference> Plans
    {
      get
      {
        if (this.m_plans == null)
          this.m_plans = new List<TaskOrchestrationPlanReference>();
        return this.m_plans;
      }
      set
      {
        if (value == null)
          return;
        this.m_plans = new List<TaskOrchestrationPlanReference>((IEnumerable<TaskOrchestrationPlanReference>) value);
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
      set => this.m_properties = value;
    }

    public TriggeredByBuild TriggeredByBuild { get; set; }

    public bool AppendCommitMessageToRunName { get; set; } = true;

    public string SourceVersionInfoString { get; set; }

    public SourceVersionInfo SourceVersionInfo
    {
      get
      {
        if (this.m_sourceVersionInfo == null)
          this.SummonSourceVersionInfo();
        return this.m_sourceVersionInfo;
      }
      set
      {
        if (value == null)
          return;
        this.m_sourceVersionInfo = value;
      }
    }

    private void SummonSourceVersionInfo()
    {
      if (!string.IsNullOrEmpty(this.SourceVersionInfoString))
        this.m_sourceVersionInfo = JsonUtility.FromString<SourceVersionInfo>(this.SourceVersionInfoString);
      else
        this.m_sourceVersionInfo = new SourceVersionInfo();
    }

    public string TriggerInfoString { get; set; }

    public IDictionary<string, string> TriggerInfo
    {
      get
      {
        if (this.m_triggerInfo == null)
          this.SummonTriggerInfo();
        return (IDictionary<string, string>) this.m_triggerInfo;
      }
      set
      {
        if (value == null)
          return;
        this.m_triggerInfo = new Dictionary<string, string>(value);
      }
    }

    private void SummonTriggerInfo()
    {
      if (!string.IsNullOrEmpty(this.TriggerInfoString))
        this.m_triggerInfo = JsonUtility.FromString<Dictionary<string, string>>(this.TriggerInfoString);
      else
        this.m_triggerInfo = new Dictionary<string, string>();
    }

    public string ValidationResultsString { get; set; }

    public List<BuildRequestValidationResult> ValidationResults
    {
      get
      {
        if (this.m_validationResults == null)
          this.SummonValidationResults();
        return this.m_validationResults;
      }
      set => this.m_validationResults = value;
    }

    public ISet<string> StagesToSkip
    {
      get
      {
        if (this.m_stagesToSkip == null)
          this.m_stagesToSkip = new HashSet<string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
        return (ISet<string>) this.m_stagesToSkip;
      }
    }

    public RunResourcesParameters Resources { get; set; }

    public HashSet<Microsoft.TeamFoundation.DistributedTask.Pipelines.RepositoryResource> RepositoryResources { get; set; }

    public Dictionary<string, object> TemplateParameters
    {
      get
      {
        if (this.m_templateParameters == null)
          this.m_templateParameters = new Dictionary<string, object>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
        return this.m_templateParameters;
      }
      set => this.m_templateParameters = value;
    }

    internal BuildData Clone()
    {
      BuildData buildData1 = new BuildData()
      {
        Id = this.Id,
        ProjectId = this.ProjectId,
        BuildNumber = this.BuildNumber,
        BuildNumberRevision = this.BuildNumberRevision,
        SourceBranch = this.SourceBranch,
        SourceVersion = this.SourceVersion,
        Parameters = this.Parameters,
        TemplateParameters = this.TemplateParameters,
        Status = this.Status,
        Priority = this.Priority,
        StartTime = this.StartTime,
        FinishTime = this.FinishTime,
        QueueTime = this.QueueTime,
        Reason = this.Reason,
        RequestedFor = this.RequestedFor,
        RequestedBy = this.RequestedBy,
        LastChangedDate = this.LastChangedDate,
        LastChangedBy = this.LastChangedBy,
        Deleted = this.Deleted,
        DeletedBy = this.DeletedBy,
        DeletedDate = this.DeletedDate,
        DeletedReason = this.DeletedReason,
        SourceVersionInfoString = this.SourceVersionInfoString,
        TriggerInfoString = this.TriggerInfoString,
        QueueId = this.QueueId,
        LegacyInputKeepForever = this.LegacyInputKeepForever,
        LegacyInputRetainedByRelease = this.LegacyInputRetainedByRelease,
        Result = this.Result,
        Uri = this.Uri,
        ValidationResultsString = this.ValidationResultsString,
        QueueOptions = this.QueueOptions,
        ChangesCalculated = this.ChangesCalculated,
        AppendCommitMessageToRunName = this.AppendCommitMessageToRunName
      };
      if (this.Repository != null)
      {
        BuildData buildData2 = buildData1;
        BuildRepository buildRepository = new BuildRepository();
        buildRepository.Id = this.Repository.Id;
        buildRepository.Type = this.Repository.Type;
        buildData2.Repository = (MinimalBuildRepository) buildRepository;
      }
      if (this.Definition != null)
        buildData1.Definition = new MinimalBuildDefinition()
        {
          ProjectId = this.ProjectId,
          Id = this.Definition.Id,
          Revision = this.Definition.Revision,
          Name = this.Definition.Name,
          QueueStatus = this.Definition.QueueStatus,
          Path = this.Definition.Path
        };
      if (this.Logs != null)
        buildData1.Logs = new LogReference()
        {
          Type = this.Logs.Type
        };
      if (this.TriggeredByBuild != null)
        buildData1.TriggeredByBuild = new TriggeredByBuild()
        {
          ProjectId = this.TriggeredByBuild.ProjectId,
          DefinitionId = this.TriggeredByBuild.DefinitionId,
          DefinitionVersion = this.TriggeredByBuild.DefinitionVersion,
          BuildId = this.TriggeredByBuild.BuildId
        };
      if (this.Tags != null)
      {
        foreach (string tag in this.Tags)
          buildData1.Tags.Add(tag);
      }
      if (this.OrchestrationPlan != null)
        buildData1.OrchestrationPlan = new TaskOrchestrationPlanReference()
        {
          OrchestrationType = this.OrchestrationPlan.OrchestrationType,
          PlanId = this.OrchestrationPlan.PlanId
        };
      if (this.StagesToSkip.Count > 0)
        buildData1.StagesToSkip.UnionWith((IEnumerable<string>) this.StagesToSkip);
      if (this.Resources != null)
        buildData1.Resources = this.Resources.Clone();
      if (this.TemplateParameters.Count > 0)
        buildData1.TemplateParameters = new Dictionary<string, object>((IDictionary<string, object>) this.TemplateParameters);
      if (this.RetentionLeases.Count > 0)
        buildData1.RetentionLeases = new List<RetentionLease>((IEnumerable<RetentionLease>) this.RetentionLeases);
      return buildData1;
    }

    private void SummonValidationResults()
    {
      if (!string.IsNullOrEmpty(this.ValidationResultsString))
        this.m_validationResults = JsonUtility.FromString<List<BuildRequestValidationResult>>(this.ValidationResultsString);
      else
        this.m_validationResults = new List<BuildRequestValidationResult>();
    }
  }
}
