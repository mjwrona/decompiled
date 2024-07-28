// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.WebApi.Build
// Assembly: Microsoft.TeamFoundation.Build2.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0683407D-7C61-4505-8CA6-86AD7E3B6BCA
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Build2.WebApi.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Build.WebApi
{
  [DataContract]
  public class Build : ISecuredObject
  {
    [DataMember(Name = "_links", EmitDefaultValue = false)]
    private ReferenceLinks m_links;
    [DataMember(EmitDefaultValue = false, Name = "Properties")]
    private PropertiesCollection m_properties;
    [DataMember(EmitDefaultValue = false, Name = "Tags")]
    private List<string> m_tags;
    [DataMember(EmitDefaultValue = false, Name = "ValidationResults")]
    private List<BuildRequestValidationResult> m_validationResults;
    [DataMember(EmitDefaultValue = false, Name = "Plans")]
    private List<TaskOrchestrationPlanReference> m_plans;
    [DataMember(EmitDefaultValue = false, Name = "TemplateParameters")]
    private Dictionary<string, string> m_templateParameters;
    [DataMember(EmitDefaultValue = false, Name = "TriggerInfo")]
    private Dictionary<string, string> m_triggerInfo;
    private string m_nestingToken = string.Empty;

    public Build()
    {
      this.Reason = BuildReason.Manual;
      this.Priority = QueuePriority.Normal;
    }

    [DataMember(EmitDefaultValue = false)]
    [Key]
    public int Id { get; [EditorBrowsable(EditorBrowsableState.Never)] set; }

    [DataMember(EmitDefaultValue = false)]
    public string BuildNumber { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public BuildStatus? Status { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public BuildResult? Result { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public DateTime? QueueTime { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public DateTime? StartTime { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public DateTime? FinishTime { get; set; }

    public ReferenceLinks Links
    {
      get
      {
        if (this.m_links == null)
          this.m_links = new ReferenceLinks();
        return this.m_links;
      }
    }

    [DataMember(EmitDefaultValue = false)]
    public string Url { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public DefinitionReference Definition { get; [EditorBrowsable(EditorBrowsableState.Never)] set; }

    [DataMember(EmitDefaultValue = false)]
    public int? BuildNumberRevision { get; [EditorBrowsable(EditorBrowsableState.Never)] set; }

    [DataMember(EmitDefaultValue = false)]
    public TeamProjectReference Project { get; [EditorBrowsable(EditorBrowsableState.Never)] set; }

    [DataMember(EmitDefaultValue = false)]
    public Uri Uri { get; [EditorBrowsable(EditorBrowsableState.Never)] set; }

    [DataMember(EmitDefaultValue = false)]
    public string SourceBranch { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string SourceVersion { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public AgentPoolQueue Queue { get; [EditorBrowsable(EditorBrowsableState.Never)] set; }

    [DataMember(EmitDefaultValue = false)]
    public AgentSpecification AgentSpecification { get; [EditorBrowsable(EditorBrowsableState.Never)] set; }

    [DataMember(EmitDefaultValue = false)]
    public BuildController Controller { get; [EditorBrowsable(EditorBrowsableState.Never)] set; }

    [DataMember(EmitDefaultValue = false)]
    public int? QueuePosition { get; [EditorBrowsable(EditorBrowsableState.Never)] set; }

    [DataMember(EmitDefaultValue = false)]
    public QueuePriority Priority { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public BuildReason Reason { get; [EditorBrowsable(EditorBrowsableState.Never)] set; }

    [DataMember(EmitDefaultValue = false)]
    public IdentityRef RequestedFor { get; [EditorBrowsable(EditorBrowsableState.Never)] set; }

    [DataMember(EmitDefaultValue = false)]
    public IdentityRef RequestedBy { get; [EditorBrowsable(EditorBrowsableState.Never)] set; }

    [DataMember(EmitDefaultValue = false)]
    public DateTime LastChangedDate { get; [EditorBrowsable(EditorBrowsableState.Never)] set; }

    [DataMember(EmitDefaultValue = false)]
    public IdentityRef LastChangedBy { get; [EditorBrowsable(EditorBrowsableState.Never)] set; }

    [DataMember(EmitDefaultValue = false)]
    public DateTime? DeletedDate { get; [EditorBrowsable(EditorBrowsableState.Never)] set; }

    [DataMember(EmitDefaultValue = false)]
    public IdentityRef DeletedBy { get; [EditorBrowsable(EditorBrowsableState.Never)] set; }

    [DataMember(EmitDefaultValue = false)]
    public string DeletedReason { get; [EditorBrowsable(EditorBrowsableState.Never)] set; }

    [DataMember(EmitDefaultValue = false)]
    public string Parameters { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public List<Demand> Demands { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public TaskOrchestrationPlanReference OrchestrationPlan { get; [EditorBrowsable(EditorBrowsableState.Never)] set; }

    public List<TaskOrchestrationPlanReference> Plans
    {
      get
      {
        if (this.m_plans == null)
          this.m_plans = new List<TaskOrchestrationPlanReference>();
        return this.m_plans;
      }
      set => this.m_plans = value;
    }

    [DataMember(EmitDefaultValue = false)]
    public BuildLogReference Logs { get; [EditorBrowsable(EditorBrowsableState.Never)] set; }

    [DataMember(EmitDefaultValue = false)]
    public BuildRepository Repository { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public QueueOptions QueueOptions { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public bool Deleted { get; set; }

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

    public List<BuildRequestValidationResult> ValidationResults
    {
      get
      {
        if (this.m_validationResults == null)
          this.m_validationResults = new List<BuildRequestValidationResult>();
        return this.m_validationResults;
      }
    }

    [DataMember(EmitDefaultValue = false)]
    [Obsolete("The KeepForever flag has been deprecated in favor of retention leases. To get or set if a build is retained, check for or create an appropriate retention lease.")]
    public bool? KeepForever { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string Quality { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public bool? RetainedByRelease { get; set; }

    public Dictionary<string, string> TemplateParameters
    {
      get
      {
        if (this.m_templateParameters == null)
          this.m_templateParameters = new Dictionary<string, string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
        return this.m_templateParameters;
      }
      set => this.m_templateParameters = value;
    }

    [DataMember]
    public Microsoft.TeamFoundation.Build.WebApi.Build TriggeredByBuild { get; set; }

    public IDictionary<string, string> TriggerInfo
    {
      get
      {
        if (this.m_triggerInfo == null)
          this.m_triggerInfo = new Dictionary<string, string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
        return (IDictionary<string, string>) this.m_triggerInfo;
      }
      internal set
      {
        if (value == null)
          return;
        this.m_triggerInfo = new Dictionary<string, string>(value, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      }
    }

    [DataMember(EmitDefaultValue = false, Name = "AppendCommitMessageToRunName")]
    public bool AppendCommitMessageToRunName { get; set; } = true;

    Guid ISecuredObject.NamespaceId => Security.BuildNamespaceId;

    int ISecuredObject.RequiredPermissions => BuildPermissions.ViewBuilds;

    string ISecuredObject.GetToken()
    {
      if (!string.IsNullOrEmpty(this.m_nestingToken))
        return this.m_nestingToken;
      return ((ISecuredObject) this.Definition)?.GetToken();
    }

    internal void SetNestingSecurityToken(string tokenValue) => this.m_nestingToken = tokenValue;
  }
}
