// Decompiled with JetBrains decompiler
// Type: Nest.PutDatafeedResponse
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Utf8Json;
using System.Runtime.Serialization;

namespace Nest
{
  public class PutDatafeedResponse : ResponseBase
  {
    [DataMember(Name = "aggregations")]
    public AggregationDictionary Aggregations { get; internal set; }

    [DataMember(Name = "chunking_config")]
    public IChunkingConfig ChunkingConfig { get; internal set; }

    [DataMember(Name = "datafeed_id")]
    public string DatafeedId { get; internal set; }

    [DataMember(Name = "frequency")]
    public Time Frequency { get; internal set; }

    [DataMember(Name = "indices")]
    [JsonFormatter(typeof (IndicesFormatter))]
    public Indices Indices { get; internal set; }

    [DataMember(Name = "job_id")]
    public string JobId { get; internal set; }

    [DataMember(Name = "query")]
    public QueryContainer Query { get; internal set; }

    [DataMember(Name = "query_delay")]
    public Time QueryDelay { get; internal set; }

    [DataMember(Name = "script_fields")]
    public IScriptFields ScriptFields { get; internal set; }

    [DataMember(Name = "scroll_size")]
    public int? ScrollSize { get; internal set; }

    [DataMember(Name = "max_empty_searches")]
    public int? MaximumEmptySearches { get; set; }
  }
}
