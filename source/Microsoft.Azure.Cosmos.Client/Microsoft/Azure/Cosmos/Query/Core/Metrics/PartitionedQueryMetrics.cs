// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Query.Core.Metrics.PartitionedQueryMetrics
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.Azure.Cosmos.Query.Core.Metrics
{
  internal sealed class PartitionedQueryMetrics : 
    IReadOnlyDictionary<string, QueryMetrics>,
    IEnumerable<KeyValuePair<string, QueryMetrics>>,
    IEnumerable,
    IReadOnlyCollection<KeyValuePair<string, QueryMetrics>>
  {
    private readonly Dictionary<string, QueryMetrics> partitionedQueryMetrics;

    public PartitionedQueryMetrics(IReadOnlyDictionary<string, QueryMetrics> other)
      : this()
    {
      foreach (KeyValuePair<string, QueryMetrics> keyValuePair in (IEnumerable<KeyValuePair<string, QueryMetrics>>) other)
        this.partitionedQueryMetrics[keyValuePair.Key] = keyValuePair.Value;
    }

    public PartitionedQueryMetrics() => this.partitionedQueryMetrics = new Dictionary<string, QueryMetrics>();

    public int Count => this.partitionedQueryMetrics.Count;

    public IEnumerable<string> Keys => (IEnumerable<string>) this.partitionedQueryMetrics.Keys;

    public IEnumerable<QueryMetrics> Values => (IEnumerable<QueryMetrics>) this.partitionedQueryMetrics.Values;

    public QueryMetrics this[string key] => this.partitionedQueryMetrics[key];

    public static PartitionedQueryMetrics CreateFromIEnumerable(
      IEnumerable<PartitionedQueryMetrics> partitionedQueryMetricsList)
    {
      return new PartitionedQueryMetrics((IReadOnlyDictionary<string, QueryMetrics>) partitionedQueryMetricsList.SelectMany<PartitionedQueryMetrics, KeyValuePair<string, QueryMetrics>>((Func<PartitionedQueryMetrics, IEnumerable<KeyValuePair<string, QueryMetrics>>>) (partitionedQueryMetrics => (IEnumerable<KeyValuePair<string, QueryMetrics>>) partitionedQueryMetrics)).ToLookup<KeyValuePair<string, QueryMetrics>, string, QueryMetrics>((Func<KeyValuePair<string, QueryMetrics>, string>) (pair => pair.Key), (Func<KeyValuePair<string, QueryMetrics>, QueryMetrics>) (pair => pair.Value)).ToDictionary<IGrouping<string, QueryMetrics>, string, QueryMetrics>((Func<IGrouping<string, QueryMetrics>, string>) (group => group.Key), (Func<IGrouping<string, QueryMetrics>, QueryMetrics>) (group => QueryMetrics.CreateFromIEnumerable((IEnumerable<QueryMetrics>) group))));
    }

    public static PartitionedQueryMetrics operator +(
      PartitionedQueryMetrics partitionedQueryMetrics1,
      PartitionedQueryMetrics partitionedQueryMetrics2)
    {
      return partitionedQueryMetrics1.Add(partitionedQueryMetrics2);
    }

    public PartitionedQueryMetrics Add(
      params PartitionedQueryMetrics[] partitionedQueryMetricsList)
    {
      List<PartitionedQueryMetrics> partitionedQueryMetricsList1 = new List<PartitionedQueryMetrics>(partitionedQueryMetricsList.Length + 1);
      partitionedQueryMetricsList1.Add(this);
      partitionedQueryMetricsList1.AddRange((IEnumerable<PartitionedQueryMetrics>) partitionedQueryMetricsList);
      return PartitionedQueryMetrics.CreateFromIEnumerable((IEnumerable<PartitionedQueryMetrics>) partitionedQueryMetricsList1);
    }

    public override string ToString() => this.ToTextString();

    public bool ContainsKey(string key) => this.partitionedQueryMetrics.ContainsKey(key);

    public IEnumerator<KeyValuePair<string, QueryMetrics>> GetEnumerator() => (IEnumerator<KeyValuePair<string, QueryMetrics>>) this.partitionedQueryMetrics.GetEnumerator();

    public bool TryGetValue(string key, out QueryMetrics value) => this.partitionedQueryMetrics.TryGetValue(key, out value);

    IEnumerator IEnumerable.GetEnumerator() => (IEnumerator) this.GetEnumerator();

    private string ToTextString()
    {
      StringBuilder stringBuilder = new StringBuilder();
      foreach (KeyValuePair<string, QueryMetrics> keyValuePair in (IEnumerable<KeyValuePair<string, QueryMetrics>>) this.partitionedQueryMetrics.OrderBy<KeyValuePair<string, QueryMetrics>, string>((Func<KeyValuePair<string, QueryMetrics>, string>) (kvp => kvp.Key)))
      {
        stringBuilder.AppendFormat("Partition {0}", (object) keyValuePair.Key);
        stringBuilder.AppendLine();
        stringBuilder.Append((object) keyValuePair.Value);
        stringBuilder.AppendLine();
      }
      return stringBuilder.ToString();
    }
  }
}
