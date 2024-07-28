// Decompiled with JetBrains decompiler
// Type: Nest.CustomNormalizerDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;
using System.Collections.Generic;

namespace Nest
{
  public class CustomNormalizerDescriptor : 
    NormalizerDescriptorBase<CustomNormalizerDescriptor, ICustomNormalizer>,
    ICustomNormalizer,
    INormalizer
  {
    protected override string Type => "custom";

    IEnumerable<string> ICustomNormalizer.CharFilter { get; set; }

    IEnumerable<string> ICustomNormalizer.Filter { get; set; }

    public CustomNormalizerDescriptor Filters(params string[] filters) => this.Assign<string[]>(filters, (Action<ICustomNormalizer, string[]>) ((a, v) => a.Filter = (IEnumerable<string>) v));

    public CustomNormalizerDescriptor Filters(IEnumerable<string> filters) => this.Assign<IEnumerable<string>>(filters, (Action<ICustomNormalizer, IEnumerable<string>>) ((a, v) => a.Filter = v));

    public CustomNormalizerDescriptor CharFilters(params string[] charFilters) => this.Assign<string[]>(charFilters, (Action<ICustomNormalizer, string[]>) ((a, v) => a.CharFilter = (IEnumerable<string>) v));

    public CustomNormalizerDescriptor CharFilters(IEnumerable<string> charFilters) => this.Assign<IEnumerable<string>>(charFilters, (Action<ICustomNormalizer, IEnumerable<string>>) ((a, v) => a.CharFilter = v));
  }
}
