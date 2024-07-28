// Decompiled with JetBrains decompiler
// Type: Nest.IAcknowledgeWatchRequest
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Specification.WatcherApi;
using Elasticsearch.Net.Utf8Json;
using System.Runtime.Serialization;

namespace Nest
{
  [InterfaceDataContract]
  [MapsApi("watcher.ack_watch.json")]
  public interface IAcknowledgeWatchRequest : IRequest<AcknowledgeWatchRequestParameters>, IRequest
  {
    [IgnoreDataMember]
    Id WatchId { get; }

    [IgnoreDataMember]
    Ids ActionId { get; }
  }
}
