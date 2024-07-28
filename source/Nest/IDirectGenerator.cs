// Decompiled with JetBrains decompiler
// Type: Nest.IDirectGenerator
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Utf8Json;
using System.Runtime.Serialization;

namespace Nest
{
  [InterfaceDataContract]
  [ReadAs(typeof (DirectGenerator))]
  public interface IDirectGenerator
  {
    [DataMember(Name = "field")]
    Field Field { get; set; }

    [DataMember(Name = "max_edits")]
    int? MaxEdits { get; set; }

    [DataMember(Name = "max_inspections")]
    float? MaxInspections { get; set; }

    [DataMember(Name = "max_term_freq")]
    float? MaxTermFrequency { get; set; }

    [DataMember(Name = "min_doc_freq")]
    float? MinDocFrequency { get; set; }

    [DataMember(Name = "min_word_length")]
    int? MinWordLength { get; set; }

    [DataMember(Name = "post_filter")]
    string PostFilter { get; set; }

    [DataMember(Name = "pre_filter")]
    string PreFilter { get; set; }

    [DataMember(Name = "prefix_length")]
    int? PrefixLength { get; set; }

    [DataMember(Name = "size")]
    int? Size { get; set; }

    [DataMember(Name = "suggest_mode")]
    Elasticsearch.Net.SuggestMode? SuggestMode { get; set; }
  }
}
