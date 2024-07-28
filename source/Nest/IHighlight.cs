// Decompiled with JetBrains decompiler
// Type: Nest.IHighlight
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Utf8Json;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Nest
{
  [InterfaceDataContract]
  [ReadAs(typeof (Highlight))]
  public interface IHighlight
  {
    [DataMember(Name = "boundary_chars")]
    string BoundaryChars { get; set; }

    [DataMember(Name = "boundary_max_scan")]
    int? BoundaryMaxScan { get; set; }

    [DataMember(Name = "boundary_scanner")]
    Nest.BoundaryScanner? BoundaryScanner { get; set; }

    [DataMember(Name = "boundary_scanner_locale")]
    string BoundaryScannerLocale { get; set; }

    [DataMember(Name = "encoder")]
    HighlighterEncoder? Encoder { get; set; }

    [DataMember(Name = "fields")]
    [JsonFormatter(typeof (VerbatimDictionaryKeysFormatter<Field, IHighlightField>))]
    Dictionary<Field, IHighlightField> Fields { get; set; }

    [DataMember(Name = "fragmenter")]
    HighlighterFragmenter? Fragmenter { get; set; }

    [DataMember(Name = "fragment_offset")]
    int? FragmentOffset { get; set; }

    [DataMember(Name = "fragment_size")]
    int? FragmentSize { get; set; }

    [DataMember(Name = "highlight_query")]
    QueryContainer HighlightQuery { get; set; }

    [DataMember(Name = "max_analyzed_offset")]
    int? MaxAnalyzedOffset { get; set; }

    [DataMember(Name = "max_fragment_length")]
    int? MaxFragmentLength { get; set; }

    [DataMember(Name = "no_match_size")]
    int? NoMatchSize { get; set; }

    [DataMember(Name = "number_of_fragments")]
    int? NumberOfFragments { get; set; }

    [DataMember(Name = "order")]
    HighlighterOrder? Order { get; set; }

    [DataMember(Name = "post_tags")]
    IEnumerable<string> PostTags { get; set; }

    [DataMember(Name = "pre_tags")]
    IEnumerable<string> PreTags { get; set; }

    [DataMember(Name = "require_field_match")]
    bool? RequireFieldMatch { get; set; }

    [DataMember(Name = "tags_schema")]
    HighlighterTagsSchema? TagsSchema { get; set; }
  }
}
