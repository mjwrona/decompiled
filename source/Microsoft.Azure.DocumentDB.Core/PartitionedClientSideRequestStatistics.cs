// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.PartitionedClientSideRequestStatistics
// Assembly: Microsoft.Azure.DocumentDB.Core, Version=2.10.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 20BB0EB7-1465-494C-8F4E-F898448B85D9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DocumentDB.Core.dll

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.Azure.Documents
{
  internal sealed class PartitionedClientSideRequestStatistics : 
    IReadOnlyDictionary<string, IReadOnlyList<IClientSideRequestStatistics>>,
    IEnumerable<KeyValuePair<string, IReadOnlyList<IClientSideRequestStatistics>>>,
    IEnumerable,
    IReadOnlyCollection<KeyValuePair<string, IReadOnlyList<IClientSideRequestStatistics>>>
  {
    private readonly Dictionary<string, List<IClientSideRequestStatistics>> partitionedClientSideRequestStatistics;

    private PartitionedClientSideRequestStatistics() => this.partitionedClientSideRequestStatistics = new Dictionary<string, List<IClientSideRequestStatistics>>();

    public IReadOnlyList<IClientSideRequestStatistics> this[string key] => (IReadOnlyList<IClientSideRequestStatistics>) this.partitionedClientSideRequestStatistics[key];

    public IEnumerable<string> Keys => (IEnumerable<string>) this.partitionedClientSideRequestStatistics.Keys;

    public IEnumerable<IReadOnlyList<IClientSideRequestStatistics>> Values => (IEnumerable<IReadOnlyList<IClientSideRequestStatistics>>) this.partitionedClientSideRequestStatistics.Values;

    public int Count => this.partitionedClientSideRequestStatistics.Count;

    public bool ContainsKey(string key) => this.partitionedClientSideRequestStatistics.ContainsKey(key);

    public IEnumerator<KeyValuePair<string, IReadOnlyList<IClientSideRequestStatistics>>> GetEnumerator() => (IEnumerator<KeyValuePair<string, IReadOnlyList<IClientSideRequestStatistics>>>) new PartitionedClientSideRequestStatistics.PartitionedClientSideRequestStatisticsEnumerator((IReadOnlyDictionary<string, List<IClientSideRequestStatistics>>) this.partitionedClientSideRequestStatistics);

    public bool TryGetValue(
      string key,
      out IReadOnlyList<IClientSideRequestStatistics> value)
    {
      value = (IReadOnlyList<IClientSideRequestStatistics>) null;
      List<IClientSideRequestStatistics> requestStatisticsList;
      int num = this.partitionedClientSideRequestStatistics.TryGetValue(key, out requestStatisticsList) ? 1 : 0;
      value = (IReadOnlyList<IClientSideRequestStatistics>) requestStatisticsList;
      return num != 0;
    }

    IEnumerator IEnumerable.GetEnumerator() => (IEnumerator) this.GetEnumerator();

    public void AddClientSideRequestStatisticsToPartition(
      string partitionId,
      IClientSideRequestStatistics clientSideRequestStatistics)
    {
      if (partitionId == null)
        throw new ArgumentNullException(nameof (partitionId));
      if (clientSideRequestStatistics == null)
        throw new ArgumentNullException(nameof (partitionId));
      List<IClientSideRequestStatistics> requestStatisticsList;
      if (!this.partitionedClientSideRequestStatistics.TryGetValue(partitionId, out requestStatisticsList))
      {
        requestStatisticsList = new List<IClientSideRequestStatistics>();
        this.partitionedClientSideRequestStatistics[partitionId] = requestStatisticsList;
      }
      requestStatisticsList.Add(clientSideRequestStatistics);
    }

    public override string ToString()
    {
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.AppendLine("{");
      foreach (KeyValuePair<string, List<IClientSideRequestStatistics>> keyValuePair in (IEnumerable<KeyValuePair<string, List<IClientSideRequestStatistics>>>) this.partitionedClientSideRequestStatistics.OrderBy<KeyValuePair<string, List<IClientSideRequestStatistics>>, double>((Func<KeyValuePair<string, List<IClientSideRequestStatistics>>, double>) (kvp => kvp.Value.Sum<IClientSideRequestStatistics>((Func<IClientSideRequestStatistics, double>) (stats => stats.RequestLatency.TotalMilliseconds)))))
      {
        stringBuilder.Append(keyValuePair.Key);
        stringBuilder.Append(":");
        stringBuilder.Append("[");
        stringBuilder.AppendLine();
        List<IClientSideRequestStatistics> source = keyValuePair.Value;
        for (int index = 0; index < source.Count<IClientSideRequestStatistics>(); ++index)
        {
          if (index > 0)
            stringBuilder.AppendLine(",");
          source[index].AppendToBuilder(stringBuilder);
        }
        stringBuilder.AppendLine();
        stringBuilder.Append("]");
      }
      stringBuilder.AppendLine("}");
      return stringBuilder.ToString();
    }

    public void AddTo(PartitionedClientSideRequestStatistics destination)
    {
      foreach (KeyValuePair<string, IReadOnlyList<IClientSideRequestStatistics>> keyValuePair in this)
      {
        string key = keyValuePair.Key;
        foreach (ClientSideRequestStatistics clientSideRequestStatistics in (IEnumerable<IClientSideRequestStatistics>) keyValuePair.Value)
          destination.AddClientSideRequestStatisticsToPartition(key, (IClientSideRequestStatistics) clientSideRequestStatistics);
      }
    }

    public static PartitionedClientSideRequestStatistics CreateFromIEnumerable(
      IEnumerable<PartitionedClientSideRequestStatistics> partitionedClientSideRequestStatisticsList)
    {
      if (partitionedClientSideRequestStatisticsList == null)
        throw new ArgumentNullException(nameof (partitionedClientSideRequestStatisticsList));
      PartitionedClientSideRequestStatistics empty = PartitionedClientSideRequestStatistics.CreateEmpty();
      foreach (PartitionedClientSideRequestStatistics requestStatistics in partitionedClientSideRequestStatisticsList)
        requestStatistics.AddTo(empty);
      return empty;
    }

    public static PartitionedClientSideRequestStatistics CreateFromDictionary(
      IReadOnlyDictionary<string, IReadOnlyList<IClientSideRequestStatistics>> dictionary)
    {
      if (dictionary == null)
        throw new ArgumentNullException(nameof (dictionary));
      PartitionedClientSideRequestStatistics empty = PartitionedClientSideRequestStatistics.CreateEmpty();
      foreach (KeyValuePair<string, IReadOnlyList<IClientSideRequestStatistics>> keyValuePair in (IEnumerable<KeyValuePair<string, IReadOnlyList<IClientSideRequestStatistics>>>) dictionary)
      {
        string key = keyValuePair.Key;
        foreach (ClientSideRequestStatistics clientSideRequestStatistics in (IEnumerable<IClientSideRequestStatistics>) keyValuePair.Value)
          empty.AddClientSideRequestStatisticsToPartition(key, (IClientSideRequestStatistics) clientSideRequestStatistics);
      }
      return empty;
    }

    public static PartitionedClientSideRequestStatistics CreateEmpty() => new PartitionedClientSideRequestStatistics();

    public static PartitionedClientSideRequestStatistics CreateFromSingleRequest(
      string id,
      IClientSideRequestStatistics clientSideRequestStatistics)
    {
      PartitionedClientSideRequestStatistics empty = PartitionedClientSideRequestStatistics.CreateEmpty();
      empty.AddClientSideRequestStatisticsToPartition(id, clientSideRequestStatistics);
      return empty;
    }

    private sealed class PartitionedClientSideRequestStatisticsEnumerator : 
      IEnumerator<KeyValuePair<string, IReadOnlyList<IClientSideRequestStatistics>>>,
      IEnumerator,
      IDisposable
    {
      private readonly IEnumerator<KeyValuePair<string, List<IClientSideRequestStatistics>>> enumerator;

      public PartitionedClientSideRequestStatisticsEnumerator(
        IReadOnlyDictionary<string, List<IClientSideRequestStatistics>> dictionary)
      {
        this.enumerator = dictionary != null ? dictionary.GetEnumerator() : throw new ArgumentNullException(nameof (dictionary));
      }

      public KeyValuePair<string, IReadOnlyList<IClientSideRequestStatistics>> Current
      {
        get
        {
          KeyValuePair<string, List<IClientSideRequestStatistics>> current = this.enumerator.Current;
          string key = current.Key;
          current = this.enumerator.Current;
          List<IClientSideRequestStatistics> requestStatisticsList = current.Value;
          return new KeyValuePair<string, IReadOnlyList<IClientSideRequestStatistics>>(key, (IReadOnlyList<IClientSideRequestStatistics>) requestStatisticsList);
        }
      }

      object IEnumerator.Current => (object) this.Current;

      public void Dispose() => this.enumerator.Dispose();

      public bool MoveNext() => this.enumerator.MoveNext();

      public void Reset() => this.enumerator.Reset();
    }
  }
}
