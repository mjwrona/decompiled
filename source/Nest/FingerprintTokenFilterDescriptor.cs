// Decompiled with JetBrains decompiler
// Type: Nest.FingerprintTokenFilterDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;

namespace Nest
{
  public class FingerprintTokenFilterDescriptor : 
    TokenFilterDescriptorBase<FingerprintTokenFilterDescriptor, IFingerprintTokenFilter>,
    IFingerprintTokenFilter,
    ITokenFilter
  {
    protected override string Type => "fingerprint";

    int? IFingerprintTokenFilter.MaxOutputSize { get; set; }

    string IFingerprintTokenFilter.Separator { get; set; }

    public FingerprintTokenFilterDescriptor Separator(string separator) => this.Assign<string>(separator, (Action<IFingerprintTokenFilter, string>) ((a, v) => a.Separator = v));

    public FingerprintTokenFilterDescriptor MaxOutputSize(int? maxOutputSize) => this.Assign<int?>(maxOutputSize, (Action<IFingerprintTokenFilter, int?>) ((a, v) => a.MaxOutputSize = v));
  }
}
