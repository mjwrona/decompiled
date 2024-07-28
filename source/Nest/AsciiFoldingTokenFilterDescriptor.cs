// Decompiled with JetBrains decompiler
// Type: Nest.AsciiFoldingTokenFilterDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;

namespace Nest
{
  public class AsciiFoldingTokenFilterDescriptor : 
    TokenFilterDescriptorBase<AsciiFoldingTokenFilterDescriptor, IAsciiFoldingTokenFilter>,
    IAsciiFoldingTokenFilter,
    ITokenFilter
  {
    protected override string Type => "asciifolding";

    bool? IAsciiFoldingTokenFilter.PreserveOriginal { get; set; }

    public AsciiFoldingTokenFilterDescriptor PreserveOriginal(bool? preserve = true) => this.Assign<bool?>(preserve, (Action<IAsciiFoldingTokenFilter, bool?>) ((a, v) => a.PreserveOriginal = v));
  }
}
