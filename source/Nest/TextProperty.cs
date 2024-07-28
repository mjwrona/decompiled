// Decompiled with JetBrains decompiler
// Type: Nest.TextProperty
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System.Diagnostics;

namespace Nest
{
  [DebuggerDisplay("{DebugDisplay}")]
  public class TextProperty : 
    CorePropertyBase,
    ITextProperty,
    ICoreProperty,
    IProperty,
    IFieldMapping
  {
    public TextProperty()
      : base(FieldType.Text)
    {
    }

    public string Analyzer { get; set; }

    public double? Boost { get; set; }

    public bool? EagerGlobalOrdinals { get; set; }

    public bool? Fielddata { get; set; }

    public IFielddataFrequencyFilter FielddataFrequencyFilter { get; set; }

    public bool? Index { get; set; }

    public Nest.IndexOptions? IndexOptions { get; set; }

    public bool? IndexPhrases { get; set; }

    public ITextIndexPrefixes IndexPrefixes { get; set; }

    public bool? Norms { get; set; }

    public int? PositionIncrementGap { get; set; }

    public string SearchAnalyzer { get; set; }

    public string SearchQuoteAnalyzer { get; set; }

    public TermVectorOption? TermVector { get; set; }
  }
}
