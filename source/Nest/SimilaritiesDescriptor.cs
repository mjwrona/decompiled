// Decompiled with JetBrains decompiler
// Type: Nest.SimilaritiesDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;

namespace Nest
{
  public class SimilaritiesDescriptor : 
    IsADictionaryDescriptorBase<SimilaritiesDescriptor, ISimilarities, string, ISimilarity>
  {
    public SimilaritiesDescriptor()
      : base((ISimilarities) new Similarities())
    {
    }

    public SimilaritiesDescriptor BM25(
      string name,
      Func<BM25SimilarityDescriptor, IBM25Similarity> selector)
    {
      return this.Assign(name, selector != null ? (ISimilarity) selector(new BM25SimilarityDescriptor()) : (ISimilarity) null);
    }

    public SimilaritiesDescriptor LMDirichlet(
      string name,
      Func<LMDirichletSimilarityDescriptor, ILMDirichletSimilarity> selector)
    {
      return this.Assign(name, selector != null ? (ISimilarity) selector(new LMDirichletSimilarityDescriptor()) : (ISimilarity) null);
    }

    public SimilaritiesDescriptor LMJelinek(
      string name,
      Func<LMJelinekMercerSimilarityDescriptor, ILMJelinekMercerSimilarity> selector)
    {
      return this.Assign(name, selector != null ? (ISimilarity) selector(new LMJelinekMercerSimilarityDescriptor()) : (ISimilarity) null);
    }

    public SimilaritiesDescriptor DFI(
      string name,
      Func<DFISimilarityDescriptor, IDFISimilarity> selector)
    {
      return this.Assign(name, selector != null ? (ISimilarity) selector(new DFISimilarityDescriptor()) : (ISimilarity) null);
    }

    public SimilaritiesDescriptor DFR(
      string name,
      Func<DFRSimilarityDescriptor, IDFRSimilarity> selector)
    {
      return this.Assign(name, selector != null ? (ISimilarity) selector(new DFRSimilarityDescriptor()) : (ISimilarity) null);
    }

    public SimilaritiesDescriptor IB(
      string name,
      Func<IBSimilarityDescriptor, IIBSimilarity> selector)
    {
      return this.Assign(name, selector != null ? (ISimilarity) selector(new IBSimilarityDescriptor()) : (ISimilarity) null);
    }

    public SimilaritiesDescriptor Custom(
      string name,
      string type,
      Func<CustomSimilarityDescriptor, IPromise<ICustomSimilarity>> selector)
    {
      return this.Assign(name, selector != null ? (ISimilarity) selector(new CustomSimilarityDescriptor().Type(type))?.Value : (ISimilarity) null);
    }

    public SimilaritiesDescriptor Scripted(
      string name,
      Func<ScriptedSimilarityDescriptor, IScriptedSimilarity> selector)
    {
      return this.Assign(name, selector != null ? (ISimilarity) selector(new ScriptedSimilarityDescriptor()) : (ISimilarity) null);
    }
  }
}
