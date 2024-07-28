// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.PipelineContext
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using System;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  public class PipelineContext
  {
    public int Id;
    public string Uri;
    public Guid ProjectId;
    public string ProjectUrl;
    public string Result;
    public int? PipelineId;
    public string DefinitionName;
    public int? DefinitionId;
    public DateTime FinishTime;
    public DateTime CreatedDate;
    public string Number;
    public string SourceVersion;
    public Guid RepositoryId;
    public string SourceRepositoryUri;
    public string BranchName;
    public string RepositoryType;
    public string BuildSystem;
    public bool IsPullRequestScenario;
    public int PullRequestId;
    public string SourceCommitId;
    public int PullRequestIterationId;
    public string PullRequestIterationUrl;
    public string CommonRefCommitId;
    public string SourceRefCommitId;
    public string CoverageReportUrl;
    public Guid E2ETrackingId;

    public CoverageStatusCheckConfiguration CodeCoverageSettings { get; set; }
  }
}
