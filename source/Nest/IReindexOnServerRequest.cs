// Decompiled with JetBrains decompiler
// Type: Nest.IReindexOnServerRequest
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using Elasticsearch.Net.Utf8Json;
using System;
using System.Runtime.Serialization;

namespace Nest
{
  [MapsApi("reindex.json")]
  [InterfaceDataContract]
  public interface IReindexOnServerRequest : IRequest<ReindexOnServerRequestParameters>, IRequest
  {
    [DataMember(Name = "conflicts")]
    Elasticsearch.Net.Conflicts? Conflicts { get; set; }

    [DataMember(Name = "dest")]
    IReindexDestination Destination { get; set; }

    [DataMember(Name = "script")]
    IScript Script { get; set; }

    [DataMember(Name = "size")]
    [Obsolete("Deprecated. Use MaximumDocuments")]
    long? Size { get; set; }

    [DataMember(Name = "max_docs")]
    long? MaximumDocuments { get; set; }

    [DataMember(Name = "source")]
    IReindexSource Source { get; set; }
  }
}
