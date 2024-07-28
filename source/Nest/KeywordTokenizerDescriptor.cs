// Decompiled with JetBrains decompiler
// Type: Nest.KeywordTokenizerDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;

namespace Nest
{
  public class KeywordTokenizerDescriptor : 
    TokenizerDescriptorBase<KeywordTokenizerDescriptor, IKeywordTokenizer>,
    IKeywordTokenizer,
    ITokenizer
  {
    protected override string Type => "keyword";

    int? IKeywordTokenizer.BufferSize { get; set; }

    public KeywordTokenizerDescriptor BufferSize(int? size) => this.Assign<int?>(size, (Action<IKeywordTokenizer, int?>) ((a, v) => a.BufferSize = v));
  }
}
