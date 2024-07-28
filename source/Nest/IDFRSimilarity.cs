// Decompiled with JetBrains decompiler
// Type: Nest.IDFRSimilarity
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System.Runtime.Serialization;

namespace Nest
{
  public interface IDFRSimilarity : ISimilarity
  {
    [DataMember(Name = "after_effect")]
    DFRAfterEffect? AfterEffect { get; set; }

    [DataMember(Name = "basic_model")]
    DFRBasicModel? BasicModel { get; set; }

    [DataMember(Name = "normalization")]
    Nest.Normalization? Normalization { get; set; }

    [DataMember(Name = "normalization.h1.c")]
    double? NormalizationH1C { get; set; }

    [DataMember(Name = "normalization.h2.c")]
    double? NormalizationH2C { get; set; }

    [DataMember(Name = "normalization.h3.c")]
    double? NormalizationH3C { get; set; }

    [DataMember(Name = "normalization.z.z")]
    double? NormalizationZZ { get; set; }
  }
}
