// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ContinuousDeploymentTriggerIssue
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Data, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A69B0FB7-3028-4162-BA38-794D80D7A49A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Data.dll

using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations;
using System;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model
{
  public class ContinuousDeploymentTriggerIssue : AutoTriggerIssue
  {
    public string ArtifactType { get; set; }

    public string SourceId { get; set; }

    public string ArtifactVersionId { get; set; }

    public ContinuousDeploymentTriggerIssue()
    {
    }

    public ContinuousDeploymentTriggerIssue(
      Guid projectId,
      string artifactVersionId,
      string sourceId,
      int releaseDefinitionId,
      string artifactType,
      ReleaseTriggerType triggerType,
      IssueSource issueSource,
      Issue issue)
      : base(triggerType, projectId, releaseDefinitionId, issueSource, issue)
    {
      this.ProjectId = projectId;
      this.ArtifactVersionId = artifactVersionId;
      this.SourceId = sourceId;
      this.ReleaseDefinitionId = releaseDefinitionId;
      this.ArtifactType = artifactType;
      this.ReleaseTriggerType = triggerType;
      this.IssueSource = issueSource;
      this.Issue = issue;
    }
  }
}
