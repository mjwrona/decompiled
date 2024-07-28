// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.GitAdvSecProjectStats
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using System;

namespace Microsoft.TeamFoundation.Git.Server
{
  public class GitAdvSecProjectStats
  {
    public GitAdvSecProjectStats(
      Guid projectId,
      string projectName,
      int totalRepositories,
      int enabledRepositories,
      int validCommitters,
      int totalCommitters,
      int totalPushers)
    {
      this.ProjectId = projectId;
      this.ProjectName = projectName;
      this.TotalRepositories = totalRepositories;
      this.EnabledRepositories = enabledRepositories;
      this.ValidCommitters = validCommitters;
      this.TotalCommitters = totalCommitters;
      this.TotalPushers = totalPushers;
    }

    public Guid ProjectId { get; }

    public string ProjectName { get; }

    public int TotalRepositories { get; }

    public int EnabledRepositories { get; }

    public int ValidCommitters { get; }

    public int TotalCommitters { get; }

    public int TotalPushers { get; }
  }
}
