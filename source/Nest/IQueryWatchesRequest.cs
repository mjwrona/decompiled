// Decompiled with JetBrains decompiler
// Type: Nest.IQueryWatchesRequest
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Specification.WatcherApi;
using Elasticsearch.Net.Utf8Json;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Nest
{
  [InterfaceDataContract]
  [MapsApi("watcher.query_watches.json")]
  [ReadAs(typeof (QueryWatchesRequest))]
  public interface IQueryWatchesRequest : IRequest<QueryWatchesRequestParameters>, IRequest
  {
    [DataMember(Name = "from")]
    int? From { get; set; }

    [DataMember(Name = "query")]
    QueryContainer Query { get; set; }

    [DataMember(Name = "search_after")]
    IList<object> SearchAfter { get; set; }

    [DataMember(Name = "size")]
    int? Size { get; set; }

    [DataMember(Name = "sort")]
    IList<ISort> Sort { get; set; }
  }
}
