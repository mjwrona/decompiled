// Decompiled with JetBrains decompiler
// Type: Nest.IBSimilarityDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;

namespace Nest
{
  public class IBSimilarityDescriptor : 
    DescriptorBase<IBSimilarityDescriptor, IIBSimilarity>,
    IIBSimilarity,
    ISimilarity
  {
    IBDistribution? IIBSimilarity.Distribution { get; set; }

    IBLambda? IIBSimilarity.Lambda { get; set; }

    Normalization? IIBSimilarity.Normalization { get; set; }

    double? IIBSimilarity.NormalizationH1C { get; set; }

    double? IIBSimilarity.NormalizationH2C { get; set; }

    double? IIBSimilarity.NormalizationH3C { get; set; }

    double? IIBSimilarity.NormalizationZZ { get; set; }

    string ISimilarity.Type => "IB";

    public IBSimilarityDescriptor Distribution(IBDistribution? distribution) => this.Assign<IBDistribution?>(distribution, (Action<IIBSimilarity, IBDistribution?>) ((a, v) => a.Distribution = v));

    public IBSimilarityDescriptor Lambda(IBLambda? lambda) => this.Assign<IBLambda?>(lambda, (Action<IIBSimilarity, IBLambda?>) ((a, v) => a.Lambda = v));

    public IBSimilarityDescriptor NoNormalization() => this.Assign<Normalization>(Normalization.No, (Action<IIBSimilarity, Normalization>) ((a, v) => a.Normalization = new Normalization?(v)));

    public IBSimilarityDescriptor NormalizationH1(double? c) => this.Assign<double?>(c, (Action<IIBSimilarity, double?>) ((a, v) =>
    {
      a.Normalization = new Normalization?(Normalization.H1);
      a.NormalizationH1C = v;
    }));

    public IBSimilarityDescriptor NormalizationH2(double? c) => this.Assign<double?>(c, (Action<IIBSimilarity, double?>) ((a, v) =>
    {
      a.Normalization = new Normalization?(Normalization.H2);
      a.NormalizationH1C = v;
    }));

    public IBSimilarityDescriptor NormalizationH3(double? mu) => this.Assign<double?>(mu, (Action<IIBSimilarity, double?>) ((a, v) =>
    {
      a.Normalization = new Normalization?(Normalization.H3);
      a.NormalizationH1C = v;
    }));

    public IBSimilarityDescriptor NormalizationZ(double? z) => this.Assign<double?>(z, (Action<IIBSimilarity, double?>) ((a, v) =>
    {
      a.Normalization = new Normalization?(Normalization.Z);
      a.NormalizationH1C = v;
    }));
  }
}
