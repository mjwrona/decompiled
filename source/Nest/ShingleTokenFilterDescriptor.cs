// Decompiled with JetBrains decompiler
// Type: Nest.ShingleTokenFilterDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;

namespace Nest
{
  public class ShingleTokenFilterDescriptor : 
    TokenFilterDescriptorBase<ShingleTokenFilterDescriptor, IShingleTokenFilter>,
    IShingleTokenFilter,
    ITokenFilter
  {
    protected override string Type => "shingle";

    string IShingleTokenFilter.FillerToken { get; set; }

    int? IShingleTokenFilter.MaxShingleSize { get; set; }

    int? IShingleTokenFilter.MinShingleSize { get; set; }

    bool? IShingleTokenFilter.OutputUnigrams { get; set; }

    bool? IShingleTokenFilter.OutputUnigramsIfNoShingles { get; set; }

    string IShingleTokenFilter.TokenSeparator { get; set; }

    public ShingleTokenFilterDescriptor OutputUnigrams(bool? output = true) => this.Assign<bool?>(output, (Action<IShingleTokenFilter, bool?>) ((a, v) => a.OutputUnigrams = v));

    public ShingleTokenFilterDescriptor OutputUnigramsIfNoShingles(bool? outputIfNo = true) => this.Assign<bool?>(outputIfNo, (Action<IShingleTokenFilter, bool?>) ((a, v) => a.OutputUnigramsIfNoShingles = v));

    public ShingleTokenFilterDescriptor MinShingleSize(int? minShingleSize) => this.Assign<int?>(minShingleSize, (Action<IShingleTokenFilter, int?>) ((a, v) => a.MinShingleSize = v));

    public ShingleTokenFilterDescriptor MaxShingleSize(int? maxShingleSize) => this.Assign<int?>(maxShingleSize, (Action<IShingleTokenFilter, int?>) ((a, v) => a.MaxShingleSize = v));

    public ShingleTokenFilterDescriptor TokenSeparator(string separator) => this.Assign<string>(separator, (Action<IShingleTokenFilter, string>) ((a, v) => a.TokenSeparator = v));

    public ShingleTokenFilterDescriptor FillerToken(string filler) => this.Assign<string>(filler, (Action<IShingleTokenFilter, string>) ((a, v) => a.FillerToken = v));
  }
}
