// Decompiled with JetBrains decompiler
// Type: Nest.TermSuggester
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

namespace Nest
{
  public class TermSuggester : SuggesterBase, ITermSuggester, ISuggester
  {
    public bool? LowercaseTerms { get; set; }

    public int? MaxEdits { get; set; }

    public int? MaxInspections { get; set; }

    public float? MaxTermFrequency { get; set; }

    public float? MinDocFrequency { get; set; }

    public int? MinWordLength { get; set; }

    public int? PrefixLength { get; set; }

    public int? ShardSize { get; set; }

    public SuggestSort? Sort { get; set; }

    public Nest.StringDistance? StringDistance { get; set; }

    public Elasticsearch.Net.SuggestMode? SuggestMode { get; set; }

    public string Text { get; set; }
  }
}
