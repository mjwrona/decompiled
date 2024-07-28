// Decompiled with JetBrains decompiler
// Type: Nest.CharFilterDescriptorBase`2
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;

namespace Nest
{
  public abstract class CharFilterDescriptorBase<TCharFilter, TCharFilterInterface> : 
    DescriptorBase<TCharFilter, TCharFilterInterface>,
    ICharFilter
    where TCharFilter : CharFilterDescriptorBase<TCharFilter, TCharFilterInterface>, TCharFilterInterface
    where TCharFilterInterface : class, ICharFilter
  {
    protected abstract string Type { get; }

    string ICharFilter.Type => this.Type;

    string ICharFilter.Version { get; set; }

    public TCharFilter Version(string version) => this.Assign<string>(version, (Action<TCharFilterInterface, string>) ((a, v) => a.Version = v));
  }
}
