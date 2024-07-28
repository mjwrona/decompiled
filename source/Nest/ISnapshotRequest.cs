// Decompiled with JetBrains decompiler
// Type: Nest.ISnapshotRequest
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Specification.SnapshotApi;
using Elasticsearch.Net.Utf8Json;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Nest
{
  [MapsApi("snapshot.create.json")]
  [InterfaceDataContract]
  public interface ISnapshotRequest : IRequest<SnapshotRequestParameters>, IRequest
  {
    [DataMember(Name = "ignore_unavailable")]
    bool? IgnoreUnavailable { get; set; }

    [DataMember(Name = "include_global_state")]
    bool? IncludeGlobalState { get; set; }

    [DataMember(Name = "indices")]
    [JsonFormatter(typeof (IndicesMultiSyntaxFormatter))]
    Indices Indices { get; set; }

    [DataMember(Name = "partial")]
    bool? Partial { get; set; }

    [DataMember(Name = "metadata")]
    IDictionary<string, object> Metadata { get; set; }

    [IgnoreDataMember]
    Name RepositoryName { get; }

    [IgnoreDataMember]
    Name Snapshot { get; }
  }
}
