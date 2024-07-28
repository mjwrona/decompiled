// Decompiled with JetBrains decompiler
// Type: Nest.IndexingStats
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Utf8Json;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Nest
{
  [DataContract]
  public class IndexingStats
  {
    [DataMember(Name = "index_current")]
    public long Current { get; set; }

    [DataMember(Name = "delete_current")]
    public long DeleteCurrent { get; set; }

    [DataMember(Name = "delete_time")]
    public string DeleteTime { get; set; }

    [DataMember(Name = "delete_time_in_millis")]
    public long DeleteTimeInMilliseconds { get; set; }

    [DataMember(Name = "delete_total")]
    public long DeleteTotal { get; set; }

    [DataMember(Name = "is_throttled")]
    public bool IsThrottled { get; set; }

    [DataMember(Name = "noop_update_total")]
    public long NoopUpdateTotal { get; set; }

    [DataMember(Name = "throttle_time")]
    public string ThrottleTime { get; set; }

    [DataMember(Name = "throttle_time_in_millis")]
    public long ThrottleTimeInMilliseconds { get; set; }

    [DataMember(Name = "index_time")]
    public string Time { get; set; }

    [DataMember(Name = "index_time_in_millis")]
    public long TimeInMilliseconds { get; set; }

    [DataMember(Name = "index_total")]
    public long Total { get; set; }

    [DataMember(Name = "types")]
    [JsonFormatter(typeof (VerbatimInterfaceReadOnlyDictionaryKeysFormatter<string, IndexingStats>))]
    public IReadOnlyDictionary<string, IndexingStats> Types { get; set; }
  }
}
