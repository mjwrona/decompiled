// Decompiled with JetBrains decompiler
// Type: Nest.LMDirichletSimilarityDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;

namespace Nest
{
  public class LMDirichletSimilarityDescriptor : 
    DescriptorBase<LMDirichletSimilarityDescriptor, ILMDirichletSimilarity>,
    ILMDirichletSimilarity,
    ISimilarity
  {
    int? ILMDirichletSimilarity.Mu { get; set; }

    string ISimilarity.Type => "LMDirichlet";

    public LMDirichletSimilarityDescriptor Mu(int? mu) => this.Assign<int?>(mu, (Action<ILMDirichletSimilarity, int?>) ((a, v) => a.Mu = v));
  }
}
