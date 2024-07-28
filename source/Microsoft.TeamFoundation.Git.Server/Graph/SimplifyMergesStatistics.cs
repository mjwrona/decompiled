// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.Graph.SimplifyMergesStatistics
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

namespace Microsoft.TeamFoundation.Git.Server.Graph
{
  public sealed class SimplifyMergesStatistics
  {
    public long WalkGraphMillis { get; set; }

    public long SimplifyAllLabelsMillis { get; set; }

    public long NumReachableByFirstParent { get; set; }

    public long NumReachableBySimplification { get; set; }

    public long NumReachabilityQueries { get; set; }

    public int NumSimplifiedVertices { get; set; }
  }
}
