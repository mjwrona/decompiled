// Decompiled with JetBrains decompiler
// Type: Nest.CharGroupTokenizerDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;
using System.Collections.Generic;

namespace Nest
{
  public class CharGroupTokenizerDescriptor : 
    TokenizerDescriptorBase<CharGroupTokenizerDescriptor, ICharGroupTokenizer>,
    ICharGroupTokenizer,
    ITokenizer
  {
    protected override string Type => "char_group";

    IEnumerable<string> ICharGroupTokenizer.TokenizeOnCharacters { get; set; }

    int? ICharGroupTokenizer.MaxTokenLength { get; set; }

    public CharGroupTokenizerDescriptor TokenizeOnCharacters(params string[] characters) => this.Assign<string[]>(characters, (Action<ICharGroupTokenizer, string[]>) ((a, v) => a.TokenizeOnCharacters = (IEnumerable<string>) v));

    public CharGroupTokenizerDescriptor TokenizeOnCharacters(IEnumerable<string> characters) => this.Assign<IEnumerable<string>>(characters, (Action<ICharGroupTokenizer, IEnumerable<string>>) ((a, v) => a.TokenizeOnCharacters = v));

    public CharGroupTokenizerDescriptor MaxTokenLength(int? maxTokenLength) => this.Assign<int?>(maxTokenLength, (Action<ICharGroupTokenizer, int?>) ((a, v) => a.MaxTokenLength = v));
  }
}
