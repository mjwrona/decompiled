// Decompiled with JetBrains decompiler
// Type: Nest.ICreateRollupJobRequest
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Specification.RollupApi;
using Elasticsearch.Net.Utf8Json;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Nest
{
  [InterfaceDataContract]
  [MapsApi("rollup.put_job.json")]
  [ReadAs(typeof (CreateRollupJobRequest))]
  public interface ICreateRollupJobRequest : IRequest<CreateRollupJobRequestParameters>, IRequest
  {
    [IgnoreDataMember]
    Id Id { get; }

    [DataMember(Name = "cron")]
    string Cron { get; set; }

    [DataMember(Name = "groups")]
    IRollupGroupings Groups { get; set; }

    [DataMember(Name = "index_pattern")]
    string IndexPattern { get; set; }

    [DataMember(Name = "metrics")]
    IEnumerable<IRollupFieldMetric> Metrics { get; set; }

    [DataMember(Name = "page_size")]
    long? PageSize { get; set; }

    [DataMember(Name = "rollup_index")]
    IndexName RollupIndex { get; set; }
  }
}
