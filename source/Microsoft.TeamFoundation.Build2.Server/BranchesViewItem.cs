// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Server.BranchesViewItem
// Assembly: Microsoft.TeamFoundation.Build2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 680FF5F5-CB5D-4078-8EFA-56C292BFDBB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.Server.dll

using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Build2.Server
{
  public class BranchesViewItem
  {
    public BranchesViewItem(
      int buildId,
      string buildNumber,
      BuildStatus status,
      BuildResult? result,
      string repositoryType,
      int repositoryId,
      string sourceBranch,
      int sourceBranchId,
      DateTime queueTime,
      DateTime? startTime,
      DateTime? finishTime,
      string sourceVersionMessage,
      bool appendCommitMessageToRunName)
    {
      this.BuildId = buildId;
      this.BuildNumber = buildNumber;
      this.Status = status;
      this.Result = result;
      this.RepositoryType = repositoryType;
      this.RepositoryId = repositoryId;
      this.SourceBranch = sourceBranch;
      this.SourceBranchId = sourceBranchId;
      this.QueueTime = queueTime;
      this.StartTime = startTime;
      this.FinishTime = finishTime;
      this.SourceVersionMessage = sourceVersionMessage ?? string.Empty;
      this.RepositoryBranches = (IList<BranchesViewItemRepositoryBranch>) new List<BranchesViewItemRepositoryBranch>();
      this.AppendCommitMessageToRunName = appendCommitMessageToRunName;
    }

    public int BuildId { get; private set; }

    public string BuildNumber { get; private set; }

    public BuildStatus Status { get; private set; }

    public BuildResult? Result { get; private set; }

    public string RepositoryType { get; private set; }

    public int RepositoryId { get; private set; }

    public string SourceBranch { get; private set; }

    public int SourceBranchId { get; private set; }

    public DateTime QueueTime { get; private set; }

    public DateTime? StartTime { get; private set; }

    public DateTime? FinishTime { get; private set; }

    public string SourceVersionMessage { get; private set; }

    public bool AppendCommitMessageToRunName { get; private set; }

    public IList<BranchesViewItemRepositoryBranch> RepositoryBranches { get; }
  }
}
