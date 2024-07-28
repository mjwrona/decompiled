// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.WebApi.PullRequestTrigger
// Assembly: Microsoft.TeamFoundation.Build2.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0683407D-7C61-4505-8CA6-86AD7E3B6BCA
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Build2.WebApi.dll

using Microsoft.TeamFoundation.Build.WebApi.Contracts;
using Microsoft.VisualStudio.Services.WebApi;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Build.WebApi
{
  [DataContract]
  public class PullRequestTrigger : BuildTrigger
  {
    [DataMember(Name = "SettingsSourceType", EmitDefaultValue = false)]
    private int m_settingsSourceType;
    [DataMember(Name = "BranchFilters", EmitDefaultValue = false)]
    private List<string> m_branchFilters;
    [DataMember(Name = "Forks", EmitDefaultValue = false)]
    private Forks m_forks;
    [DataMember(Name = "PathFilters", EmitDefaultValue = false)]
    private List<string> m_pathFilters;

    public PullRequestTrigger()
      : this((ISecuredObject) null)
    {
    }

    internal PullRequestTrigger(ISecuredObject securedObject)
      : base(DefinitionTriggerType.PullRequest, securedObject)
    {
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public int SettingsSourceType
    {
      get
      {
        if (this.m_settingsSourceType == 0)
          this.m_settingsSourceType = 1;
        return this.m_settingsSourceType;
      }
      set => this.m_settingsSourceType = value;
    }

    public Forks Forks
    {
      get
      {
        if (this.m_forks == null)
          this.m_forks = new Forks();
        return this.m_forks;
      }
      set => this.m_forks = value;
    }

    public List<string> BranchFilters
    {
      get
      {
        if (this.m_branchFilters == null)
          this.m_branchFilters = new List<string>();
        return this.m_branchFilters;
      }
      set => this.m_branchFilters = value;
    }

    public List<string> PathFilters
    {
      get
      {
        if (this.m_pathFilters == null)
          this.m_pathFilters = new List<string>();
        return this.m_pathFilters;
      }
      set => this.m_pathFilters = value;
    }

    [DataMember(Name = "AutoCancel", EmitDefaultValue = false)]
    public bool? AutoCancel { get; set; }

    [DataMember(Name = "RequireCommentsForNonTeamMembersOnly")]
    public bool RequireCommentsForNonTeamMembersOnly { get; set; }

    [DataMember(Name = "RequireCommentsForNonTeamMemberAndNonContributors")]
    public bool RequireCommentsForNonTeamMemberAndNonContributors { get; set; }

    [DataMember(Name = "IsCommentRequiredForPullRequest")]
    public bool IsCommentRequiredForPullRequest { get; set; }

    [DataMember(Name = "PipelineTriggerSettings", EmitDefaultValue = false)]
    public PipelineTriggerSettings PipelineTriggerSettings { get; set; }

    [System.Runtime.Serialization.OnSerializing]
    private void OnSerializing(StreamingContext context)
    {
      if (this.m_settingsSourceType != 1)
        return;
      this.m_settingsSourceType = 0;
    }
  }
}
