// Decompiled with JetBrains decompiler
// Type: Microsoft.Cloud.Metrics.Client.Query.QueryResultQualityInfo
// Assembly: Microsoft.Cloud.Metrics.Client, Version=2.2023.705.2051, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 06B39E1C-7DF0-4BC1-AFBA-9AD635E73CB0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Cloud.Metrics.Client.dll

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;

namespace Microsoft.Cloud.Metrics.Client.Query
{
  public sealed class QueryResultQualityInfo
  {
    private readonly ConcurrentDictionary<string, int> droppedTimeSeries = new ConcurrentDictionary<string, int>();
    private int totalDroppedTimeSeries;
    private int totalEstimatedTimeSeries;

    public int TotalEstimatedTimeSeries
    {
      get => this.totalEstimatedTimeSeries;
      set => this.totalEstimatedTimeSeries = value;
    }

    public int TotalDroppedTimeSeries => this.totalDroppedTimeSeries;

    public int TotalEvaluatedTimeSeries => this.TotalEstimatedTimeSeries - this.totalDroppedTimeSeries;

    public ICollection<string> GetDroppedTimeSeriesReasons() => this.droppedTimeSeries.Keys;

    public int GetDroppedTimeSeriesByReason(string reason) => this.droppedTimeSeries[reason];

    public void RegisterDroppedTimeSeries(string reason, int count)
    {
      if (count <= 0)
        return;
      Interlocked.Add(ref this.totalDroppedTimeSeries, count);
      this.droppedTimeSeries.AddOrUpdate(reason, count, (Func<string, int, int>) ((key, existingValue) => existingValue + count));
    }

    public void RegisterEstimatedTimeSeries(int count) => Interlocked.Add(ref this.totalEstimatedTimeSeries, count);

    public override string ToString()
    {
      string seed = string.Format("Total Estimated TimeSeries:{0}, Total Dropped TimeSeries:{1}.", (object) this.TotalEstimatedTimeSeries, (object) this.totalDroppedTimeSeries);
      if (this.totalDroppedTimeSeries > 0)
        seed = this.droppedTimeSeries.Aggregate<KeyValuePair<string, int>, string>(seed, (Func<string, KeyValuePair<string, int>, string>) ((current, dropReason) => current + string.Format("{0}:{1}", (object) dropReason.Key, (object) dropReason.Value)));
      return seed;
    }

    public void Deserialize(BinaryReader reader)
    {
      int num1 = (int) reader.ReadByte();
      this.TotalEstimatedTimeSeries = reader.ReadInt32();
      this.totalDroppedTimeSeries = reader.ReadInt32();
      if (this.totalDroppedTimeSeries <= 0)
        return;
      int num2 = reader.ReadInt32();
      for (int index = 0; index < num2; ++index)
        this.RegisterDroppedTimeSeries(reader.ReadString(), reader.ReadInt32());
    }

    public void Serialize(BinaryWriter writer)
    {
      writer.Write((byte) 0);
      writer.Write(this.TotalEstimatedTimeSeries);
      writer.Write(this.totalDroppedTimeSeries);
      if (this.totalDroppedTimeSeries <= 0)
        return;
      writer.Write(this.droppedTimeSeries.Count);
      foreach (KeyValuePair<string, int> keyValuePair in this.droppedTimeSeries)
      {
        writer.Write(keyValuePair.Key);
        writer.Write(keyValuePair.Value);
      }
    }

    public void Aggregate(QueryResultQualityInfo source)
    {
      if (source.totalDroppedTimeSeries <= 0)
        return;
      foreach (KeyValuePair<string, int> keyValuePair in source.droppedTimeSeries)
        this.RegisterDroppedTimeSeries(keyValuePair.Key, keyValuePair.Value);
    }
  }
}
