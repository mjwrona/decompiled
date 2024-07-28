// Decompiled with JetBrains decompiler
// Type: Nest.UniqueTokenFilterDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;

namespace Nest
{
  public class UniqueTokenFilterDescriptor : 
    TokenFilterDescriptorBase<UniqueTokenFilterDescriptor, IUniqueTokenFilter>,
    IUniqueTokenFilter,
    ITokenFilter
  {
    protected override string Type => "unique";

    bool? IUniqueTokenFilter.OnlyOnSamePosition { get; set; }

    public UniqueTokenFilterDescriptor OnlyOnSamePosition(bool? samePositionOnly = true) => this.Assign<bool?>(samePositionOnly, (Action<IUniqueTokenFilter, bool?>) ((a, v) => a.OnlyOnSamePosition = v));
  }
}
