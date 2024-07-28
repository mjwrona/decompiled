// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.GitAdvSecEnablementStatus
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using System;

namespace Microsoft.TeamFoundation.Git.Server
{
  public class GitAdvSecEnablementStatus
  {
    public GitAdvSecEnablementStatus(
      Guid projectId,
      Guid repositoryId,
      bool? enabled,
      DateTime? enabledChangedOnDate)
    {
      this.ProjectId = projectId;
      this.RepositoryId = repositoryId;
      this.Enabled = enabled;
      this.EnabledChangedOnDate = enabledChangedOnDate;
      this.ChangedOnDate = enabledChangedOnDate;
      this.ChangedBy = Guid.Empty;
    }

    public GitAdvSecEnablementStatus(
      Guid projectId,
      Guid repositoryId,
      bool? enabled,
      DateTime? changedOnDate,
      Guid changedBy)
    {
      this.ProjectId = projectId;
      this.RepositoryId = repositoryId;
      this.Enabled = enabled;
      this.EnabledChangedOnDate = changedOnDate;
      this.ChangedOnDate = changedOnDate;
      this.ChangedBy = changedBy;
    }

    public Guid ProjectId { get; }

    public Guid RepositoryId { get; }

    public bool? Enabled { get; }

    public DateTime? EnabledChangedOnDate { get; }

    public DateTime? ChangedOnDate { get; }

    public Guid ChangedBy { get; }
  }
}
