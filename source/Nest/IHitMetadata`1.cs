// Decompiled with JetBrains decompiler
// Type: Nest.IHitMetadata`1
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Utf8Json;
using System.Runtime.Serialization;

namespace Nest
{
  [InterfaceDataContract]
  public interface IHitMetadata<out TDocument> where TDocument : class
  {
    [DataMember(Name = "_id")]
    string Id { get; }

    [DataMember(Name = "_index")]
    string Index { get; }

    [DataMember(Name = "_primary_term")]
    long? PrimaryTerm { get; }

    [DataMember(Name = "_routing")]
    string Routing { get; }

    [DataMember(Name = "_seq_no")]
    long? SequenceNumber { get; }

    [DataMember(Name = "_source")]
    [JsonFormatter(typeof (SourceFormatter<>))]
    TDocument Source { get; }

    [DataMember(Name = "_type")]
    string Type { get; }

    [DataMember(Name = "_version")]
    long Version { get; }
  }
}
