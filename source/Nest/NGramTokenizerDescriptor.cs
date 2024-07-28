// Decompiled with JetBrains decompiler
// Type: Nest.NGramTokenizerDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;
using System.Collections.Generic;

namespace Nest
{
  public class NGramTokenizerDescriptor : 
    TokenizerDescriptorBase<NGramTokenizerDescriptor, INGramTokenizer>,
    INGramTokenizer,
    ITokenizer
  {
    protected override string Type => "ngram";

    int? INGramTokenizer.MaxGram { get; set; }

    int? INGramTokenizer.MinGram { get; set; }

    IEnumerable<TokenChar> INGramTokenizer.TokenChars { get; set; }

    string INGramTokenizer.CustomTokenChars { get; set; }

    public NGramTokenizerDescriptor MinGram(int? minGram) => this.Assign<int?>(minGram, (Action<INGramTokenizer, int?>) ((a, v) => a.MinGram = v));

    public NGramTokenizerDescriptor MaxGram(int? minGram) => this.Assign<int?>(minGram, (Action<INGramTokenizer, int?>) ((a, v) => a.MaxGram = v));

    public NGramTokenizerDescriptor TokenChars(IEnumerable<TokenChar> tokenChars) => this.Assign<IEnumerable<TokenChar>>(tokenChars, (Action<INGramTokenizer, IEnumerable<TokenChar>>) ((a, v) => a.TokenChars = v));

    public NGramTokenizerDescriptor TokenChars(params TokenChar[] tokenChars) => this.Assign<TokenChar[]>(tokenChars, (Action<INGramTokenizer, TokenChar[]>) ((a, v) => a.TokenChars = (IEnumerable<TokenChar>) v));

    public NGramTokenizerDescriptor CustomTokenChars(string customTokenChars) => this.Assign<string>(customTokenChars, (Action<INGramTokenizer, string>) ((a, v) => a.CustomTokenChars = v));
  }
}
