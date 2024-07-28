// Decompiled with JetBrains decompiler
// Type: Nest.SuggestOption`1
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Utf8Json;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Nest
{
  public class SuggestOption<TDocument> : ISuggestOption<TDocument> where TDocument : class
  {
    [DataMember(Name = "collate_match")]
    public bool CollateMatch { get; internal set; }

    [DataMember(Name = "contexts")]
    public IDictionary<string, IEnumerable<Context>> Contexts { get; internal set; }

    [DataMember(Name = "_score")]
    public double? DocumentScore { get; internal set; }

    [DataMember(Name = "fields")]
    public FieldValues Fields { get; internal set; }

    [DataMember(Name = "freq")]
    public long Frequency { get; set; }

    [DataMember(Name = "highlighted")]
    public string Highlighted { get; internal set; }

    [DataMember(Name = "_id")]
    public string Id { get; internal set; }

    [DataMember(Name = "_index")]
    public IndexName Index { get; internal set; }

    [IgnoreDataMember]
    public double Score => this.DocumentScore ?? this.SuggestScore.GetValueOrDefault();

    [DataMember(Name = "_source")]
    [JsonFormatter(typeof (SourceFormatter<>))]
    public TDocument Source { get; internal set; }

    [DataMember(Name = "score")]
    public double? SuggestScore { get; internal set; }

    [DataMember(Name = "text")]
    public string Text { get; internal set; }
  }
}
