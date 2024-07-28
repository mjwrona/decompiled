// Decompiled with JetBrains decompiler
// Type: Nest.IPutIndexTemplateV2Request
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Specification.IndicesApi;
using Elasticsearch.Net.Utf8Json;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Nest
{
  [MapsApi("indices.put_index_template.json")]
  [InterfaceDataContract]
  public interface IPutIndexTemplateV2Request : 
    IRequest<PutIndexTemplateV2RequestParameters>,
    IRequest
  {
    [DataMember(Name = "index_patterns")]
    IEnumerable<string> IndexPatterns { get; set; }

    [DataMember(Name = "composed_of")]
    IEnumerable<string> ComposedOf { get; set; }

    [DataMember(Name = "template")]
    ITemplate Template { get; set; }

    [DataMember(Name = "data_stream")]
    DataStream DataStream { get; set; }

    [DataMember(Name = "priority")]
    int? Priority { get; set; }

    [DataMember(Name = "version")]
    long? Version { get; set; }

    [DataMember(Name = "_meta")]
    IDictionary<string, object> Meta { get; set; }

    [IgnoreDataMember]
    Name Name { get; }
  }
}
