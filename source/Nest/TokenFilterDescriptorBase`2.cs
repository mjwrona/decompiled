// Decompiled with JetBrains decompiler
// Type: Nest.TokenFilterDescriptorBase`2
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;

namespace Nest
{
  public abstract class TokenFilterDescriptorBase<TTokenFilter, TTokenFilterInterface> : 
    DescriptorBase<TTokenFilter, TTokenFilterInterface>,
    ITokenFilter
    where TTokenFilter : TokenFilterDescriptorBase<TTokenFilter, TTokenFilterInterface>, TTokenFilterInterface
    where TTokenFilterInterface : class, ITokenFilter
  {
    protected abstract string Type { get; }

    string ITokenFilter.Type => this.Type;

    string ITokenFilter.Version { get; set; }

    public TTokenFilter Version(string version) => this.Assign<string>(version, (Action<TTokenFilterInterface, string>) ((a, v) => a.Version = v));
  }
}
