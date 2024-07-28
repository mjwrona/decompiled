// Decompiled with JetBrains decompiler
// Type: Nest.IMultiTermVectorOperation
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Utf8Json;
using System.Runtime.Serialization;

namespace Nest
{
  public interface IMultiTermVectorOperation
  {
    [DataMember(Name = "doc")]
    [JsonFormatter(typeof (SourceFormatter<>))]
    object Document { get; set; }

    [DataMember(Name = "field_statistics")]
    bool? FieldStatistics { get; set; }

    [DataMember(Name = "filter")]
    ITermVectorFilter Filter { get; set; }

    [DataMember(Name = "_id")]
    Id Id { get; set; }

    [DataMember(Name = "_index")]
    IndexName Index { get; set; }

    [DataMember(Name = "offsets")]
    bool? Offsets { get; set; }

    [DataMember(Name = "payloads")]
    bool? Payloads { get; set; }

    [DataMember(Name = "positions")]
    bool? Positions { get; set; }

    [DataMember(Name = "routing")]
    Routing Routing { get; set; }

    [DataMember(Name = "fields")]
    Fields Fields { get; set; }

    [DataMember(Name = "term_statistics")]
    bool? TermStatistics { get; set; }

    [DataMember(Name = "version")]
    long? Version { get; set; }

    [DataMember(Name = "version_type")]
    Elasticsearch.Net.VersionType? VersionType { get; set; }
  }
}
