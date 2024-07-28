// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.MetricsTrendItem
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using Microsoft.TeamFoundation.SourceControl.WebApi;

namespace Microsoft.TeamFoundation.Git.Server
{
  internal class MetricsTrendItem
  {
    public MetricsTrendItem(GitMetrics gitMetricType, int timeBucket, int intervalValue)
    {
      this.MetricType = gitMetricType;
      this.TimeBucket = timeBucket;
      this.IntervalValue = intervalValue;
    }

    public GitMetrics MetricType { get; }

    public int TimeBucket { get; }

    public int IntervalValue { get; }
  }
}
