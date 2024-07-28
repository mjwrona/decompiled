// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.AutoTriggerIssue
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Data, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A69B0FB7-3028-4162-BA38-794D80D7A49A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Data.dll

using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations;
using System;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model
{
  public abstract class AutoTriggerIssue
  {
    protected AutoTriggerIssue()
    {
    }

    protected AutoTriggerIssue(
      ReleaseTriggerType triggerType,
      Guid projectId,
      int releaseDefinitionId,
      IssueSource issueSource,
      Issue issue)
    {
      this.ReleaseTriggerType = triggerType;
      this.ProjectId = projectId;
      this.ReleaseDefinitionId = releaseDefinitionId;
      this.IssueSource = issueSource;
      this.Issue = issue;
    }

    public ReleaseTriggerType ReleaseTriggerType { get; set; }

    public Guid ProjectId { get; set; }

    public int ReleaseDefinitionId { get; set; }

    public string ReleaseDefinitionName { get; set; }

    public IssueSource IssueSource { get; set; }

    public Issue Issue { get; set; }
  }
}
