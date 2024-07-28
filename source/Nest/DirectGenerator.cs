// Decompiled with JetBrains decompiler
// Type: Nest.DirectGenerator
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

namespace Nest
{
  public class DirectGenerator : IDirectGenerator
  {
    public Field Field { get; set; }

    public int? MaxEdits { get; set; }

    public float? MaxInspections { get; set; }

    public float? MaxTermFrequency { get; set; }

    public float? MinDocFrequency { get; set; }

    public int? MinWordLength { get; set; }

    public string PostFilter { get; set; }

    public string PreFilter { get; set; }

    public int? PrefixLength { get; set; }

    public int? Size { get; set; }

    public Elasticsearch.Net.SuggestMode? SuggestMode { get; set; }
  }
}
