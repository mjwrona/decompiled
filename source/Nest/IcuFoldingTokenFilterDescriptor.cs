// Decompiled with JetBrains decompiler
// Type: Nest.IcuFoldingTokenFilterDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;

namespace Nest
{
  public class IcuFoldingTokenFilterDescriptor : 
    TokenFilterDescriptorBase<IcuFoldingTokenFilterDescriptor, IIcuFoldingTokenFilter>,
    IIcuFoldingTokenFilter,
    ITokenFilter
  {
    protected override string Type => "icu_folding";

    string IIcuFoldingTokenFilter.UnicodeSetFilter { get; set; }

    public IcuFoldingTokenFilterDescriptor UnicodeSetFilter(string filter) => this.Assign<string>(filter, (Action<IIcuFoldingTokenFilter, string>) ((a, v) => a.UnicodeSetFilter = v));
  }
}
