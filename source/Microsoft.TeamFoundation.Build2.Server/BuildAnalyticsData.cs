// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Server.BuildAnalyticsData
// Assembly: Microsoft.TeamFoundation.Build2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 680FF5F5-CB5D-4078-8EFA-56C292BFDBB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.Server.dll

using System;

namespace Microsoft.TeamFoundation.Build2.Server
{
  public class BuildAnalyticsData : ShallowBuildAnalyticsData
  {
    public int DefinitionId { get; internal set; }

    public int DefinitionVersion { get; internal set; }

    public string BuildNumber { get; internal set; }

    public int? BuildNumberRevision { get; internal set; }

    public string RepositoryId { get; internal set; }

    public string RepositoryType { get; internal set; }

    public string BranchName { get; set; }

    public BuildStatus? Status { get; internal set; }

    public DateTime? StartTime { get; internal set; }

    public DateTime? FinishTime { get; internal set; }

    public DateTime? QueueTime { get; internal set; }

    public BuildReason Reason { get; internal set; }

    public BuildResult? Result { get; internal set; }

    public Guid PlanId { get; internal set; }
  }
}
