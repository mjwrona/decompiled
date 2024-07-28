// Decompiled with JetBrains decompiler
// Type: Nest.IcuNormalizationCharFilterDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;

namespace Nest
{
  public class IcuNormalizationCharFilterDescriptor : 
    CharFilterDescriptorBase<IcuNormalizationCharFilterDescriptor, IIcuNormalizationCharFilter>,
    IIcuNormalizationCharFilter,
    ICharFilter
  {
    protected override string Type => "icu_normalizer";

    IcuNormalizationMode? IIcuNormalizationCharFilter.Mode { get; set; }

    IcuNormalizationType? IIcuNormalizationCharFilter.Name { get; set; }

    public IcuNormalizationCharFilterDescriptor Name(IcuNormalizationType? name = null) => this.Assign<IcuNormalizationType?>(name, (Action<IIcuNormalizationCharFilter, IcuNormalizationType?>) ((a, v) => a.Name = v));

    public IcuNormalizationCharFilterDescriptor Mode(IcuNormalizationMode? mode = null) => this.Assign<IcuNormalizationMode?>(mode, (Action<IIcuNormalizationCharFilter, IcuNormalizationMode?>) ((a, v) => a.Mode = v));
  }
}
