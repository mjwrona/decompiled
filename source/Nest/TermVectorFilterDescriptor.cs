// Decompiled with JetBrains decompiler
// Type: Nest.TermVectorFilterDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;

namespace Nest
{
  public class TermVectorFilterDescriptor : 
    DescriptorBase<TermVectorFilterDescriptor, ITermVectorFilter>,
    ITermVectorFilter
  {
    int? ITermVectorFilter.MaximumDocumentFrequency { get; set; }

    int? ITermVectorFilter.MaximumNumberOfTerms { get; set; }

    int? ITermVectorFilter.MaximumTermFrequency { get; set; }

    int? ITermVectorFilter.MaximumWordLength { get; set; }

    int? ITermVectorFilter.MinimumDocumentFrequency { get; set; }

    int? ITermVectorFilter.MinimumTermFrequency { get; set; }

    int? ITermVectorFilter.MinimumWordLength { get; set; }

    public TermVectorFilterDescriptor MaximimumNumberOfTerms(int? maxNumTerms) => this.Assign<int?>(maxNumTerms, (Action<ITermVectorFilter, int?>) ((a, v) => a.MaximumNumberOfTerms = v));

    public TermVectorFilterDescriptor MinimumTermFrequency(int? minTermFreq) => this.Assign<int?>(minTermFreq, (Action<ITermVectorFilter, int?>) ((a, v) => a.MinimumTermFrequency = v));

    public TermVectorFilterDescriptor MaximumTermFrequency(int? maxTermFreq) => this.Assign<int?>(maxTermFreq, (Action<ITermVectorFilter, int?>) ((a, v) => a.MaximumTermFrequency = v));

    public TermVectorFilterDescriptor MinimumDocumentFrequency(int? minDocFreq) => this.Assign<int?>(minDocFreq, (Action<ITermVectorFilter, int?>) ((a, v) => a.MinimumDocumentFrequency = v));

    public TermVectorFilterDescriptor MaximumDocumentFrequency(int? maxDocFreq) => this.Assign<int?>(maxDocFreq, (Action<ITermVectorFilter, int?>) ((a, v) => a.MaximumDocumentFrequency = v));

    public TermVectorFilterDescriptor MinimumWordLength(int? minWordLength) => this.Assign<int?>(minWordLength, (Action<ITermVectorFilter, int?>) ((a, v) => a.MinimumWordLength = v));

    public TermVectorFilterDescriptor MaximumWordLength(int? maxWordLength) => this.Assign<int?>(maxWordLength, (Action<ITermVectorFilter, int?>) ((a, v) => a.MaximumWordLength = v));
  }
}
