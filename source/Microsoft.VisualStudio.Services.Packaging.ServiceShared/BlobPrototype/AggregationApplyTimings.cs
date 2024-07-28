// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.AggregationApplyTimings
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype
{
  public class AggregationApplyTimings
  {
    public Dictionary<string, long> AggNameToElapsedMsMap { get; } = new Dictionary<string, long>();

    public void AddTime(string key, long incrementMilliseconds)
    {
      long num;
      if (!this.AggNameToElapsedMsMap.TryGetValue(key, out num))
        num = 0L;
      this.AggNameToElapsedMsMap[key] = num + incrementMilliseconds;
    }

    public static AggregationApplyTimings FromSingleSource(string key, long timeMilliseconds)
    {
      AggregationApplyTimings aggregationApplyTimings = new AggregationApplyTimings();
      aggregationApplyTimings.AddTime(key, timeMilliseconds);
      return aggregationApplyTimings;
    }

    public void MergeWith(AggregationApplyTimings source)
    {
      if (source == null)
        return;
      foreach (KeyValuePair<string, long> aggNameToElapsedMs in source.AggNameToElapsedMsMap)
        this.AddTime(aggNameToElapsedMs.Key, aggNameToElapsedMs.Value);
    }
  }
}
