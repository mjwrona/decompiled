// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.TimeFilter
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using Microsoft.TeamFoundation.SourceControl.WebApi;
using System;

namespace Microsoft.TeamFoundation.Git.Server
{
  public class TimeFilter
  {
    public TimeFilter(GitPullRequestSearchCriteria searchCriteria)
    {
      this.TimeRangeType = (PullRequestTimeRangeType) ((int) searchCriteria.QueryTimeRangeType ?? 1);
      this.MinTime = searchCriteria.MinTime;
      this.MaxTime = searchCriteria.MaxTime;
    }

    public DateTime? MinTime { get; set; }

    public DateTime? MaxTime { get; set; }

    public PullRequestTimeRangeType TimeRangeType { get; set; }

    public DateTime? GetClosedMinTime() => this.TimeRangeType != PullRequestTimeRangeType.Closed ? new DateTime?() : this.MinTime;

    public DateTime? GetClosedMaxTime() => this.TimeRangeType != PullRequestTimeRangeType.Closed ? new DateTime?() : this.MaxTime;

    public DateTime? GetCreationMinTime() => this.TimeRangeType != PullRequestTimeRangeType.Created ? new DateTime?() : this.MinTime;

    public DateTime? GetCreationMaxTime() => this.TimeRangeType != PullRequestTimeRangeType.Created ? new DateTime?() : this.MaxTime;
  }
}
