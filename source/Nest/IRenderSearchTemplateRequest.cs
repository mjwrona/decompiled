// Decompiled with JetBrains decompiler
// Type: Nest.IRenderSearchTemplateRequest
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using Elasticsearch.Net.Utf8Json;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Nest
{
  [InterfaceDataContract]
  [MapsApi("render_search_template.json")]
  public interface IRenderSearchTemplateRequest : 
    IRequest<RenderSearchTemplateRequestParameters>,
    IRequest
  {
    [IgnoreDataMember]
    Id Id { get; }

    [DataMember(Name = "file")]
    string File { get; set; }

    [DataMember(Name = "params")]
    [JsonFormatter(typeof (VerbatimDictionaryKeysBaseFormatter<Dictionary<string, object>, string, object>))]
    Dictionary<string, object> Params { get; set; }

    [DataMember(Name = "source")]
    string Source { get; set; }
  }
}
