// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.ProjectAnalysis.Server.LanguageMetadataRecord
// Assembly: Microsoft.TeamFoundation.ProjectAnalysis.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 076482BC-74A4-4A35-9427-1E61C33D1FA6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.ProjectAnalysis.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Git.Server;
using Microsoft.TeamFoundation.ProjectAnalysis.WebApi;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.ProjectAnalysis.Server
{
  public class LanguageMetadataRecord
  {
    internal LanguageMetadataRecord(
      Guid projectId,
      byte repositoryType,
      Guid repositoryId,
      int fileCount,
      DateTime updatedTime,
      int? changesetId,
      Sha1Id? commitId,
      string branch,
      string languageBreakdown,
      byte resultPhase,
      int recordVersion)
    {
      this.ProjectId = projectId;
      this.RepositoryType = (RepositoryType) repositoryType;
      this.RepositoryId = repositoryId;
      this.FileCount = fileCount;
      this.UpdatedTime = updatedTime;
      this.CommitId = commitId;
      this.Branch = branch;
      this.ChangesetId = changesetId;
      this.LanguageBreakdown = JsonUtilities.Deserialize<List<LanguageStatistics>>(languageBreakdown, true);
      this.ResultPhase = (ResultPhase) resultPhase;
      this.RecordVersion = recordVersion;
      foreach (LanguageMetricsSecuredObject metricsSecuredObject in this.LanguageBreakdown)
        metricsSecuredObject.ProjectId = this.ProjectId;
    }

    public LanguageMetadataRecord(
      Guid projectId,
      RepositoryType repositoryType,
      Guid repositoryId,
      int fileCount,
      DateTime updatedTime,
      int? changesetId,
      Sha1Id? commitId,
      string branch,
      List<LanguageStatistics> languageBreakdown,
      ResultPhase resultPhase,
      int recordVersion)
    {
      this.ProjectId = projectId;
      this.RepositoryType = repositoryType;
      this.RepositoryId = repositoryId;
      this.FileCount = fileCount;
      this.UpdatedTime = updatedTime;
      this.CommitId = commitId;
      this.Branch = branch;
      this.ChangesetId = changesetId;
      this.LanguageBreakdown = languageBreakdown;
      this.ResultPhase = resultPhase;
      this.RecordVersion = recordVersion;
    }

    public Guid ProjectId { get; }

    public RepositoryType RepositoryType { get; }

    public Guid RepositoryId { get; }

    public int FileCount { get; }

    public DateTime UpdatedTime { get; }

    public Sha1Id? CommitId { get; }

    public int? ChangesetId { get; }

    public string Branch { get; }

    public List<LanguageStatistics> LanguageBreakdown { get; }

    public ResultPhase ResultPhase { get; }

    public int RecordVersion { get; }
  }
}
