// Decompiled with JetBrains decompiler
// Type: Nest.IcuTransformTokenFilterDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;

namespace Nest
{
  public class IcuTransformTokenFilterDescriptor : 
    TokenFilterDescriptorBase<IcuTransformTokenFilterDescriptor, IIcuTransformTokenFilter>,
    IIcuTransformTokenFilter,
    ITokenFilter
  {
    protected override string Type => "icu_transform";

    IcuTransformDirection? IIcuTransformTokenFilter.Direction { get; set; }

    string IIcuTransformTokenFilter.Id { get; set; }

    public IcuTransformTokenFilterDescriptor Direction(IcuTransformDirection? direction) => this.Assign<IcuTransformDirection?>(direction, (Action<IIcuTransformTokenFilter, IcuTransformDirection?>) ((a, v) => a.Direction = v));

    public IcuTransformTokenFilterDescriptor Id(string id) => this.Assign<string>(id, (Action<IIcuTransformTokenFilter, string>) ((a, v) => a.Id = v));
  }
}
