// Decompiled with JetBrains decompiler
// Type: Nest.IcuNormalizationTokenFilterDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;

namespace Nest
{
  public class IcuNormalizationTokenFilterDescriptor : 
    TokenFilterDescriptorBase<IcuNormalizationTokenFilterDescriptor, IIcuNormalizationTokenFilter>,
    IIcuNormalizationTokenFilter,
    ITokenFilter
  {
    protected override string Type => "icu_normalizer";

    IcuNormalizationType? IIcuNormalizationTokenFilter.Name { get; set; }

    public IcuNormalizationTokenFilterDescriptor Name(IcuNormalizationType? name) => this.Assign<IcuNormalizationType?>(name, (Action<IIcuNormalizationTokenFilter, IcuNormalizationType?>) ((a, v) => a.Name = v));
  }
}
