// Decompiled with JetBrains decompiler
// Type: Nest.ITermVectorsRequest`1
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using Elasticsearch.Net.Utf8Json;
using System.Runtime.Serialization;

namespace Nest
{
  [MapsApi("termvectors.json")]
  [InterfaceDataContract]
  public interface ITermVectorsRequest<TDocument> : IRequest<TermVectorsRequestParameters>, IRequest where TDocument : class
  {
    [DataMember(Name = "doc")]
    [JsonFormatter(typeof (SourceFormatter<>))]
    TDocument Document { get; set; }

    [DataMember(Name = "filter")]
    ITermVectorFilter Filter { get; set; }

    [DataMember(Name = "per_field_analyzer")]
    IPerFieldAnalyzer PerFieldAnalyzer { get; set; }

    [IgnoreDataMember]
    IndexName Index { get; }

    [IgnoreDataMember]
    Id Id { get; }
  }
}
