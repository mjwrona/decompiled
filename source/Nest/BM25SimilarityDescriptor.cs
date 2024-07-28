// Decompiled with JetBrains decompiler
// Type: Nest.BM25SimilarityDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;

namespace Nest
{
  public class BM25SimilarityDescriptor : 
    DescriptorBase<BM25SimilarityDescriptor, IBM25Similarity>,
    IBM25Similarity,
    ISimilarity
  {
    double? IBM25Similarity.B { get; set; }

    bool? IBM25Similarity.DiscountOverlaps { get; set; }

    double? IBM25Similarity.K1 { get; set; }

    string ISimilarity.Type => "BM25";

    public BM25SimilarityDescriptor DiscountOverlaps(bool? discount = true) => this.Assign<bool?>(discount, (Action<IBM25Similarity, bool?>) ((a, v) => a.DiscountOverlaps = v));

    public BM25SimilarityDescriptor K1(double? k1) => this.Assign<double?>(k1, (Action<IBM25Similarity, double?>) ((a, v) => a.K1 = v));

    public BM25SimilarityDescriptor B(double? b) => this.Assign<double?>(b, (Action<IBM25Similarity, double?>) ((a, v) => a.B = v));
  }
}
