// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.GitCommitScanData
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using System;

namespace Microsoft.TeamFoundation.Git.Server
{
  public class GitCommitScanData
  {
    internal GitCommitScanData(
      Sha1Id commitId,
      DateTime commitTime,
      int toolId,
      int versionId,
      int scanStatus,
      int attemptCount,
      DateTime scanTime)
    {
      this.CommitId = commitId;
      this.CommitTime = commitTime;
      this.ToolId = toolId;
      this.VersionId = versionId;
      this.ScanStatus = scanStatus;
      this.AttemptCount = attemptCount;
      this.ScanTime = scanTime;
    }

    public Sha1Id CommitId { get; }

    public DateTime CommitTime { get; }

    public int ToolId { get; }

    public int VersionId { get; }

    public int ScanStatus { get; set; }

    public int AttemptCount { get; set; }

    public DateTime ScanTime { get; set; }
  }
}
