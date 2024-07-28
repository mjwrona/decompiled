// Decompiled with JetBrains decompiler
// Type: Nest.DFRSimilarityDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;

namespace Nest
{
  public class DFRSimilarityDescriptor : 
    DescriptorBase<DFRSimilarityDescriptor, IDFRSimilarity>,
    IDFRSimilarity,
    ISimilarity
  {
    DFRAfterEffect? IDFRSimilarity.AfterEffect { get; set; }

    DFRBasicModel? IDFRSimilarity.BasicModel { get; set; }

    Normalization? IDFRSimilarity.Normalization { get; set; }

    double? IDFRSimilarity.NormalizationH1C { get; set; }

    double? IDFRSimilarity.NormalizationH2C { get; set; }

    double? IDFRSimilarity.NormalizationH3C { get; set; }

    double? IDFRSimilarity.NormalizationZZ { get; set; }

    string ISimilarity.Type => "DFR";

    public DFRSimilarityDescriptor BasicModel(DFRBasicModel? model) => this.Assign<DFRBasicModel?>(model, (Action<IDFRSimilarity, DFRBasicModel?>) ((a, v) => a.BasicModel = v));

    public DFRSimilarityDescriptor AfterEffect(DFRAfterEffect? afterEffect) => this.Assign<DFRAfterEffect?>(afterEffect, (Action<IDFRSimilarity, DFRAfterEffect?>) ((a, v) => a.AfterEffect = v));

    public DFRSimilarityDescriptor NoNormalization() => this.Assign<Normalization>(Normalization.No, (Action<IDFRSimilarity, Normalization>) ((a, v) => a.Normalization = new Normalization?(v)));

    public DFRSimilarityDescriptor NormalizationH1(double? c) => this.Assign<double?>(c, (Action<IDFRSimilarity, double?>) ((a, v) =>
    {
      a.Normalization = !v.HasValue ? new Normalization?() : new Normalization?(Normalization.H1);
      a.NormalizationH1C = v;
    }));

    public DFRSimilarityDescriptor NormalizationH2(double? c) => this.Assign<double?>(c, (Action<IDFRSimilarity, double?>) ((a, v) =>
    {
      a.Normalization = !v.HasValue ? new Normalization?() : new Normalization?(Normalization.H2);
      a.NormalizationH1C = v;
    }));

    public DFRSimilarityDescriptor NormalizationH3(double? mu) => this.Assign<double?>(mu, (Action<IDFRSimilarity, double?>) ((a, v) =>
    {
      a.Normalization = !v.HasValue ? new Normalization?() : new Normalization?(Normalization.H3);
      a.NormalizationH1C = v;
    }));

    public DFRSimilarityDescriptor NormalizationZ(double? z) => this.Assign<double?>(z, (Action<IDFRSimilarity, double?>) ((a, v) =>
    {
      a.Normalization = !v.HasValue ? new Normalization?() : new Normalization?(Normalization.Z);
      a.NormalizationH1C = v;
    }));
  }
}
