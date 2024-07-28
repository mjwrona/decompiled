// Decompiled with JetBrains decompiler
// Type: Nest.NGramTokenFilterDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;

namespace Nest
{
  public class NGramTokenFilterDescriptor : 
    TokenFilterDescriptorBase<NGramTokenFilterDescriptor, INGramTokenFilter>,
    INGramTokenFilter,
    ITokenFilter
  {
    protected override string Type => "ngram";

    int? INGramTokenFilter.MaxGram { get; set; }

    int? INGramTokenFilter.MinGram { get; set; }

    bool? INGramTokenFilter.PreserveOriginal { get; set; }

    public NGramTokenFilterDescriptor MinGram(int? minGram) => this.Assign<int?>(minGram, (Action<INGramTokenFilter, int?>) ((a, v) => a.MinGram = v));

    public NGramTokenFilterDescriptor MaxGram(int? maxGram) => this.Assign<int?>(maxGram, (Action<INGramTokenFilter, int?>) ((a, v) => a.MaxGram = v));

    public NGramTokenFilterDescriptor PreserveOriginal(bool? preserveOriginal = true) => this.Assign<bool?>(preserveOriginal, (Action<INGramTokenFilter, bool?>) ((a, v) => a.PreserveOriginal = v));
  }
}
