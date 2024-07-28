// Decompiled with JetBrains decompiler
// Type: Nest.MultiplexerTokenFilterDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;
using System.Collections.Generic;

namespace Nest
{
  public class MultiplexerTokenFilterDescriptor : 
    TokenFilterDescriptorBase<MultiplexerTokenFilterDescriptor, IMultiplexerTokenFilter>,
    IMultiplexerTokenFilter,
    ITokenFilter
  {
    protected override string Type => "multiplexer";

    IEnumerable<string> IMultiplexerTokenFilter.Filters { get; set; }

    bool? IMultiplexerTokenFilter.PreserveOriginal { get; set; }

    public MultiplexerTokenFilterDescriptor Filters(IEnumerable<string> filters) => this.Assign<IEnumerable<string>>(filters, (Action<IMultiplexerTokenFilter, IEnumerable<string>>) ((a, v) => a.Filters = v));

    public MultiplexerTokenFilterDescriptor Filters(params string[] filters) => this.Assign<string[]>(filters, (Action<IMultiplexerTokenFilter, string[]>) ((a, v) => a.Filters = (IEnumerable<string>) v));

    public MultiplexerTokenFilterDescriptor PreserveOriginal(bool? preserve = true) => this.Assign<bool?>(preserve, (Action<IMultiplexerTokenFilter, bool?>) ((a, v) => a.PreserveOriginal = v));
  }
}
