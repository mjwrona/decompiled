// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.Graph.PathInfoStatistics
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

namespace Microsoft.TeamFoundation.Git.Server.Graph
{
  internal class PathInfoStatistics
  {
    public long NumTreesamePairQueries { get; set; }

    public long NumTreesameFirstParentQueries { get; set; }

    public long NumBloomFiltersNotComputed { get; set; }

    public long NumBloomFiltersTooLarge { get; set; }

    public long NumBloomFiltersTrueNegative { get; set; }

    public long NumBloomFiltersTruePositive { get; set; }

    public long NumBloomFiltersFalsePositive { get; set; }

    public long NumWalkPathCalls { get; set; }

    public long NumComparePathCalls { get; set; }

    public double FalsePositiveRate
    {
      get
      {
        long num = this.NumBloomFiltersTruePositive + this.NumBloomFiltersFalsePositive + this.NumBloomFiltersTrueNegative;
        return num == 0L ? 0.0 : (double) this.NumBloomFiltersFalsePositive / (double) num;
      }
    }
  }
}
