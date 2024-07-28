// Decompiled with JetBrains decompiler
// Type: Nest.IPercolateQuery
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Utf8Json;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Nest
{
  [InterfaceDataContract]
  [ReadAs(typeof (PercolateQuery))]
  public interface IPercolateQuery : IQuery
  {
    [DataMember(Name = "document")]
    [JsonFormatter(typeof (SourceFormatter<object>))]
    object Document { get; set; }

    [DataMember(Name = "documents")]
    [JsonFormatter(typeof (SourceFormatter<IEnumerable<object>>))]
    IEnumerable<object> Documents { get; set; }

    [DataMember(Name = "field")]
    Field Field { get; set; }

    [DataMember(Name = "id")]
    Id Id { get; set; }

    [DataMember(Name = "index")]
    IndexName Index { get; set; }

    [DataMember(Name = "preference")]
    string Preference { get; set; }

    [DataMember(Name = "routing")]
    Routing Routing { get; set; }

    [DataMember(Name = "version")]
    long? Version { get; set; }
  }
}
