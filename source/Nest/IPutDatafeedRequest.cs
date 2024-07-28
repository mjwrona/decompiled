// Decompiled with JetBrains decompiler
// Type: Nest.IPutDatafeedRequest
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Specification.MachineLearningApi;
using Elasticsearch.Net.Utf8Json;
using System.Runtime.Serialization;

namespace Nest
{
  [InterfaceDataContract]
  [MapsApi("ml.put_datafeed.json")]
  public interface IPutDatafeedRequest : IRequest<PutDatafeedRequestParameters>, IRequest
  {
    [IgnoreDataMember]
    Id DatafeedId { get; }

    [DataMember(Name = "aggregations")]
    AggregationDictionary Aggregations { get; set; }

    [DataMember(Name = "chunking_config")]
    IChunkingConfig ChunkingConfig { get; set; }

    [DataMember(Name = "frequency")]
    Time Frequency { get; set; }

    [DataMember(Name = "indices")]
    [JsonFormatter(typeof (IndicesFormatter))]
    Indices Indices { get; set; }

    [DataMember(Name = "job_id")]
    Id JobId { get; set; }

    [DataMember(Name = "query")]
    QueryContainer Query { get; set; }

    [DataMember(Name = "query_delay")]
    Time QueryDelay { get; set; }

    [DataMember(Name = "script_fields")]
    IScriptFields ScriptFields { get; set; }

    [DataMember(Name = "scroll_size")]
    int? ScrollSize { get; set; }

    [DataMember(Name = "max_empty_searches")]
    int? MaximumEmptySearches { get; set; }
  }
}
