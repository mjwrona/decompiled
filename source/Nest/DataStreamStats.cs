// Decompiled with JetBrains decompiler
// Type: Nest.DataStreamStats
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;
using System.Runtime.Serialization;

namespace Nest
{
  public class DataStreamStats
  {
    [DataMember(Name = "data_stream")]
    public string DataStream { get; internal set; }

    [DataMember(Name = "backing_indices")]
    public int BackingIndices { get; internal set; }

    [DataMember(Name = "store_size")]
    public string StoreSize { get; internal set; }

    [DataMember(Name = "store_size_bytes")]
    public long StoreSizeBytes { get; internal set; }

    [DataMember(Name = "maximum_timestamp")]
    public long MaximumTimestamp { get; internal set; }

    public DateTimeOffset MaximumTimestampDateTimeOffset => DateTimeUtil.UnixEpoch.AddMilliseconds((double) this.MaximumTimestamp);
  }
}
