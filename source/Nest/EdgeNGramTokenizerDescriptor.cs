// Decompiled with JetBrains decompiler
// Type: Nest.EdgeNGramTokenizerDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;
using System.Collections.Generic;

namespace Nest
{
  public class EdgeNGramTokenizerDescriptor : 
    TokenizerDescriptorBase<EdgeNGramTokenizerDescriptor, IEdgeNGramTokenizer>,
    IEdgeNGramTokenizer,
    ITokenizer
  {
    protected override string Type => "edge_ngram";

    int? IEdgeNGramTokenizer.MaxGram { get; set; }

    int? IEdgeNGramTokenizer.MinGram { get; set; }

    IEnumerable<TokenChar> IEdgeNGramTokenizer.TokenChars { get; set; }

    string IEdgeNGramTokenizer.CustomTokenChars { get; set; }

    public EdgeNGramTokenizerDescriptor MinGram(int? minGram) => this.Assign<int?>(minGram, (Action<IEdgeNGramTokenizer, int?>) ((a, v) => a.MinGram = v));

    public EdgeNGramTokenizerDescriptor MaxGram(int? maxGram) => this.Assign<int?>(maxGram, (Action<IEdgeNGramTokenizer, int?>) ((a, v) => a.MaxGram = v));

    public EdgeNGramTokenizerDescriptor TokenChars(IEnumerable<TokenChar> tokenChars) => this.Assign<IEnumerable<TokenChar>>(tokenChars, (Action<IEdgeNGramTokenizer, IEnumerable<TokenChar>>) ((a, v) => a.TokenChars = v));

    public EdgeNGramTokenizerDescriptor TokenChars(params TokenChar[] tokenChars) => this.Assign<TokenChar[]>(tokenChars, (Action<IEdgeNGramTokenizer, TokenChar[]>) ((a, v) => a.TokenChars = (IEnumerable<TokenChar>) v));

    public EdgeNGramTokenizerDescriptor CustomTokenChars(string customTokenChars) => this.Assign<string>(customTokenChars, (Action<IEdgeNGramTokenizer, string>) ((a, v) => a.CustomTokenChars = v));
  }
}
