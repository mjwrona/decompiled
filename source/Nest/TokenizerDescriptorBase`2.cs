// Decompiled with JetBrains decompiler
// Type: Nest.TokenizerDescriptorBase`2
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;

namespace Nest
{
  public abstract class TokenizerDescriptorBase<TTokenizer, TTokenizerInterface> : 
    DescriptorBase<TTokenizer, TTokenizerInterface>,
    ITokenizer
    where TTokenizer : TokenizerDescriptorBase<TTokenizer, TTokenizerInterface>, TTokenizerInterface
    where TTokenizerInterface : class, ITokenizer
  {
    protected abstract string Type { get; }

    string ITokenizer.Type => this.Type;

    string ITokenizer.Version { get; set; }

    public TTokenizer Version(string version) => this.Assign<string>(version, (Action<TTokenizerInterface, string>) ((a, v) => a.Version = v));
  }
}
