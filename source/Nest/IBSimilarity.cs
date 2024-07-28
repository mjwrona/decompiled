// Decompiled with JetBrains decompiler
// Type: Nest.IBSimilarity
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

namespace Nest
{
  public class IBSimilarity : IIBSimilarity, ISimilarity
  {
    public IBDistribution? Distribution { get; set; }

    public IBLambda? Lambda { get; set; }

    public Nest.Normalization? Normalization { get; set; }

    public double? NormalizationH1C { get; set; }

    public double? NormalizationH2C { get; set; }

    public double? NormalizationH3C { get; set; }

    public double? NormalizationZZ { get; set; }

    public string Type => "IB";
  }
}
