// Decompiled with JetBrains decompiler
// Type: Nest.IUpdateRequest`2
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using Elasticsearch.Net.Utf8Json;
using System.Runtime.Serialization;

namespace Nest
{
  [MapsApi("update.json")]
  [InterfaceDataContract]
  public interface IUpdateRequest<TDocument, TPartialDocument> : 
    IRequest<UpdateRequestParameters>,
    IRequest
    where TDocument : class
    where TPartialDocument : class
  {
    [DataMember(Name = "detect_noop")]
    bool? DetectNoop { get; set; }

    [DataMember(Name = "doc")]
    [JsonFormatter(typeof (SourceFormatter<>))]
    TPartialDocument Doc { get; set; }

    [DataMember(Name = "doc_as_upsert")]
    bool? DocAsUpsert { get; set; }

    [DataMember(Name = "script")]
    IScript Script { get; set; }

    [DataMember(Name = "scripted_upsert")]
    bool? ScriptedUpsert { get; set; }

    [DataMember(Name = "_source")]
    Union<bool, ISourceFilter> Source { get; set; }

    [DataMember(Name = "upsert")]
    [JsonFormatter(typeof (SourceFormatter<>))]
    TDocument Upsert { get; set; }

    [IgnoreDataMember]
    IndexName Index { get; }

    [IgnoreDataMember]
    Id Id { get; }
  }
}
