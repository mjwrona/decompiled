// Decompiled with JetBrains decompiler
// Type: Nest.KeywordProperty
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System.Diagnostics;

namespace Nest
{
  [DebuggerDisplay("{DebugDisplay}")]
  public class KeywordProperty : 
    DocValuesPropertyBase,
    IKeywordProperty,
    IDocValuesProperty,
    ICoreProperty,
    IProperty,
    IFieldMapping
  {
    public KeywordProperty()
      : base(FieldType.Keyword)
    {
    }

    public double? Boost { get; set; }

    public bool? EagerGlobalOrdinals { get; set; }

    public int? IgnoreAbove { get; set; }

    public bool? Index { get; set; }

    public Nest.IndexOptions? IndexOptions { get; set; }

    public string Normalizer { get; set; }

    public bool? Norms { get; set; }

    public string NullValue { get; set; }

    public bool? SplitQueriesOnWhitespace { get; set; }
  }
}
