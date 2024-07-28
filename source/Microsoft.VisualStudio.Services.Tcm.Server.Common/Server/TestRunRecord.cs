// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.TestRunRecord
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using System;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  public class TestRunRecord
  {
    public Guid ProjectGuid { get; internal set; }

    public int TestRunId { get; internal set; }

    public string TestRunTitle { get; internal set; }

    public DateTime? DateStarted { get; internal set; }

    public DateTime? DateCompleted { get; internal set; }

    public bool IsAutomated { get; internal set; }

    public int TotalTests { get; internal set; }

    public DateTime ChangedDate { get; internal set; }

    public string SourceWorkflow { get; internal set; }

    public int BuildDefinitionId { get; internal set; }

    public int BuildId { get; internal set; }

    public string RepositoryId { get; internal set; }

    public string BranchName { get; internal set; }

    public int ReleaseDefinitionId { get; internal set; }

    public int ReleaseEnvironmentDefinitionId { get; internal set; }

    public int ReleaseId { get; internal set; }

    public int ReleaseEnvironmentId { get; internal set; }

    public int Attempt { get; internal set; }

    public TestArtifactSource DataSourceId { get; internal set; }
  }
}
