// Decompiled with JetBrains decompiler
// Type: Nest.KeepTypesTokenFilterDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;
using System.Collections.Generic;

namespace Nest
{
  public class KeepTypesTokenFilterDescriptor : 
    TokenFilterDescriptorBase<KeepTypesTokenFilterDescriptor, IKeepTypesTokenFilter>,
    IKeepTypesTokenFilter,
    ITokenFilter
  {
    protected override string Type => "keep_types";

    KeepTypesMode? IKeepTypesTokenFilter.Mode { get; set; }

    IEnumerable<string> IKeepTypesTokenFilter.Types { get; set; }

    public KeepTypesTokenFilterDescriptor Types(IEnumerable<string> types) => this.Assign<IEnumerable<string>>(types, (Action<IKeepTypesTokenFilter, IEnumerable<string>>) ((a, v) => a.Types = v));

    public KeepTypesTokenFilterDescriptor Types(params string[] types) => this.Assign<string[]>(types, (Action<IKeepTypesTokenFilter, string[]>) ((a, v) => a.Types = (IEnumerable<string>) v));

    public KeepTypesTokenFilterDescriptor Mode(KeepTypesMode? mode) => this.Assign<KeepTypesMode?>(mode, (Action<IKeepTypesTokenFilter, KeepTypesMode?>) ((a, v) => a.Mode = v));
  }
}
