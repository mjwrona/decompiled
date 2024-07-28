// Decompiled with JetBrains decompiler
// Type: Nest.MultiGetHit`1
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using Elasticsearch.Net.Utf8Json;
using System.Runtime.Serialization;

namespace Nest
{
  [DataContract]
  public class MultiGetHit<TDocument> : IMultiGetHit<TDocument> where TDocument : class
  {
    [DataMember(Name = "error")]
    public Error Error { get; internal set; }

    [DataMember(Name = "fields")]
    public FieldValues Fields { get; internal set; }

    [DataMember(Name = "found")]
    public bool Found { get; internal set; }

    [DataMember(Name = "_id")]
    public string Id { get; internal set; }

    [DataMember(Name = "_index")]
    public string Index { get; internal set; }

    [DataMember(Name = "_routing")]
    public string Routing { get; internal set; }

    [DataMember(Name = "_source")]
    [JsonFormatter(typeof (SourceFormatter<>))]
    public TDocument Source { get; internal set; }

    [DataMember(Name = "_type")]
    public string Type { get; internal set; }

    [DataMember(Name = "_version")]
    public long Version { get; internal set; }

    [DataMember(Name = "_seq_no")]
    public long? SequenceNumber { get; internal set; }

    [DataMember(Name = "_primary_term")]
    public long? PrimaryTerm { get; internal set; }
  }
}
