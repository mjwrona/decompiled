// Decompiled with JetBrains decompiler
// Type: Nest.TokenCountProperty
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System.Diagnostics;

namespace Nest
{
  [DebuggerDisplay("{DebugDisplay}")]
  public class TokenCountProperty : 
    DocValuesPropertyBase,
    ITokenCountProperty,
    IDocValuesProperty,
    ICoreProperty,
    IProperty,
    IFieldMapping
  {
    public TokenCountProperty()
      : base(FieldType.TokenCount)
    {
    }

    public string Analyzer { get; set; }

    public double? Boost { get; set; }

    public bool? EnablePositionIncrements { get; set; }

    public bool? Index { get; set; }

    public double? NullValue { get; set; }
  }
}
