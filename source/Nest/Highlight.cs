// Decompiled with JetBrains decompiler
// Type: Nest.Highlight
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System.Collections.Generic;

namespace Nest
{
  public class Highlight : IHighlight
  {
    public string BoundaryChars { get; set; }

    public int? BoundaryMaxScan { get; set; }

    public Nest.BoundaryScanner? BoundaryScanner { get; set; }

    public string BoundaryScannerLocale { get; set; }

    public HighlighterEncoder? Encoder { get; set; }

    public Dictionary<Nest.Field, IHighlightField> Fields { get; set; }

    public HighlighterFragmenter? Fragmenter { get; set; }

    public int? FragmentOffset { get; set; }

    public int? FragmentSize { get; set; }

    public QueryContainer HighlightQuery { get; set; }

    public int? MaxAnalyzedOffset { get; set; }

    public int? MaxFragmentLength { get; set; }

    public int? NoMatchSize { get; set; }

    public int? NumberOfFragments { get; set; }

    public HighlighterOrder? Order { get; set; }

    public IEnumerable<string> PostTags { get; set; }

    public IEnumerable<string> PreTags { get; set; }

    public bool? RequireFieldMatch { get; set; }

    public HighlighterTagsSchema? TagsSchema { get; set; }

    public static Highlight Field(Nest.Field field) => new Highlight()
    {
      Fields = new Dictionary<Nest.Field, IHighlightField>()
      {
        {
          field,
          (IHighlightField) new HighlightField()
        }
      }
    };
  }
}
