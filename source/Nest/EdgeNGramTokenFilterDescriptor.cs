// Decompiled with JetBrains decompiler
// Type: Nest.EdgeNGramTokenFilterDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;

namespace Nest
{
  public class EdgeNGramTokenFilterDescriptor : 
    TokenFilterDescriptorBase<EdgeNGramTokenFilterDescriptor, IEdgeNGramTokenFilter>,
    IEdgeNGramTokenFilter,
    ITokenFilter
  {
    protected override string Type => "edge_ngram";

    int? IEdgeNGramTokenFilter.MaxGram { get; set; }

    int? IEdgeNGramTokenFilter.MinGram { get; set; }

    EdgeNGramSide? IEdgeNGramTokenFilter.Side { get; set; }

    bool? IEdgeNGramTokenFilter.PreserveOriginal { get; set; }

    public EdgeNGramTokenFilterDescriptor MinGram(int? minGram) => this.Assign<int?>(minGram, (Action<IEdgeNGramTokenFilter, int?>) ((a, v) => a.MinGram = v));

    public EdgeNGramTokenFilterDescriptor MaxGram(int? maxGram) => this.Assign<int?>(maxGram, (Action<IEdgeNGramTokenFilter, int?>) ((a, v) => a.MaxGram = v));

    public EdgeNGramTokenFilterDescriptor Side(EdgeNGramSide? side) => this.Assign<EdgeNGramSide?>(side, (Action<IEdgeNGramTokenFilter, EdgeNGramSide?>) ((a, v) => a.Side = v));

    public EdgeNGramTokenFilterDescriptor PreserveOriginal(bool? preserveOriginal = true) => this.Assign<bool?>(preserveOriginal, (Action<IEdgeNGramTokenFilter, bool?>) ((a, v) => a.PreserveOriginal = v));
  }
}
