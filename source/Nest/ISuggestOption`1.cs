// Decompiled with JetBrains decompiler
// Type: Nest.ISuggestOption`1
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Utf8Json;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Nest
{
  [InterfaceDataContract]
  [ReadAs(typeof (SuggestOption<>))]
  public interface ISuggestOption<out TDocument> where TDocument : class
  {
    [DataMember(Name = "collate_match")]
    bool CollateMatch { get; }

    [DataMember(Name = "contexts")]
    IDictionary<string, IEnumerable<Context>> Contexts { get; }

    [DataMember(Name = "_score")]
    double? DocumentScore { get; }

    [DataMember(Name = "fields")]
    FieldValues Fields { get; }

    [DataMember(Name = "freq")]
    long Frequency { get; set; }

    [DataMember(Name = "highlighted")]
    string Highlighted { get; }

    [DataMember(Name = "_id")]
    string Id { get; }

    [DataMember(Name = "_index")]
    IndexName Index { get; }

    [IgnoreDataMember]
    double Score { get; }

    [DataMember(Name = "_source")]
    [JsonFormatter(typeof (SourceFormatter<>))]
    TDocument Source { get; }

    [DataMember(Name = "score")]
    double? SuggestScore { get; }

    [DataMember(Name = "text")]
    string Text { get; }
  }
}
