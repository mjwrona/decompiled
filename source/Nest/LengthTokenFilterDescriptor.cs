// Decompiled with JetBrains decompiler
// Type: Nest.LengthTokenFilterDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;

namespace Nest
{
  public class LengthTokenFilterDescriptor : 
    TokenFilterDescriptorBase<LengthTokenFilterDescriptor, ILengthTokenFilter>,
    ILengthTokenFilter,
    ITokenFilter
  {
    protected override string Type => "length";

    int? ILengthTokenFilter.Max { get; set; }

    int? ILengthTokenFilter.Min { get; set; }

    public LengthTokenFilterDescriptor Min(int? minimum) => this.Assign<int?>(minimum, (Action<ILengthTokenFilter, int?>) ((a, v) => a.Min = v));

    public LengthTokenFilterDescriptor Max(int? maximum) => this.Assign<int?>(maximum, (Action<ILengthTokenFilter, int?>) ((a, v) => a.Max = v));
  }
}
