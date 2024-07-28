// Decompiled with JetBrains decompiler
// Type: Nest.TokenCountPropertyDescriptor`1
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;
using System.Diagnostics;

namespace Nest
{
  [DebuggerDisplay("{DebugDisplay}")]
  public class TokenCountPropertyDescriptor<T> : 
    DocValuesPropertyDescriptorBase<TokenCountPropertyDescriptor<T>, ITokenCountProperty, T>,
    ITokenCountProperty,
    IDocValuesProperty,
    ICoreProperty,
    IProperty,
    IFieldMapping
    where T : class
  {
    public TokenCountPropertyDescriptor()
      : base(FieldType.TokenCount)
    {
    }

    string ITokenCountProperty.Analyzer { get; set; }

    double? ITokenCountProperty.Boost { get; set; }

    bool? ITokenCountProperty.EnablePositionIncrements { get; set; }

    bool? ITokenCountProperty.Index { get; set; }

    double? ITokenCountProperty.NullValue { get; set; }

    public TokenCountPropertyDescriptor<T> Analyzer(string analyzer) => this.Assign<string>(analyzer, (Action<ITokenCountProperty, string>) ((a, v) => a.Analyzer = v));

    [Obsolete("The server always treated this as a noop and has been removed in 7.10")]
    public TokenCountPropertyDescriptor<T> Boost(double? boost) => this.Assign<double?>(boost, (Action<ITokenCountProperty, double?>) ((a, v) => a.Boost = v));

    public TokenCountPropertyDescriptor<T> EnablePositionIncrements(bool? enablePositionIncrements = true) => this.Assign<bool?>(enablePositionIncrements, (Action<ITokenCountProperty, bool?>) ((a, v) => a.EnablePositionIncrements = v));

    public TokenCountPropertyDescriptor<T> Index(bool? index = true) => this.Assign<bool?>(index, (Action<ITokenCountProperty, bool?>) ((a, v) => a.Index = v));

    public TokenCountPropertyDescriptor<T> NullValue(double? nullValue) => this.Assign<double?>(nullValue, (Action<ITokenCountProperty, double?>) ((a, v) => a.NullValue = v));
  }
}
