// Decompiled with JetBrains decompiler
// Type: Nest.CustomAnalyzerDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;
using System.Collections.Generic;

namespace Nest
{
  public class CustomAnalyzerDescriptor : 
    AnalyzerDescriptorBase<CustomAnalyzerDescriptor, ICustomAnalyzer>,
    ICustomAnalyzer,
    IAnalyzer
  {
    protected override string Type => "custom";

    IEnumerable<string> ICustomAnalyzer.CharFilter { get; set; }

    IEnumerable<string> ICustomAnalyzer.Filter { get; set; }

    int? ICustomAnalyzer.PositionOffsetGap { get; set; }

    int? ICustomAnalyzer.PositionIncrementGap { get; set; }

    string ICustomAnalyzer.Tokenizer { get; set; }

    public CustomAnalyzerDescriptor Filters(params string[] filters) => this.Assign<string[]>(filters, (Action<ICustomAnalyzer, string[]>) ((a, v) => a.Filter = (IEnumerable<string>) v));

    public CustomAnalyzerDescriptor Filters(IEnumerable<string> filters) => this.Assign<IEnumerable<string>>(filters, (Action<ICustomAnalyzer, IEnumerable<string>>) ((a, v) => a.Filter = v));

    public CustomAnalyzerDescriptor CharFilters(params string[] charFilters) => this.Assign<string[]>(charFilters, (Action<ICustomAnalyzer, string[]>) ((a, v) => a.CharFilter = (IEnumerable<string>) v));

    public CustomAnalyzerDescriptor CharFilters(IEnumerable<string> charFilters) => this.Assign<IEnumerable<string>>(charFilters, (Action<ICustomAnalyzer, IEnumerable<string>>) ((a, v) => a.CharFilter = v));

    public CustomAnalyzerDescriptor Tokenizer(string tokenizer) => this.Assign<string>(tokenizer, (Action<ICustomAnalyzer, string>) ((a, v) => a.Tokenizer = v));

    [Obsolete("Deprecated, use PositionIncrementGap instead")]
    public CustomAnalyzerDescriptor PositionOffsetGap(int? positionOffsetGap) => this.Assign<int?>(positionOffsetGap, (Action<ICustomAnalyzer, int?>) ((a, v) => a.PositionOffsetGap = v));

    public CustomAnalyzerDescriptor PositionIncrementGap(int? positionIncrementGap) => this.Assign<int?>(positionIncrementGap, (Action<ICustomAnalyzer, int?>) ((a, v) => a.PositionIncrementGap = v));
  }
}
