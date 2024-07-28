// Decompiled with JetBrains decompiler
// Type: Nest.ILikeDocument
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Utf8Json;
using System.Runtime.Serialization;

namespace Nest
{
  [InterfaceDataContract]
  [ReadAs(typeof (LikeDocument<object>))]
  public interface ILikeDocument
  {
    [DataMember(Name = "doc")]
    [JsonFormatter(typeof (SourceFormatter<object>))]
    object Document { get; set; }

    [DataMember(Name = "fields")]
    Fields Fields { get; set; }

    [DataMember(Name = "_id")]
    Id Id { get; set; }

    [DataMember(Name = "_index")]
    IndexName Index { get; set; }

    [DataMember(Name = "per_field_analyzer")]
    IPerFieldAnalyzer PerFieldAnalyzer { get; set; }

    [DataMember(Name = "routing")]
    Routing Routing { get; set; }
  }
}
